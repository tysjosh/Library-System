using System;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GraceChapelLibraryWebApp.Core.Scheduler
{
    public class ScheduleTaskChangeBorrowerStatus : ScheduledProcessor
    {
        
        public ScheduleTaskChangeBorrowerStatus(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {}

        protected override string Schedule => "*/2 * * * *"; // 0 0 12 * * ?	Every day at noon - 12pm

        public override Task ProcessInScope(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
            try
            {
                context.Borrower.UpdateStatusToExpired();
            }
            catch (Exception)
            {

                throw;
            }
            return Task.CompletedTask;
        }
    }
}
