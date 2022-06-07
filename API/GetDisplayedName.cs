using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stories.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Stories.API
{
    [Route("api/getdisplayedname")]
    [ApiController]
    public class GetDisplayedName : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private ILogger<Index> _logger;

        public GetDisplayedName(Data.ApplicationDbContext dbContext, ILogger<Index> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET api/<GetDisplayedName>/5
        [HttpGet("{id}")]
        public async Task<string> OnGetAsync(string id)
        {
            return await (from user in _dbContext.Users where user.Email == id select user.DisplayedName).FirstOrDefaultAsync();
        }

        // POST api/<GetDisplayedName>
        [HttpPost]
        public async Task<string> OnPostAsync([FromBody] string id)
        {
            return await (from user in _dbContext.Users where user.Email == id select user.DisplayedName).FirstOrDefaultAsync();
        }

        // PUT api/<GetDisplayedName>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GetDisplayedName>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}