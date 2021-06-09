using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PPProject.Client.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(int productId);
        Task<HttpResponseMessage> PostProductAsync(Product product);
        Task<HttpResponseMessage> DeleteProductAsync(int productId);
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<HttpResponseMessage> UpdateProductAsync(Product product);
        /// <summary>
        /// Shows products that the user might like based on carts he discarded with
        /// those items in it.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Product>> GetProductsThatUserMightLike();
    }
}
