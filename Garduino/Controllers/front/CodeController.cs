using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Mvc;
using Garduino.Data;
using Garduino.Data.Interfaces;
using Garduino.Models;
using Garduino.Models.ViewModels;
using GarduinoUniversal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace Garduino.Controllers.front
{
    [Authorize] //TODO:  create Device model.
    public class CodeController : Controller
    {

        private readonly ICodeRepository _repository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppState _appState;

        public CodeController(AppState appState, IUserRepository userRepository, 
            IDeviceRepository deviceRepository, 
            ICodeRepository repository, UserManager<ApplicationUser> userManager)
        {
            _appState = appState;
            _userRepository = userRepository;
            _deviceRepository = deviceRepository;
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(Guid deviceId)
        {
            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View(_repository.GetActive(await GetDeviceAsync(deviceId)));
        }

        public async Task<IActionResult> All(Guid deviceId)
        {
            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View(_repository.GetAll(await GetDeviceAsync(deviceId)));
        }

        public IActionResult Create(Guid deviceId)
        {
            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid deviceId, Guid id, [Bind("Action,ActionName,DeviceName")] Code code)
        {
            if (!ModelState.IsValid) return View(code);
            await _repository.AddAsync(code, await GetDeviceAsync(deviceId));
            return RedirectToAction("Index", new { deviceId });
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            Guid deviceId = await GetDeviceIdAsync(id.Value);
            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View(await _repository.GetAsync(id.Value));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, Action, ActionName, IsCompleted, DeviceName")] Code code)
        {
            if (id != code.Id) return NotFound();

            if (!ModelState.IsValid) return View(code);
            try
            {
                await _repository.UpdateAsync(id, code);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ContainsAsync(code.Id))
                {
                    return NotFound();
                }
                throw;
            }
            Guid deviceId = await GetDeviceIdAsync(id);
            return RedirectToAction(nameof(Index), new { deviceId });
        }
        
        public async Task<IActionResult> Details(Guid id)
        {
            var code = await _repository.GetAsync(id);
            Guid deviceId = code.Device.Id;
            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View(code);
        }

        private async Task<Guid> GetDeviceIdAsync(Guid codeId)
        {
            Code code = await _repository.GetAsync(codeId);
            Guid deviceId = code.Device.Id;
            return deviceId;
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