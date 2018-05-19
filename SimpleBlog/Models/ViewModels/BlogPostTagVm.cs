using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models.ViewModels
{
    public class BlogPostTagVm
    {
        public int Id { get; set; }

        [Required]
        public string TagText { get; set; }

        public IEnumerable<BlogPostTag> BlogPostTagList { get; set; }
    }
}