using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.EFModels.Workflows;

public class WellRegistryWorkflowProgress
{
    public enum WellRegistryWorkflowStep
    {
        SelectAParcel,
        MapYourWell,
        ConfirmLocation,
        IrrigatedParcels,
        Contacts,
        BasicInformation,
        SupportingInformation,
        Attachments
    }

    public static WellRegistryWorkflowProgressDto GetProgress(WellRegistration wellRegistration)
    {
        return new WellRegistryWorkflowProgressDto
        {
            WellRegistrationID = wellRegistration.WellRegistrationID,
            GeographyID = wellRegistration.GeographyID,
            WellRegistrationStatus = wellRegistration.WellRegistrationStatus.AsSimpleDto(),
            Steps = Enum.GetValuesAsUnderlyingType<WellRegistryWorkflowStep>().Cast<WellRegistryWorkflowStep>()
                .ToDictionary(x => x, y => WorkflowStepComplete(wellRegistration, y))
        };
    }

    public static bool CanSubmit(QanatDbContext dbContext, WellRegistration wellRegistration)
    {
        var steps = Enum.GetValuesAsUnderlyingType<WellRegistryWorkflowStep>().Cast<WellRegistryWorkflowStep>();
        foreach (var step in steps)
        {
            var stepComplete = WorkflowStepComplete(wellRegistration, step);
            if (!stepComplete) return false;
        }

        return true;
    }

    public static bool CanDelete(QanatDbContext dbContext, WellRegistration wellRegistration, UserDto currentUser)
    {
        // can delete if the user is the owner and the well is in the draft, returned, or submitted stage?
        if (currentUser.UserGuid == wellRegistration.CreateUserGuid || currentUser.UserID == wellRegistration.CreateUserID)
        {
            var statusesThatOwnerCanDelete = new List<WellRegistrationStatusEnum>() { WellRegistrationStatusEnum.Draft, WellRegistrationStatusEnum.Returned, WellRegistrationStatusEnum.Submitted };
            return statusesThatOwnerCanDelete.Contains(wellRegistration.WellRegistrationStatus.ToEnum);
        }

        // can delete if the user is a water manager for the geography in any stage
        if (currentUser.GeographyRights.TryGetValue(wellRegistration.GeographyID, out Dictionary<string, Rights> geographyRights))
        {
            var geographyCanDelete = geographyRights[PermissionEnum.WellRights.ToString()].CanDelete;
            if (geographyCanDelete != null && geographyCanDelete.Value)
            {
                return true;
            }
        }

        // can delete if the user has global delete rights
        var globalCanDelete = currentUser.Rights[PermissionEnum.WellRights.ToString()].CanDelete;
        return globalCanDelete != null && globalCanDelete.Value;
    }

    public static bool WorkflowStepComplete(WellRegistration wellRegistration, WellRegistryWorkflowStep wellRegistryWorkflowStep)
    {
        switch (wellRegistryWorkflowStep)
        {
            case WellRegistryWorkflowStep.SelectAParcel:
                return true;
            case WellRegistryWorkflowStep.MapYourWell:
                return wellRegistration.Latitude != null 
                       && wellRegistration.Longitude != null 
                       && wellRegistration.LocationPoint != null 
                       && wellRegistration.LocationPoint4326 != null;
            case WellRegistryWorkflowStep.ConfirmLocation:
                return wellRegistration.Latitude != null
                       && wellRegistration.Longitude != null
                       && wellRegistration.LocationPoint != null
                       && wellRegistration.LocationPoint4326 != null
                       && wellRegistration.ConfirmedWellLocation;
            case WellRegistryWorkflowStep.IrrigatedParcels:
                return wellRegistration.WellRegistrationIrrigatedParcels.Any()
                        && wellRegistration.SelectedIrrigatedParcels;
            case WellRegistryWorkflowStep.Contacts:
                return wellRegistration.WellRegistrationContacts.Any(x => x.WellRegistrationContactType == WellRegistrationContactType.Landowner)
                       && wellRegistration.WellRegistrationContacts.Any(x => x.WellRegistrationContactType == WellRegistrationContactType.OwnerOperator);
            case WellRegistryWorkflowStep.BasicInformation:
                return !string.IsNullOrWhiteSpace(wellRegistration.WellName) 
                       && wellRegistration.DateDrilled != null 
                       && wellRegistration.WellRegistrationWaterUses.Any();
            case WellRegistryWorkflowStep.SupportingInformation:
                return wellRegistration.WellRegistrationMetadatum?.WellDepth != null
                       && wellRegistration.WellRegistrationMetadatum?.PumpDischargeDiameter != null
                       && wellRegistration.WellRegistrationMetadatum?.FuelType != null;
            case WellRegistryWorkflowStep.Attachments:
                return true;
            default:
                throw new ArgumentOutOfRangeException(nameof(wellRegistryWorkflowStep), wellRegistryWorkflowStep, null);
        }
    }

    public class WellRegistryWorkflowProgressDto
    {   
        public int WellRegistrationID { get; set; }
        public int GeographyID { get; set; }
        public WellRegistrationStatusSimpleDto WellRegistrationStatus { get; set; }
        public Dictionary<WellRegistryWorkflowStep, bool> Steps { get; set; }
    }
}