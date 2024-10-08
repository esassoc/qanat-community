using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Services;
using Qanat.API.Services.GoogleDrive;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Qanat.Tests.IntegrationTests.GoogleCloud;

[TestClass]
public class GoogleDriveTests
{
    private readonly DriveService _driveService;

    public GoogleDriveTests()
    {
        var type = "service_account";
        var projectID = AssemblySteps.Configuration["googleCloudPrivateKeyID"];
        var privateKeyID = AssemblySteps.Configuration["googleCloudPrivateKeyID"];
        var privateKeyBase64 = AssemblySteps.Configuration["googleCloudPrivateKey"];
        var clientEmail = AssemblySteps.Configuration["googleClientEmail"];
        var clientID = AssemblySteps.Configuration["googleClientID"];
        var authURI = "https://accounts.google.com/o/oauth2/auth";
        var tokenURI = "https://oauth2.googleapis.com/token";
        var authProviderX509CertURL = "https://www.googleapis.com/oauth2/v1/certs";
        var clientX509CertURL = AssemblySteps.Configuration["googleClientCertURL"];
        var universeDomain = "googleapis.com";

        var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
        var privateKey = Encoding.UTF8.GetString(privateKeyBytes);
        //MK 8/29/2024 - Azure pipelines mangles the newlines in the private key need to have it be a base64 string and then replace the \\n with actual newlines. Probably a better way to solve this.
        privateKey = privateKey.Replace("\\n", "\n"); 

        var googleCredentials = new GoogleCloudConfiguration()
        {
            type = type,
            project_id = projectID,
            private_key_id = privateKeyID,
            private_key = privateKey,
            client_email = clientEmail,
            client_id = clientID,
            auth_uri = authURI,
            token_uri = tokenURI,
            auth_provider_x509_cert_url = authProviderX509CertURL,
            client_x509_cert_url = clientX509CertURL,
            universe_domain = universeDomain
        };
    
        var googleCloudConfigurationAsDeserializedJSONString = JsonSerializer.Serialize(googleCredentials);
        var googleCredential = GoogleCredential.FromJson(googleCloudConfigurationAsDeserializedJSONString).CreateScoped(DriveService.Scope.Drive);
        Assert.IsNotNull(googleCredential);

        _driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = googleCredential
        });
    }

    [TestMethod]
    public async Task CanListFileMetadata()
    {
        var files = await _driveService.ListFiles();
        Assert.IsNotNull(files);
        Assert.IsTrue(files.Files.Count > 0);
    }

    [TestMethod]
    public async Task CanListFilesAndCheckTiffBytes()
    {
        var files = await _driveService.ListFiles(5);
        var tiffFiles = files.Files.Where(f => f.MimeType == "image/tiff");
        foreach (var tiffFile in tiffFiles)
        {
            using var fileStream = new MemoryStream();
            await _driveService.Files.Get(tiffFile.Id).DownloadAsync(fileStream);

            Assert.IsNotNull(fileStream);
            Assert.AreEqual(tiffFile.Size, fileStream.Length);
            Console.WriteLine($"File ID: {tiffFile.Id}, File Name: {tiffFile.Name}, ByteCount: {fileStream.Length}");
        }
    }

    [DataRow("1XxbYDKusVOjvczSycf_-dOQcUxoghsA6")]
    [DataRow("1KtDvVgU5qPsaK0sG2S718qno0IjiTFC5")]
    [DataTestMethod]
    public async Task CanDownloadFileFromGoogleDrive(string googleDriveFileID)
    { 
        using var fileStream = new MemoryStream();
        await _driveService.Files.Get(googleDriveFileID).DownloadAsync(fileStream);
        Assert.IsNotNull(fileStream);
        Console.WriteLine($"File ID: {googleDriveFileID}, ByteCount: {fileStream.Length}");
    }
}