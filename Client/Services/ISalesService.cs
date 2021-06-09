using PPProject.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPProject.Client.Services
{
    public interface ISalesService
    {
        /// <summary>
        /// Returns the TKEY Category and its TVALUE Percentage in unformatted percentage (0.0- 1.0)
        /// </summary>
        /// <returns></returns>
        public Task<IDictionary<string, float>> GetSalePercentageByCategoryAsync();
        /// <summary>
        /// Returns the number of sold products each month as DATETIME and INT
        /// </summary>
        /// <returns></returns>
        public Task<IDictionary<DateTime, int>> GetSalesPerMonthAsync();
        /// <summary>
        /// Gets a dictionary of products that were sold this date of the year,
        /// and how much that they were sold in desscending order
        /// </summary>
        /// <returns></returns>
        public Task<IDictionary<int, Tuple<string,int>>> GetMostSoldToday();
        /// <summary>
        /// Gets a dictionary of products that were sold this month of the year,
        /// and how much that they were sold in desscending order
        /// </summary>
        /// <returns></returns>
        public Task<IDictionary<int, Tuple<string, int>>> GetMostSoldThisMonth();
        
    }
}
