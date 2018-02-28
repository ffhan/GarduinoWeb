using System;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Mvc;
using Garduino.Data;
using Garduino.Data.Interfaces;
using Garduino.Hubs;
using Garduino.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace Garduino.Controllers.front
{
    [Authorize]
    public class DeviceController : Controller
    {
        private readonly IDeviceRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<DeviceHub> _hubContext;

        public DeviceController(AppState appState, IUserRepository userRepository, 
            IDeviceRepository repository, UserManager<ApplicationUser> userManager,
            IHubContext<DeviceHub> hubContext)
        {
            _hubContext = hubContext;
            _userRepository = userRepository;
            _repository = repository;
            _userManager = userManager;
        }

        // GET: Device
        public async Task<IActionResult> Index()
        {
            return View(_repository.GetAll(await GetCurrentUserAsync()));
        }

        // GET: Device/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _repository.GetAsync(id.Value);
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
        public async Task<IActionResult> Create([Bind("Name")] Device device)
        {
            if (!ModelState.IsValid) return View(device);
            await _repository.AddAsync(device, await GetCurrentUserAsync());
            return RedirectToAction(nameof(Index));
        }

        // GET: Device/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _repository.GetAsync(id.Value);
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
                    await _repository.UpdateAsync(id, device);
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

            var device = await _repository.GetAsync(id.Value);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        [HttpPost]
        public async Task<PartialViewResult> GetDeviceItems()
        {

            var devices = _repository.GetAll(await GetCurrentUserAsync());
            //ModelState.Clear();
            return PartialView("DeviceItems", devices);
        }

        // POST: Device/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var device = await _repository.GetAsync(id);
            await _repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> DeviceExists(Guid id)
        {
            return await _repository.IsContainedAsync(id, await GetCurrentUserAsync());
        }

        private async Task<User> GetCurrentUserAsync() => await _userRepository.GetAsync(await GetCurrentUserIdAsync());

        private async Task<string> GetCurrentUserIdAsync()
        {
            var userId = await _userManager.GetUserAsync(HttpContext.User);
            return userId?.Id;
        }

        public IActionResult SendToEntry(Guid deviceId)
        {
            return RedirectToAction("Index", "Entry", new { deviceId });
        }

        public IActionResult SendToCode(Guid deviceId)
        {
            return RedirectToAction("Index", "Code", new { deviceId });
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyName(string name)
        {
            if (!await _repository.DeviceExistsAsync(name, await GetCurrentUserAsync())) return Json(true);
            return Json($"Device named {name} already exists!");
        }
    }
}
