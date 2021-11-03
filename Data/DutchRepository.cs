using DutchTreat.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Data
{
    public class DutchRepository : IDutchRepository
    {
        private readonly DutchContext _context;
        private readonly ILogger<DutchRepository> _logger;

        public DutchRepository(DutchContext dutchContext,ILogger<DutchRepository> logger)
        {
            this._context = dutchContext;
            this._logger = logger;
        }
        public IEnumerable<Order> GetAllOrdersByUser(string username)
        {
            try
            {
                _logger.LogInformation("GetAllOrders was called ");
                return _context.Orders
                                .Where(u => u.User.UserName== username)
                                .Include(o => o.Items)
                                .ThenInclude(p => p.Product)
                                .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Order GetOrderById(string username, int id)
        {
            return _context.Orders
                                .Where(u=>u.User.UserName==username)
                                .Include(o => o.Items)
                                .ThenInclude(p => p.Product)
                                .Where(o => o.Id == id)
                                .FirstOrDefault();
        }
        public IEnumerable<Order> GetAllOrders()
        {
            try
            {
                _logger.LogInformation("GetAllOrders was called ");
                return _context.Orders
                                .Include(o => o.Items)
                                .ThenInclude(p => p.Product)
                                .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("GetAllProducts was called");
                return _context.Products
                               .OrderBy(product => product.Title)
                               .ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get all products : {e}");
                return null;
            }
            
        }

        public IEnumerable<Product> GetProductsByCategory(String category)
        {
            return _context.Products
                           .Where(product => product.Category == category)
                           .ToList();
        }
        public bool SaveAll()
        {
            return _context.SaveChanges() > 0;
        }

        public void AddEntity(object model)
        {
            _context.Add(model);
        }
    }
}
