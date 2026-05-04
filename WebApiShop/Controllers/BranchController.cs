using DTOs;
using Microsoft.AspNetCore.Mvc;
using Service;
using WebApiShop.Attributes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase, IBranchController
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        // GET: api/<BranchController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BranchDTO>>> Get([FromQuery] string? query)
        {
            var branches = await _branchService.GetBranches(query);
            if (branches == null)
                return NotFound();

            return Ok(branches);
        }

        // GET api/<BranchController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BranchController>
        [AuthorizeAdmin]
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BranchController>/5
        [AuthorizeAdmin]
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BranchController>/5
        [AuthorizeAdmin]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
