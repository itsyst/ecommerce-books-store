using AutoMapper;
using Books.Domain.Entities;
using Books.Interfaces;
using Books.Models;
using Books.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Books.Areas.Admin.Controllers.Api
{
#pragma warning disable

    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork<OrderHeader> _orderHeader;
        private readonly IUnitOfWork<ApplicationUser> _applicationUser;
        private readonly IMapper _mapper;

        public OrdersController(
            IUnitOfWork<OrderHeader> orderHeader,
            IUnitOfWork<ApplicationUser> applicationUser,
            IMapper mapper)
        {
            _applicationUser = applicationUser;
            _orderHeader = orderHeader;
            _mapper = mapper;
        }

        // GET: /api/orders
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetOrders()
        {
            IEnumerable<OrderHeader> orderHeaderInDB;
            if (User.IsInRole(Roles.RoleType.Admin.ToString()) || User.IsInRole(Roles.RoleType.Employee.ToString()))
                orderHeaderInDB = await _orderHeader.Entity.GetAllAsync(includeProperties: o=>o.ApplicationUser);
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                orderHeaderInDB = await _orderHeader.Entity.GetAllAsync(u => u.ApplicationUserId == userId, includeProperties: o => o.ApplicationUser);

            }
            return Ok(orderHeaderInDB);
        }
    }
}
