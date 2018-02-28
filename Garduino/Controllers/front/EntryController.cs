using System;
using System.Collections;
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
using ChartJSCore.Models;
using Chart = ChartJSCore.Models.Chart;

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

        public async Task<IList<ChartJSCore.Models.Chart>> CreateCharts(IEnumerable<Entry> measures)
        {
            ChartJSCore.Models.Chart soilM = new ChartJSCore.Models.Chart();
            ChartJSCore.Models.Chart air = new ChartJSCore.Models.Chart();

            soilM.Type = "line";
            air.Type = "line";

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();
            ChartJSCore.Models.Data data2 = new ChartJSCore.Models.Data();
            data.Labels = measures.Select(g => g.DateTime.ToString("G")).ToList();
            data2.Labels = measures.Select(g => g.DateTime.ToString("G")).ToList();

            LineDataset dataset = new LineDataset()
            {//collection.Skip(Math.Max(0, collection.Count() - N));
                Label = "Soil moisture",
                Data = measures.Skip(Math.Max(0, measures.Count() - 350)).Select(g => (double)g.SoilMoisture).ToList(),
                Fill = false,
                LineTension = 0.1,
                BackgroundColor = "rgba(75, 192, 192, 0.4)",
                BorderColor = "rgba(75,192,192,1)",
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<string>() { "rgba(75,192,192,1)" },
                PointBackgroundColor = new List<string>() { "#fff" },
                PointBorderWidth = new List<int> { 1 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<string>() { "rgba(75,192,192,1)" },
                PointHoverBorderColor = new List<string>() { "rgba(220,220,220,1)" },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };
            LineDataset dataset2 = new LineDataset()
            {
                Label = "Air humidity",
                Data = measures.Skip(Math.Max(0, measures.Count() - 350)).Select(g => (double)g.AirHumidity).ToList(),
                Fill = false,
                LineTension = 0.1,
                BackgroundColor = "rgba(75, 192, 192, 0.4)",
                BorderColor = "rgba(75,192,192,1)",
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<string>() { "rgba(75,192,192,1)" },
                PointBackgroundColor = new List<string>() { "#fff" },
                PointBorderWidth = new List<int> { 1 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<string>() { "rgba(75,192,192,1)" },
                PointHoverBorderColor = new List<string>() { "rgba(75,192,192,1)" },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };
            LineDataset dataset3 = new LineDataset()
            {
                Label = "Air temperature",
                Data = measures.Skip(Math.Max(0, measures.Count() - 350)).Select(g => (double)g.AirTemperature).ToList(),
                Fill = false,
                LineTension = 0.1,
                BackgroundColor = "rgba(200, 60, 40, 0.4)",
                BorderColor = "rgba(200, 60, 40,1)",
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<string>() { "rgba(200, 60, 40,1)" },
                PointBackgroundColor = new List<string>() { "#fff" },
                PointBorderWidth = new List<int> { 1 },
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<string>() { "rgba(200, 60, 40,1)" },
                PointHoverBorderColor = new List<string>() { "rgba(200, 60, 40,1)" },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };

            data.Datasets = new List<Dataset>();
            data2.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);
            data2.Datasets.Add(dataset2);
            data2.Datasets.Add(dataset3);

            soilM.Data = data;
            air.Data = data2;

            return new List<ChartJSCore.Models.Chart> {soilM, air};
        }

        // GET: Entry
        public async Task<IActionResult> Index(Guid deviceId)
        {
            IEnumerable<Entry> measures = _repository.GetAll(await GetDeviceAsync(deviceId)).Reverse();

            IList<ChartJSCore.Models.Chart> charts = await CreateCharts(measures);

            ViewData["chart"] = charts[0];
            ViewData["chart2"] = charts[1];

            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View(measures);
        }

        // GET: Entry/Details/5
        public async Task<IActionResult> Details(Guid? id)
        { 
            if (!id.HasValue) return NotFound();
            Entry entry = await _repository.GetAsync(id.Value);
            Guid deviceId = entry.Device.Id;
            ViewData[GarduinoConstants.DeviceId] = deviceId;
            return View(entry);
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
        public async Task<IActionResult> Create(Guid deviceId, [Bind("Id, DateTime,SoilMoisture,SoilDescription,AirHumidity,AirTemperature,LightState,DeviceName")] Entry entry)
        {
            if (!ModelState.IsValid) return View(entry);
            await _repository.AddAsync(entry, await GetDeviceAsync(deviceId));
            return RedirectToAction(nameof(Index), new { deviceId });
        }

        // GET: Entry/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            Entry entry = await _repository.GetAsync(id.Value);
            if (entry == null) return NotFound();
            Guid deviceId = await GetDeviceId(id.Value);
            ViewData["deviceId"] = deviceId;
            return View(entry);
        }

        // POST: Entry/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, DateTime,SoilMoisture,SoilDescription,AirHumidity,AirTemperature,LightState,DeviceName")] Entry entry)
        {
            if (id != entry.Id) return NotFound();
            if (!ModelState.IsValid) return View(entry);
            try
            {
                await _repository.UpdateAsync(id, entry);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MeasureExists(entry.Id))
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
            Entry entry = await _repository.GetAsync(id.Value);
            if (entry == null) return NotFound();
            Guid deviceId = entry.Device.Id;
            ViewData["deviceId"] = deviceId;
            return View(entry);
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

        public class AjaxPost
        {
            public Guid DeviceId { get; set; }
        }

        [HttpPost]
        public async Task<PartialViewResult> GetCharts([FromBody] AjaxPost ajaxPost)
        {
            IEnumerable<Entry> measures = _repository.GetAll(await GetDeviceAsync(ajaxPost.DeviceId));
            IList<ChartJSCore.Models.Chart> charts = await CreateCharts(measures);
            return PartialView("Graphs", charts);
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
