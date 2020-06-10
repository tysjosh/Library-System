using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Models;
using Microsoft.AspNetCore.Hosting;

namespace GraceChapelLibraryWebApp.Core.Templates
{
    public class EmailTemplate: IEmailTemplate
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        
        
        public EmailTemplate(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
       
        public async Task<string> MakeTemplateForMonthlyReport(ApplicationUser user, DateTime firstDay, List<Borrower> borrowedPastMonth, List<Borrower> returnedPastMonth, List<Borrower> overduePastMonth)
        {
            
            var mailBody = new StringBuilder();
            var pathToFile = _hostingEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString() + "Templates"
                + Path.DirectorySeparatorChar.ToString() + "Email" + Path.DirectorySeparatorChar.ToString() + "MonthlyReport.html";

            using (StreamReader streamReader = File.OpenText(pathToFile))
            {
                mailBody.Append(streamReader.ReadToEnd());
            }

            if (borrowedPastMonth.Count > 0)
            {
                var sectionContentFornBorrow = _prepareSectionTemplate(borrowedPastMonth);
                mailBody.Replace("{{tabledata-borrow-section}}", sectionContentFornBorrow);
            }
            else {
                mailBody.Replace("{{tabledata-borrow-section}}", "");
            }
            if (overduePastMonth.Count > 0)
            {
                var sectionContentFornOverDue = _prepareSectionTemplate(overduePastMonth);
                mailBody.Replace("{{tabledata-overdue-section}}", sectionContentFornOverDue);
            }
            else
            {
                mailBody.Replace("{{tabledata-overdue-section}}", "");
            }

            if (returnedPastMonth.Count > 0)
            {
                var sectionContentFornReturned = _prepareSectionTemplate(returnedPastMonth);
                mailBody.Replace("{{tabledata-return-section}}", sectionContentFornReturned);
            }
            else
            {
                mailBody.Replace("{{tabledata-return-section}}", "");
            }
            mailBody.Replace("{{admin-name}}", user.FullName);
            var reportingMonth = firstDay.ToString("MMMM") + " , " + firstDay.Year;
            mailBody.Replace("{{reporting-month}}", reportingMonth);
            mailBody.Replace("{{total-borrowed}}", borrowedPastMonth.Count.ToString());
            mailBody.Replace("{{total-returned}}", returnedPastMonth.Count.ToString());
            mailBody.Replace("{{total-overdue}}", overduePastMonth.Count.ToString());
            var messageMonthlyReport = mailBody.ToString();
            return messageMonthlyReport;
        }

        private string _prepareSectionTemplate(List<Borrower> borrowers) {

            string tabledata = "";
            borrowers.ForEach(b => {
                tabledata = tabledata + "<tr style='box-sizing: border-box;'>"
                                        + $"<td style = 'box-sizing:border-box;opacity:0.8;padding:16px;'>{b.MemberId}</td>"
                                        + $"<td style = 'box-sizing:border-box;opacity:0.8;padding:16px;'>{b.FullName}</td>"
                                        + $"<td style = 'box-sizing:border-box;opacity:0.8;padding:16px;'>{b.Email}</td>"
                                        + $"<td style = 'box-sizing:border-box;opacity:0.8;padding:16px;'>{b.Book.Title}</td>"
                                        + $"</tr>";
            });
            return tabledata;
        }
    }
}
