﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Garduino.Data;
using Garduino.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Controllers.api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Code")]
    public class CodeController : Controller
    {
        private readonly ICodeRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CodeController(ICodeRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // GET: api/Code
        [HttpGet("all")]
        public async Task<IEnumerable<Code>> GetCode()
        {
            var t = _repository.GetAll(await GetCurrentUserIdAsync());
            return t;
        }

        [HttpGet]
        public async Task<IEnumerable<Code>> GetActiveCode()
        {
            return _repository.GetActive(await GetCurrentUserIdAsync());
        }

        [HttpPost("complete")]
        public async Task Complete([FromBody] DateTime dateTime) //TODO: Complete & Fix.
        {
            _repository.GetLatest(await GetCurrentUserIdAsync()).Complete(dateTime);
        }


        // GET: api/Code/5
        [HttpGet("id={id}")]
        public async Task<IActionResult> GetCode([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Code code = await _repository.GetAsync(id, await GetCurrentUserIdAsync());

            if (code == null)
            {
                return NotFound();
            }

            return Ok(code);
        }

        // PUT: api/Code/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCode([FromRoute] Guid id, [FromBody] Code code)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!await _repository.UpdateAsync(id, code, await GetCurrentUserIdAsync())) return NoContent();
            return Ok();
        }

        // POST: api/Code
        [HttpPost]
        public async Task<IActionResult> PostCode([FromBody] Code code)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _repository.AddAsync(code, await GetCurrentUserIdAsync())) return Ok();
            return BadRequest();
        }

        // DELETE: api/Code/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCode([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _repository.DeleteAsync(id, await GetCurrentUserIdAsync())) return Ok(await _repository.GetAsync(id,
                await GetCurrentUserIdAsync()));
            return BadRequest();
        }

        [HttpDelete("deleteall")] //ONLY FOR DEVELOPMENT!
        public async Task<IActionResult> DeleteAll()
        {
            if (await _repository.DeleteAllAsync()) return Ok();
            return BadRequest();
        }

        private async Task<string> GetCurrentUserIdAsync()
        {
            var userId = await _userManager.Users.FirstOrDefaultAsync(g => g.Email.Equals(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            return userId?.Id;
        }
    }
}