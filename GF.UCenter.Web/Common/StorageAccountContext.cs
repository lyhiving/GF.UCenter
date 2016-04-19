namespace GF.UCenter.Web
{
    using System;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Settings;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    [Export]
    public class StorageAccountContext
    {
        private readonly CloudBlobContainer container;
        private readonly CloudBlobContainer secondaryContainer;

        [ImportingConstructor]
        public StorageAccountContext(Settings settings)
        {
            var account = CloudStorageAccount.Parse(settings.UCStorageConnectionString);
            var client = account.CreateCloudBlobClient();
            this.container = client.GetContainerReference(settings.ImageContainerName);
            this.container.CreateIfNotExistsAsync();

            if (!string.IsNullOrEmpty(settings.SecondaryStorageConnectionString))
            {
                this.secondaryContainer = CloudStorageAccount.Parse(settings.SecondaryStorageConnectionString)
                    .CreateCloudBlobClient()
                    .GetContainerReference(settings.ImageContainerName);
            }
        }

        public async Task CreateContainerIfNotExists(CancellationToken token)
        {
            await this.container.CreateIfNotExistsAsync(token);

            if (this.secondaryContainer != null)
            {
                await this.secondaryContainer.CreateIfNotExistsAsync(token);
            }
        }

        public async Task<string> UploadBlobAsync(string blobName, Stream stream, CancellationToken token)
        {
            var blob = this.container.GetBlockBlobReference(blobName);
            await blob.UploadFromStreamAsync(stream, token);

            if (this.secondaryContainer != null)
            {
                var secondBlob = this.secondaryContainer.GetBlockBlobReference(blobName);
                await this.CopyBlobAsync(blob, secondBlob, token);
            }

            return blob.Uri.AbsoluteUri;
        }

        public async Task<string> CopyBlobAsync(string sourceBlobName, string targetBlobName, CancellationToken token)
        {
            var sourceBlob = this.container.GetBlockBlobReference(sourceBlobName);
            var targetBlob = this.container.GetBlockBlobReference(targetBlobName);
            await targetBlob.StartCopyAsync(sourceBlob, token);

            return targetBlob.Uri.AbsoluteUri;
        }

        public async Task<string> CopyBlobAsync(CloudBlockBlob sourceBlob, CloudBlockBlob targetBlob, CancellationToken token)
        {
            bool tryCopy = true;
            string copyId = string.Empty;
            int retryCount = 3;
            TimeSpan timeOut = TimeSpan.FromMinutes(3);
            while (tryCopy)
            {
                if (!string.IsNullOrEmpty(copyId))
                {
                    await targetBlob.AbortCopyAsync(copyId, token);
                }

                DateTime startTime = DateTime.Now;
                copyId = await targetBlob.StartCopyAsync(sourceBlob, token);
                while ((DateTime.Now - startTime) < timeOut)
                {
                    Thread.Sleep(100);
                    await targetBlob.FetchAttributesAsync(token);
                    switch (targetBlob.CopyState.Status)
                    {
                        case CopyStatus.Invalid:
                        case CopyStatus.Pending:
                            continue;
                        case CopyStatus.Success:
                            return targetBlob.Uri.AbsoluteUri;
                        case CopyStatus.Aborted:
                        case CopyStatus.Failed:
                        default:
                            retryCount--;
                            if (retryCount == 0)
                            {
                                tryCopy = false;
                            }

                            break;
                    }
                }
            }

            throw new Exception("Copy blob failed");
        }
    }
}