using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Garduino.Data;
using Garduino.Models;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Garduino.Controllers.api
{
    [Authorize]//TODO: FIX AUTHORIZATION
    [Produces("application/json")]
    [Microsoft.AspNetCore.Mvc.Route("api/Entry")]
    public class EntryController : Controller
    {
        private readonly IEntryRepository _repository;

        public EntryController(IEntryRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Entry
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public IEnumerable<Measure> GetMeasure()
        {
            return _repository.GetAll();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("{dateTime}")]
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

        [Microsoft.AspNetCore.Mvc.HttpGet("{dateTime1}&{dateTime2}")]
        public async Task<IActionResult> GetMeasure([FromRoute] DateTime dateTime1, DateTime dateTime2)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var measure = await _repository.GetRangeAsync(dateTime1, dateTime2);

            if (measure == null) return NotFound();

            return Ok(measure);
        }

        // GET: api/Entry/5
        [Microsoft.AspNetCore.Mvc.HttpGet("{id}")]
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

        [Microsoft.AspNetCore.Mvc.HttpPut]
        public async Task<IActionResult> PutMeasure([FromBody] Measure measure)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Guid id = await _repository.GetId(measure);

            if (!await _repository.UpdateAsync(id, measure)) return NoContent();
            return Ok();
        }

        // PUT: api/Entry/5
        [Microsoft.AspNetCore.Mvc.HttpPut("{id}")]
        public async Task<IActionResult> PutMeasure([FromRoute] Guid id, [FromBody] Measure measure)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await _repository.UpdateAsync(id, measure)) return NoContent();
            return Ok();
        }

        // POST: api/Entry
        [Microsoft.AspNetCore.Mvc.HttpPost]
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
        [Microsoft.AspNetCore.Mvc.HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMeasure([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _repository.DeleteAsync(id)) return Ok(await _repository.GetAsync(id));
            return BadRequest();
        }

        [Microsoft.AspNetCore.Mvc.HttpGet("cmp/{dateTime1}&{dateTime2}")]
        public async Task<IActionResult> Compare(DateTime dateTime1, DateTime dateTime2)
        {
            if (await _repository.GetAsync(dateTime1) == await _repository.GetAsync(dateTime2)) return Ok();
            return BadRequest();
        }

        [Microsoft.AspNetCore.Mvc.HttpDelete("all")]
        public async Task<IActionResult> DeleteAll()
        {
            if(await _repository.DeleteAllAsync()) return Ok();
            return BadRequest();
        }

    }
}