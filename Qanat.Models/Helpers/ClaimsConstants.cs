namespace Qanat.Models.Helpers
{
    public static class ClaimsConstants
    {
        public static string Sub = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"; 
        public static string Emails = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        public static string FamilyName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
        public static string GivenName = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        public static string ClientID = "azp";
        public static string IsClient = "azpacr";
    }
}
