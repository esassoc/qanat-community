using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class WaterAccountUserExtensionMethods
{
    public static WaterAccountUserMinimalDto AsWaterAccountUserMinimalDto(this WaterAccountUser waterAccountUser)
    {
        var dto = new WaterAccountUserMinimalDto()
        {
            WaterAccountUserID = waterAccountUser.WaterAccountUserID,
            UserID = waterAccountUser.UserID,
            UserFullName = waterAccountUser.User.FullName,
            WaterAccountID = waterAccountUser.WaterAccountID,
            WaterAccountRoleID = waterAccountUser.WaterAccountRoleID,
            ClaimDate = waterAccountUser.ClaimDate,
            User = waterAccountUser.User.AsUserWithFullNameDto(),
            WaterAccountRole = waterAccountUser.WaterAccountRole.AsSimpleDto(),
            WaterAccount = waterAccountUser.WaterAccount.AsWaterAccountMinimalDto(),
            UserEmail = waterAccountUser.User.Email
        };
        return dto;
    }

    public static List<OnboardWaterAccountPINDto> GetOnboardWaterAccountPINDtos(QanatDbContext dbContext, int geographyID, int userID)
    {
        var waterAccountUserStagings = dbContext.WaterAccountUserStagings
            .Include(x => x.WaterAccount)
            .ThenInclude(x => x.Parcels)
            .Where(x => x.WaterAccount.GeographyID == geographyID && x.UserID == userID).ToList();

        var returnDtos = waterAccountUserStagings.Select(x => new OnboardWaterAccountPINDto()
        {
            WaterAccountPIN = x.WaterAccount.WaterAccountPIN,
            WaterAccountNumber = x.WaterAccount.WaterAccountNumber,
            Parcels = x.WaterAccount.Parcels?.Select(y => y.AsParcelMinimalDto()).ToList()
        }).ToList();
      
        return returnDtos;
    }
}