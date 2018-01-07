using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GarduinoWebAPI.Models;

namespace GarduinoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Home")]
    public class HomeController : Controller
    {
        private readonly MeasureContext _context;

        public HomeController(MeasureContext context)
        {
            _context = context;
        }

        // GET: api/Home
        [HttpGet]
        public IEnumerable<Measure> GetMeasure()
        {
            return _context.Measure;
        }

        // GET: api/Home/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMeasure([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var measure = await _context.Measure.SingleOrDefaultAsync(m => m.Id == id);

            if (measure == null)
            {
                return NotFound();
            }

            return Ok(measure);
        }

        // PUT: api/Home/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeasure([FromRoute] Guid id, [FromBody] Measure measure)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != measure.Id)
            {
                return BadRequest();
            }

            _context.Entry(measure).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeasureExists(id))
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

        // POST: api/Home
        [HttpPost]
        public async Task<IActionResult> PostMeasure([FromBody] Measure measure)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Measure.Add(measure);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMeasure", new { id = measure.Id }, measure);
        }

        // DELETE: api/Home/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeasure([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var measure = await _context.Measure.SingleOrDefaultAsync(m => m.Id == id);
            if (measure == null)
            {
                return NotFound();
            }

            _context.Measure.Remove(measure);
            await _context.SaveChangesAsync();

            return Ok(measure);
        }

        private bool MeasureExists(Guid id)
        {
            return _context.Measure.Any(e => e.Id == id);
        }
    }
}