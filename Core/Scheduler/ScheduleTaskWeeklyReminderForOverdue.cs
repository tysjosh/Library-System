using System;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GraceChapelLibraryWebApp.Core.Scheduler
{
    public class ScheduleTaskWeeklyReminderForOverdue : ScheduledProcessor
    {

        public ScheduleTaskWeeklyReminderForOverdue(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        { }

        //protected override string Schedule => "*/1 * * * *"; //every minute
        protected override string Schedule => "01 08 * * 1"; // At 08:01 on Monday

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
            try
            {
                context.Borrower.WeeklyReminderForOverdue();
            }
            catch (Exception)
            {

                throw;
            }
            return Task.CompletedTask;
        }
    }
}
