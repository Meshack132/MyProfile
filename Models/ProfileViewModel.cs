using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Myprofile.Models
{
    public class ProfileViewModel
    {
        public IFormFile CvFile { get; set; }
    }
}