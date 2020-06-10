using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Models;

namespace GraceChapelLibraryWebApp.Core.Templates
{
    public interface IEmailTemplate
    {
        Task<string> MakeTemplateForMonthlyReport(ApplicationUser user, DateTime firstDay, List<Borrower> borrowedPastMonth, 
            List<Borrower> returnedPastMonth, List<Borrower> overduePastMonth);
    }
}