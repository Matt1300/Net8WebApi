using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using LearnAPI.Modal;
using LearnAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Data;

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
        private readonly IWebHostEnvironment _environment;
        public CustomerController(ICustomerService customerService, IWebHostEnvironment environment)
        {
            _customerService = customerService;
            _environment = environment;
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

        [AllowAnonymous]
        [HttpGet("ExportExcel")]
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                string Filepath = GetFilepath();
                string excelpath = Filepath + "\\customerinfo.xlsx";

                var dt = new System.Data.DataTable();
                dt.Columns.Add("Code", typeof(string));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Email", typeof(string));
                dt.Columns.Add("Phone", typeof(string));
                dt.Columns.Add("CreditLimit", typeof(int));

                var data = await _customerService.GetAll();
                if (data != null && data.Count > 0)
                {
                    data.ForEach(x =>
                    {
                        dt.Rows.Add(x.Code, x.Name, x.Email, x.Phone, x.Creditlimit);
                    });
                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.AddWorksheet(dt,"Customer Info");
                    using (MemoryStream ms = new MemoryStream())
                    {
                        wb.SaveAs(ms);

                        if (System.IO.File.Exists(excelpath))
                        {
                            System.IO.File.Delete(excelpath);
                        }
                        wb.SaveAs(excelpath);

                        return File(ms.ToArray(),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","Customer.xlsx");
                    }
                }

            }
            catch (Exception ex)
            {

                return NotFound();
            }
        }

        [NonAction]
        private string GetFilepath()
        {
            return _environment.WebRootPath + "\\Export";
        }
    }
}
