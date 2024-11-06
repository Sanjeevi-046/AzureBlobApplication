// ***********************************************************************************************
//
//  (c) Copyright 2024, Sanjeevi 
//
//  This software is licensed under a commercial license agreement. For the full copyright and
//  license information, please contact CTG for more information.
//
//  Description: Azure blob storage sample console application. 
//
// ***********************************************************************************************

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureBlobApplication.Service
{
    public class AzureBlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName = "invoices";

        public AzureBlobService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> UploadFileAsync(string filePath)
        {
            try
            {
                BlobServiceClient blobServiceClient = new(_connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();
                BlobClient blobClient = containerClient.GetBlobClient(Path.GetFileName(filePath));

                using FileStream uploadFileStream = File.OpenRead(filePath);
                await blobClient.UploadAsync(uploadFileStream, true);
                uploadFileStream.Close();
                return true;
            }
            catch (Exception ex) 
            { 
                return false;
            }  
        }

        public async Task<bool> DownloadFileAsync(string blobName, string downloadFilePath)
        {
            try
            {
                BlobServiceClient blobServiceClient = new(_connectionString);
                BlobContainerClient containerClient = blobServiceClient
                    .GetBlobContainerClient(_containerName);
                BlobClient blobClient = containerClient
                    .GetBlobClient(blobName);
                if (!Directory.Exists(downloadFilePath))
                {
                    Directory.CreateDirectory(downloadFilePath);
                }
                string localFilePath = Path.Combine(downloadFilePath, blobName);

                BlobDownloadInfo download = await blobClient.DownloadAsync();

                using FileStream downloadFileStream = File.OpenWrite(localFilePath);
                await download.Content.CopyToAsync(downloadFileStream);
                downloadFileStream.Close();
                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }

        public string GenerateSasToken(string blobName, int expiryTimeInMinutes)
        {
            try
            {
                BlobServiceClient blobServiceClient = new(_connectionString);
                BlobContainerClient containerClient = blobServiceClient
                    .GetBlobContainerClient(_containerName);
                BlobClient blobClient = containerClient
                    .GetBlobClient(blobName);

                BlobSasBuilder sasBuilder = new()
                {
                    BlobContainerName = _containerName,
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow
                    .AddMinutes(expiryTimeInMinutes)
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }
            catch (Exception ex) 
            {
                return string.Empty;
            }
        }
    }
}

