using System;

namespace Qanat.API.Services;

public class QanatConfiguration
{
    public string DB_CONNECTION_STRING { get; set; }
    public string SITKA_EMAIL_REDIRECT { get; set; }
    public string PlatformLongName { get; set; }
    public string SupportEmail { get; set; }
    public string SendGridApiKey { get; set; }
    public AzureADB2CConfiguration AzureAdB2C { get; set; }
    public OpenETConfiguration OpenET { get; set; }
    public GoogleCloudConfiguration GoogleCloud { get; set; }
    public string CNRAFeatureServerBaseUrl { get; set; }
    public string AdminClientFlowClientID { get; set; }
    public string InactiveClientFlowClientID { get; set; }
    public string AzureBlobStorageConnectionString { get; set; }
    public int GETRunCustomerID { get; set; }
    public int GETRunUserID { get; set; }
    public string GETAPIBaseURL { get; set; }
    public string GETAPISubscriptionKey { get; set; }
    public string HostName { get; set; }
    public string YoloWRIDAPIBaseUrl { get; set; }
    public string YoloWRIDAPIUsername { get; set; }
    public string YoloWRIDAPIPassword { get; set; }
    public string GDALAPIBaseUrl { get; set; }
}

public class OpenETConfiguration
{
    public string ApiKey{ get; set; }
    public string ApiBaseUrl { get; set; }
}

public class AzureADB2CConfiguration
{
    public string Instance { get; set; }
    public string ClientId { get; set; }
    public string Domain { get; set; }
    public string SignUpSignInPolicyId { get; set; }
    public string EditProfilePolicyId { get; set; }
    public string[] Scopes { get; set; }
}

public class GoogleCloudConfiguration: ICloneable
{
    public string type { get; set; }
    public string project_id { get; set; }
    public string private_key_id { get; set; }
    public string private_key { get; set; }
    public string client_email { get; set; }
    public string client_id { get; set; }
    public string auth_uri { get; set; }
    public string token_uri { get; set; }
    public string auth_provider_x509_cert_url { get; set; }
    public string client_x509_cert_url { get; set; }
    public string universe_domain { get; set; }

    public object Clone()
    {
        return MemberwiseClone();
    }
}