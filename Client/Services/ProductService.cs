using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PPProject.Client.Services
{
    public class ProductService: IProductService
    {
        private readonly HttpClient httpClient;
        private readonly PublicClient httpClientUnathorized;
        public ProductService(HttpClient httpClient, PublicClient visistorShoppingCartClient)
        {
            this.httpClient = httpClient;
            this.httpClientUnathorized = visistorShoppingCartClient;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await httpClientUnathorized.Client.GetFromJsonAsync<Category[]>("api/category");
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await httpClientUnathorized.Client.GetFromJsonAsync<Product[]>("api/products");
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            return await httpClientUnathorized.Client.GetFromJsonAsync<Product>($"api/products/{productId}");
        }

        public async Task<HttpResponseMessage> PostProductAsync(Product product)
        {
            return await httpClient.PostAsJsonAsync("api/products", product);
        }

        public async Task<HttpResponseMessage> DeleteProductAsync(int productId)
        {
            var result= await httpClient.DeleteAsync($"api/products/{productId}");
            var error = result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(error);
            return result;
        }

        public async Task<HttpResponseMessage> UpdateProductAsync(Product product)
        {
            return await httpClient.PutAsJsonAsync($"api/products/{product.ID}", product);
        }

        public async Task<IEnumerable<Product>> GetProductsThatUserMightLike()
        {
            return await httpClient.GetFromJsonAsync<Product[]>("api/products/suggestions");
        }
    }
}
