using System;
using System.DirectoryServices.AccountManagement;


namespace GraceChapelLibraryWebApp.Core.Services
{
    public class ActiveDService : IActiveDService
    {
        public string FindEmail(int memberId)
        {
            var empEmail = "";

            using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain))
            {
                UserPrincipal searchTemplate = new UserPrincipal(principalContext)
                {
                    EmployeeId = memberId.ToString()
                };

                PrincipalSearcher ps = new PrincipalSearcher(searchTemplate);
                UserPrincipal foundUser = (UserPrincipal)ps.FindOne();
                if (foundUser != null) 
                    empEmail = foundUser.EmailAddress;
            }
            return empEmail;
        }

        public bool ValidateCredentials(string username, string password)
        {
            try
            {
                using (PrincipalContext principalContext = new PrincipalContext(ContextType.Domain))
                {
                    return principalContext.ValidateCredentials(username, password);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
