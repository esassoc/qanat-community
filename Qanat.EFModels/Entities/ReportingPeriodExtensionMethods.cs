using System;
using System.Collections.Generic;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class ReportingPeriodExtensionMethods
{
    public static ReportingPeriodDto AsDto(this ReportingPeriod reportingPeriod)
    {
        var reportingPeriodDto = new ReportingPeriodDto()
        {
            ReportingPeriodID = reportingPeriod.ReportingPeriodID,
            Geography = reportingPeriod.Geography.AsSimpleDto(),
            Name = reportingPeriod.Name,
            StartDate = reportingPeriod.StartDate,
            EndDate = reportingPeriod.EndDate,
            ReadyForAccountHolders = reportingPeriod.ReadyForAccountHolders,
            CreateDate = reportingPeriod.CreateDate,
            CreateUser = reportingPeriod.CreateUser.AsSimpleDto(),
            UpdateDate = reportingPeriod.UpdateDate,
            UpdateUser = reportingPeriod.UpdateUser?.AsSimpleDto()
        };

        return reportingPeriodDto;
    }
}