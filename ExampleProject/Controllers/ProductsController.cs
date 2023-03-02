using ExampleProject.Contexts;
using ExampleProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExampleProject.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IAsyncEnumerable<Product> GetProducts()
        {
            return _context.Products.AsAsyncEnumerable();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(long id, [FromServices] ILogger<ProductsController> logger)
        {
            logger.LogDebug("GetProduct Action Invoked");

            Product? p = await _context.Products.FindAsync(id);

            return p != null
                ? Ok(p)
                : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> SaveProduct([FromBody] ProductBindingTarget target)
        {
            if (ModelState.IsValid)
            {
                Product? p = target.ToProduct();

                await _context.Products.AddAsync(p);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(GetProduct), new { Id = p.ProductId });
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut]
        public async Task<Product?> UpdateProduct([FromBody] Product product)
        {
            Product? p = await _context.Products.FindAsync(product.ProductId);

            if(p != null)
            { 
                p.Name = product.Name;
                p.Price = product.Price;
                p.SupplierId = product.SupplierId;
                p.CategoryId = product.CategoryId;

                _context.Products.Update(p);
                await _context.SaveChangesAsync();
            }


            return _context.Products?.Find(product.ProductId);
        }

        [HttpDelete("{id}")]
        public async Task DeleteProduct(long id, [FromServices] ILogger<ProductsController> logger)
        {
            logger.LogDebug("Starting to remove the product");

            var product = await _context.Products.FindAsync(id);

            if(product != null)
            {
                logger.LogDebug("Product was found");

                try
                {
                    _context.Products.Remove(product);

                    await _context.SaveChangesAsync();

                    logger.LogDebug("Product removed");

                }
                catch(Exception ex)
                {
                    throw new Exception($"{ex.Message}");
                }
            }
        }
    }
}
