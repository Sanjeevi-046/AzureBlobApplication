using Azure.Storage.Blobs;
using AzureBlobApplication.Service;
using Microsoft.Extensions.Configuration;

namespace AzureBlobApplication
{
    class Program
    {
        static async Task Main()
        {

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string? connectionString = config["AzureBlobConnection:ConnectionString"];

            AzureBlobService blobService = new(connectionString);

            string filePathToUpload = "C://Invoices/1e6edd99-2a6b-45ce-babb-f4df140a6378.pdf";
            
            if(await blobService.UploadFileAsync(filePathToUpload))
            {
                Console.WriteLine("File uploaded successfully.");
            }
            
            string blobName = "1e6edd99-2a6b-45ce-babb-f4df140a6378.pdf";
            string downloadFilePath = "C://AzureBlobLearning/BlobDowloads";

            await blobService.DownloadFileAsync(blobName, downloadFilePath);
            Console.WriteLine("File downloaded successfully.");

            string sasToken = blobService.GenerateSasToken(blobName, 2); 
            Console.WriteLine($"SAS Token: {sasToken}");
        }
    }
}
