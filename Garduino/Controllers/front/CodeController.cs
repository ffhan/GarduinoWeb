using System;
using System.Collections.Generic;
using System.Threading.Tasks;
//using System.Web.Mvc;
using Garduino.Data;
using Garduino.Models;
using Garduino.Models.ViewModels;
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

        public CodeController(ICodeRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            HashSet<Code> codes = new HashSet<Code>(_repository.GetActive(await GetCurrentUserIdAsync()));
            return View(codes);
        }

        public async Task<IActionResult> All()
        {
            HashSet<Code> codes = new HashSet<Code>(_repository.GetAll(await GetCurrentUserIdAsync()));
            return View(codes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CodeViewModel codel)
        {
            if (!ModelState.IsValid) return View(codel);
            Code code = new Code(codel);
            await _repository.AddAsync(code, await GetCurrentUserIdAsync());
            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> Details(Guid id)
        {
            var code = await _repository.GetAsync(id, await GetCurrentUserIdAsync());
            return View(code);
        }

        public string CurrentUserName => User.Identity.Name;

        private async Task<string> GetCurrentUserIdAsync()
        {
            var userId = await _userManager.GetUserAsync(HttpContext.User);
            return userId?.Id;
        }
    }
}