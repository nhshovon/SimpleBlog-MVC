using System.Linq;
using System.Web.Mvc;
using SimpleBlog.Models;
using SimpleBlog.Models.ViewModels;

namespace SimpleBlog.Controllers
{
    [Authorize(Roles = "Developer,Admin,User")]
    public class tagController : Controller
    {
        private Blog_Db_Entities _entities = new Blog_Db_Entities();

        [HttpGet]
        public ActionResult Index()
        {
            BlogPostTagVm tagVm = new BlogPostTagVm();
            tagVm.BlogPostTagList = _entities.BlogPostTags.ToList();
            return View(tagVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult create(BlogPostTagVm model)
        {
            model.BlogPostTagList = _entities.BlogPostTags.ToList();
            if (!ModelState.IsValid)
            {
                TempData["ErrorMsg"] = "Please enter tag text";
                return PartialView("_create_partial", model);
            }

            BlogPostTag tag = new BlogPostTag();
            tag.TagName = model.TagText;
            _entities.BlogPostTags.Add(tag);
            _entities.SaveChanges();
            TempData["SuccessMsg"] = "Tag Created Successfully";

            model.BlogPostTagList = _entities.BlogPostTags.ToList();

            return PartialView("_create_partial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult Delete(int? id)
        {
            var model = new BlogPostTagVm();
            model.BlogPostTagList = _entities.BlogPostTags.ToList();
            if (id == null)
            {
                TempData["ErrorMsg"] = "Please select category to delete";
                return PartialView("_create_partial", model);
            }
            var obj = _entities.BlogPostTags.Find(id);
            if (obj == null)
            {
                TempData["ErrorMsg"] = "Nothing's found";
                return PartialView("_create_partial", model);
            }
            _entities.BlogPostTags.Remove(obj);
            _entities.SaveChanges();
            TempData["SuccessMsg"] = "Tag removed successfully";
            model.BlogPostTagList = _entities.BlogPostTags.ToList();
            return PartialView("_create_partial", model);
        }
    }
}