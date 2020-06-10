using System;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GraceChapelLibraryWebApp.Core.Scheduler
{
    public class ScheduleTaskPreExpiryReminder : ScheduledProcessor
    {

        public ScheduleTaskPreExpiryReminder(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        { }

        //protected override string Schedule => "*/1 * * * *";
        protected override string Schedule => "01 17 * * *"; // every day 5pm

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
            try
            {
                context.Borrower.PreExpiryReminder();
            }
            catch (Exception)
            {

                throw;
            }
            return Task.CompletedTask;
        }
    }
}
