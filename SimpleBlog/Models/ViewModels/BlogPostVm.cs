using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SimpleBlog.Models.ViewModels
{
    public class BlogPostVm
    {
        public int Id { get; set; }

        public string PostTitle { get; set; }

        [AllowHtml]
        public string PostContent { get; set; }

        public bool IsDrafted { get; set; }

        public bool IsPublished { get; set; }

        public bool IsApproved { get; set; }

        public string ExcerptText { get; set; }

        public string YoutudeUrlCodec { get; set; }

        public BlogPostCategoryVm PostCategory { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string CreatedDate { get; set; }

        public int? CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string UpdatedDate { get; set; }

        public int? UpdatedBy { get; set; }

        public string BlogPostsImageUrl { get; set; }

        public bool IsDeleted { get; set; }

        public string TagText { get; set; }

        public List<BlogPostCategoryVm> CategoryList { get; set; }
        public int? PostFormat { get; set; }
    }
}