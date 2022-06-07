using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stories.Data;
using System;
using System.Threading.Tasks;

namespace Stories.API
{
    [Route("api/getprefix")]
    [ApiController]
    public class GetPrefix : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private ILogger<Index> _logger;

        public GetPrefix(ApplicationDbContext dbContext, ILogger<Index> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<int> OnGetAsync(int Id)
        {
            var country = await _dbContext.Countries.FindAsync(Id);
            return country.PhonePrefix;
        }
        [HttpGet]
        public int OnGetAsync()
        {
            return 9999;
        }
    }
}