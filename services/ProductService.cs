using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cmdev_dotnet_api.Data;
using cmdev_dotnet_api.DTOs.Product;
using cmdev_dotnet_api.Entities;
using cmdev_dotnet_api.interfaces;
using Microsoft.EntityFrameworkCore;

namespace cmdev_dotnet_api.services
{
    public class ProductService : IProductService
    {
        private readonly DatabaseContext databaseContext;
        private readonly IUploadFileService uploadFileService;

        public ProductService(DatabaseContext databaseContext,IUploadFileService uploadFileService)
        {
            this.databaseContext = databaseContext;
            this.uploadFileService = uploadFileService;
        }

        public async Task<IEnumerable<Product>> FindAll()
        {
            List<Product> products = await this.databaseContext.Products.Include(p => p.Category)
                                    .OrderByDescending(p => p.ProductId)
                                    .ToListAsync();
            return products;
        }
        public async Task<Product> FindById(int id)
        {
            var products = await databaseContext.Products.Include(p => p.Category)
                            .SingleOrDefaultAsync(p => p.ProductId == id);
            return products;
        }

        public async Task Create(Product product)
        {
            databaseContext.Products.Add(product);
            await databaseContext.SaveChangesAsync();
        }

        public async Task Update(Product product)
        {
            databaseContext.Products.Update(product);
            await databaseContext.SaveChangesAsync();
        }

        public async Task Delete(Product product)
        {
            databaseContext.Products.Remove(product);
            await databaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> Search(string keyword)
        {
            List<Product> result = await databaseContext.Products.Include(p => p.Category)
            .Where(p => p.Name.ToLower().Contains(keyword.ToLower()))
            .ToListAsync();
            return result;
        }

        public async Task<(string? errorMessage, string imageName)> UploadImages(List<IFormFile> formFiles)
        {
            string errorMessage = "";
            string imageName = "";
            if (uploadFileService.IsUpload(formFiles))
            {
                errorMessage = uploadFileService.ValidateFile(formFiles);
                if (errorMessage == null)
                {
                    imageName = (await uploadFileService.UploadImages(formFiles))[0];
                }
            }
            else
            {
                errorMessage = "The file is not upload";
            }
            return (errorMessage, imageName);
        }
    }
}