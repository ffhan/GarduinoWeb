using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartJSCore.Models;
using ChartJSCore.Models.Bar;
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
using Microsoft.EntityFrameworkCore.Diagnostics;
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

        public async Task<IActionResult> Index(Guid deviceId, string searchString)
        {
            string srch = StringOperations.PrepareForSearch(searchString);
            Device dev = await GetDeviceAsync(deviceId);
            IEnumerable<Code> codes = _repository.GetActive(dev);
            if (!string.IsNullOrWhiteSpace(srch))
            {
                codes = codes?.Where(g => g.ActionName.Equals(srch));
                ViewData["searchInput"] = srch;
            }
            List<ChartJSCore.Models.Chart> charts = CreateCharts(dev);
            ViewData["freq"] = charts[0];
            ViewData["compl"] = charts[1];
            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View(codes);
        }

        private List<ChartJSCore.Models.Chart> CreateCharts(Device device)
        {
            int binNum = 6;

            IEnumerable<Code> codes = _repository.GetAll(device);
            var freq = codes.GroupBy(x => x.Action).ToDictionary(x => x.Key, x => x.Count());
            IEnumerable<double> compl = codes.Where(x => x.DateArrived != DateTime.MinValue && x.DateCompleted != DateTime.MinValue &&
                                        x.DateExecuted != DateTime.MinValue).Select(x => (x.DateCompleted - x.DateArrived).TotalSeconds);

            ChartJSCore.Models.Chart frequency = new ChartJSCore.Models.Chart();
            ChartJSCore.Models.Chart timeToComplete = new ChartJSCore.Models.Chart();

            frequency.Type = "bar";
            timeToComplete.Type = "bar";


            ChartJSCore.Models.Data freqData = new ChartJSCore.Models.Data();
            freqData.Labels = freq.Keys.Select(x => x.ToString()).ToList();

            double lowerCompl;
            double upperCompl;
            if (compl.Any())
            {
                lowerCompl = compl.Min();
                upperCompl = compl.Max();
            }
            else
            {
                lowerCompl = 0;
                upperCompl = 0;
            }
            
            double complRange = (upperCompl - lowerCompl) / binNum;

            List<string> complBins = new List<string>();
            for (int i = 0; i < binNum; i++)
            {
                complBins.Add($"{lowerCompl + i * complRange:f1}-{lowerCompl + (i + 1) * complRange:f1}");
            }

            int[] complPoints = new int[binNum];
            foreach (var value in compl)
            {
                int bucketIndex = 0;
                if (complRange > 0.0)
                {
                    bucketIndex = (int)((value - lowerCompl) / complRange);
                    if (bucketIndex == binNum)
                    {
                        bucketIndex--;
                    }
                }
                complPoints[bucketIndex]++;
            }


            ChartJSCore.Models.Data completeData = new ChartJSCore.Models.Data();
            completeData.Labels = complBins;

            BarDataset freqDataset = new BarDataset()
            {
                Label = "Frequency of Actions",
                Data = freq.Values.Select(x => (double)x).ToList(),
                BackgroundColor = new List<string>(),
                BorderWidth = new List<int>() { 1 }
            };

            freqData.Datasets = new List<Dataset>();
            freqData.Datasets.Add(freqDataset);

            BarDataset complDateDataset = new BarDataset()
            {
                Label = "Complete time in seconds",
                Data = complPoints.Select(x => (double)x).ToList(),
                BackgroundColor = new List<string>(),
                BorderWidth = new List<int>() { 1 }
            };

            completeData.Datasets = new List<Dataset>();
            completeData.Datasets.Add(complDateDataset);

            frequency.Data = freqData;
            timeToComplete.Data = completeData;

            BarOptions options = new BarOptions()
            {
                Scales = new Scales(),
                BarPercentage = 0.7
            };

            Scales scales = new Scales()
            {
                YAxes = new List<Scale>()
                {
                    new CartesianScale()
                    {
                        Ticks = new CartesianLinearTick()
                        {
                            BeginAtZero = true
                        }
                    }
                }
            };

            options.Scales = scales;

            frequency.Options = options;
            timeToComplete.Options = options;

            return new List<ChartJSCore.Models.Chart>
            {
                frequency,
                timeToComplete
            };
        }

        public async Task<IActionResult> All(Guid deviceId, string searchString)
        {
            string srch = StringOperations.PrepareForSearch(searchString);
            Device dev = await GetDeviceAsync(deviceId);
            IEnumerable<Code> codes = _repository.GetAll(dev);
            if (!string.IsNullOrWhiteSpace(srch))
            {
                codes = codes?.Where(g => g.ActionName.Equals(srch));
                ViewData["searchInput"] = srch;
            }
            List<ChartJSCore.Models.Chart> charts = CreateCharts(dev);
            ViewData["freq"] = charts[0];
            ViewData["compl"] = charts[1];
            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View(codes);
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

        private async Task<IActionResult> CreateExisting(Code code, Guid deviceId)
        {
            Device dev = await _deviceRepository.GetAsync(deviceId);
            if (dev == null) return NotFound();
            await _repository.AddAsync(code, dev);
            return RedirectToAction(nameof(Index), new {deviceId});
        }

        public async Task<IActionResult> CreateGlobalLock(Guid deviceId)
        {
            Code code = new Code(0, "Global Lock");
            return await CreateExisting(code, deviceId);
        }
        public async Task<IActionResult> CreateLightAdmin(Guid deviceId)
        {
            Code code = new Code(4, "Light Admin");
            return await CreateExisting(code, deviceId);
        }
        public async Task<IActionResult> CreateLightState(Guid deviceId)
        {
            Code code = new Code(1, "Light state");
            return await CreateExisting(code, deviceId);
        }
        public async Task<IActionResult> CreateMeasure(Guid deviceId)
        {
            Code code = new Code(5, "Measure");
            return await CreateExisting(code, deviceId);
        }
        public async Task<IActionResult> CreatePrintTime(Guid deviceId)
        {
            Code code = new Code(8, "Print time");
            return await CreateExisting(code, deviceId);
        }
        public async Task<IActionResult> CreateHeatState(Guid deviceId)
        {
            Code code = new Code(3, "Heating state");
            return await CreateExisting(code, deviceId);
        }
        public async Task<IActionResult> CreateHeatAdmin(Guid deviceId)
        {
            Code code = new Code(6, "Heat admin");
            return await CreateExisting(code, deviceId);
        }
    }
}