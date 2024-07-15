using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetAPI.Data;
using dotnetAPI.Models;

namespace dotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderItemsController: ControllerBase
    {

        private readonly ECommerceContext _context;


        public OrderItemsController(ECommerceContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetOrderItems")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(int pageNumber = 1, int pageSize = 10)
        {
            var totalData = await _context.OrderItems.CountAsync();
            var totalPages = (int)Math.Ceiling(totalData / (double)pageSize);

            if (pageNumber < 1 || pageNumber > totalPages)
            {
                return BadRequest("Invalid page number");
            }

            return await _context.OrderItems
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        [HttpGet("{id}", Name = "GetOrderItemsById")]
        public async Task<ActionResult<OrderItem>> GetOrderItems(int id)
        {
            var orderItems = await _context.OrderItems.FindAsync(id);

            if (orderItems == null)
            {
                return NotFound();
            }

            return orderItems;
        }

        [HttpPost(Name = "CreateOrderItems")]
        public async Task<ActionResult<OrderItem>> PostOrderItems(OrderItem orderItems)
        {
            _context.OrderItems.Add(orderItems);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderItems", new { id = orderItems.Id }, orderItems);
        }


        [HttpPut("{id}", Name = "UpdateOrderItems")]
        public async Task<IActionResult> PutCategory(int id, OrderItem orderItem)
        {
            if (id != orderItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(orderItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteOrderItems")]
        public async Task<IActionResult> DeleteOrderItems(int id)
        {
            var orderItems = await _context.OrderItems.FindAsync(id);
            if (orderItems == null)
            {
                return NotFound();
            }

            _context.OrderItems.Remove(orderItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.Id == id);
        }

    }

}
