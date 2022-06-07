using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stories.Data;
using Stories.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;



namespace Stories.API
{
    [Authorize]
    [Route("Api/InfoLogs")]
    [ApiController]
    public class InfoLogs : ControllerBase
    {
        private readonly LogDbContext _db;

        public InfoLogs(LogDbContext db)
        {
            _db = db;
        }

        // GET: api/<InfoLogs>

        public IList<Log> infoLogsList { get; set; }

        // GET:
        [HttpGet]
        public async Task<string> OnGetAsync()
        {
            if (User.IsInRole("Administrator"))
            {
                infoLogsList = await _db.Log.ToArrayAsync();
                return JsonSerializer.Serialize(infoLogsList);
            }
            Console.WriteLine("User is not admin");
            return "";
        }

        // GET api/<InfoLogs>/5
        [HttpGet("{id}")]
        public async Task<string> OnGetAsync(int id)
        {
            if (User.IsInRole("Administrator"))
            {
                Log log = new Log();
                log = await _db.Log.FirstOrDefaultAsync(i => i.Id == id);
                return JsonSerializer.Serialize(log);
            }
            else
            {
                Console.WriteLine("Only for admin");
                return "";
            }
        }
    }
}