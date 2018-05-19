using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SimpleBlog.Models.ViewModels
{
    public class BlogPostCategoryVm
    {
        public int Id { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public bool IsChecked { get; set; }
        public IEnumerable<BlogPostCategory> PostCategoriesList { get; set; }
    }
}