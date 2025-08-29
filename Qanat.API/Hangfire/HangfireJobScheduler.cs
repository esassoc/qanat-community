using Hangfire;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Qanat.API.Hangfire
{
    public class HangfireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            var recurringJobIds = new List<string>();

            AddRecurringJob<OpenETSyncJob>(OpenETSyncJob.JobName, x => x.RunJob(JobCancellationToken.Null), Cron.Monthly(8, 1, 00), recurringJobIds);
            AddRecurringJob<MonitoringWellCNRAUpdateJob>(MonitoringWellCNRAUpdateJob.JobName, x => x.RunJob(JobCancellationToken.Null), Cron.Monthly(9, 1, 00), recurringJobIds);

            // Remove any jobs we haven't explicitly scheduled
            RemoveExtraneousJobs(recurringJobIds);
        }

        public static void EnqueueRecurringJob(string jobName)
        {
            RecurringJob.TriggerJob(jobName);
        }

        private static void AddRecurringJob<T>(string jobName, Expression<Action<T>> methodCallExpression, string cronExpression, ICollection<string> recurringJobIds)
        {
            RecurringJob.AddOrUpdate<T>(jobName, methodCallExpression, cronExpression);
            recurringJobIds.Add(jobName);
        }

        private static void RemoveExtraneousJobs(List<string> recurringJobIds)
        {
            using var connection = JobStorage.Current.GetConnection();
            var recurringJobs = connection.GetRecurringJobs();
            var jobsToRemove = recurringJobs.Where(x => !recurringJobIds.Contains(x.Id)).ToList();
            foreach (var job in jobsToRemove)
            {
                RecurringJob.RemoveIfExists(job.Id);
            }
        }
    }
}
