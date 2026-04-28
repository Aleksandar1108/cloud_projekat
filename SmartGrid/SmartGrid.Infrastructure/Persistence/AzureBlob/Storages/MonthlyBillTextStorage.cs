using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Common;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Infrastructure.Common.Options;

namespace SmartGrid.Infrastructure.Persistence.AzureBlob.Storages
{
    internal class MonthlyBillTextStorage(
        BlobServiceClient blobServiceClient,
        ILogger<MonthlyBillTextStorage> logger,
        IOptions<AzureBlobOptions> options)
        : AzureBlobStorage<MonthlyBillTextMetadata>(
            blobServiceClient.GetBlobContainerClient(options.Value.MonthlyBillsBlob),
            logger),
          IMonthlyBillTextStorage
    {
        public override string GetBlobPath(MonthlyBillTextMetadata metadata)
        {
            return $"{metadata.Year:D4}/{metadata.Month:D2}/device-{metadata.DeviceId}.pdf";
        }
    }
}
