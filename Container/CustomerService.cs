using AutoMapper;
using LearnAPI.Helper;
using LearnAPI.Modal;
using LearnAPI.Repos;
using LearnAPI.Repos.Models;
using LearnAPI.Service;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LearnAPI.Container
{
    public class CustomerService : ICustomerService
    {
        private readonly LearndataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;
        public CustomerService(LearndataContext context, IMapper mapper, ILogger<CustomerService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<APIResponse> Create(CustomerModal data)
        {
            APIResponse _response = new APIResponse();
            try
            {
                _logger.LogInformation("Create Begins");
                TblCustomer customer = _mapper.Map<TblCustomer>(data);
                await _context.TblCustomers.AddAsync(customer);
                await _context.SaveChangesAsync();

                _response.ResponseCode = 200;
                _response.Result = data.Code;

            }
            catch (Exception ex)
            {

                _response.ResponseCode = 400;
                _response.ErrorMessage = ex.Message;
                _logger.LogError(ex.ToString());
            }

            return _response;
        }

        public async Task<List<CustomerModal>> GetAll()
        {
            List<CustomerModal> _response = new List<CustomerModal>();
            try
            {
                _logger.LogInformation("Get All Customers Logs Test");
                var data = await _context.TblCustomers.ToListAsync();
                if (data != null)
                {
                    _response = _mapper.Map<List<CustomerModal>>(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            return _response;
        }

        public async Task<CustomerModal> GetByCode(string code)
        {
            CustomerModal _response = new CustomerModal();
            var data = await _context.TblCustomers.FindAsync(code);
            if (data != null)
            {
                _response = _mapper.Map<CustomerModal>(data);
            }
            return _response;
        }

        public async Task<APIResponse> Remove(string code)
        {
            APIResponse _response = new APIResponse();
            try
            {
                var customer = await _context.TblCustomers.FindAsync(code);

                if (customer != null)
                {
                    _context.TblCustomers.Remove(customer);
                    await _context.SaveChangesAsync();
                    _response.ResponseCode = 200;
                    _response.Result = customer.Code;

                }
                else
                {
                    _response.ResponseCode = 404;
                    _response.ErrorMessage = "Data not found";
                }


            }
            catch (Exception ex)
            {

                _response.ResponseCode = 400;
                _response.ErrorMessage = ex.Message;
            }

            return _response;
        }

        public async Task<APIResponse> Update(CustomerModal data, string code)
        {
            APIResponse _response = new APIResponse();
            try
            {
                var customer = await _context.TblCustomers.FindAsync(code);
                if (customer != null)
                {
                    _context.Entry(customer).CurrentValues.SetValues(data);
                    await _context.SaveChangesAsync();

                    _response.ResponseCode = 200;
                    _response.Result = customer.Code;
                }
                else
                {
                    _response.ResponseCode = 404;
                    _response.ErrorMessage = "Data not found";
                }

            }
            catch (DbUpdateException ex)
            {

                _response.ResponseCode = 400;
                _response.ErrorMessage = "Error updating data" + ex.Message;
            }

            return _response;
        }
    }
}
