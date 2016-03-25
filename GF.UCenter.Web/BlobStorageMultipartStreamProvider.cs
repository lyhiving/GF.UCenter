using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NLog;

namespace GF.UCenter.Web
{
    public class BlobStorageMultipartStreamProvider : MultipartStreamProvider
    {
        private readonly string _fileName;

        public string BlobUrl { get; private set; }

        public BlobStorageMultipartStreamProvider(string fileName)
        {
            _fileName = fileName;
        }

        public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
        {
            Stream stream = null;

            if (!string.IsNullOrWhiteSpace(_fileName))
            {
                string connectionString = @"DefaultEndpointsProtocol=http;AccountName=ucstormagewestus;AccountKey=a4ahcg9gTTdvw6GLKAir+qp/ThVlASxcUjjwgksXqge39z1v7NL9LmIHzvRpRRsXEGQNVQM2vLNzhEGGj5HbDw==";
                string container = "images";
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var blobContainer = blobClient.GetContainerReference(container);
                var blob = blobContainer.GetBlockBlobReference(_fileName);
                BlobUrl = blob.Uri.AbsoluteUri;
                stream = blob.OpenWrite();
            }
            return stream;
        }
    }
}