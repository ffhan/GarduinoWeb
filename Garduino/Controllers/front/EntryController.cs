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
using GarduinoUniversal;

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
        public async Task<IActionResult> Index(Guid deviceId)
        {
            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View(_repository.GetAll(await GetDeviceAsync(deviceId)));
        }

        // GET: Entry/Details/5
        public async Task<IActionResult> Details(Guid? id)
        { 
            if (!id.HasValue) return NotFound();
            Measure measure = await _repository.GetAsync(id.Value);
            Guid deviceId = measure.Device.Id;
            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View(measure);
        }

        // GET: Entry/Create
        public IActionResult Create(Guid deviceId)
        {
            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View();
        }

        // POST: Entry/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid deviceId, [Bind("Id, DateTime,SoilMoisture,SoilDescription,AirHumidity,AirTemperature,LightState,DeviceName")] Measure measure)
        {
            if (!ModelState.IsValid) return View(measure);
            await _repository.AddAsync(measure, await GetDeviceAsync(deviceId));
            return RedirectToAction(nameof(Index), new { deviceId });
        }

        // GET: Entry/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            Measure measure = await _repository.GetAsync(id.Value);
            if (measure == null) return NotFound();
            Guid deviceId = await GetDeviceId(id.Value);
            ViewData["deviceId"] = deviceId;
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
            Guid deviceId = await GetDeviceId(id);
            return RedirectToAction("Index", "Entry", new { deviceId });
        }

        // GET: Entry/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null) return NotFound();
            Measure measure = await _repository.GetAsync(id.Value);
            if (measure == null) return NotFound();
            Guid deviceId = measure.Device.Id;
            ViewData["deviceId"] = deviceId;
            return View(measure);
        }

        // POST: Entry/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            Guid deviceId = await GetDeviceId(id);
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { deviceId });
        }

        private async Task<bool> MeasureExists(Guid id)
        {
            return await _repository.ContainsAsync(id);
        }

        private async Task<Guid> GetDeviceId(Guid measureId)
        {
            return (await _repository.GetAsync(measureId)).Device.Id;
        }

        private async Task<Device> _GetDeviceAsync(Guid deviceId) => 
            await _deviceRepository.GetAsync(deviceId);

        private async Task<Device> GetDeviceAsync(Guid? deviceId)
        {
            if (deviceId == null) return null;
            var device = await _GetDeviceAsync(deviceId.Value);
            return device ?? null;
        }
    }
}
