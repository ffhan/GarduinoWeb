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


        public CodeController(IUserRepository userRepository, IDeviceRepository deviceRepository, 
            ICodeRepository repository, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _deviceRepository = deviceRepository;
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(Guid deviceId) => View((await GetDevice(deviceId)).Codes);

        public async Task<IActionResult> All(Guid deviceId) => View((await GetDevice(deviceId)).Codes);

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid id, [Bind("Action,ActionName,DeviceName")] Code code)
        {
            if (!ModelState.IsValid) return View(code);
            await _repository.AddAsync(code, await GetDevice(id));
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            return View(await _repository.GetAsync(id.Value, await GetDevice(id.Value)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, Action, ActionName, IsCompleted, DeviceName")] Code code)
        {
            if (id != code.Id) return NotFound();

            if (!ModelState.IsValid) return View(code);
            try
            {
                await _repository.UpdateAsync(id, code, await GetDevice(id));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ContainsAsync(code, await GetDevice(id)))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Details(Guid id)
        {
            var code = await _repository.GetAsync(id, await GetDevice(id));
            return View(code);
        }

        public string CurrentUserName => User.Identity.Name;

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