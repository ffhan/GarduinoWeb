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

        public async Task<IActionResult> Index() => View(_repository.GetActive(
            await GetCurrentDeviceAsync()));

        public async Task<IActionResult> All() => View(_repository.GetAll(await GetCurrentDeviceAsync()));

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("Action,ActionName,DeviceName")] Code code)
        {
            if (!ModelState.IsValid) return View(code);
            await _repository.AddAsync(code, await GetCurrentDeviceAsync());
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
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
                if (!await _repository.ContainsAsync(code, await GetCurrentDeviceAsync()))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Details(Guid id)
        {
            var code = await _repository.GetAsync(id);
            return View(code);
        }

        public string CurrentUserName => User.Identity.Name;

        private async Task<Device> GetCurrentDeviceAsync() => await GetDeviceAsync(_appState.CurrentDeviceId);

        private async Task<Device> _GetDeviceAsync(Guid deviceId) =>
            await _deviceRepository.GetAsync(deviceId);

        private async Task<Device> GetDeviceAsync(Guid? deviceId)
        {
            if (deviceId == null) return null;
            var device = await _GetDeviceAsync(deviceId.Value);
            return device ?? null;
        }

        private async Task<User> GetCurrentUserAsync() => await _userRepository.GetAsync(await GetCurrentUserIdAsync());

        private async Task<string> GetCurrentUserIdAsync()
        {
            var userId = await _userManager.GetUserAsync(HttpContext.User);
            return userId?.Id;
        }
    }
}