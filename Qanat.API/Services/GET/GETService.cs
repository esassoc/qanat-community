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

    public async Task StartNewCustomScenarioRun(int scenarioRunID, int? getRunCustomerID, int? getRunUserID)
    {
        var scenarioRun = ScenarioRuns.GetByID(_dbContext, scenarioRunID);
        var scenarioID = _dbContext.ModelScenarios.Single(x => x.ModelID == scenarioRun.ModelID && x.ScenarioID == scenarioRun.ScenarioID).GETScenarioID;

        if (scenarioRun == null)
        {
            return;
        }

        if (!await IsAPIResponsive())
        {
            ScenarioRuns.MarkAsTerminalWithIntegrationFailure(_dbContext, scenarioRun);
            return;
        }

        var customerID = getRunCustomerID ?? _qanatConfiguration.GETRunCustomerID;
        var userID = getRunUserID ?? _qanatConfiguration.GETRunUserID;
        var model = new GETNewRunModel(scenarioRunID, scenarioRun, customerID, userID, scenarioID);

        // for the custom scenario runs we need to submit as "multipart/form-data"
        var multipartFormData = new MultipartFormDataContent
        {
            // the model can come in as Json under the name of "request"
            { new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"), "\"request\"" }
        };

        if (scenarioRun.ScenarioRunFileResources.Any())
        {
            foreach (var fileResource in scenarioRun.ScenarioRunFileResources.Select(x => x.FileResource))
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
            ScenarioRuns.MarkAsTerminalWithIntegrationFailure(_dbContext, scenarioRun);
            return;
        }

        var getRunResponseModel = await JsonSerializer.DeserializeAsync<GETRunResponseModel>(await response.Content.ReadAsStreamAsync());

        scenarioRun.LastUpdateDate = DateTime.UtcNow;
        scenarioRun.GETRunID = getRunResponseModel.RunID;
        scenarioRun.ScenarioRunStatusID = ConvertGETRunStatusIDToGETActionStatusID(getRunResponseModel).ScenarioRunStatusID;
        await _dbContext.SaveChangesAsync();
    }

    private ScenarioRunStatus ConvertGETRunStatusIDToGETActionStatusID(GETRunResponseModel getRunResponseModel)
    {
        var scenarioRunStatus = ScenarioRunStatus.All.SingleOrDefault(x => x.GETRunStatusID == getRunResponseModel.RunStatus.RunStatusID);
        if (scenarioRunStatus == null)
        {
            _logger.LogError($"GETRunStatus {getRunResponseModel.RunStatus.RunStatusDisplayName} (ID: {getRunResponseModel.RunStatus.RunStatusID}) not mapped to any ScenarioRunStatus");
            return ScenarioRunStatus.SystemError;
        }

        return scenarioRunStatus;
    }

    public async Task UpdateCurrentlyRunningRunStatus(int scenarioRunID, int? getRunCustomerID)
    {
        var scenarioRun = ScenarioRuns.GetByID(_dbContext, scenarioRunID);
        if (scenarioRun == null)
        {
            _logger.LogError($"getAction with {scenarioRunID} was not found.");
            return;
        }

        if (!await IsAPIResponsive())
        {
            ScenarioRuns.MarkAsTerminalWithIntegrationFailure(_dbContext, scenarioRun);
            _logger.LogError("GET Api was non-responsive");
            return;
        }

        var customerID = getRunCustomerID ?? _qanatConfiguration.GETRunCustomerID;
        var getRunDetailModel = new GETRunDetailModel(scenarioRun.GETRunID.Value, customerID);
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

        scenarioRun.LastUpdateDate = DateTime.UtcNow;
        scenarioRun.ScenarioRunStatusID = ConvertGETRunStatusIDToGETActionStatusID(getRunResponseModel).ScenarioRunStatusID;

        if (getRunResponseModel.RunStatus.RunStatusID == GETActionStatus.Complete.GETRunStatusID)
        {
            foreach (var outputFileType in ScenarioRunOutputFileType.All)
            {
                RunResultResponseModel runResultResponseModel = null;

                _logger.LogInformation($"Trying to retrieve the {outputFileType.ScenarioRunOutputFileTypeName} output file for {scenarioRun.ScenarioRunID} (GET RunID:{scenarioRun.GETRunID}).");

                // skip the file download if we already have it. AFAIK they should never change after the run finishes...
                if (scenarioRun.ScenarioRunOutputFiles.Any(x => x.ScenarioRunOutputFileTypeID == outputFileType.ScenarioRunOutputFileTypeID))
                {
                    _logger.LogWarning($"The {outputFileType.ScenarioRunOutputFileTypeName} output file already exists for {scenarioRun.ScenarioRunID} (GET RunID:{scenarioRun.GETRunID}). It will be overwritten.");
                    //continue;
                }
                scenarioRun.LastUpdateDate = DateTime.UtcNow;
                switch (outputFileType.ToEnum)
                {
                    case ScenarioRunOutputFileTypeEnum.GroundWaterBudget:
                        runResultResponseModel = await RetrieveResultImpl(scenarioRun, outputFileType.ScenarioRunOutputFileTypeName, outputFileType.ScenarioRunOutputFileTypeExtension, customerID);
                        break;
                    case ScenarioRunOutputFileTypeEnum.TimeSeriesData:
                        runResultResponseModel = await RetrieveResultImpl(scenarioRun, outputFileType.ScenarioRunOutputFileTypeName, outputFileType.ScenarioRunOutputFileTypeExtension, customerID);
                        break;
                    case ScenarioRunOutputFileTypeEnum.WaterBudget:
                        runResultResponseModel = await RetrieveResultImpl(scenarioRun, outputFileType.ScenarioRunOutputFileTypeName, outputFileType.ScenarioRunOutputFileTypeExtension, customerID);
                        break;
                    case ScenarioRunOutputFileTypeEnum.PointsofInterest:
                        runResultResponseModel = await RetrieveResultImpl(scenarioRun, outputFileType.ScenarioRunOutputFileTypeName, outputFileType.ScenarioRunOutputFileTypeExtension, customerID);
                        break;
                }

                if (runResultResponseModel != null)
                {
                    var existingOutputFile = scenarioRun.ScenarioRunOutputFiles.SingleOrDefault(x => x.ScenarioRunOutputFileTypeID == outputFileType.ScenarioRunOutputFileTypeID);
                    if (existingOutputFile != null)
                    {
                        _dbContext.ScenarioRunOutputFiles.Remove(existingOutputFile);
                        _dbContext.FileResources.Remove(existingOutputFile.FileResource);
                        _fileService.DeleteFileStreamFromBlobStorage(existingOutputFile.FileResource.FileResourceCanonicalName);
                    }

                    _logger.LogInformation("Retrieved runResultResponseModel, trying to create file resource.");
                    var outputFileResource =  await CreateOutputFileResource(runResultResponseModel, outputFileType, scenarioRun);

                    scenarioRun.ScenarioRunOutputFiles.Add(new ScenarioRunOutputFile()
                    {
                        FileResourceID = outputFileResource.FileResourceID,
                        ScenarioRunID = scenarioRunID,
                        ScenarioRunOutputFileTypeID = outputFileType.ScenarioRunOutputFileTypeID
                    });
                }
            }

        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<FileResource> CreateOutputFileResource(RunResultResponseModel runResultResponseModel, ScenarioRunOutputFileType outputFileType, ScenarioRun scenarioRun)
    {
        return await _fileService.CreateFileResource(
            _dbContext,
            new MemoryStream(Encoding.UTF8.GetBytes(runResultResponseModel.FileDetails)),
            $"{outputFileType.ScenarioRunOutputFileTypeName}{outputFileType.ScenarioRunOutputFileTypeExtension}",
            scenarioRun.UserID);
    }


    private async Task<RunResultResponseModel> RetrieveResultImpl(ScenarioRun scenarioRun, string outputFileName, string outputFileExtension, int getRunCustomerID)
    {
        _logger.LogInformation($"Trying to retrieve the {outputFileName}{outputFileExtension} output file.");
        var retrieveResultModel = new RetrieveResultModel(scenarioRun.GETRunID.Value, getRunCustomerID, outputFileName, outputFileExtension);

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