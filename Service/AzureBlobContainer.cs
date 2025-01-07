using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
namespace CarDealer.Services
{
    public class AzureBlobContainer
    {
        static string BlobServer { get; } = "https://stcardealerdev001.blob.core.windows.net/";

        public static async Task<(bool, string)> UploadImageAsync(BlobServiceClient blobService, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length <= 0 || string.IsNullOrWhiteSpace(imageFile.FileName))
            {
                return (false, "");
            }
            string imageExtension = Path.GetExtension(imageFile.FileName);
            if (imageExtension != ".png")
            {
                return (false, "");
            }
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(imageExtension)}";
            var carImageContainer = blobService.GetBlobContainerClient("cardealer");
            await carImageContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);
            await carImageContainer.UploadBlobAsync(uniqueFileName, imageFile.OpenReadStream());
            return (true, $"{BlobServer}cardealer/{uniqueFileName}");
        }

        public static async Task<bool> DeleteImageAsync(BlobServiceClient blobService, string imageFileName)
        {
            var carImageContainer = blobService.GetBlobContainerClient("cardealer");
            if (!await carImageContainer.ExistsAsync())
            {
                return false;
            }
            await carImageContainer.DeleteBlobIfExistsAsync(imageFileName, DeleteSnapshotsOption.IncludeSnapshots);
            return true;
        }
    }

}