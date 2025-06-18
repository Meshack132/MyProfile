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
            if (model.CvFile == null || model.CvFile.Length == 0L)
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

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

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

        public IActionResult DownloadCV()
        {
            var filePath = Path.Combine(_env.WebRootPath, CV_FOLDER, CV_FILENAME);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, "application/pdf", CV_FILENAME);
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

            ViewBag.Projects = new List<ProjectViewModel>
            {
                
                new ProjectViewModel
                {
                    Title = "Book API",
                    Description = "CRUD API to manage book records. Great for learning ASP.NET Core, API design, model validation, and clean architecture.",
                    Technologies = new List<string> { "C#", "ASP.NET Core", "Entity Framework", "SQL Server" },
                    Link = "https://github.com/Meshack132/BookApi",
                    ImageUrl = "/images/pic1.png"
                },
                new ProjectViewModel
                {
                    Title = "Crypto ETL Pipeline",
                    Description = "ETL pipeline that pulls real-time cryptocurrency data, transforms it, and stores it for analysis. Ideal for data engineering skills.",
                    Technologies = new List<string> { "Python", "Pandas", "APIs", "ETL" },
                    Link = "https://github.com/Meshack132/Crypto_etl",
                    ImageUrl = "/images/pic2.png"
                },
                new ProjectViewModel
                {
                    Title = "AI Study Planner",
                    Description = "Smart study planner that uses AI to generate personalized schedules based on time, goals, and topics. Includes progress tracking.",
                    Technologies = new List<string> { "Python", "Flask", "JavaScript", "OpenAI API" },
                    Link = "https://github.com/Meshack132/AI-Study-Planner",
                    ImageUrl = "/images/pic3.png"
                }
            };
        }
    }
}
