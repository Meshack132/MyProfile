using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Myprofile.Models;

namespace Myprofile.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private const string CV_FOLDER = "CVs";
        private const string CV_FILENAME = "Meshack_CV.pdf";

        public HomeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            SetViewBagData();
            return View(new ProfileViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> UploadCV(ProfileViewModel model)
        {
            // Manual validation
            if (model.CvFile == null || model.CvFile.Length == 0)
            {
                ModelState.AddModelError("CvFile", "Please select a PDF file");
            }
            else
            {
                var extension = Path.GetExtension(model.CvFile.FileName).ToLower();
                if (extension != ".pdf")
                {
                    ModelState.AddModelError("CvFile", "Only PDF files allowed");
                }
            }

            if (!ModelState.IsValid)
            {
                SetViewBagData();
                return View("Index", model);
            }

            var uploads = Path.Combine(_env.WebRootPath, CV_FOLDER);
            Directory.CreateDirectory(uploads);
            var filePath = Path.Combine(uploads, CV_FILENAME);

            // Delete existing file if exists
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Save new file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.CvFile.CopyToAsync(stream);
            }

            TempData["Message"] = "CV uploaded successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult ViewCV()
        {
            var filePath = Path.Combine(_env.WebRootPath, CV_FOLDER, CV_FILENAME);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            return PhysicalFile(filePath, "application/pdf");
        }

        private void SetViewBagData()
        {
            ViewBag.GitHubUrl = "https://github.com/Meshack132";
            ViewBag.LinkedInUrl = "https://www.linkedin.com/in/meshackmthimkhulu-software-engineer/";

            ViewBag.Stacks = new List<string>
            {
                "C#", "ASP.NET Core", "Entity Framework",
                "JavaScript", "HTML/CSS", "SQL Server",
                "Git", "Azure", "REST APIs"
            };

            ViewBag.HasCV = System.IO.File.Exists(Path.Combine(
                _env.WebRootPath,
                CV_FOLDER,
                CV_FILENAME
            ));
        }
    }
}