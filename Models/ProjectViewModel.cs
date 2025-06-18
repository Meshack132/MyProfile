using System.Collections.Generic;

namespace Myprofile.Models
{
    public class ProjectViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Technologies { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
    }
}