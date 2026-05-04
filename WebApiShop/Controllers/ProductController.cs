using DTOs;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Service;
using WebApiShop.Attributes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase, IProductController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [AuthorizeAdmin]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            var created = await _productService.AddProductAsync(product);
            return CreatedAtAction(nameof(GetProductById),
                new { id = created.ProductId }, created);
        }

        // GET: api/<ValuesController>
        [HttpGet]
        async public Task<ActionResult<PageResponseDTO<ProductDTO>>> Get(int position, int skip, [FromQuery] ProductSearchParams parameters)
        {
            PageResponseDTO<ProductDTO> pageResponse = await _productService.GetProducts(position, skip, parameters);
            if (pageResponse.Data.Count() > 0)
                return Ok(pageResponse);
            return NotFound();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductById(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }


        // PUT api/<ValuesController>/5
        [AuthorizeAdmin]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.ProductId)
                return BadRequest();

            var result = await _productService.UpdateProductAsync(product);

            if (!result)
                return NotFound();

            return NoContent();
        }

        // DELETE api/<ValuesController>/5
        [AuthorizeAdmin]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);

            if (!result)
                return BadRequest("לא ניתן למחוק מוצר שנרכש");

            return NoContent();
        }
    }
}
