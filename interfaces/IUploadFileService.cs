namespace cmdev_dotnet_api.interfaces
{
    public interface IUploadFileService
    {
        bool IsUpload(List<IFormFile> formFiles);
        string ValidateFile(List<IFormFile> formFiles);
        Task<List<string>> UploadImages(List<IFormFile> formFiles);
 
    }
}
