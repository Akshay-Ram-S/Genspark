using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Company.FunctionApp2;

public class Function
{
    private readonly ILogger<Function> _logger;

    public Function(ILogger<Function> logger)
    {
        _logger = logger;
    }

    [Function("Function")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "generate-sas/{blobName}")] HttpRequestData req,
        string blobName)
    {
        _logger.LogInformation("Starting SAS generation for blob: {blobName}", blobName);

        string connectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString");
        string containerName = Environment.GetEnvironmentVariable("ContainerName");
        string keyVaultUri = Environment.GetEnvironmentVariable("KeyVaultUri");

        _logger.LogInformation("ConnectionString null? {isNull}", string.IsNullOrEmpty(connectionString));
        _logger.LogInformation("ContainerName: '{containerName}'", containerName);
        _logger.LogInformation("KeyVaultUri: '{keyVaultUri}'", keyVaultUri);

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(keyVaultUri))
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("Missing required configuration settings.");
            return errorResponse;
        }

        var blobServiceClient = new BlobServiceClient(connectionString);
        var accountName = blobServiceClient.AccountName;

        // Parse AccountKey from connection string
        string accountKey = null;
        foreach (var part in connectionString.Split(';'))
        {
            if (part.StartsWith("AccountKey=", StringComparison.OrdinalIgnoreCase))
            {
                accountKey = part.Substring("AccountKey=".Length);
                break;
            }
        }

        if (string.IsNullOrEmpty(accountKey))
        {
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("AccountKey not found in connection string.");
            return errorResponse;
        }

        var credential = new StorageSharedKeyCredential(accountName, accountKey);

        // Create blob client to get blob URI
        var blobClient = blobServiceClient
            .GetBlobContainerClient(containerName)
            .GetBlobClient(blobName);

        var blobUri = blobClient.Uri;

        // Build SAS token
        DateTimeOffset expiresOn = DateTimeOffset.UtcNow.AddHours(1);
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = expiresOn
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);

        var sasQuery = sasBuilder.ToSasQueryParameters(credential);
        var sasUri = new Uri($"{blobUri}?{sasQuery}");

        // Optional: store SAS in Key Vault
        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        string secretName = "myblobakshay";

        var secretToStore = new KeyVaultSecret(secretName, sasUri.ToString())
        {
            Properties = { Tags = { { "ExpiresOn", expiresOn.UtcDateTime.ToString("o") } } }
        };

        await secretClient.SetSecretAsync(secretToStore);

        // Return SAS URL
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            sasUrl = sasUri.ToString(),
            expiresOn
        });

        return response;
    }
}
