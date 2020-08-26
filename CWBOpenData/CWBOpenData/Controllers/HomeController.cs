using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CWBOpenData.Models;
using CWBOpenData.Services;

namespace CWBOpenData.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICWBAPIService _CWBAPIService;
        public HomeController(ICWBAPIService cwbAPIService)
        {
            _CWBAPIService = cwbAPIService;
        }

        public IActionResult Index()
        {
            var result = _CWBAPIService.GetWeatherForecastTo36Hour();
            return Json(result);
        }

        public IActionResult Create()
        {
            _CWBAPIService.CreateWeatherForecastTo36Hour();
            return Ok();
        }
    }
}
