using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garduino.Data;
using Garduino.Data.Interfaces;
using Garduino.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Garduino.Controllers.front
{
    [Authorize]
    public class EntryController : Controller
    {
        private readonly IMeasureRepository _repository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public EntryController(IUserRepository userRepository, IDeviceRepository deviceRepository, IMeasureRepository repository, 
            UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _deviceRepository = deviceRepository;
            _repository = repository;
            _userManager = userManager;
        }

        // GET: Entry
        [Route("{deviceId}")]
        public async Task<IActionResult> Index(Guid deviceId)
        {
            return View(_repository.GetAll(await GetDevice(deviceId)));
        }

        // GET: Entry/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();
            return View(await _repository.GetAsync(id.Value, await GetDevice(id.Value)));
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
                //await _repository.AddAsync(measure, );
                return RedirectToAction(nameof(Index));
            }
            return View(measure);
        }

        // GET: Entry/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var measure = await _repository.GetAsync(id.Value, await GetDevice(id.Value));
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
                await _repository.UpdateAsync(id, measure, await GetDevice(id));
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
            var measure = await _repository.GetAsync(id.Value, await GetDevice(id.Value));
            if (measure == null) return NotFound();
            return View(measure);
        }

        // POST: Entry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _repository.DeleteAsync(id, await GetDevice(id));
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MeasureExists(Guid id)
        {
            var device = await GetDevice(id);
            return await _repository.ContainsAsync(await _repository.GetAsync(id, device), device);
        }

        private async Task<Device> GetDevice(Guid id) => 
            await _deviceRepository.GetDevice(id, await GetCurrentUserAsync());

        private async Task<User> GetCurrentUserAsync() => await _userRepository.GetAsync(await GetCurrentUserIdAsync());

        private async Task<string> GetCurrentUserIdAsync()
        {
            var userId = await _userManager.GetUserAsync(HttpContext.User);
            return userId?.Id;
        }
    }
}
