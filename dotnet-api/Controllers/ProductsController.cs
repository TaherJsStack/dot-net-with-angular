using Microsoft.AspNetCore.Mvc;
using dotnetAPI.Models;
using dotnetAPI.Data;
using Microsoft.EntityFrameworkCore;
using dotnetAPI.Helper;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly IStringLocalizer<ProductsController> _localizer;

        public ProductsController(ECommerceContext context, IStringLocalizer<ProductsController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        // GET: /products
        [HttpGet(Name = "GetProducts")]
        public async Task<ActionResult<BaseResult<IEnumerable<Product>>>> GetProducts(int pageNumber = 1, int pageSize = 10)
        {
            var totalData = await _context.Products.CountAsync();
            var totalPages = (int)Math.Ceiling(totalData / (double)pageSize);

            if (pageNumber < 1 || pageNumber > totalPages)
            {
                return BadRequest(new BaseResult<IEnumerable<Product>>
                {
                    Success = false,
                    Errors = new List<string> { _localizer["InvalidPageNumber"] },
                    Message = _localizer["InvalidPageNumber"],
                    Status = StatusCodes.Status400BadRequest,
                    TotalData = totalData,
                    Data = null
                });
            }

            var products = await _context.Products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new BaseResult<IEnumerable<Product>>
            {
                Success = true,
                Errors = new List<string>(),
                Message = _localizer["ProductsRetrievedSuccessfully"],
                Status = StatusCodes.Status200OK,
                TotalData = totalData,
                Data = products
            });
        }

        // GET: /products/{id}
        [HttpGet("{id}", Name = "GetProductById")]
        public async Task<ActionResult<BaseResult<Product>>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new BaseResult<Product>
                {
                    Success = false,
                    Errors = new List<string> { _localizer["ProductNotFound"] },
                    Message = _localizer["ProductNotFound"],
                    Status = StatusCodes.Status404NotFound,
                    Data = null
                });
            }

            return Ok(new BaseResult<Product>
            {
                Success = true,
                Errors = new List<string>(),
                Message = _localizer["ProductsRetrievedSuccessfully"],
                Status = StatusCodes.Status200OK,
                Data = product
            });
        }

        // POST: /products/{categoryId}
        [HttpPost("{categoryId}", Name = "CreateProduct")]
        public async Task<ActionResult<BaseResult<Product>>> PostProduct(int categoryId, Product product)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return NotFound(new BaseResult<Product>
                {
                    Success = false,
                    Errors = new List<string> { _localizer["CategoryIdRequired"] },
                    Message = _localizer["CategoryIdRequired"],
                    Status = StatusCodes.Status404NotFound,
                    Data = null
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResult<Product>
                {
                    Success = false,
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
                    Message = _localizer["ValidationFailed"],
                    Status = StatusCodes.Status400BadRequest,
                    Data = null
                });
            }

            product.CategoryId = categoryId;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, new BaseResult<Product>
            {
                Success = true,
                Errors = new List<string>(),
                Message = _localizer["ProductCreatedSuccessfully"],
                Status = StatusCodes.Status201Created,
                Data = product
            });
        }

        // PUT: /products/{id}
        [HttpPut("{id}", Name = "UpdateProduct")]
        public async Task<ActionResult<BaseResult<Product>>> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest(new BaseResult<Product>
                {
                    Success = false,
                    Errors = new List<string> { _localizer["ProductIDMismatch"] },
                    Message = _localizer["ProductIDMismatch"],
                    Status = StatusCodes.Status400BadRequest,
                    Data = null
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResult<Product>
                {
                    Success = false,
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList(),
                    Message = _localizer["ValidationFailed"],
                    Status = StatusCodes.Status400BadRequest,
                    Data = null
                });
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound(new BaseResult<Product>
                    {
                        Success = false,
                        Errors = new List<string> { _localizer["ProductNotFound"] },
                        Message = _localizer["ProductNotFound"],
                        Status = StatusCodes.Status404NotFound,
                        Data = null
                    });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: /products/{id}
        [HttpDelete("{id}", Name = "DeleteProduct")]
        public async Task<ActionResult<BaseResult<Product>>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new BaseResult<Product>
                {
                    Success = false,
                    Errors = new List<string> { _localizer["ProductNotFound"] },
                    Message = _localizer["ProductNotFound"],
                    Status = StatusCodes.Status404NotFound,
                    Data = null
                });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new BaseResult<Product>
            {
                Success = true,
                Errors = new List<string>(),
                Message = _localizer["ProductDeletedSuccessfully"],
                Status = StatusCodes.Status200OK,
                Data = product
            });
        }

        // GET: /products/search
        [HttpGet("search", Name = "SearchProducts")]
        public async Task<ActionResult<BaseResult<IEnumerable<Product>>>> SearchProducts(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new BaseResult<IEnumerable<Product>>
                {
                    Success = false,
                    Errors = new List<string> { _localizer["KeywordRequired"] },
                    Message = _localizer["KeywordRequired"],
                    Status = StatusCodes.Status400BadRequest,
                    TotalData = 0,
                    Data = null
                });
            }

            var products = await _context.Products
                .Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword))
                .ToListAsync();

            var totalData = products.Count;

            return Ok(new BaseResult<IEnumerable<Product>>
            {
                Success = true,
                Errors = new List<string>(),
                Message = _localizer["ProductsRetrievedSuccessfully"],
                Status = StatusCodes.Status200OK,
                TotalData = totalData,
                Data = products
            });
        }

        // GET: /products/filter
        [HttpGet("filter", Name = "FilterProducts")]
        public async Task<ActionResult<BaseResult<IEnumerable<Product>>>> FilterProducts(int? categoryId, decimal? minPrice, decimal? maxPrice, DateTime? fromDate, DateTime? toDate, bool? activeState = true)
        {
            var query = _context.Products.AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt <= toDate);
            }

            if (activeState.HasValue)
            {
                query = query.Where(p => p.ActiveState == activeState);
            }

            var products = await query.ToListAsync();
            var totalData = products.Count;

            return Ok(new BaseResult<IEnumerable<Product>>
            {
                Success = true,
                Errors = new List<string>(),
                Message = _localizer["ProductsFilteredSuccessfully"],
                Status = StatusCodes.Status200OK,
                TotalData = totalData,
                Data = products
            });
        }



        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
