using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static ProductService.ProductViewModels.ProductViewModels;


[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductDbContext _context;

    public ProductsController(ProductDbContext context)
    {
        _context = context;
    }

    // GET: api/products
    [HttpGet]
    public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10)
    {
        var products = await _context.Products
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();
        return Ok(products);
    }

    [HttpPost]
    //public async Task<IActionResult> CreateProduct([FromBody] ProductInputDto productInputDto)
    //{
    //    var validator = new ProductValidator();
    //    var validationResult = await validator.ValidateAsync(productInputDto);

    //    if (!validationResult.IsValid)
    //    {
    //        var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
    //        string errorMessage = string.Join(Environment.NewLine, errorMessages);
    //    }

    //    if (!ModelState.IsValid)
    //    {
    //        return BadRequest(ModelState);
    //    }
    //    var product = new Product
    //    {
    //        Name = productInputDto.Name,
    //        Description = productInputDto.Description,
    //        Price = productInputDto.Price
    //    };

    //    _context.Products.Add(product);
    //    await _context.SaveChangesAsync();

    //    return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    //}
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductInputDto productInputDto)
    {
        var validator = new ProductValidator();
        var validationResult = await validator.ValidateAsync(productInputDto);

        if (!validationResult.IsValid)
        {
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
            string combinedErrorMessage = string.Join(Environment.NewLine, errorMessages);

            throw new Exception(combinedErrorMessage);
        }
        var product = new Product
        {
            Name = productInputDto.Name,
            Description = productInputDto.Description,
            Price = productInputDto.Price
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }


    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        return Ok(product);
    }


    //public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductInputDto productInputDto)
    //{
    //    if (id <= 0) return BadRequest("Invalid product ID.");


    //    var product = await _context.Products.FindAsync(id);
    //    if (product == null) return NotFound();


    //    product.Name = productInputDto.Name;
    //    product.Description = productInputDto.Description;
    //    product.Price = productInputDto.Price;

    //    _context.Entry(product).State = EntityState.Modified;

    //    await _context.SaveChangesAsync();

    //    return Ok(productInputDto);
    //}


    // PUT: api/products/{id}
    [HttpPut("{id}")]

    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductInputDto productInputDto)
    {
        if (id <= 0) return BadRequest("Invalid product ID.");

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // Validate the incoming data
        var validator = new ProductValidator();
        var validationResult = await validator.ValidateAsync(productInputDto);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

            return BadRequest(new
            {
    
                title = "One or more validation errors occurred.",
                status = (int)HttpStatusCode.BadRequest,
                errors
            });
        }

        // Update the product
        product.Name = productInputDto.Name;
        product.Description = productInputDto.Description;
        product.Price = productInputDto.Price;

        _context.Entry(product).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return Ok(productInputDto);
    }


    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Product deleted successfully." });
    }

}