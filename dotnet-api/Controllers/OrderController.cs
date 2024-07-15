using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetAPI.Data;
using dotnetAPI.Models;

namespace dotnetAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController: ControllerBase
    {

        private readonly ECommerceContext _context;


        public OrderController(ECommerceContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(int pageNumber = 1, int pageSize = 10)
        {
            var totalData = await _context.Orders.CountAsync();
            var totalPages = (int)Math.Ceiling(totalData / (double)pageSize);

            if (pageNumber < 1 || pageNumber > totalPages)
            {
                return BadRequest("Invalid page number");
            }

            return await _context.Orders
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        [HttpGet("{id}", Name = "GetOrderById")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpPost(Name = "CreateOrder")]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }


        [HttpPut("{id}", Name = "UpdateOrder")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            var orderf = await _context.Orders.FindAsync(id);
            if (id != orderf.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        [HttpDelete("{id}", Name = "DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

    }



}
