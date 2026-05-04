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
    public class CategoryController : ControllerBase, ICategoryController
    {

        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/<CategoryController>
        [HttpGet]
        async public Task<ActionResult<IEnumerable<List<CategoryDTO>>>> Get()
        {
            var categories = await _categoryService.GetCategories();
            if (categories == null)
                return NotFound();
            return Ok(categories);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CategoryController>
        [AuthorizeAdmin]
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CategoryController>/5
        [AuthorizeAdmin]
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CategoryController>/5
        [AuthorizeAdmin]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
