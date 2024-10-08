using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.OpenET;
using Qanat.EFModels.Entities;

namespace Qanat.API.Hangfire;

public class OpenETSyncJob : ScheduledBackgroundJobBase<OpenETSyncJob>
{
    private readonly OpenETSyncService _openETSyncService;

    public OpenETSyncJob(ILogger<OpenETSyncJob> logger, IWebHostEnvironment webHostEnvironment, QanatDbContext qanatDbContext, IOptions<QanatConfiguration> qanatConfiguration, OpenETSyncService openETSyncService, SitkaSmtpClientService sitkaSmtpClientService)
        : base(JobName, logger, webHostEnvironment, qanatDbContext, qanatConfiguration, sitkaSmtpClientService)
    {
        _openETSyncService = openETSyncService;
    }

    public override List<RunEnvironment> RunEnvironments => new()
    {
        //RunEnvironment.Development, 
        RunEnvironment.Production
    };

    public const string JobName = "OpenET Sync";

    protected override async void RunJobImplementation()
    {
        // we need to create any missing OpenETSync year month combos for each geography, if they have OpenETSync turned on
        var today = DateTime.Today;
        var currentYear = today.Year;
        var geographies = DbContext.Geographies.Include(x => x.OpenETSyncs).ToList().Where(x => x.IsOpenETActive).ToList();
        var newOpenETSyncs = new List<OpenETSync>();
        foreach (var geography in geographies)
        {
            var existingOpenETSyncs = geography.OpenETSyncs.ToDictionary(x => $"{x.Year}_{x.Month}_{x.OpenETDataTypeID}");
            for (var year = geography.StartYear; year <= currentYear; year++)
            {
                var finalMonth = year == currentYear ? today.Month - 1 : 12;
                for (var month = 1; month <= finalMonth; month++)
                {
                    foreach (var openETDataType in OpenETDataType.All)
                    {
                        var openETDataTypeID = openETDataType.OpenETDataTypeID;
                        if (!existingOpenETSyncs.ContainsKey($"{year}_{month}_{openETDataTypeID}"))
                        {
                            var openETSync = new OpenETSync()
                            {
                                GeographyID = geography.GeographyID,
                                Year = year,
                                Month = month,
                                OpenETDataTypeID = openETDataTypeID
                            };
                            newOpenETSyncs.Add(openETSync);
                        }
                    }
                }
            }
        }

        if (newOpenETSyncs.Any())
        {
            await DbContext.OpenETSyncs.AddRangeAsync(newOpenETSyncs);
            await DbContext.SaveChangesAsync();
        }

        // 12/11/23 SMG - Commented out the part that actually runs the OpenET Sync. We still want the months and years to be populated automatically
        // todo: uncomment when this functionality is ready
        //var openETSyncs = DbContext.OpenETSyncs.Include(x => x.Geography).ThenInclude(x => x.GeographyBoundary).AsNoTracking().Where(x => !x.FinalizeDate.HasValue);
        //if (!openETSyncs.Any())
        //{
        //    return;
        //}

        //foreach (var openETSync in openETSyncs)
        //{
        //    await _openETService.TriggerOpenETGoogleBucketRefresh(openETSync.Geography, openETSync.Year, openETSync.Month, openETSync.OpenETDataTypeID);
        //    Thread.Sleep(1000); // intentional sleep here to make sure we don't hit the maximum rate limit

        // trigger calc dependencies
        //}
    }
}