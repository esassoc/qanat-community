using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using System.Linq;
using System.Text.RegularExpressions;
using Qanat.Models.DataTransferObjects.Geography;

namespace Qanat.API.Services;

public class HierarchyContext
{
    private readonly QanatDbContext _dbContext;

    public HierarchyContext(QanatDbContext dbContext, HttpContext httpContext)
    {
        _dbContext = dbContext;

        var path = httpContext.Request.Path;

        GetGeographyFromPath(path);
        GetAccountFromPath(path);
        GetWellFromPath(path);
        GetWellRegistrationFromPath(path);
        GetParcelFromPath(path);
        GetUsageLocationFromPath(path);
        GetFallowStatusFromPath(path);
        GetCoverCropStatusFromPath(path);
        GetWaterAccountContactFromPath(path);
    }

    #region Geography

    /// <summary>
    /// Used for testing
    /// </summary>
    /// <param name="geographyIDFromPath"></param>
    /// <param name="geographyDisplayDto"></param>
    /// <param name="waterAccountIDFromPath"></param>
    /// <param name="waterAccountDisplayDto"></param>
    /// <param name="wellRegistrationIDFromPath"></param>
    /// <param name="wellRegistrationDto"></param>
    /// <param name="parcelIDFromPath"></param>
    /// <param name="parcelDisplayDto"></param>
    public HierarchyContext(
        int geographyIDFromPath, GeographyDisplayDto geographyDisplayDto,
        int waterAccountIDFromPath, WaterAccountDisplayDto waterAccountDisplayDto,
        int wellRegistrationIDFromPath, WellRegistrationMinimalDto wellRegistrationDto,
        int parcelIDFromPath, ParcelDisplayDto parcelDisplayDto)
    {
        GeographyIDFromPath = geographyIDFromPath;
        GeographyDisplayDto = geographyDisplayDto;
        WaterAccountIDFromPath = waterAccountIDFromPath;
        WaterAccountDisplayDto = waterAccountDisplayDto;
        WellRegistrationIDFromPath = wellRegistrationIDFromPath;
        WellRegistrationDto = wellRegistrationDto;
        ParcelIDFromPath = parcelIDFromPath;
        ParcelDisplayDto = parcelDisplayDto;
    }

