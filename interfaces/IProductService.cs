using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cmdev_dotnet_api.Entities;

namespace cmdev_dotnet_api.interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> FindAll();
        Task<Product> FindById(int id);
        Task Create(Product product);
        Task Update(Product product);
        Task Delete(Product product);
        Task<IEnumerable<Product>> Search(string keyword);
    }
}