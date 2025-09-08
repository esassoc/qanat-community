using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Collections.Generic;
using System.Linq;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class AccountReconciliationController : SitkaController<AccountReconciliationController>
{
    public AccountReconciliationController(QanatDbContext dbContext, ILogger<AccountReconciliationController> logger, IOptions<QanatConfiguration> qanatConfiguration) : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("/account-reconciliations")]
    [WithRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<WaterAccountReconciliationCustomDto>> ListAllAccounts()
    {
        var accountParcels = _dbContext.WaterAccountParcels.ToList();
        var accountReconciliationDtos = _dbContext.WaterAccountReconciliations
            .Include(x => x.Parcel)
            .Include(x => x.WaterAccount)
            .ThenInclude(x => x.Geography)
            .ToList()
            .GroupBy(x => x.ParcelID)
            .Select(x =>
            {
                return new WaterAccountReconciliationCustomDto()
                {
                    Parcel = x.First().Parcel.AsParcelMinimalDto(),
                    LastKnownOwner = GetLastOwnerOfParcelByParcelID(x.Key, accountParcels),
                    AccountsClaimingOwnership = x.Select(y => y.WaterAccount.AsWaterAccountMinimalDto()).ToList()
                };
            }).ToList();
        return accountReconciliationDtos;
    }

    private static WaterAccountMinimalDto GetLastOwnerOfParcelByParcelID(int parcelID, List<WaterAccountParcel> waterAccountParcels)
    {
        return waterAccountParcels
            .Where(x => x.ParcelID == parcelID)?.MaxBy(x => x.EffectiveYear)
            ?.WaterAccount?.AsWaterAccountMinimalDto();
    }
}