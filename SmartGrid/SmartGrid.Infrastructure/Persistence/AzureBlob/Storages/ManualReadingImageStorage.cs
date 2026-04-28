using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Common;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Infrastructure.Common.Options;

namespace SmartGrid.Infrastructure.Persistence.AzureBlob.Storages
{
    internal class ManualReadingImageStorage(
        BlobServiceClient blobServiceClient,
        ILogger<ManualReadingImageStorage> logger,
        IOptions<AzureBlobOptions> options)
        : AzureBlobStorage<ManualReadingImageMetadata>(
            blobServiceClient.GetBlobContainerClient(options.Value.ManualReadingsBlob),
            logger),
          IManualReadingImageStorage
    {
        public override string GetBlobPath(ManualReadingImageMetadata metadata)
        {
            return $"{metadata.ReadingId}/{metadata.Variant}.{metadata.FileExtension}";
        }
    }
}
