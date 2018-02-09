using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garduino.Data;
using Garduino.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Garduino.Controllers.front
{
    [Authorize]
    public class EntryController : Controller
    {
        private readonly IRepository<Measure> _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public EntryController(IRepository<Measure> repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // GET: Entry
        public async Task<IActionResult> Index(string id)
        {
            string trimmed = (id ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                ViewData["searchInput"] = trimmed;
                return View(_repository.GetDevice(trimmed, await GetCurrentUserIdAsync()));
            }

            return View(_repository.GetAll(await GetCurrentUserIdAsync()));
        }

        // GET: Entry/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            return View(await _repository.GetAsync(id.Value, await GetCurrentUserIdAsync()));
        }

        // GET: Entry/Create
        public IActionResult Create() => View();

        // POST: Entry/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, DateTime,SoilMoisture,SoilDescription,AirHumidity,AirTemperature,LightState,DeviceName")] Measure measure)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(measure, await GetCurrentUserIdAsync());
                return RedirectToAction(nameof(Index));
            }
            return View(measure);
        }

        // GET: Entry/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var measure = await _repository.GetAsync(id.Value, await GetCurrentUserIdAsync());
            if (measure == null)
            {
                return NotFound();
            }
            return View(measure);
        }

        // POST: Entry/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, DateTime,SoilMoisture,SoilDescription,AirHumidity,AirTemperature,LightState,DeviceName")] Measure measure)
        {
            if (id != measure.Id) return NotFound();

            if (!ModelState.IsValid) return View(measure);
            try
            {
                await _repository.UpdateAsync(id, measure, await GetCurrentUserIdAsync());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MeasureExists(measure.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Entry/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            var measure = await _repository.GetAsync(id.Value, await GetCurrentUserIdAsync());
            if (measure == null) return NotFound();
            return View(measure);
        }

        // POST: Entry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _repository.DeleteAsync(id, await GetCurrentUserIdAsync());
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MeasureExists(Guid id)
        {
            var userId = await GetCurrentUserIdAsync();
            return await _repository.ContainsAsync(await _repository.GetAsync(id, userId), userId);
        }

        private async Task<string> GetCurrentUserIdAsync()
        {
            var userId = await _userManager.GetUserAsync(HttpContext.User);
            return userId?.Id;
        }
    }
}
