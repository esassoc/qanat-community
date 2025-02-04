using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.Geography;

namespace Qanat.API.Services;

public class HierarchyContext
{
    private QanatDbContext _dbContext;

    public HierarchyContext(QanatDbContext dbContext, HttpContext httpContext)
    {
        _dbContext = dbContext;

        var path = httpContext.Request.Path;

        GetGeographyFromPath(path);
        GetAccountFromPath(path);
        GetWellFromPath(path);
        GetWellRegistrationFromPath(path);
        GetParcelFromPath(path);
        GetUsageEntityFromPath(path);
    }

    #region Geography

    /// <summary>
    /// Used for testing
    /// </summary>
    /// <param name="geographyIDFromPath"></param>
    /// <param name="geographyDto"></param>
    /// <param name="waterAccountIDFromPath"></param>
    /// <param name="waterAccountDto"></param>
    /// <param name="wellRegistrationIDFromPath"></param>
    /// <param name="wellRegistrationDto"></param>
    /// <param name="parcelIDFromPath"></param>
    /// <param name="parcelDto"></param>
    public HierarchyContext(
        int geographyIDFromPath, GeographyDto geographyDto, 
        int waterAccountIDFromPath, WaterAccountDto waterAccountDto, 
        int wellRegistrationIDFromPath, WellRegistrationMinimalDto wellRegistrationDto, 
        int parcelIDFromPath, ParcelMinimalDto parcelDto)
    {
        GeographyIDFromPath = geographyIDFromPath;
        GeographyDto = geographyDto;
        WaterAccountIDFromPath = waterAccountIDFromPath;
        WaterAccountDto = waterAccountDto;
        WellRegistrationIDFromPath = wellRegistrationIDFromPath;
        WellRegistrationDto = wellRegistrationDto;
        ParcelIDFromPath = parcelIDFromPath;
        ParcelDto = parcelDto;
    }

    public int GeographyIDFromPath { get; set; }
    public GeographyDto GeographyDto { get; set; }
    public void GetGeographyFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/geographies\b)\/(?<geographyID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var geographyIDAsString = match.Groups["geographyID"].Value;
            var geographyIDIsInt = int.TryParse(geographyIDAsString, out var geographyID);
            if (geographyIDIsInt)
            {
                GeographyDto = Geographies.GetByIDAsDto(_dbContext, geographyID);
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
    public WaterAccountDto WaterAccountDto { get; set; }
    public void GetAccountFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/water-accounts\b)\/(?<waterAccountID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var waterAccountIDAsString = match.Groups["waterAccountID"].Value;
            var waterAccountIDIsInt = int.TryParse(waterAccountIDAsString, out var waterAccountID);
            if (waterAccountIDIsInt)
            {
                WaterAccountDto = WaterAccounts.GetByIDAsDto(_dbContext, waterAccountID);
                if (WaterAccountDto?.Geography != null)
                {
                    GeographyDto = Geographies.GetByIDAsDto(_dbContext, WaterAccountDto.Geography.GeographyID);
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
    public WellMinimalDto WellDto { get; set; }
    public void GetWellFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/wells\b)\/(?<wellID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var wellIDAsString = match.Groups["wellID"].Value;
            var wellIDIsInt = int.TryParse(wellIDAsString, out var wellID);
            if (wellIDIsInt)
            {
                WellDto = Wells.GetByID(_dbContext, wellID).AsMinimalDto();
                if (WellDto?.GeographyID != null)
                {
                    GeographyDto = Geographies.GetByIDAsDto(_dbContext, WellDto.GeographyID);
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
                    GeographyDto = Geographies.GetByIDAsDto(_dbContext, WellRegistrationDto.GeographyID);
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
    public ParcelMinimalDto ParcelDto { get; set; }
    public void GetParcelFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/parcels\b)\/(?<parcelID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var parcelIDAsString = match.Groups["parcelID"].Value;
            var parcelIDIsInt = int.TryParse(parcelIDAsString, out var parcelID);
            if (parcelIDIsInt)
            {
                ParcelDto = Parcels.GetByID(_dbContext, parcelID).AsParcelMinimalDto();
                if (ParcelDto?.GeographyID != null)
                {
                    GeographyDto = Geographies.GetByIDAsDto(_dbContext, ParcelDto.GeographyID);
                }

                if (ParcelDto?.WaterAccount != null)
                {
                    WaterAccountDto = WaterAccounts.GetByIDAsDto(_dbContext, ParcelDto.WaterAccount.WaterAccountID);
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
    
    #region UsageEntity
    public int UsageEntityIDFromPath { get; set; }
    public UsageEntityHierarchyDto UsageEntityHierarchyDto { get; set; }
    public void GetUsageEntityFromPath(PathString requestPath)
    {
        var match = new Regex(@"(?:\/usage-entities\b)\/(?<usageEntityID>[\d]+)", RegexOptions.IgnoreCase).Match(requestPath);
        if (match.Success)
        {
            var usageEntityIdAsString = match.Groups["usageEntityID"].Value;
            var parcelIDIsInt = int.TryParse(usageEntityIdAsString, out var usageEntityID);
            if (parcelIDIsInt)
            {
                UsageEntityHierarchyDto = UsageEntities.GetHierarchyDtoByID(_dbContext, usageEntityID);
                if (UsageEntityHierarchyDto?.GeographyID != null)
                {
                    GeographyDto = Geographies.GetByIDAsDto(_dbContext, UsageEntityHierarchyDto.GeographyID);
                }

                if (UsageEntityHierarchyDto?.WaterAccountID != null)
                {
                    WaterAccountDto = WaterAccounts.GetByIDAsDto(_dbContext, UsageEntityHierarchyDto.WaterAccountID.Value);
                }

                UsageEntityIDFromPath = usageEntityID;
            }
        }
        else
        {
            UsageEntityIDFromPath = -1;
        }
    }
    #endregion
}