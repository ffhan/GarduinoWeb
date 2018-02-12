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
    [Authorize] //TODO:  create Device model.
    public class CodeController : Controller
    {

        private readonly ICodeRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        private delegate IEnumerable<Code> RepositoryQuery(string id, ApplicationUser user); //used to ease search

        public CodeController(ICodeRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string id) => await SearchOperator(_repository.GetDeviceFromActiveCodes, _repository.GetActive, id);

        public async Task<IActionResult> All(string id) => await SearchOperator(_repository.GetDevice, _repository.GetAll, id);

        private async Task<IActionResult> SearchOperator(RepositoryQuery mainSearch, Func<ApplicationUser, IEnumerable<Code>> alternativeSearch, string id)
        {
            string trimmed = StringOperations.PrepareForSearch(id);
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                ViewData["searchInput"] = trimmed;
                return View(mainSearch.Invoke(id, await GetCurrentUser()));
            }
            return View(alternativeSearch.Invoke(await GetCurrentUser()));
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Action,ActionName,DeviceName")] Code code)
        {
            if (!ModelState.IsValid) return View(code);
            await _repository.AddAsync(code, await GetCurrentUser());
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            return View(await _repository.GetAsync(id.Value, await GetCurrentUser()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, Action, ActionName, IsCompleted, DeviceName")] Code code)
        {
            if (id != code.Id) return NotFound();

            if (!ModelState.IsValid) return View(code);
            try
            {
                await _repository.UpdateAsync(id, code, await GetCurrentUser());
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ContainsAsync(code, await GetCurrentUser()))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Details(Guid id)
        {
            var code = await _repository.GetAsync(id, await GetCurrentUser());
            return View(code);
        }

        public string CurrentUserName => User.Identity.Name;

        private async Task<ApplicationUser> GetCurrentUser() => await _userManager.GetUserAsync(HttpContext.User);

        private async Task<string> GetCurrentUserIdAsync()
        {
            var userId = await _userManager.GetUserAsync(HttpContext.User);
            return userId?.Id;
        }
    }
}