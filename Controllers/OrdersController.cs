using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<StoreUser> _userManager;

        public OrdersController(
            IDutchRepository repository, 
            ILogger<OrdersController> logger,
            IMapper mapper,
            UserManager<StoreUser> userManager)
        {
            this._repository = repository;
            this._logger = logger;
            this._mapper = mapper;
            this._userManager = userManager;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Order>> Get()
        {
            try
            {
                var username = User.Identity.Name;
                
                var result = _repository.GetAllOrdersByUser(username);
                _logger.LogInformation($"Get Orders for {username} ");
                return Ok(_mapper.Map<IEnumerable<OrderViewModel>>(result));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get orders {e}");
                return BadRequest("Failed to get orders");
            }
        }
        [HttpGet("{id:int}")]
        public IActionResult Get(string username,int id)
        {
            try
            {
                username = User.Identity.Name;
                _logger.LogInformation($"Get Order id : {id}");
                var order = _repository.GetOrderById(username,id);

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
        public async Task<IActionResult> PostAsync([FromBody] OrderViewModel model)
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
                    var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);
                    newOrder.User = currentUser;
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
