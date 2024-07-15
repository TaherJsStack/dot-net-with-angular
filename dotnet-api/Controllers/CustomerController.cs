using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetAPI.Data;
using dotnetAPI.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using dotnetAPI.Helper;
using dotnetAPI.Migrations;

namespace dotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly IStringLocalizer<CustomerController> _localizer;

        public CustomerController(ECommerceContext context, IStringLocalizer<CustomerController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        /*
        [HttpGet(Name = "GetCustomers")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(int pageNumber = 1, int pageSize = 10)
        {
            var totalData = await _context.Customers.CountAsync();
            var totalPages = (int)Math.Ceiling(totalData / (double)pageSize);

            if (pageNumber < 1 || pageNumber > totalPages)
            {
                return BadRequest("Invalid page number");
            }

            var customers = await _context.Customers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new BaseResult<Customer>
            {
                Success = true,
                Errors = new List<string>(),
                Message = _localizer["CustomersRetrievedSuccessfully"],
                Status = StatusCodes.Status200OK,
                Data = customers
            });
        }
        */

        [HttpGet(Name = "GetCustomers")]
        public async Task<ActionResult<BaseResult<IEnumerable<Customer>>>> GetCustomers(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalData = await _context.Customers.CountAsync();
                var totalPages = (int)Math.Ceiling(totalData / (double)pageSize);

                if (pageNumber < 1 || pageNumber > totalPages)
                {
                    return BadRequest(new BaseResult<IEnumerable<Customer>>
                    {
                        Success = false,
                        Errors = new List<string> { _localizer["InvalidPageNumber"] },
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var customers = await _context.Customers
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new BaseResult<IEnumerable<Customer>>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CustomersRetrievedSuccessfully"],
                    Status = StatusCodes.Status200OK,
                    Data = customers
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<IEnumerable<Customer>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }


        [HttpGet("{id}", Name = "GetCustomerById")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(new BaseResult<Customer>
            {
                Success = true,
                Errors = new List<string>(),
                Message = _localizer["CustomerRetrievedSuccessfully"],
                Status = StatusCodes.Status200OK,
                Data = customer
            });
        }

        [HttpPost(Name = "CreateCustomer")]
        public async Task<ActionResult<BaseResult<Customer>>> PostCustomer(Customer customer)
        {
            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, new BaseResult<Customer>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CustomerCreatedSuccessfully"],
                    Status = StatusCodes.Status201Created,
                    Data = customer
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<Customer>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPut("{id}", Name = "UpdateCustomer")]
        public async Task<ActionResult<BaseResult<Customer>>> PutCustomer(int id, Customer customer)
        {
            try
            {
                var cust = await _context.Customers.FindAsync(id);

                if (cust == null)
                {
                    return NotFound(new BaseResult<Customer>
                    {
                        Success = false,
                        Errors = new List<string> { _localizer["CustomerNotFound"] },
                        Status = StatusCodes.Status404NotFound
                    });
                }

                if (id != cust.Id)
                {
                    return BadRequest(new BaseResult<Customer>
                    {
                        Success = false,
                        Errors = new List<string> { _localizer["InvalidCustomerId"] },
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // Update the entity with the new values
                _context.Entry(cust).CurrentValues.SetValues(customer);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Refresh the entity with the latest values from the database
                    await ex.Entries.Single().ReloadAsync();

                    // Update the entity with the new values again
                    _context.Entry(cust).CurrentValues.SetValues(customer);

                    await _context.SaveChangesAsync();
                }

                return Ok(new BaseResult<Customer>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CustomerUpdatedSuccessfully"],
                    Status = StatusCodes.Status200OK,
                    Data = customer
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<Customer>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
        [HttpDelete("{id}", Name = "DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return NotFound(new BaseResult<Customer>
                    {
                        Success = false,
                        Errors = new List<string> { _localizer["CustomerNotFound"] },
                        Status = StatusCodes.Status404NotFound
                    });
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();


                return Ok(new BaseResult<Customer>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CustomerDeletedSuccessfully"],
                    Status = StatusCodes.Status200OK,
                    Data = customer
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<Customer>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }


        [HttpGet("filter", Name = "FilterCustomers")]
        public async Task<ActionResult<BaseResult<IEnumerable<Customer>>>> FilterCustomers(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.Customers.AsQueryable();


                if (startDate.HasValue)
                {
                    query = query.Where(c => c.CreatedAt >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(c => c.CreatedAt <= endDate.Value);
                }

                var customers = await query.ToListAsync();

                return Ok(new BaseResult<IEnumerable<Customer>>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CustomersFilteredSuccessfully"],
                    Status = StatusCodes.Status200OK,
                    Data = customers
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<IEnumerable<Customer>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }


        [HttpGet("search", Name = "SearchCustomers")]
        public async Task<ActionResult<BaseResult<IEnumerable<Customer>>>> SearchCustomers(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new BaseResult<IEnumerable<Customer>>
                    {
                        Success = false,
                        Errors = new List<string> { _localizer["SearchKeywordRequired"] },
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var customers = await _context.Customers
                 .Where(c => c.FirstName.Contains(keyword) || c.LastName.Contains(keyword) || c.Email.Contains(keyword) || c.Phone.Contains(keyword))
                 .ToListAsync();

                return Ok(new BaseResult<IEnumerable<Customer>>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CustomersRetrievedSuccessfully"],
                    Status = StatusCodes.Status200OK,
                    Data = customers
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<IEnumerable<Customer>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
