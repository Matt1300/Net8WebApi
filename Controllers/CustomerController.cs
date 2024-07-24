using LearnAPI.Modal;
using LearnAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LearnAPI.Controllers
{
    [Authorize]
    [EnableCors]
    [EnableRateLimiting("fixedwindow")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _customerService.GetAll();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [DisableRateLimiting]
        [HttpGet("GetByCode")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var data = await _customerService.GetByCode(code);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(CustomerModal customer)
        {
            var data = await _customerService.Create(customer);
            return Ok(data);
        }

        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(CustomerModal customer, string code)
        {
            var data = await _customerService.Update(customer, code);
            return Ok(data);
        }

        [HttpDelete("RemoveCustomer")]
        public async Task<IActionResult> RemoveCustomer(string code)
        {
            var data = await _customerService.Remove(code);
            return Ok(data);
        }
    }
}