    public int GeographyIDFromPath { get; set; }
    public GeographyDisplayDto GeographyDisplayDto { get; set; }
    public void GetGeographyFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/geographies\b)\/(?<geographyID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var geographyIDAsString = match.Groups["geographyID"].Value;
            var geographyIDIsInt = int.TryParse(geographyIDAsString, out var geographyID);
            if (geographyIDIsInt)
            {
                GeographyDisplayDto = Geographies.GetByIDAsDisplayDto(_dbContext, geographyID);
                GeographyIDFromPath = geographyID;
            }
        }
        else
        {
            GeographyIDFromPath = -1;
        }
    }
    #endregion

    #region WaterAccount
    public int WaterAccountIDFromPath { get; set; }
    public WaterAccountDisplayDto WaterAccountDisplayDto { get; set; }
    public void GetAccountFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/water-accounts\b)\/(?<waterAccountID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var waterAccountIDAsString = match.Groups["waterAccountID"].Value;
            var waterAccountIDIsInt = int.TryParse(waterAccountIDAsString, out var waterAccountID);
            if (waterAccountIDIsInt)
            {
                WaterAccountDisplayDto = WaterAccounts.GetByIDAsDisplayDto(_dbContext, waterAccountID);
                if (WaterAccountDisplayDto != null)
                {
                    GeographyDisplayDto = Geographies.GetByIDAsDisplayDto(_dbContext, WaterAccountDisplayDto.GeographyID);
                }
                WaterAccountIDFromPath = waterAccountID;
            }
        }
        else
        {
            WaterAccountIDFromPath = -1;
        }
    }
    #endregion

    #region Well
    public int WellIDFromPath { get; set; }
    public WellDisplayDto WellDisplayDto { get; set; }
    public void GetWellFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/wells\b)\/(?<wellID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var wellIDAsString = match.Groups["wellID"].Value;
            var wellIDIsInt = int.TryParse(wellIDAsString, out var wellID);
            if (wellIDIsInt)
            {
                WellDisplayDto = Wells.GetByIDAsDisplayDto(_dbContext, wellID);
                if (WellDisplayDto?.GeographyID != null)
                {
                    GeographyDisplayDto = Geographies.GetByIDAsDisplayDto(_dbContext, WellDisplayDto.GeographyID);
                }
                if (WellDisplayDto?.ParcelID != null)
                {
                    ParcelDisplayDto = Parcels.GetByIDAsDisplayDto(_dbContext, WellDisplayDto.ParcelID.Value);

                    if (ParcelDisplayDto?.WaterAccountID != null)
                    {
                        WaterAccountDisplayDto = WaterAccounts.GetByIDAsDisplayDto(_dbContext, ParcelDisplayDto.WaterAccountID.Value);
                    }
                }
                WellIDFromPath = wellID;
            }
        }
        else
        {
            WellIDFromPath = -1;
        }
    }
    #endregion

    #region WellRegistration
    public int WellRegistrationIDFromPath { get; set; }
    public WellRegistrationMinimalDto WellRegistrationDto { get; set; }
    public void GetWellRegistrationFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/well-registrations\b)\/(?<wellRegistrationID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var wellRegistrationIDAsString = match.Groups["wellRegistrationID"].Value;
            var wellRegistrationIDIsInt = int.TryParse(wellRegistrationIDAsString, out var wellRegistrationID);
            if (wellRegistrationIDIsInt)
            {
                WellRegistrationDto = WellRegistrations.GetByID(_dbContext, wellRegistrationID).AsMinimalDto();
                if (WellRegistrationDto?.GeographyID != null)
                {
                    GeographyDisplayDto = Geographies.GetByIDAsDisplayDto(_dbContext, WellRegistrationDto.GeographyID);
                }
                WellRegistrationIDFromPath = wellRegistrationID;
            }
        }
        else
        {
            WellRegistrationIDFromPath = -1;
        }
    }
    #endregion

    #region Parcel
    public int ParcelIDFromPath { get; set; }
    public ParcelDisplayDto ParcelDisplayDto { get; set; }
    public void GetParcelFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/parcels\b)\/(?<parcelID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var parcelIDAsString = match.Groups["parcelID"].Value;
            var parcelIDIsInt = int.TryParse(parcelIDAsString, out var parcelID);
            if (parcelIDIsInt)
            {
                var parcel = Parcels.GetByIDAsDisplayDto(_dbContext, parcelID);
                if (parcel != null)
                {
                    ParcelDisplayDto = parcel;
                    if (ParcelDisplayDto?.GeographyID != null)
                    {
                        GeographyDisplayDto = Geographies.GetByIDAsDisplayDto(_dbContext, ParcelDisplayDto.GeographyID);
                    }

                    if (ParcelDisplayDto?.WaterAccountID != null)
                    {
                        WaterAccountDisplayDto = WaterAccounts.GetByIDAsDisplayDto(_dbContext, ParcelDisplayDto.WaterAccountID.Value);
                    }
                    else
                    {
                        var defaultReportingPeriod = _dbContext.ReportingPeriods.AsNoTracking().First(x => x.GeographyID == parcel.GeographyID && x.IsDefault);
                        var waterAccountParcels = _dbContext.WaterAccountParcels.AsNoTracking()
                            .Where(x => x.ParcelID == ParcelDisplayDto.ParcelID).OrderByDescending(x => x.ReportingPeriodID).ToList();
                        var waterAccountParcel = waterAccountParcels.FirstOrDefault(x => x.ReportingPeriodID == defaultReportingPeriod.ReportingPeriodID);
                        if (waterAccountParcel != null)
                        {
                            WaterAccountDisplayDto = WaterAccounts.GetByIDAsDisplayDto(_dbContext, waterAccountParcel.WaterAccountID);
                        }
                        else if (waterAccountParcels.Any())
                        {
                            waterAccountParcel = waterAccountParcels.First();
                            WaterAccountDisplayDto = WaterAccounts.GetByIDAsDisplayDto(_dbContext, waterAccountParcel.WaterAccountID);
                        }
                    }
                }

                ParcelIDFromPath = parcelID;
            }
        }
        else
        {
            ParcelIDFromPath = -1;
        }
    }
    #endregion

    #region UsageLocation
    public int UsageLocationIDFromPath { get; set; }
    public UsageLocationHierarchyDto UsageLocationHierarchyDto { get; set; }
    public void GetUsageLocationFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/usage-locations\b)\/(?<usageLocationID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var usageLocationIDAsString = match.Groups["usageLocationID"].Value;
            var usageLocationIDIsInt = int.TryParse(usageLocationIDAsString, out var usageLocationID);
            if (usageLocationIDIsInt)
            {
                UsageLocationHierarchyDto = UsageLocations.GetHierarchyDtoByID(_dbContext, usageLocationID);
                if (UsageLocationHierarchyDto?.GeographyID != null)
                {
                    GeographyDisplayDto = Geographies.GetByIDAsDisplayDto(_dbContext, UsageLocationHierarchyDto.GeographyID);
                }

                if (UsageLocationHierarchyDto?.WaterAccountID != null)
                {
                    WaterAccountDisplayDto = WaterAccounts.GetByIDAsDisplayDto(_dbContext, UsageLocationHierarchyDto.WaterAccountID.Value);
                }

                if (UsageLocationHierarchyDto?.ParcelID != null)
                {
                    ParcelDisplayDto = Parcels.GetByIDAsDisplayDto(_dbContext, UsageLocationHierarchyDto.ParcelID);
                }

                UsageLocationIDFromPath = usageLocationID;
            }
        }
        else
        {
            UsageLocationIDFromPath = -1;
        }
    }
    #endregion

    #region Fallow Status

    public void GetFallowStatusFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/water-account-fallow-statuses\b)\/(?<fallowStatusID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var fallowStatusAsString = match.Groups["fallowStatusID"].Value;
            var fallowStatusIDIsInt = int.TryParse(fallowStatusAsString, out var fallowStatusID);
            if (fallowStatusIDIsInt)
            {
                var fallowStatus = _dbContext.WaterAccountFallowStatuses.AsNoTracking().FirstOrDefault(x => x.WaterAccountFallowStatusID == fallowStatusID);
                if (fallowStatus != null)
                {
                    WaterAccountDisplayDto = WaterAccounts.GetByIDAsDisplayDto(_dbContext, fallowStatus.WaterAccountID);
                    if (WaterAccountDisplayDto != null)
                    {
                        GeographyDisplayDto = Geographies.GetByIDAsDisplayDto(_dbContext, WaterAccountDisplayDto.GeographyID);
                    }
                    WaterAccountIDFromPath = WaterAccountDisplayDto?.WaterAccountID ?? -1;
                }
            }
        }
        else
        {
            WaterAccountIDFromPath = -1;
        }
    }

    #endregion

    #region Cover Crop Status

    public void GetCoverCropStatusFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/water-account-cover-crop-statuses\b)\/(?<CoverCropStatusID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var coverCropStatusAsString = match.Groups["CoverCropStatusID"].Value;
            var CoverCropStatusIDIsInt = int.TryParse(coverCropStatusAsString, out var coverCropID);
            if (CoverCropStatusIDIsInt)
            {
                var coverCropStatus = _dbContext.WaterAccountCoverCropStatuses.AsNoTracking().FirstOrDefault(x => x.WaterAccountCoverCropStatusID == coverCropID);
                if (coverCropStatus != null)
                {
                    WaterAccountDisplayDto = WaterAccounts.GetByIDAsDisplayDto(_dbContext, coverCropStatus.WaterAccountID);
                    if (WaterAccountDisplayDto != null)
                    {
                        GeographyDisplayDto = Geographies.GetByIDAsDisplayDto(_dbContext, WaterAccountDisplayDto.GeographyID);
                    }
                    WaterAccountIDFromPath = WaterAccountDisplayDto?.WaterAccountID ?? -1;
                }
            }
        }
        else
        {
            WaterAccountIDFromPath = -1;
        }
    }

    #endregion

    #region Water Account Contact

    public void GetWaterAccountContactFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/water-account-contacts\b)\/(?<waterAccountContactID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var waterAccountContactIDAsString = match.Groups["waterAccountContactID"].Value;
            var waterAccountContactIDIsInt = int.TryParse(waterAccountContactIDAsString, out var waterAccountContactID);
            if (waterAccountContactIDIsInt)
            {
                var waterAccountContact = _dbContext.WaterAccountContacts.AsNoTracking().SingleOrDefault(x => x.WaterAccountContactID == waterAccountContactID);
                if (waterAccountContact != null)
                {
                        GeographyDisplayDto = Geographies.GetByIDAsDisplayDto(_dbContext, waterAccountContact.GeographyID);
                }
            }
        }
    }

    #endregion
}