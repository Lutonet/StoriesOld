using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stories.Data;
using System;
using System.Linq;

namespace Stories.API
{
    [Route("api/emailExists")]
    [ApiController]
    public class EmailExists : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        private ILogger<Index> _logger;

        public EmailExists(ApplicationDbContext dbContext, ILogger<Index> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public ActionResult<string> OnGet(string id)
        {
            if ((from user in _dbContext.Users where user.Email.ToLower() == id.ToLower().Trim() select user).Count() > 0) return "true";
            else return "false";
        }
    }
}