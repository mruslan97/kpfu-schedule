using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace BotHost.Controllers
{
    public class StatusController : Controller
    {
        /// <summary> Check server </summary>
        [HttpGet("/")]
        public string Index()
        {
            return $"{DateTime.Now.ToString(CultureInfo.CurrentCulture)} Schedule server is working fine";
        }   
    }
}