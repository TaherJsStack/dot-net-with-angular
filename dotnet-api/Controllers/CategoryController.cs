using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetAPI.Data;
using dotnetAPI.Models;
using dotnetAPI.Helper;

namespace dotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ECommerceContext _context;
        private readonly IStringLocalizer<CategoryController> _localizer;

        public CategoryController(ECommerceContext context, IStringLocalizer<CategoryController> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        [HttpGet(Name = "GetCategories")]
        public async Task<ActionResult<BaseResult<IEnumerable<Category>>>> GetCategories(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var totalData = await _context.Categories.CountAsync();
                var totalPages = (int)Math.Ceiling(totalData / (double)pageSize);

                if (pageNumber < 1 || pageNumber > totalPages)
                {
                    return BadRequest(new BaseResult<IEnumerable<Category>>
                    {
                        Success     = false,
                        Errors      = new List<string> { _localizer["InvalidPageNumber"] },
                        Status      = StatusCodes.Status400BadRequest,
                        Message     = _localizer["InvalidPageNumber"],
                        TotalData   = totalData,
                        Data        = null,
                    });
                }

                var categories = await _context.Categories
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Console.WriteLine("_localizer ", _localizer);
                Console.WriteLine("_localizer CategoriesRetrievedSuccessfully", _localizer["CategoriesRetrievedSuccessfully"]);

                return Ok(new BaseResult<IEnumerable<Category>>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CategoriesRetrievedSuccessfully"],
                    Status = StatusCodes.Status200OK,
                    TotalData = totalData,
                    Data = categories
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<IEnumerable<Category>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError,
                    Message = _localizer["InvalidPageNumber"],
                    TotalData = 0,
                    Data = null,
                });
            }
        }

        [HttpGet("{id}", Name = "GetCategoryById")]
        public async Task<ActionResult<BaseResult<Category>>> GetCategory(int id)
        {
            try
            {
                var totalData = await _context.Categories.CountAsync();
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return NotFound(new BaseResult<Category>
                    {
                        Success = false,
                        Errors = new List<string> { _localizer["CategoryNotFound"] },
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return Ok(new BaseResult<Category>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CategoryRetrievedSuccessfully"],
                    Status = StatusCodes.Status200OK,
                    Data = category
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<Category>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpPost(Name = "CreateCategory")]
        public async Task<ActionResult<BaseResult<Category>>> PostCategory(Category category)
        {
            try
            {

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                var totalData = await _context.Categories.CountAsync();

                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new BaseResult<Category>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CategoryCreatedSuccessfully"],
                    Status = StatusCodes.Status201Created,
                    Data = category
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<Category>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
        [HttpPut("{id}", Name = "UpdateCategory")]
        public async Task<ActionResult<BaseResult<Category>>> PutCategory(int id, Category category)
        {
            try
            {
                var cat = await _context.Categories.FindAsync(id);
                if (cat == null)
                {
                    return NotFound(new BaseResult<Category>
                    {
                        Success = false,
                        Errors = new List<string> { _localizer["CategoryNotFound"] },
                        Status = StatusCodes.Status404NotFound
                    });
                }

                if (id != cat.Id)
                {
                    return BadRequest(new BaseResult<Category>
                    {
                        Success = false,
                        Errors = new List<string> { _localizer["InvalidCategoryId"] },
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // Update fields
                cat.Name = category.Name;
                cat.Description = category.Description;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!CategoryExists(id))
                    {
                        return NotFound(new BaseResult<Category>
                        {
                            Success = false,
                            Errors = new List<string> { _localizer["CategoryNotFound"] },
                            Status = StatusCodes.Status404NotFound
                        });
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<Category>
                        {
                            Success = false,
                            Errors = new List<string> { ex.Message },
                            Status = StatusCodes.Status500InternalServerError
                        });
                    }
                }

                return Ok(new BaseResult<Category>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CategoryUpdatedSuccessfully"],
                    Status = StatusCodes.Status200OK,
                    Data = cat
                });
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<Category>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        [HttpDelete("{id}", Name = "DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new BaseResult<Category>
                    {
                        Success = false,
                        Errors = new List<string> { _localizer["CategoryNotFound"] },
                        Status = StatusCodes.Status404NotFound
                    });
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                //return NoContent();
                return Ok(new BaseResult<Category>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CategoryDeletedSuccessfully"],
                    Status = StatusCodes.Status200OK,
                    Data = category
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<Category>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

         [HttpGet("filter", Name = "FilterCategories")]
        public async Task<ActionResult<BaseResult<IEnumerable<Category>>>> FilterCategories( DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.Categories.AsQueryable();


                if (startDate.HasValue)
                {
                    query = query.Where(c => c.CreatedAt >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(c => c.CreatedAt <= endDate.Value);
                }

                var categories = await query.ToListAsync();

                return Ok(new BaseResult<IEnumerable<Category>>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CategoriesFilteredSuccessfully"],
                    Status = StatusCodes.Status200OK,
                    Data = categories
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<IEnumerable<Category>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }


        [HttpGet("search", Name = "SearchCategories")]
        public async Task<ActionResult<BaseResult<IEnumerable<Category>>>> SearchCategories(string keyword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    return BadRequest(new BaseResult<IEnumerable<Category>>
                    {
                        Success = false,
                        Errors = new List<string> { _localizer["KeywordRequiredForSearch"] },
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var categories = await _context.Categories
                    .Where(c => c.Name.Contains(keyword) || c.Description.Contains(keyword))
                    .ToListAsync();

                return Ok(new BaseResult<IEnumerable<Category>>
                {
                    Success = true,
                    Errors = new List<string>(),
                    Message = _localizer["CategoriesSearchedSuccessfully"],
                    Status = StatusCodes.Status200OK,
                    Data = categories
                });
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResult<IEnumerable<Category>>
                {
                    Success = false,
                    Errors = new List<string> { ex.Message },
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
