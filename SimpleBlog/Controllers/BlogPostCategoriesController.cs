using System;
using System.Linq;
using System.Web.Mvc;
using SimpleBlog.Models;
using SimpleBlog.Models.ViewModels;

namespace SimpleBlog.Controllers
{
    [Authorize(Roles = "Developer,Admin,User")]
    public class BlogPostCategoriesController : Controller
    {
        private readonly Blog_Db_Entities entities = new Blog_Db_Entities();

        public ActionResult Index()
        {
            BlogPostCategoryVm model = new BlogPostCategoryVm();
            model.PostCategoriesList = entities.BlogPostCategories.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult create(BlogPostCategoryVm model)
        {
            model.PostCategoriesList = entities.BlogPostCategories.ToList();
            if (ModelState.IsValid)
            {
                if (entities.BlogPostCategories.Where(x => x.CategoryName.ToLower().Trim() == model.CategoryName.ToLower().Trim()).Any())
                {
                    TempData["ErrorMsg"] = "There is already a Category on same name";
                    return PartialView("Blog_post_categories_partial", model);
                }
                BlogPostCategory category = new BlogPostCategory();
                category.CategoryName = model.CategoryName;
                entities.BlogPostCategories.Add(category);
                entities.SaveChanges();

                TempData["SuccessMsg"] = "Category created successfully";
                model.PostCategoriesList = entities.BlogPostCategories.ToList();
            }
            else
            {
                TempData["ErrorMsg"] = "Please enter Category name";
            }
            return PartialView("Blog_post_categories_partial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult delete(int id)
        {
            var model = new BlogPostCategoryVm();
            model.PostCategoriesList = entities.BlogPostCategories.ToList();
            try
            {
                var details = entities.BlogPostCategories.Find(id);
                if (details == null)
                {
                    TempData["ErrorMsg"] = "No data found";
                    return PartialView("Blog_post_categories_partial", model);
                }

                entities.BlogPostCategories.Remove(details);
                entities.SaveChanges();
                TempData["SuccessMsg"] = "Category Removed successfully";
                model.PostCategoriesList = entities.BlogPostCategories.ToList();
            }
            catch (Exception)
            {
                TempData["ErrorMsg"] = "You can not remove this category";
            }
            return PartialView("Blog_post_categories_partial", model);
        }
    }
}