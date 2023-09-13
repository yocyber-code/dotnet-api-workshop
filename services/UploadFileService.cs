using cmdev_dotnet_api.interfaces;

namespace cmdev_dotnet_api.services
{
    public class UploadFileService : IUploadFileService
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IConfiguration configuration;
        private static readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png" };

        public UploadFileService(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.configuration = configuration;
        }

        public bool IsUpload(List<IFormFile> formFiles)
        {
            return formFiles != null && formFiles.Count > 0 && formFiles.Sum(f => f.Length) > 0;
        }

        public async Task<List<string>> UploadImages(List<IFormFile> formFiles)
        {
            List<string> fileNames = new List<string>();
            string uploadPath = $"{webHostEnvironment.WebRootPath}/images/";

            foreach (IFormFile file in formFiles)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                string fullPath = Path.Combine(uploadPath, fileName);
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                fileNames.Add(fileName);
            }
            return fileNames;
        }

        public string? ValidateFile(List<IFormFile> formFiles)
        {
            foreach (IFormFile file in formFiles)
            {
                if (!ValidationExtension(file.FileName))
                {
                    return $"The extension {Path.GetExtension(file.FileName)} is not permitted";
                }
                if (!ValidationSize(file.Length))
                {
                    return $"The file {file.FileName} {file.Length} {configuration.GetValue<long>("FileSizeLimit")} exceeds the size limit";
                }
            }
            return null;
        }

        public bool ValidationExtension(string filename)
        {
            string ext = Path.GetExtension(filename).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }
            return true;
        }

        public bool ValidationSize(long filesize)
        {

            return filesize <= configuration.GetValue<long>("FileSizeLimit");
        }
    }
}
