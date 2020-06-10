namespace GraceChapelLibraryWebApp.Core.Services
{
   public  interface IActiveDService
    {
        bool ValidateCredentials(string username, string password);
        string FindEmail(int memberId);
    }

}
