using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.API.Services.GET;

public class GETService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GETService> _logger;
    private readonly QanatDbContext _dbContext;
    private readonly FileService _fileService;
    private readonly QanatConfiguration _qanatConfiguration;

    public GETService(HttpClient httpClient, ILogger<GETService> logger, QanatDbContext dbContext, IOptions<QanatConfiguration> qanatConfiguration, FileService fileService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _dbContext = dbContext;
        _fileService = fileService;
        _qanatConfiguration = qanatConfiguration.Value;
    }

    public async Task<bool> IsAPIResponsive()
    {
        try
        {
            var response = await _httpClient.GetAsync("Health");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred when checking GET API Health. Error:{ex.Message}");
            return false;
        }
    }

    public async Task StartNewCustomScenarioRun(int getActionID, int? getRunCustomerID, int? getRunUserID)
    {
        var getAction = GETActions.GetByID(_dbContext, getActionID);
        var getScenarioID = _dbContext.ModelScenarios.Single(x =>
            x.ModelID == getAction.ModelID && x.ScenarioID == getAction.ScenarioID).GETScenarioID;

        if (getAction == null)
        {
            return;
        }

        if (!await IsAPIResponsive())
        {
            GETActions.MarkAsTerminalWithIntegrationFailure(_dbContext, getAction);
            return;
        }

        var customerID = getRunCustomerID ?? _qanatConfiguration.GETRunCustomerID;
        var userID = getRunUserID ?? _qanatConfiguration.GETRunUserID;
        var model = new GETNewRunModel(getActionID, getAction, customerID, userID, getScenarioID);

        // for the custom scenario runs we need to submit as "multipart/form-data"
        var multipartFormData = new MultipartFormDataContent
        {
            // the model can come in as Json under the name of "request"
            { new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"), "\"request\"" }
        };

        if (getAction.GETActionFileResources.Any())
        {
            foreach (var fileResource in getAction.GETActionFileResources.Select(x => x.FileResource))
            {
                var stream = await _fileService.GetFileStreamFromBlobStorage(fileResource.FileResourceCanonicalName);
                var fileContent = new StreamContent(stream);
                var fullFileName = $"{fileResource.OriginalBaseFilename}.{fileResource.OriginalFileExtension}";
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "\"files\"", // You can specify a different name for each file
                    FileName = fullFileName
                };
                multipartFormData.Add(fileContent, "\"files\"", fullFileName);
            }
        }

        var response = await _httpClient.PostAsync("StartRun", multipartFormData);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            _logger.LogError(errorMessage);
            GETActions.MarkAsTerminalWithIntegrationFailure(_dbContext, getAction);
            return;
        }

        var getRunResponseModel = await JsonSerializer.DeserializeAsync<GETRunResponseModel>(await response.Content.ReadAsStreamAsync());

        getAction.LastUpdateDate = DateTime.UtcNow;
        getAction.GETRunID = getRunResponseModel.RunID;
        getAction.GETActionStatusID = ConvertGETRunStatusIDToGETActionStatusID(getRunResponseModel).GETActionStatusID;
        await _dbContext.SaveChangesAsync();
    }

    private GETActionStatus ConvertGETRunStatusIDToGETActionStatusID(GETRunResponseModel getRunResponseModel)
    {
        var getActionStatus = GETActionStatus.All.SingleOrDefault(x => x.GETRunStatusID == getRunResponseModel.RunStatus.RunStatusID);
        if (getActionStatus == null)
        {
            _logger.LogError($"GETRunStatus {getRunResponseModel.RunStatus.RunStatusDisplayName} (ID: {getRunResponseModel.RunStatus.RunStatusID}) not mapped to any GETActionStatus");
            return GETActionStatus.SystemError;
        }
        return getActionStatus;
    }

    public async Task UpdateCurrentlyRunningRunStatus(int getActionID, int? getRunCustomerID)
    {
        var getAction = GETActions.GetByID(_dbContext, getActionID);

        if (getAction == null)
        {
            _logger.LogError($"getAction with {getActionID} was not found.");
            return;
        }

        if (!await IsAPIResponsive())
        {
            GETActions.MarkAsTerminalWithIntegrationFailure(_dbContext, getAction);
            _logger.LogError("GET Api was non-responsive");
            return;
        }

        var customerID = getRunCustomerID ?? _qanatConfiguration.GETRunCustomerID;
        var getRunDetailModel = new GETRunDetailModel(getAction.GETRunID.Value, customerID);
        var response = await _httpClient.PostAsJsonAsync("GetRunStatus", getRunDetailModel);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            _logger.LogError(errorMessage);
            return;
        }

        var getRunResponseModel = await JsonSerializer.DeserializeAsync<GETRunResponseModel>(await response.Content.ReadAsStreamAsync());

        var runResponseString = JsonSerializer.Serialize(getRunResponseModel);
        _logger.LogInformation($"Retrieved getRunResponseModel with {runResponseString}");

        getAction.LastUpdateDate = DateTime.UtcNow;
        getAction.GETActionStatusID = ConvertGETRunStatusIDToGETActionStatusID(getRunResponseModel).GETActionStatusID;

        if (getRunResponseModel.RunStatus.RunStatusID == GETActionStatus.Complete.GETRunStatusID)
        {
            foreach (var outputFileType in GETActionOutputFileType.All)
            {
                RunResultResponseModel runResultResponseModel = null;

                _logger.LogInformation($"Trying to retrieve the {outputFileType.GETActionOutputFileTypeName} output file for {getAction.GETActionID} (GET RunID:{getAction.GETRunID}).");

                // skip the file download if we already have it. AFAIK they should never change after the run finishes...
                if (getAction.GETActionOutputFiles.Any(x =>
                        x.GETActionOutputFileTypeID == outputFileType.GETActionOutputFileTypeID))
                {
                    _logger.LogWarning($"The {outputFileType.GETActionOutputFileTypeName} output file already exists for {getAction.GETActionID} (GET RunID:{getAction.GETRunID}). It will be overwritten.");
                    //continue;
                }
                getAction.LastUpdateDate = DateTime.UtcNow;
                switch (outputFileType.ToEnum)
                {
                    case GETActionOutputFileTypeEnum.GroundWaterBudget:
                        runResultResponseModel = await RetrieveResultImpl(getAction, outputFileType.GETActionOutputFileTypeName, outputFileType.GETActionOutputFileTypeExtension, customerID);
                        break;
                    case GETActionOutputFileTypeEnum.TimeSeriesData:
                        runResultResponseModel = await RetrieveResultImpl(getAction, outputFileType.GETActionOutputFileTypeName, outputFileType.GETActionOutputFileTypeExtension, customerID);
                        break;
                    case GETActionOutputFileTypeEnum.WaterBudget:
                        runResultResponseModel = await RetrieveResultImpl(getAction, outputFileType.GETActionOutputFileTypeName, outputFileType.GETActionOutputFileTypeExtension, customerID);
                        break;
                    case GETActionOutputFileTypeEnum.PointsofInterest:
                        runResultResponseModel = await RetrieveResultImpl(getAction, outputFileType.GETActionOutputFileTypeName, outputFileType.GETActionOutputFileTypeExtension, customerID);
                        break;
                }

                if (runResultResponseModel != null)
                {
                    var existingOutputFile = getAction.GETActionOutputFiles.SingleOrDefault(x =>
                        x.GETActionOutputFileTypeID == outputFileType.GETActionOutputFileTypeID);
                    if (existingOutputFile != null)
                    {
                        _dbContext.GETActionOutputFiles.Remove(existingOutputFile);
                        _dbContext.FileResources.Remove(existingOutputFile.FileResource);
                        _fileService.DeleteFileStreamFromBlobStorage(existingOutputFile.FileResource.FileResourceCanonicalName);
                    }

                    _logger.LogInformation("Retrieved runResultResponseModel, trying to create file resource.");
                    var outputFileResource =
                        await CreateOutputFileResource(runResultResponseModel, outputFileType, getAction);

                    getAction.GETActionOutputFiles.Add(new GETActionOutputFile()
                    {
                        FileResourceID = outputFileResource.FileResourceID,
                        GETActionID = getActionID,
                        GETActionOutputFileTypeID = outputFileType.GETActionOutputFileTypeID
                    });
                }
            }

        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<FileResource> CreateOutputFileResource(RunResultResponseModel runResultResponseModel, GETActionOutputFileType outputFileType, GETAction getAction)
    {
        return await _fileService.CreateFileResource(
            _dbContext,
            new MemoryStream(Encoding.UTF8.GetBytes(runResultResponseModel.FileDetails)),
            $"{outputFileType.GETActionOutputFileTypeName}{outputFileType.GETActionOutputFileTypeExtension}",
            getAction.UserID);
    }


    private async Task<RunResultResponseModel> RetrieveResultImpl(GETAction getAction, string outputFileName, string outputFileExtension, int getRunCustomerID)
    {
        _logger.LogInformation($"Trying to retrieve the {outputFileName}{outputFileExtension} output file.");
        var retrieveResultModel = new RetrieveResultModel(getAction.GETRunID.Value, getRunCustomerID, outputFileName, outputFileExtension);

        var response = await _httpClient.PostAsJsonAsync("RetrieveResult", retrieveResultModel);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            _logger.LogError(errorMessage);
            return null;
        }

        var runResultResponseModel = await JsonSerializer.DeserializeAsync<RunResultResponseModel>(await response.Content.ReadAsStreamAsync());
        return runResultResponseModel;
    }

    public static IFormFile ToUserDataJsonFile(AddAWellScenarioDto addAWellScenarioDto)
    {
        var wellInputs = addAWellScenarioDto.ScenarioPumpingWells.Select(x => new UserDataJsonFile.PivotedRunWellInput()
        {
            AverageValue = -x.EstimatedExtraction, // negative because GET expects a negative value for taking water out
            Lat = x.Latitude,
            Lng = x.Longitude,
            Name = x.PumpingWellName
        }).ToList();

        var observationPoints = addAWellScenarioDto.ScenarioObservationPoints.Select(x => new UserDataJsonFile.PivotedRunWellInput()
        {
            Lat = x.Latitude,
            Lng = x.Longitude,
            Name = x.ObservationPointName
        }).ToList();

        var userDataJsonFile = new UserDataJsonFile()
        {
            Name = addAWellScenarioDto.ScenarioRunName,
            PivotedRunWellInputs = new List<UserDataJsonFile.PivotedRunWellInput>()
        };
        userDataJsonFile.PivotedRunWellInputs.AddRange(wellInputs);
        userDataJsonFile.PivotedRunWellInputs.AddRange(observationPoints);

        var jsonString = JsonSerializer.Serialize(userDataJsonFile);
        byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
        MemoryStream stream = new MemoryStream(bytes);

        var file = new FormFile(stream, 0, stream.Length, null, "userdata.json")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/json"
        };
        return file;
    }

    public static IFormFile ToUserDataJsonFile(RechargeScenarioDto rechargeScenarioDto)
    {
        var wellInputs = rechargeScenarioDto.ScenarioRechargeSites.Select(x => new UserDataJsonFile.PivotedRunWellInput()
        {
            AverageValue = x.EstimatedVolume, // negative because GET expects a negative value for taking water out
            Lat = x.Latitude,
            Lng = x.Longitude,
            Name = x.RechargeSiteName
        }).ToList();

        var observationPoints = rechargeScenarioDto.ScenarioObservationPoints.Select(x => new UserDataJsonFile.PivotedRunWellInput()
        {
            Lat = x.Latitude,
            Lng = x.Longitude,
            Name = x.ObservationPointName
        }).ToList();

        var userDataJsonFile = new UserDataJsonFile()
        {
            Name = rechargeScenarioDto.ScenarioRunName,
            PivotedRunWellInputs = new List<UserDataJsonFile.PivotedRunWellInput>()
        };
        userDataJsonFile.PivotedRunWellInputs.AddRange(wellInputs);
        userDataJsonFile.PivotedRunWellInputs.AddRange(observationPoints);

        var jsonString = JsonSerializer.Serialize(userDataJsonFile);
        byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
        MemoryStream stream = new MemoryStream(bytes);

        var file = new FormFile(stream, 0, stream.Length, null, "userdata.json")
        {
            Headers = new HeaderDictionary(),
            ContentType = "application/json"
        };
        return file;
    }
}