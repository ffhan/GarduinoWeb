using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Mvc;
using Garduino.Data;
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
    [Authorize]
    public class DeviceController : Controller
        /*
         * TODO: MEDIA CARDS FOR DEVICES INSTEAD OF TABLE
         * TODO: IMPLEMENT ADDING CODES AND MEASURES DIRECTLY TO DEVICE
         * TODO: ROUTE CODES AND MEASURES DIRECTLY TO Device/device_id/Entry or Code/
         */
    {
        private readonly IDeviceRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DeviceController(IUserRepository userRepository, IDeviceRepository repository, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _repository = repository;
            _userManager = userManager;
        }

        // GET: Device
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser(); //TODO: fix model binding
            return View(user.Device);
        }

        public async Task<IActionResult> See(Guid? id)
        {
            if (id != null) return NotFound();
            return View(await _repository.GetAsync(id.Value, await GetCurrentUser()));
        }

        // GET: Device/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _repository.GetAsync(id.Value, await GetCurrentUser());
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // GET: Device/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Device/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Device device)
        {
            if (!ModelState.IsValid) return View(device);
            await _repository.AddAsync(device, await GetCurrentUser());
            return RedirectToAction(nameof(Index));
        }

        // GET: Device/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _repository.GetAsync(id.Value, await GetCurrentUser());
            if (device == null)
            {
                return NotFound();
            }
            return View(device);
        }

        // POST: Device/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,UserId")] Device device)
        {
            if (id != device.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateAsync(id, device, await GetCurrentUser());
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await DeviceExists(device.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(device);
        }

        // GET: Device/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _repository.GetAsync(id.Value, await GetCurrentUser());
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Device/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var device = await _repository.GetAsync(id, await GetCurrentUser());
            await _repository.DeleteAsync(id, await GetCurrentUser());
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DeviceExists(Guid id)
        {
            return await _repository.ContainsAsync(id, await GetCurrentUser());
        }

        private async Task<User> GetCurrentUser() => await _userRepository.GetAsync(await _userManager.GetUserAsync(HttpContext.User));

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyName(string name)
        {
            if (!await _repository.DeviceExists(name, await GetCurrentUser())) return Json(true);
            return Json($"Device named {name} already exists!");
        }
    }
}
