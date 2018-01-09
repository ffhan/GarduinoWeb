using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Garduino.Data;
using Garduino.Models;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace Garduino.Controllers.api
{
    [Authorize]
    [Produces("application/json")]
    [Microsoft.AspNetCore.Mvc.Route("api/Code")]
    public class CodeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Code
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public IEnumerable<Code> GetCode()
        {
            return _context.Code;
        }

        // GET: api/Code/5
        [Microsoft.AspNetCore.Mvc.HttpGet("{id}")]
        public async Task<IActionResult> GetCode([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var code = await _context.Code.SingleOrDefaultAsync(m => m.Id == id);

            if (code == null)
            {
                return NotFound();
            }

            return Ok(code);
        }

        // PUT: api/Code/5
        [Microsoft.AspNetCore.Mvc.HttpPut("{id}")]
        public async Task<IActionResult> PutCode([FromRoute] Guid id, [FromBody] Code code)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != code.Id)
            {
                return BadRequest();
            }

            _context.Entry(code).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CodeExists(id))
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

        // POST: api/Code
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> PostCode([FromBody] Code code)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Code.Add(code);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCode", new { id = code.Id }, code);
        }

        // DELETE: api/Code/5
        [Microsoft.AspNetCore.Mvc.HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCode([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var code = await _context.Code.SingleOrDefaultAsync(m => m.Id == id);
            if (code == null)
            {
                return NotFound();
            }

            _context.Code.Remove(code);
            await _context.SaveChangesAsync();

            return Ok(code);
        }

        private bool CodeExists(Guid id)
        {
            return _context.Code.Any(e => e.Id == id);
        }
    }
}