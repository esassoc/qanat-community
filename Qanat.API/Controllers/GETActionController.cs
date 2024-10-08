using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.API.Services.GET;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Qanat.API.Services.GET.ModFlow;
using Qanat.API.Services.GET.IWFM;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class GETActionController : SitkaController<GETActionController>
{
    private readonly GETService _getService;
    private readonly FileService _fileService;

    public GETActionController(QanatDbContext dbContext, ILogger<GETActionController> logger, IOptions<QanatConfiguration> qanatConfiguration, GETService getService, FileService fileService) : base(dbContext, logger, qanatConfiguration)
    {
        _getService = getService;
        _fileService = fileService;
    }

    [HttpGet("/models")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public ActionResult<List<ModelSimpleDto>> ListModels()
    {
        var modelsDto = Model.AllAsSimpleDto;
        return Ok(modelsDto);
    }

    [HttpGet("/models/{modelShortName}")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)] 
    public ActionResult<ModelSimpleDto> GetModelByID([FromRoute] string modelShortName)
    {
        var modelDto = Model.AllAsSimpleDto.SingleOrDefault(x => x.ModelShortName == modelShortName);
        if (ThrowNotFound(modelDto, "Model", modelShortName, out var result))
        {
            return result;
        }
        return Ok(modelDto);
    }

    [HttpGet("/models/{modelID}/image")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public async Task<ActionResult<string>> GetModelImageByID([FromRoute] int modelID)
    {
        var model = Model.All.SingleOrDefault(x => x.ModelID == modelID);
        if (ThrowNotFound(model, "Model", modelID, out var result))
        {
            return result;
        }

        return Ok(model.ModelImage);
    }

    [HttpGet("/scenarios/{scenarioID}/image")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public async Task<ActionResult<string>> GetScenarioImageByID([FromRoute] int scenarioID)
    {
        var scenario = Scenario.All.SingleOrDefault(x => x.ScenarioID == scenarioID);
        if (ThrowNotFound(scenario, "Scenario", scenarioID, out var result))
        {
            return result;
        }

        return Ok(scenario.ScenarioImage);
    }

    [HttpGet("/models/{modelShortName}/boundary")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public ActionResult<ModelBoundaryDto> GetModelBoundaryByModelShortName([FromRoute] string modelShortName)
    {
        var model = Model.All.SingleOrDefault(x => x.ModelShortName == modelShortName);
        if (ThrowNotFound(model, "Model", modelShortName, out var result))
        {
            return result;
        }

        var modelBoundaryDto = _dbContext.ModelBoundaries.AsNoTracking()
            .SingleOrDefault(x => x.ModelID == model.ModelID)?.AsModelBoundaryDto();

        return Ok(modelBoundaryDto);
    }

    [HttpGet("/models/{modelShortName}/scenarios")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public ActionResult<List<ScenarioSimpleDto>> ListScenariosForModel([FromRoute] string modelShortName)
    {
        var model = Model.All.SingleOrDefault(x => x.ModelShortName == modelShortName);
        if (ThrowNotFound(model, "Model", modelShortName, out var result))
        {
            return result;
        }
        var scenarioIDs = _dbContext.ModelScenarios.AsNoTracking().Where(x => x.ModelID == model.ModelID).Select(x => x.ScenarioID);
        var scenarioDtos = Scenario.AllAsSimpleDto.Where(x => scenarioIDs.Contains(x.ScenarioID)).ToList();
        return Ok(scenarioDtos);
    }

    [HttpGet("/models/{modelShortName}/actions")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public ActionResult<List<GETActionDto>> ListActionsForModel([FromRoute] string modelShortName)
    {
        var model = Model.All.SingleOrDefault(x => x.ModelShortName == modelShortName);

        if (ThrowNotFound(model, "Model", modelShortName, out var result))
        {
            return result;
        }

        var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        if (userDto.GETRunCustomerID != null)
        {
            var byModelIDAndUserID = GETActions.ListByModelIDAndGETRunCustomerID(_dbContext, model.ModelID, userDto.GETRunCustomerID.Value);
            return Ok(byModelIDAndUserID);
        }
        var getActions = GETActions.ListByModelID(_dbContext, model.ModelID);
        return Ok(getActions);
    }

    [HttpGet("/scenarios")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public ActionResult<List<ScenarioSimpleDto>> ListScenarios()
    {
        var modelsDto = Scenario.AllAsSimpleDto;
        return Ok(modelsDto);
    }

    [HttpGet("/scenarios/{scenarioShortName}")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public ActionResult<ScenarioSimpleDto> GetScenarioByID([FromRoute] string scenarioShortName)
    {
        var scenarioDto = Scenario.AllAsSimpleDto.SingleOrDefault(x => x.ScenarioShortName == scenarioShortName);
        if (ThrowNotFound(scenarioDto, "Scenario", scenarioShortName, out var result))
        {
            return result;
        }
        return Ok(scenarioDto);
    }

    [HttpPost("scenarios/add-a-well")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public async Task<IActionResult> StartAddAWellScenarioRun([FromBody] AddAWellScenarioDto addAWellScenarioDto)
    {
        var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        ValidateAddAWellScenarioRun(addAWellScenarioDto);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // store the GETAction in our system
        var getAction = await GETActions.CreateNew(_dbContext, userDto.UserID, addAWellScenarioDto.ModelID, Scenario.AddaWell.ScenarioID, addAWellScenarioDto.ScenarioRunName);
        var userDataJsonFile = GETService.ToUserDataJsonFile(addAWellScenarioDto);
        var fileResource = await _fileService.CreateFileResource(_dbContext, userDataJsonFile, userDto.UserID);
        await GETActions.AddFileResource(_dbContext, getAction.GETActionID, fileResource);

        // start the run!
        await _getService.StartNewCustomScenarioRun(getAction.GETActionID, userDto.GETRunCustomerID, userDto.GETRunUserID);

        return Ok();
    }

    private void ValidateAddAWellScenarioRun(AddAWellScenarioDto addAWellScenarioDto)
    {
        if (addAWellScenarioDto.ScenarioPumpingWells == null || addAWellScenarioDto.ScenarioPumpingWells.Count == 0)
        {
            ModelState.AddModelError("Pumping Wells", "Please provide at least one pumping well.");
        }

        // make sure all the names are unique for both observation points and wells
        if (addAWellScenarioDto.ScenarioPumpingWells != null && addAWellScenarioDto.ScenarioPumpingWells.Any())
        {
            var names = addAWellScenarioDto.ScenarioPumpingWells.Select(x => x.PumpingWellName).ToList();

            if (addAWellScenarioDto.ScenarioObservationPoints != null &&
                addAWellScenarioDto.ScenarioObservationPoints.Any())
            {
                names.AddRange(addAWellScenarioDto.ScenarioObservationPoints.Select(x => x.ObservationPointName));
            }

            if (names.Distinct().Count() < names.Count)
            {
                ModelState.AddModelError("Labels", "The labels for pumping wells and observation points must be unique.");
            }
        }
    }

    [HttpPost("scenarios/recharge")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public async Task<IActionResult> StartRechargeScenarioRun([FromBody] RechargeScenarioDto rechargeScenarioDto)
    {
        var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        ValidateRechargeScenarioRun(rechargeScenarioDto);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // store the GETAction in our system
        var getAction = await GETActions.CreateNew(_dbContext, userDto.UserID, rechargeScenarioDto.ModelID, Scenario.Recharge.ScenarioID, rechargeScenarioDto.ScenarioRunName);
        var userDataJsonFile = GETService.ToUserDataJsonFile(rechargeScenarioDto);
        var fileResource = await _fileService.CreateFileResource(_dbContext, userDataJsonFile, userDto.UserID);
        await GETActions.AddFileResource(_dbContext, getAction.GETActionID, fileResource);

        // start the run!
        await _getService.StartNewCustomScenarioRun(getAction.GETActionID, userDto.GETRunCustomerID, userDto.GETRunUserID);

        return Ok();
    }

    private void ValidateRechargeScenarioRun(RechargeScenarioDto rechargeScenarioDto)
    {
        if (rechargeScenarioDto.ScenarioRechargeSites == null || rechargeScenarioDto.ScenarioRechargeSites.Count == 0)
        {
            ModelState.AddModelError("Recharge Site", "Please provide at least one recharge site.");
        }

        // make sure all the names are unique for both observation points and wells
        if (rechargeScenarioDto.ScenarioRechargeSites != null && rechargeScenarioDto.ScenarioRechargeSites.Any())
        {
            var names = rechargeScenarioDto.ScenarioRechargeSites.Select(x => x.RechargeSiteName).ToList();

            if (rechargeScenarioDto.ScenarioObservationPoints != null &&
                rechargeScenarioDto.ScenarioObservationPoints.Any())
            {
                names.AddRange(rechargeScenarioDto.ScenarioObservationPoints.Select(x => x.ObservationPointName));
            }

            if (names.Distinct().Count() < names.Count)
            {
                ModelState.AddModelError("Labels", "The labels for recharge sites and observation points must be unique.");
            }
        }
    }

    [HttpGet("/actions")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public ActionResult<List<GETActionDto>> GetAllActions()
    {
        var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        if (userDto.GETRunCustomerID != null)
        {
            var userActions = GETActions.ListByGETRunCustomerID(_dbContext, userDto.GETRunCustomerID.Value);
            return Ok(userActions);
        }
        var actions = GETActions.List(_dbContext).OrderByDescending(x => x.CreateDate);
        return Ok(actions);
    }

    [HttpGet("/actions/{getActionID}")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public ActionResult<GETActionDto> GetGetActionByID([FromRoute] int getActionID)
    {
        var getAction = GETActions.GetByID(_dbContext, getActionID);
        if (ThrowNotFound(getAction, "GETAction", getActionID, out var result))
        {
            return result;
        }
        return Ok(getAction.AsGETActionDto());
    }

    [HttpPost("/actions/{getActionID}/checkStatus")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public async Task<ActionResult<GETActionDto>> CheckGetActionStatus([FromRoute] int getActionID)
    {
        var getAction = GETActions.GetByID(_dbContext, getActionID);
        if(ThrowNotFound(getAction, "GETAction", getActionID, out var result))
        {
            return result;
        }

        var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        await _getService.UpdateCurrentlyRunningRunStatus(getAction.GETActionID, userDto.GETRunCustomerID);
        await _dbContext.Entry(getAction).ReloadAsync();
        return Ok(getAction.AsGETActionDto());
    }

    [HttpGet("/actions/{getActionID}/results")]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public async Task<ActionResult<GetActionResult>> GetBudgetGroundwaterOutput([FromRoute] int getActionID)
    {
        var getAction = GETActions.GetByID(_dbContext, getActionID);
        if (ThrowNotFound(getAction, "GETAction", getActionID, out var result))
        {
            return result;
        }

        var getActionOutputFileTypeBudgetEnum = getAction.Model.ToEnum switch
        {
            ModelEnum.MercedWaterResourcesModel => GETActionOutputFileTypeEnum.GroundWaterBudget,
            ModelEnum.KernC2VSimFGKern => GETActionOutputFileTypeEnum.GroundWaterBudget,
            ModelEnum.YSGAWaterResourcesModel => GETActionOutputFileTypeEnum.WaterBudget,
            _ => throw new ArgumentOutOfRangeException(
                $"Model Type {getAction.Model.ModelName} does not have a water budget schema defined!")
        };

        var getActionOutputFileTypeTimeSeriesEnum = getAction.Model.ToEnum switch
        {
            ModelEnum.MercedWaterResourcesModel => GETActionOutputFileTypeEnum.TimeSeriesData,
            ModelEnum.KernC2VSimFGKern => GETActionOutputFileTypeEnum.TimeSeriesData,
            ModelEnum.YSGAWaterResourcesModel => GETActionOutputFileTypeEnum.PointsofInterest,
            _ => throw new ArgumentOutOfRangeException(
                $"Model Type {getAction.Model.ModelName} does not have a time series schema defined!")
        };

        var budgetFile = getAction.GETActionOutputFiles.SingleOrDefault(x =>
            x.GETActionOutputFileTypeID == (int)getActionOutputFileTypeBudgetEnum);
        var timeSeriesFile = getAction.GETActionOutputFiles.SingleOrDefault(x => x.GETActionOutputFileTypeID == (int)getActionOutputFileTypeTimeSeriesEnum);

        if (budgetFile == null || timeSeriesFile == null)
        {
            return NotFound();
        }

        var budgetJsonString = await GetFileFromBlobStorageAsJsonString(budgetFile);
        var timeSeriesJsonString = await GetFileFromBlobStorageAsJsonString(timeSeriesFile);

        // each model returns different types of outputs.  We need to format them to a consistent format to display to users
        List<GetActionResultPointOfInterest> getActionResultPointOfInterests;
        GetActionResult getActionResult;
        switch (getAction.Model.ToEnum)
        {
            case ModelEnum.MercedWaterResourcesModel:
            case ModelEnum.KernC2VSimFGKern:
                getActionResult = GetActionResultForIWFM(budgetJsonString, timeSeriesJsonString);
                break;

            case ModelEnum.YSGAWaterResourcesModel:
                getActionResult = await GetActionResultForYSGAWaterResourcesModel(getAction, budgetJsonString, timeSeriesJsonString);
                break;

            default:
                throw new ArgumentOutOfRangeException($"Model Type {getAction.Model.ModelName} does not have a time series schema defined!");
        }
        return Ok(getActionResult);
    }

    private static GetActionResult GetActionResultForIWFM(string budgetJsonString, string timeSeriesJsonString)
    {
        var getActionResult = new GetActionResult();
        var budgetOutput = JsonSerializer.Deserialize<BudgetGroundwaterResult>(budgetJsonString);
        var baselineEndingStorage = budgetOutput.BaselinePeriods.Last().EndingStorage;
        var modelRunEndingStorage = budgetOutput.Periods.Last().EndingStorage;
        getActionResult.TotalChangeInAquiferStorage = modelRunEndingStorage - baselineEndingStorage;
        //getActionResult.PercentChangeInAquiferStorage = -((baselineEndingStorage - modelRunEndingStorage) / baselineEndingStorage) * 100;
        getActionResult.TotalChangeInPumping = budgetOutput.Periods.Select((t, i) => budgetOutput.BaselinePeriods[i].Pumping - t.Pumping).ToList().Sum();
        getActionResult.TotalChangeInRecharge = budgetOutput.Periods.Select((t, i) => budgetOutput.BaselinePeriods[i].Recharge - t.Recharge).ToList().Sum();

        var timeSeriesOutput = JsonSerializer.Deserialize<TimeSeriesOutput>(timeSeriesJsonString);
        var waterLevelChanges = (timeSeriesOutput.PivotedRunWellInputs.Select(input => input.TimeSteps.LastOrDefault())
            .Where(timeStep => timeStep != null)
            .Select(timeStep => -timeStep.BaselineValueDifference)).ToList();

        getActionResult.AverageChangeInWaterLevel = waterLevelChanges.Average();
        getActionResult.ModelRunEndDate = timeSeriesOutput.PivotedRunWellInputs.First().TimeSteps.Last().DateTime;

        var getActionResultPointOfInterests = timeSeriesOutput.PivotedRunWellInputs.Select(x =>
            new GetActionResultPointOfInterest()
            {
                Name = x.Name,
                AverageValue = x.AverageValue,
                Latitude = x.Lat,
                Longitude = x.Lng,
                GetActionResultTimeSeriesOutputs =
                    x.TimeSteps.GroupBy(y => y.DateTime.Year).Select(y =>
                        new GetActionResultTimeSeriesOutput()
                        {
                            Name = x.Name,
                            Date = new DateTime(y.Key,1,1),
                            Value = -y.Average(z => z.BaselineValueDifference)
                        }).ToList()
            }).ToList();
        getActionResult.PointsOfInterest = getActionResultPointOfInterests;
        return getActionResult;
    }

    private async Task<GetActionResult> GetActionResultForYSGAWaterResourcesModel(GETAction getAction, string budgetJsonString,
        string timeSeriesJsonString)
    {
        var getActionResult = new GetActionResult();
        var userDataFileResource = getAction.GETActionFileResources.Single(x =>
            x.FileResource.OriginalBaseFilename == "userdata");
        var userDataJsonString = await GetFileFromBlobStorageAsJsonString(userDataFileResource.FileResource);
        var userDataJson = JsonSerializer.Deserialize<UserDataJsonFile>(userDataJsonString);
        var budgetOutput = JsonSerializer.Deserialize<RunResult>(budgetJsonString);
        var cumulativeResult = budgetOutput.ResultSets.Single(x => x.Name == "Cumulative");
        getActionResult.TotalChangeInAquiferStorage = cumulativeResult.DataSeries.Single(x => x.Name == "Storage").DataPoints.Last().Value;
        getActionResult.TotalChangeInPumping = cumulativeResult.DataSeries.Single(x => x.Name == "Wells").DataPoints.Last().Value;
        getActionResult.TotalChangeInRecharge = cumulativeResult.DataSeries.Single(x => x.Name == "Recharge").DataPoints.Last().Value;

        var timeSeriesOutput = JsonSerializer.Deserialize<RunResult>(timeSeriesJsonString);
        var pointsOfInterest = timeSeriesOutput.ResultSets
            .Single(x => x.Name == "Points of Interest")?.DataSeries;
        var waterLevelChanges2 = new List<double>();
        foreach (var series in pointsOfInterest)
        {
            waterLevelChanges2.AddRange(series.DataPoints.Select(dataSeries => dataSeries.Value));
        }
        var averageChangeInWaterLevel = waterLevelChanges2.Average();
        getActionResult.AverageChangeInWaterLevel = averageChangeInWaterLevel;
        getActionResult.ModelRunEndDate = pointsOfInterest.First().DataPoints.Last().Date;

        var getActionResultPointOfInterests = pointsOfInterest.Select
        (x =>
        {
            var getActionResultPointOfInterest = new GetActionResultPointOfInterest()
            {
                Name = x.Name,
                GetActionResultTimeSeriesOutputs = x.DataPoints.GroupBy(y => y.Date.Year).Select(y =>
                    new GetActionResultTimeSeriesOutput()
                    {
                        Name = x.Name,
                        Date = new DateTime(y.Key,1,1),
                        Value = y.Average(z => z.Value)
                    }).ToList()
            };
            var pivotedRunWellInput = userDataJson.PivotedRunWellInputs.SingleOrDefault(y => y.Name == x.Name);
            if (pivotedRunWellInput != null)
            {
                getActionResultPointOfInterest.AverageValue = pivotedRunWellInput.AverageValue;
                getActionResultPointOfInterest.Latitude = pivotedRunWellInput.Lat;
                getActionResultPointOfInterest.Longitude = pivotedRunWellInput.Lng;
            }

            return getActionResultPointOfInterest;
        }).ToList();
        getActionResult.PointsOfInterest = getActionResultPointOfInterests;
        return getActionResult;
    }

    private async Task<string> GetFileFromBlobStorageAsJsonString(GETActionOutputFile fileResourceToDownload)
    {
        var fileResource = fileResourceToDownload.FileResource;
        return await GetFileFromBlobStorageAsJsonString(fileResource);
    }

    private async Task<string> GetFileFromBlobStorageAsJsonString(FileResource fileResource)
    {
        var timeSeriesBlob = await _fileService.GetFileStreamFromBlobStorage(fileResource.FileResourceCanonicalName);

        using var memoryStream = new MemoryStream();
        await timeSeriesBlob.CopyToAsync(memoryStream);
        var byteArray = memoryStream.ToArray();
        var jsonString = Encoding.UTF8.GetString(byteArray);
        return jsonString;
    }

    [HttpGet("/actions/{getActionID}/downloadOutputJson")]
    [SwaggerResponse(statusCode: 200, Type = typeof(FileContentResult))]
    [WithGeographyRoleFlag(FlagEnum.CanUseScenarioPlanner, true, true)]
    public async Task<IActionResult> DownloadOutputJson([FromRoute] int getActionID)
    {
        var getAction = GETActions.GetByID(_dbContext, getActionID);
        if(ThrowNotFound(getAction, "GETAction", getActionID, out var result))
        {
            return result;
        }

        // todo: handle specifying which output file to download, or should we just zip up all of them? Probably just zip up all of them

        var zipFileResult =
            await _fileService.CreateZipFileFromFileResources(getAction.GETActionOutputFiles.Select(x => x.FileResource)
                .ToList());

        return DisplayFile($"Action_${getActionID}_Output.zip", zipFileResult);


        //var iwfmBudgetGroundwaterResult = await _getService.DownloadOutputJson(getAction.GETActionID);
        //return DisplayFile($"Action_${getActionID}_Output.txt", Encoding.UTF8.GetBytes(iwfmBudgetGroundwaterResult));
    }

    private IActionResult DisplayFile(string fileName, Stream fileStream)
    {
        var contentDisposition = new System.Net.Mime.ContentDisposition
        {
            FileName = fileName,
            Inline = false
        };
        Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return File(fileStream, contentType);
    }

    private IActionResult DisplayFile(string fileName, byte[] bytes)
    {
        var contentDisposition = new System.Net.Mime.ContentDisposition
        {
            FileName = fileName,
            Inline = false
        };
        Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileName, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        return File(bytes, contentType);
    }


}