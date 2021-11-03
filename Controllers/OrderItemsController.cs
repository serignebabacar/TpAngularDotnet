using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DutchTreat.Controllers
{
    [Route("/api/orders/{orderid}/items")]
    [Authorize]
    public class OrderItemsController:Controller
    {
        private readonly IDutchRepository _repository;
        private readonly ILogger<OrderItemsController> _logger;
        private readonly IMapper _mapper;

        public OrderItemsController(IDutchRepository repository,ILogger<OrderItemsController> logger,IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get(int orderId)
        {
            try
            {
                var order = _repository.GetOrderById(orderId);
                if (order != null)
                {
                    return Ok(_mapper.Map<IEnumerable<OrderItem>, IEnumerable<OrderItemViewModel>>(order.Items));
                }
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get OrderItem for id : {orderId} : {e}");
                return BadRequest($"Failed to get OrderItem for id :{ orderId}");
            }
        }
        [HttpGet("{id}")]
        public IActionResult Get(int orderId ,int id)
        {
            try
            {
                var order = _repository.GetOrderById(orderId);
                if (order != null)
                {
                    var item = order.Items.Where(i => i.Id == id).FirstOrDefault();
                    if (item != null)
                    {
                        return Ok(_mapper.Map<OrderItem, OrderItemViewModel>(item));
                    }
                }
                    return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get OrderItemId : {id} : {e}");
                return BadRequest($"Failed to get OrderItemId :{ id}");
            }
        }

    }
}
