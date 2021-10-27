﻿using DutchTreat.Data.Entities;
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
        public IEnumerable<Product> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("GETALLProducts was called");
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
        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }
    }
}
