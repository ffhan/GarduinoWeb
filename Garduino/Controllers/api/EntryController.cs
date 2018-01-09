using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Garduino.Data;
using Garduino.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Microsoft.AspNetCore.Authorization;

namespace Garduino.Controllers.api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] //TODO: Include user id in models.
    [Produces("application/json")]
    [Route("api/Entry")]
    public class EntryController : Controller
    {
        private readonly IEntryRepository _repository;

        public EntryController(IEntryRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Entry
        [HttpGet]
        public IEnumerable<Measure> GetMeasure()
        {
            return _repository.GetAll();
        }

        [HttpGet("{dateTime}")]
        public async Task<IActionResult> GetMeasure([FromRoute] DateTime dateTime)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var measure = await _repository.GetAsync(dateTime);

            try
            {
                if (measure is null) return NotFound();
            }
            catch (Exception e)
            {
                return NotFound();
            }
            

            return Ok(measure);
        }

        [HttpGet("{dateTime1}&{dateTime2}")]
        public async Task<IActionResult> GetMeasure([FromRoute] DateTime dateTime1, DateTime dateTime2)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var measure = await _repository.GetRangeAsync(dateTime1, dateTime2);

            if (measure == null) return NotFound();

            return Ok(measure);
        }

        // GET: api/Entry/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMeasure([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var measure = await _repository.GetAsync(id);

            if (measure == null)
            {
                return NotFound();
            }

            return Ok(measure);
        }

        [HttpPut]
        public async Task<IActionResult> PutMeasure([FromBody] Measure measure)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Guid id = await _repository.GetId(measure);

            if (!await _repository.UpdateAsync(id, measure)) return NoContent();
            return Ok();
        }

        // PUT: api/Entry/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMeasure([FromRoute] Guid id, [FromBody] Measure measure)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await _repository.UpdateAsync(id, measure)) return NoContent();
            return Ok();
        }

        // POST: api/Entry
        [HttpPost]
        public async Task<IActionResult> PostMeasure([FromBody] Measure measure)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _repository.AddAsync(measure)) return Ok();
            return BadRequest();
        }

        // DELETE: api/Entry/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeasure([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _repository.DeleteAsync(id)) return Ok(await _repository.GetAsync(id));
            return BadRequest();
        }

        [HttpGet("cmp/{dateTime1}&{dateTime2}")]
        public async Task<IActionResult> Compare(DateTime dateTime1, DateTime dateTime2)
        {
            if (await _repository.GetAsync(dateTime1) == await _repository.GetAsync(dateTime2)) return Ok();
            return BadRequest();
        }

        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAll()
        {
            if(await _repository.DeleteAllAsync()) return Ok();
            return BadRequest();
        }

    }
}