using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PPProject.Client.Services
{
    public class SalesService : ISalesService
    {
        private readonly HttpClient httpClient;
        public SalesService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IDictionary<string, float>> GetSalePercentageByCategoryAsync()
        {
            return await httpClient.GetFromJsonAsync<IDictionary<string, float>>("api/sales/categories");
        }

        public async Task<IDictionary<DateTime, int>> GetSalesPerMonthAsync()
        {
            return await httpClient.GetFromJsonAsync<IDictionary<DateTime, int>>("api/sales/by_month");
        }

        public async Task<IDictionary<int, Tuple<string, int>>> GetMostSoldToday()
        {
            return await httpClient.GetFromJsonAsync<IDictionary<int, Tuple<string, int>>>("api/sales/most_sold/today");
        }

        public async Task<IDictionary<int, Tuple<string, int>>> GetMostSoldThisMonth()
        {
            return await httpClient.GetFromJsonAsync<IDictionary<int, Tuple<string, int>>>("api/sales/most_sold/this_month");
        }
    }
}
