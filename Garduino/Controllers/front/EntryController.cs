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
        private readonly AppState _appState;
        private readonly UserManager<ApplicationUser> _userManager;


        public EntryController(AppState appState, IUserRepository userRepository, IDeviceRepository deviceRepository, IMeasureRepository repository, 
            UserManager<ApplicationUser> userManager)
        {
            _appState = appState;
            _userRepository = userRepository;
            _deviceRepository = deviceRepository;
            _repository = repository;
            _userManager = userManager;
        }

        // GET: Entry
        public async Task<IActionResult> Index()
        {
            return View(_repository.GetAll(await GetCurrentDeviceAsync()));
        }

        // GET: Entry/Details/5
        public async Task<IActionResult> Details(Guid? id)
        { 
            if (!id.HasValue) return NotFound();
            return View(await _repository.GetAsync(id.Value));
        }

        // GET: Entry/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Entry/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, DateTime,SoilMoisture,SoilDescription,AirHumidity,AirTemperature,LightState,DeviceName")] Measure measure)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(measure, await GetCurrentDeviceAsync());
                return RedirectToAction(nameof(Index));
            }
            return View(measure);
        }

        // GET: Entry/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var measure = await _repository.GetAsync(id.Value);
            if (measure == null) return NotFound();
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
                await _repository.UpdateAsync(id, measure);
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
            var measure = await _repository.GetAsync(id.Value);
            if (measure == null) return NotFound();
            return View(measure);
        }

        // POST: Entry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MeasureExists(Guid id)
        {
            return await _repository.ContainsAsync(id, await GetCurrentDeviceAsync());
        }

        private async Task<Device> _GetDeviceAsync(Guid deviceId) => 
            await _deviceRepository.GetAsync(deviceId);

        private async Task<User> GetCurrentUserAsync() => await _userRepository.GetAsync(await GetCurrentUserIdAsync());

        private async Task<Device> GetDeviceAsync(Guid? deviceId)
        {
            if (deviceId == null) return null;
            var device = await _GetDeviceAsync(deviceId.Value);
            return device ?? null;
        }

        private async Task<Device> GetCurrentDeviceAsync() => await GetDeviceAsync(_appState.CurrentDeviceId);

        private async Task<string> GetCurrentUserIdAsync()
        {
            var userId = await _userManager.GetUserAsync(HttpContext.User);
            return userId?.Id;
        }
    }
}
