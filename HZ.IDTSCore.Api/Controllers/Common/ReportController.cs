using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.Common
{
    public class ReportController : Controller
    {
        private readonly IHostingEnvironment _env;
        public ReportController(IHostingEnvironment env) => _env = env;

        //public IActionResult Index()
        //{
        //    return new PhysicalFileResult(Path.Combine(_env.WebRootPath, "index.html"), "text/html");
        //}

    }
}