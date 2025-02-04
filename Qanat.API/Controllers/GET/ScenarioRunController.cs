using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.API.Services.GET;
using Qanat.API.Services.GET.IWFM;
using Qanat.API.Services.GET.ModFlow;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using Swashbuckle.AspNetCore.Annotations;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("scenario-runs")]
public class ScenarioRunController(QanatDbContext dbContext, ILogger<ScenarioRunController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser, GETService getService, FileService fileService)
    : SitkaController<ScenarioRunController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost("add-a-well")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ScenarioRunRights, RightsEnum.Create)]
    public async Task<ActionResult<ScenarioRunDto>> StartAddAWellScenarioRun([FromBody] AddAWellScenarioDto addAWellScenarioDto)
    {
        var validationMessages = Scenarios.ValidateAddAWellScenarioRun(addAWellScenarioDto);
        foreach (var message in validationMessages)
        {
            ModelState.AddModelError(message.Type, message.Message);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // store the Scenario Run in our system
        var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var scenarioRun = await ScenarioRuns.CreateNew(_dbContext, userDto.UserID, addAWellScenarioDto.ModelID, Scenario.AddaWell.ScenarioID, addAWellScenarioDto.ScenarioRunName);
       
        var userDataJsonFile = GETService.ToUserDataJsonFile(addAWellScenarioDto);
        var fileResource = await fileService.CreateFileResource(_dbContext, userDataJsonFile, userDto.UserID);
        await ScenarioRuns.AddFileResource(_dbContext, scenarioRun.ScenarioRunID, fileResource);

        // start the run!
        await getService.StartNewCustomScenarioRun(scenarioRun.ScenarioRunID, userDto.GETRunCustomerID, userDto.GETRunUserID);

        var scenarioRunDto = scenarioRun.AsSimpleDto();
        return Ok(scenarioRunDto);
    }

    [HttpPost("recharge")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ScenarioRunRights, RightsEnum.Create)]
    public async Task<ActionResult<ScenarioRunDto>> StartRechargeScenarioRun([FromBody] RechargeScenarioDto rechargeScenarioDto)
    {
        var validationMessages = Scenarios.ValidateRechargeScenarioRun(rechargeScenarioDto);
        foreach (var message in validationMessages)
        {
            ModelState.AddModelError(message.Type, message.Message);
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        // store the Scenario Run in our system
        var scenarioRun = await ScenarioRuns.CreateNew(_dbContext, userDto.UserID, rechargeScenarioDto.ModelID, Scenario.Recharge.ScenarioID, rechargeScenarioDto.ScenarioRunName);
        var userDataJsonFile = GETService.ToUserDataJsonFile(rechargeScenarioDto);
        var fileResource = await fileService.CreateFileResource(_dbContext, userDataJsonFile, userDto.UserID);
        await ScenarioRuns.AddFileResource(_dbContext, scenarioRun.ScenarioRunID, fileResource);

        // start the run!
        await getService.StartNewCustomScenarioRun(scenarioRun.ScenarioRunID, userDto.GETRunCustomerID, userDto.GETRunUserID);

        var scenarioRunDto = scenarioRun.AsSimpleDto();
        return Ok(scenarioRunDto);
    }

    [HttpGet]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ScenarioRunRights, RightsEnum.Read)]
    public ActionResult<List<ScenarioRunDto>> ListScenarioRuns()
    {
        var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var scenarioRuns = ScenarioRuns.List(_dbContext).OrderByDescending(x => x.CreateDate);

        var userIsAdmin = userDto.Flags[Flag.IsSystemAdmin.FlagName];
        if (userIsAdmin)
        {
            return Ok(scenarioRuns);
        }

        var userScenarioRuns = scenarioRuns.Where(x => x.User.UserID == userDto.UserID);
        return Ok(userScenarioRuns);
    }

    [HttpGet("{scenarioRunID}")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ScenarioRunRights, RightsEnum.Read)]
    public async Task<ActionResult<ScenarioRunDto>> GetScenarioRunByID([FromRoute] int scenarioRunID)
    {
        var scenarioRun = ScenarioRuns.GetByID(_dbContext, scenarioRunID);

        //MK 1/23/2025: Authorization checks should happen before not founds, as bad actors can get info from not founds. Probably doesn't matter much in this context but it's still good to be aware.
        if (scenarioRun != null)
        {
            var userHasAccess = await ModelUsers.CheckIfUserHasModelAccessAsync(_dbContext, scenarioRun.ModelID, callingUser);
            if (!userHasAccess)
            {
                return Forbid();
            }
        }

        if (CheckAndLogIfNotFound(scenarioRun, "ScenarioRun", scenarioRunID, out var result))
        {
            return result;
        }  

        var scenarioRunDto = scenarioRun.AsDto();
        return Ok(scenarioRunDto);
    }

    [HttpPost("{scenarioRunID}/check-status")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ScenarioRunRights, RightsEnum.Read)]
    public async Task<ActionResult<ScenarioRunDto>> CheckScenarioRunStatus([FromRoute] int scenarioRunID)
    {
        var scenarioRun = ScenarioRuns.GetByID(_dbContext, scenarioRunID);

        //MK 1/23/2025: Authorization checks should happen before not founds, as bad actors can get info from not founds. Probably doesn't matter much in this context but it's still good to be aware.
        if (scenarioRun != null)
        {
            var userHasAccess = await ModelUsers.CheckIfUserHasModelAccessAsync(_dbContext, scenarioRun.ModelID, callingUser);
            if (!userHasAccess)
            {
                return Forbid();
            }
        }

        if (CheckAndLogIfNotFound(scenarioRun, "ScenarioRun", scenarioRunID, out var result))
        {
            return result;
        }

        var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        await getService.UpdateCurrentlyRunningRunStatus(scenarioRun!.ScenarioRunID, userDto.GETRunCustomerID);
        await _dbContext.Entry(scenarioRun).ReloadAsync();

        var scenarioRunDto = scenarioRun.AsDto();
        return Ok(scenarioRunDto);
    }

    [HttpGet("{scenarioRunID}/results")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ScenarioRunRights, RightsEnum.Read)]
    public async Task<ActionResult<ScenarioRunResult>> GetBudgetGroundwaterOutput([FromRoute] int scenarioRunID)
    {
        var scenarioRun = ScenarioRuns.GetByID(_dbContext, scenarioRunID);

        //MK 1/23/2025: Authorization checks should happen before not founds, as bad actors can get info from not founds. Probably doesn't matter much in this context but it's still good to be aware.
        if (scenarioRun != null)
        {
            var userHasAccess = await ModelUsers.CheckIfUserHasModelAccessAsync(_dbContext, scenarioRun.ModelID, callingUser);
            if (!userHasAccess)
            {
                return Forbid();
            }
        }

        if (CheckAndLogIfNotFound(scenarioRun, "ScenarioRun", scenarioRunID, out var result))
        {
            return result;
        }

        var scenarioRunOutputFileTypeBudgetEnum = scenarioRun!.Model.ToEnum switch
        {
            ModelEnum.MercedWaterResourcesModel => ScenarioRunOutputFileTypeEnum.GroundWaterBudget,
            ModelEnum.KernC2VSimFGKern => ScenarioRunOutputFileTypeEnum.GroundWaterBudget,
            ModelEnum.YSGAWaterResourcesModel => ScenarioRunOutputFileTypeEnum.WaterBudget,
            _ => throw new ArgumentOutOfRangeException(
                $"Model Type {scenarioRun.Model.ModelName} does not have a water budget schema defined!")
        };

        var scenarioRunOutputFileTypeTimeSeriesEnum = scenarioRun.Model.ToEnum switch
        {
            ModelEnum.MercedWaterResourcesModel => ScenarioRunOutputFileTypeEnum.TimeSeriesData,
            ModelEnum.KernC2VSimFGKern => ScenarioRunOutputFileTypeEnum.TimeSeriesData,
            ModelEnum.YSGAWaterResourcesModel => ScenarioRunOutputFileTypeEnum.PointsofInterest,
            _ => throw new ArgumentOutOfRangeException(
                $"Model Type {scenarioRun.Model.ModelName} does not have a time series schema defined!")
        };

        var budgetFile = scenarioRun.ScenarioRunOutputFiles.SingleOrDefault(x =>x.ScenarioRunOutputFileTypeID == (int)scenarioRunOutputFileTypeBudgetEnum);
        var timeSeriesFile = scenarioRun.ScenarioRunOutputFiles.SingleOrDefault(x => x.ScenarioRunOutputFileTypeID == (int)scenarioRunOutputFileTypeTimeSeriesEnum);

        if (budgetFile == null || timeSeriesFile == null)
        {
            return NotFound();
        }

        var budgetJsonString = await GetFileFromBlobStorageAsJsonString(budgetFile);
        var timeSeriesJsonString = await GetFileFromBlobStorageAsJsonString(timeSeriesFile);

        // each model returns different types of outputs.  We need to format them to a consistent format to display to users
        ScenarioRunResult scenarioRunResult;
        switch (scenarioRun.Model.ToEnum)
        {
            case ModelEnum.MercedWaterResourcesModel:
            case ModelEnum.KernC2VSimFGKern:
                scenarioRunResult = GetScenarioRunResultForIWFM(budgetJsonString, timeSeriesJsonString);
                break;

            case ModelEnum.YSGAWaterResourcesModel:
                scenarioRunResult = await GetScenarioRunResultForYSGAWaterResourcesModel(scenarioRun, budgetJsonString, timeSeriesJsonString);
                break;

            default:
                throw new ArgumentOutOfRangeException($"Model Type {scenarioRun.Model.ModelName} does not have a time series schema defined!");
        }

        return Ok(scenarioRunResult);
    }


    [HttpGet("{scenarioRunID}/download-output-json")]
    [SwaggerResponse(statusCode: 200, Type = typeof(FileContentResult))]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ScenarioRunRights, RightsEnum.Read)]
    public async Task<IActionResult> DownloadOutputJson([FromRoute] int scenarioRunID)
    {
        var scenarioRun = ScenarioRuns.GetByID(_dbContext, scenarioRunID);

        //MK 1/23/2025: Authorization checks should happen before not founds, as bad actors can get info from not founds. Probably doesn't matter much in this context but it's still good to be aware.
        if (scenarioRun != null)
        {
            var userHasAccess = await ModelUsers.CheckIfUserHasModelAccessAsync(_dbContext, scenarioRun.ModelID, callingUser);
            if (!userHasAccess)
            {
                return Forbid();
            }
        }

        if (CheckAndLogIfNotFound(scenarioRun, "ScenarioRun", scenarioRunID, out var result))
        {
            return result;
        }

        var fileResources = scenarioRun!.ScenarioRunOutputFiles.Select(x => x.FileResource).ToList();
        var zipFileResult = await fileService.CreateZipFileFromFileResources(fileResources);
        return DisplayFile($"ScenarioRun_${scenarioRunID}_Output.zip", zipFileResult);
    }

    private static ScenarioRunResult GetScenarioRunResultForIWFM(string budgetJsonString, string timeSeriesJsonString)
    {
        var scenarioRunResult = new ScenarioRunResult();
        var budgetOutput = JsonSerializer.Deserialize<BudgetGroundwaterResult>(budgetJsonString);
        var baselineEndingStorage = budgetOutput.BaselinePeriods.Last().EndingStorage;
        var modelRunEndingStorage = budgetOutput.Periods.Last().EndingStorage;
        scenarioRunResult.TotalChangeInAquiferStorage = modelRunEndingStorage - baselineEndingStorage;
        scenarioRunResult.TotalChangeInPumping = budgetOutput.Periods.Select((t, i) => budgetOutput.BaselinePeriods[i].Pumping - t.Pumping).ToList().Sum();
        scenarioRunResult.TotalChangeInRecharge = budgetOutput.Periods.Select((t, i) => budgetOutput.BaselinePeriods[i].Recharge - t.Recharge).ToList().Sum();

        var timeSeriesOutput = JsonSerializer.Deserialize<TimeSeriesOutput>(timeSeriesJsonString);
        var waterLevelChanges = (timeSeriesOutput.PivotedRunWellInputs.Select(input => input.TimeSteps.LastOrDefault())
            .Where(timeStep => timeStep != null)
            .Select(timeStep => -timeStep.BaselineValueDifference)).ToList();

        scenarioRunResult.AverageChangeInWaterLevel = waterLevelChanges.Average();
        scenarioRunResult.ModelRunEndDate = timeSeriesOutput.PivotedRunWellInputs.First().TimeSteps.Last().DateTime;

        var scenarioRunResultPointOfInterests = timeSeriesOutput.PivotedRunWellInputs.Select(x =>
            new ScenarioRunResultPointOfInterest()
            {
                Name = x.Name,
                AverageValue = x.AverageValue,
                Latitude = x.Lat,
                Longitude = x.Lng,
                ScenarioRunResultTimeSeriesOutputs =
                    x.TimeSteps.GroupBy(y => y.DateTime.Year).Select(y =>
                        new ScenarioRunResultTimeSeriesOutput()
                        {
                            Name = x.Name,
                            Date = new DateTime(y.Key, 1, 1),
                            Value = -y.Average(z => z.BaselineValueDifference)
                        }).ToList()
            }).ToList();
        scenarioRunResult.PointsOfInterest = scenarioRunResultPointOfInterests;
        return scenarioRunResult;
    }

    private async Task<ScenarioRunResult> GetScenarioRunResultForYSGAWaterResourcesModel(ScenarioRun scenarioRun, string budgetJsonString, string timeSeriesJsonString)
    {
        var scenarioRunResult = new ScenarioRunResult();
        var userDataFileResource = scenarioRun.ScenarioRunFileResources.Single(x => x.FileResource.OriginalBaseFilename == "userdata");
        var userDataJsonString = await GetFileFromBlobStorageAsJsonString(userDataFileResource.FileResource);
        var userDataJson = JsonSerializer.Deserialize<UserDataJsonFile>(userDataJsonString);
        var budgetOutput = JsonSerializer.Deserialize<RunResult>(budgetJsonString);
        var cumulativeResult = budgetOutput.ResultSets.Single(x => x.Name == "Cumulative");
        scenarioRunResult.TotalChangeInAquiferStorage = cumulativeResult.DataSeries.Single(x => x.Name == "Storage").DataPoints.Last().Value;
        scenarioRunResult.TotalChangeInPumping = cumulativeResult.DataSeries.Single(x => x.Name == "Wells").DataPoints.Last().Value;
        scenarioRunResult.TotalChangeInRecharge = cumulativeResult.DataSeries.Single(x => x.Name == "Recharge").DataPoints.Last().Value;

        var timeSeriesOutput = JsonSerializer.Deserialize<RunResult>(timeSeriesJsonString);
        var pointsOfInterest = timeSeriesOutput.ResultSets.Single(x => x.Name == "Points of Interest")?.DataSeries;
        var waterLevelChanges2 = new List<double>();
        foreach (var series in pointsOfInterest)
        {
            waterLevelChanges2.AddRange(series.DataPoints.Select(dataSeries => dataSeries.Value));
        }

        var averageChangeInWaterLevel = waterLevelChanges2.Average();
        scenarioRunResult.AverageChangeInWaterLevel = averageChangeInWaterLevel;
        scenarioRunResult.ModelRunEndDate = pointsOfInterest.First().DataPoints.Last().Date;

        var scenarioRunResultPointOfInterests = pointsOfInterest.Select
        (x =>
        {
            var scenarioRunResultPointOfInterest = new ScenarioRunResultPointOfInterest()
            {
                Name = x.Name,
                ScenarioRunResultTimeSeriesOutputs = x.DataPoints.GroupBy(y => y.Date.Year).Select(y =>
                    new ScenarioRunResultTimeSeriesOutput()
                    {
                        Name = x.Name,
                        Date = new DateTime(y.Key, 1, 1),
                        Value = y.Average(z => z.Value)
                    }).ToList()
            };

            var pivotedRunWellInput = userDataJson.PivotedRunWellInputs.SingleOrDefault(y => y.Name == x.Name);
            if (pivotedRunWellInput != null)
            {
                scenarioRunResultPointOfInterest.AverageValue = pivotedRunWellInput.AverageValue;
                scenarioRunResultPointOfInterest.Latitude = pivotedRunWellInput.Lat;
                scenarioRunResultPointOfInterest.Longitude = pivotedRunWellInput.Lng;
            }

            return scenarioRunResultPointOfInterest;
        }).ToList();
        scenarioRunResult.PointsOfInterest = scenarioRunResultPointOfInterests;

        return scenarioRunResult;
    }

    private async Task<string> GetFileFromBlobStorageAsJsonString(ScenarioRunOutputFile fileResourceToDownload)
    {
        var fileResource = fileResourceToDownload.FileResource;
        return await GetFileFromBlobStorageAsJsonString(fileResource);
    }

    private async Task<string> GetFileFromBlobStorageAsJsonString(FileResource fileResource)
    {
        var timeSeriesBlob = await fileService.GetFileStreamFromBlobStorage(fileResource.FileResourceCanonicalName);

        using var memoryStream = new MemoryStream();
        await timeSeriesBlob.CopyToAsync(memoryStream);
        var byteArray = memoryStream.ToArray();

        var jsonString = Encoding.UTF8.GetString(byteArray);
        return jsonString;
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
}