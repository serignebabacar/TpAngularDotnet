﻿using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Controllers
{
    [Route("api/[Controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrdersController : Controller
    {
        private readonly IDutchRepository _repository;
        private readonly ILogger<OrdersController> _logger;
        private readonly IMapper _mapper;

        public OrdersController(IDutchRepository repository, ILogger<OrdersController> logger,IMapper mapper)
        {
            this._repository = repository;
            this._logger = logger;
            this._mapper = mapper;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get()
        {
            try
            {
                _logger.LogInformation($"Get Orders");
                var result = _repository.GetAllOrders();
                return Ok(_mapper.Map<IEnumerable<OrderViewModel>>(result));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get orders {e}");
                return BadRequest("Failed to get orders");
            }
        }
        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            try
            {
                _logger.LogInformation($"Get Order id : {id}");
                var order = _repository.GetOrderById(id);

                if (order != null)
                    return Ok(_mapper.Map<Order,OrderViewModel>(order));
                else 
                    return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get order {id} : {e}");
                return BadRequest("Failed to get order ");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrderViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newOrder = _mapper.Map<OrderViewModel,Order>(model);
                    if (newOrder.OrderDate == DateTime.MinValue)
                    {
                        newOrder.OrderDate = DateTime.Now;
                    }
                    _repository.AddEntity(newOrder);
                    if (_repository.SaveAll())
                    {
                        return Created($"/api/orders/{newOrder.Id}", newOrder);
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
                    
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save a new Order {ex}");

            }
            return BadRequest("Failed to save new Order");
        }
    }
}
