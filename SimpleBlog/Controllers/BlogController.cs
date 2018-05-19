using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.Models;
using SimpleBlog.Models.ViewModels;
using WebMatrix.WebData;

namespace SimpleBlog.Controllers
{
    [Authorize(Roles = "Developer,Admin,User")]
    public class BlogController : Controller
    {
        private readonly Blog_Db_Entities _entities = new Blog_Db_Entities();

        [HttpGet]
        public ActionResult Create()
        {
            var blogPostVm = new BlogPostVm();
            blogPostVm.CategoryList = CategoryListVm();
            return View(blogPostVm);
        }

        [HttpGet]
        public ActionResult Index()
        {
            var data = _entities
                .BlogPosts
                .Include(x => x.BlogPostsPostCategories)
                .Include(x => x.BlogPostsPostTags)
                .Where(x => x.IsDeleted == false)
                .OrderByDescending(x => x.Id)
                .ToList();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public ActionResult Create(BlogPostVm model, HttpPostedFileBase file, string submit)
        {
            try
            {
                string imageName = "";
                if (model.PostFormat == null)
                {
                    TempData["ErrorMsg"] = "Please select your post format";
                    return View(model);
                }
               
                if (string.IsNullOrEmpty(model.PostTitle))
                {
                    TempData["ErrorMsg"] = "Please enter Post Title";
                    return View(model);
                }
                if (model.PostFormat == 1)
                {
                    if (string.IsNullOrEmpty(model.PostContent))
                    {
                        TempData["ErrorMsg"] = "Please enter Post Description";
                        return View(model);
                    }
                }
                
                if (string.IsNullOrEmpty(model.ExcerptText))
                {
                    TempData["ErrorMsg"] = "Please enter Excerpt Text";
                    return View(model);
                }

                if (!model.CategoryList.Select(x => x.IsChecked).Any())
                {
                    TempData["ErrorMsg"] = "Please select at least one category";
                    return View(model);
                }

                if (file != null)
                {
                    long fileSize = file.ContentLength;
                    if (fileSize > 640000)
                    {
                        TempData["ErrorMsg"] = "Image file size can not be more that 512 Kb.";
                        return View(model);
                    }

                    string extension = Path.GetExtension(file.FileName);

                    if (extension != null && (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" ||
                                              extension.ToLower() == ".png"))
                    {
                        Guid imageId = Guid.NewGuid();
                        string location = Server.MapPath("~/BlogImages/");
                        var completeImageLocation = location + imageId + extension.ToLower();
                        imageName = imageId + extension.ToLower();
                        if (!Directory.Exists(location))
                        {
                            Directory.CreateDirectory(location);
                            file.SaveAs(completeImageLocation);
                        }
                        else
                        {
                            file.SaveAs(completeImageLocation);
                        }
                    }
                }

                switch (submit.ToLower())
                {
                    case "save draft":
                        {
                            model.IsDrafted = true;
                            model.IsApproved = false;
                            model.IsPublished = false;
                            model.IsDeleted = false;
                            break;
                        }
                    case "publish":
                        {
                            model.IsDrafted = false;
                            model.IsApproved = false;
                            model.IsPublished = true;
                            model.IsDeleted = false;
                            break;
                        }
                    default:
                        {
                            TempData["ErrorMsg"] = "Do you wanna publish this post or just save this post as a draft";
                            return View(model);
                        }
                }
                var userId = WebSecurity.GetUserId(User.Identity.Name);
                BlogPost post = new BlogPost();
                post.PostTitle = model.PostTitle;
                post.PostContent = model.PostContent;
                post.IsDrafted = model.IsDrafted;
                post.IsApproved = model.IsApproved;
                post.IsDeleted = model.IsDeleted;
                post.IsPublished = model.IsPublished;
                post.CreatedBy = userId;
                post.CreatedDate = DateTime.Today;
                post.UpdatedBy = userId;
                post.UpdatedDate = DateTime.Today;
                post.BlogPostsImageUrl = imageName;
                post.ExcerptText = model.ExcerptText;
                post.YoutubeVedioCodec = model.YoutudeUrlCodec;
                post.PostFormat = model.PostFormat;
                using (var dbContextTransactions = _entities.Database.BeginTransaction())
                {
                    try
                    {
                        _entities.BlogPosts.Add(post);
                        _entities.SaveChanges();

                        int postId = post.Id;

                        foreach (var category in model.CategoryList.Where(x => x.IsChecked))
                        {
                            BlogPostsPostCategory postCategory = new BlogPostsPostCategory();
                            postCategory.PostId = postId;
                            postCategory.CategoryId = category.Id;

                            _entities.BlogPostsPostCategories.Add(postCategory);
                            _entities.SaveChanges();
                        }

                        string[] allTagText = model.TagText.Split(',');
                        int tagId;
                        foreach (var tag in allTagText)
                        {
                            var tagDetails = _entities.BlogPostTags.FirstOrDefault(x => x.TagName == tag);
                            if (tagDetails == null)
                            {
                                BlogPostTag blogNewTag = new BlogPostTag();
                                blogNewTag.TagName = tag;
                                _entities.BlogPostTags.Add(blogNewTag);
                                _entities.SaveChanges();

                                tagId = blogNewTag.Id;
                            }
                            else
                            {
                                tagId = tagDetails.Id;
                            }

                            BlogPostsPostTag blogPostTag = new BlogPostsPostTag();
                            blogPostTag.TagId = tagId;
                            blogPostTag.PostId = postId;

                            _entities.BlogPostsPostTags.Add(blogPostTag);
                            _entities.SaveChanges();
                        }
                        dbContextTransactions.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransactions.Rollback();
                        throw;
                    }
                }

                if (model.IsDrafted && !model.IsPublished)
                {
                    TempData["SuccessMsg"] = "Post Successfully drafted";
                    return RedirectToAction("Create");
                }
                if (!model.IsDrafted && model.IsPublished)
                {
                    TempData["SuccessMsg"] = "Post Successfully published";
                    return RedirectToAction("Create");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.ToString();
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMsg"] = "Plese select corectly";
                return RedirectToAction("Index");
            }
            var details = _entities.BlogPosts
                .Include(x => x.BlogPostsPostCategories)
                .Include(x => x.BlogPostsPostTags)
                .SingleOrDefault(x => x.Id == id);
            if (details == null)
            {
                TempData["ErrorMsg"] = "No Information found";
                return RedirectToAction("Index");
            }

            var blogVm = new BlogPostVm();

            blogVm.Id = details.Id;
            blogVm.PostTitle = details.PostTitle;
            blogVm.PostContent = details.PostContent;
            blogVm.IsDrafted = details.IsDrafted;
            blogVm.IsPublished = details.IsPublished;
            blogVm.IsApproved = details.IsApproved;
            blogVm.ExcerptText = details.ExcerptText;
            blogVm.YoutudeUrlCodec = details.YoutubeVedioCodec;
            blogVm.CreatedDate = details.CreatedDate.ToString();
            blogVm.CreatedBy = details.CreatedBy;
            blogVm.UpdatedDate = details.UpdatedDate.ToString();
            blogVm.UpdatedBy = details.UpdatedBy;
            blogVm.BlogPostsImageUrl = details.BlogPostsImageUrl;
            blogVm.IsDeleted = details.IsDeleted;
            blogVm.TagText =
                details.BlogPostsPostTags.Select(x => x.BlogPostTag.TagName).Aggregate((i, j) => i + "," + j);
            blogVm.CategoryList = CategoryListVm();
            foreach (var initCategory in blogVm.CategoryList)
            {
                foreach (var category in details.BlogPostsPostCategories)
                {
                    if (category.CategoryId == initCategory.Id)
                    {
                        var blogPostCategoryVm = blogVm.CategoryList.FirstOrDefault(x => x.Id == initCategory.Id);
                        if (blogPostCategoryVm != null)
                            blogPostCategoryVm.IsChecked = true;
                    }
                }
            }

            return View(blogVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public ActionResult Edit(BlogPostVm model, HttpPostedFileBase file, string submit)
        {
            try
            {
                string imageName = "";
                if (string.IsNullOrEmpty(model.PostTitle))
                {
                    TempData["ErrorMsg"] = "Please enter Post Title";
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.PostContent))
                {
                    TempData["ErrorMsg"] = "Please enter Post Description";
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.ExcerptText))
                {
                    TempData["ErrorMsg"] = "Please enter Excerpt Text";
                    return View(model);
                }

                if (!model.CategoryList.Select(x => x.IsChecked).Any())
                {
                    TempData["ErrorMsg"] = "Please select at least one category";
                    return View(model);
                }

                if (file != null)
                {
                    long fileSize = file.ContentLength;
                    if (fileSize > 6400000)
                    {
                        TempData["ErrorMsg"] = "Image file size can not be more that 512 Kb.";
                        return View(model);
                    }

                    string extension = Path.GetExtension(file.FileName);

                    if (extension != null && (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" ||
                                              extension.ToLower() == ".png"))
                    {
                        Guid imageId = Guid.NewGuid();
                        string location = Server.MapPath("~/BlogImages/");
                        var completeImageLocation = location + imageId + extension.ToLower();
                        imageName = imageId + extension.ToLower();
                        if (!Directory.Exists(location))
                        {
                            Directory.CreateDirectory(location);
                            file.SaveAs(completeImageLocation);
                        }
                        else
                        {
                            file.SaveAs(completeImageLocation);
                        }
                    }
                }

                switch (submit.ToLower())
                {
                    case "save draft":
                        {
                            model.IsDrafted = true;
                            model.IsApproved = true;
                            model.IsPublished = false;
                            model.IsDeleted = false;
                            break;
                        }
                    case "publish":
                        {
                            model.IsDrafted = false;
                            model.IsApproved = true;
                            model.IsPublished = true;
                            model.IsDeleted = false;
                            break;
                        }
                    default:
                        {
                            TempData["ErrorMsg"] = "Do you wanna publish this post or just save this post as a draft";
                            return View(model);
                        }
                }
                var userId = WebSecurity.GetUserId(User.Identity.Name);
                var post = _entities.BlogPosts.Find(model.Id);
                if (post == null)
                {
                    return View(model);
                }
                post.PostTitle = model.PostTitle;
                post.PostContent = model.PostContent;
                post.IsDrafted = model.IsDrafted;
                post.IsApproved = model.IsApproved;
                post.IsDeleted = model.IsDeleted;
                post.IsPublished = model.IsPublished;
                post.UpdatedBy = userId;
                post.UpdatedDate = DateTime.Today;
                if (file != null)
                {
                    post.BlogPostsImageUrl = imageName;
                }
                post.ExcerptText = model.ExcerptText;
                post.YoutubeVedioCodec = model.YoutudeUrlCodec;

                using (var dbTransaction = _entities.Database.BeginTransaction())
                {
                    try
                    {
                        _entities.Entry(post).State = EntityState.Modified;
                        _entities.SaveChanges();

                        _entities.BlogPostsPostCategories.RemoveRange(
                            _entities.BlogPostsPostCategories.Where(x => x.PostId == post.Id));

                        foreach (var category in model.CategoryList.Where(x => x.IsChecked))
                        {
                            BlogPostsPostCategory postCategory = new BlogPostsPostCategory();
                            postCategory.PostId = model.Id;
                            postCategory.CategoryId = category.Id;

                            _entities.BlogPostsPostCategories.Add(postCategory);
                            _entities.SaveChanges();
                        }

                        _entities.BlogPostsPostTags.RemoveRange(
                            _entities.BlogPostsPostTags.Where(x => x.PostId == post.Id));

                        string[] allTagText = model.TagText.Split(',');
                        int tagId;
                        foreach (var tag in allTagText)
                        {
                            var tagDetails = _entities.BlogPostTags.FirstOrDefault(x => x.TagName == tag);
                            if (tagDetails == null)
                            {
                                BlogPostTag blogNewTag = new BlogPostTag();
                                blogNewTag.TagName = tag;
                                _entities.BlogPostTags.Add(blogNewTag);
                                _entities.SaveChanges();

                                tagId = blogNewTag.Id;
                            }
                            else
                            {
                                tagId = tagDetails.Id;
                            }

                            BlogPostsPostTag blogPostTag = new BlogPostsPostTag();
                            blogPostTag.TagId = tagId;
                            blogPostTag.PostId = model.Id;

                            _entities.BlogPostsPostTags.Add(blogPostTag);
                            _entities.SaveChanges();
                        }
                        dbTransaction.Commit();
                        TempData["SuccessMsg"] = "Post updated successfully";
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        dbTransaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.ToString();
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMsg"] = "Please select correctly";
                return RedirectToAction("Index");
            }
            var details = _entities.BlogPosts.Find(id);
            if (details == null)
            {
                TempData["ErrorMsg"] = "No data found";
                return RedirectToAction("Index");
            }

            details.IsDeleted = true;
            _entities.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult CreateCategory(BlogPostVm model)
        {
            model.CategoryList = CategoryListVm();
            if (ModelState.IsValid)
            {
                if (_entities.BlogPostCategories.Where(x => x.CategoryName.ToLower().Trim() == model.PostCategory.CategoryName.ToLower().Trim()).Any())
                {
                    TempData["ErrorMsg"] = "There is already a Category on same name";
                    return PartialView("_categories_partial", model);
                }
                BlogPostCategory category = new BlogPostCategory();
                category.CategoryName = model.PostCategory.CategoryName;
                _entities.BlogPostCategories.Add(category);
                _entities.SaveChanges();

                ModelState.Clear();
                model.PostCategory = new BlogPostCategoryVm();

                model.CategoryList = CategoryListVm();
            }
            else
            {
                TempData["ErrorMsg"] = "Please enter Category name";
            }
            return PartialView("_categories_partial", model);
        }

        private List<BlogPostCategoryVm> CategoryListVm()
        {
            var categoryVmList = new List<BlogPostCategoryVm>();
            var categoryList = _entities.BlogPostCategories.OrderByDescending(x => x.Id).ToList();
            foreach (var cat in categoryList)
            {
                var categoryVm = new BlogPostCategoryVm();
                categoryVm.Id = cat.Id;
                categoryVm.CategoryName = cat.CategoryName;
                categoryVm.IsChecked = false;
                categoryVmList.Add(categoryVm);
            }
            return categoryVmList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Developer")]
        public ActionResult ApproveOrReject(int? id, string type)
        {
            if (id == null)
            {
                TempData["ErrorMsg"] = "Please request correctly";
                return RedirectToAction("Index");
            }

            var details = _entities.BlogPosts.Find(id);
            if (details == null)
            {
                TempData["ErrorMsg"] = "No Information found";
                return RedirectToAction("Index");
            }

            switch (type)
            {
                case "Approve":
                {
                    details.IsApproved = true;
                    _entities.Entry(details).State = EntityState.Modified;
                    _entities.SaveChanges();
                    TempData["SuccessMsg"] = "Approved Successfully";
                    break;
                }
                case "Reject":
                {
                    details.IsApproved = false;
                    _entities.Entry(details).State = EntityState.Modified;
                    _entities.SaveChanges();
                    TempData["SuccessMsg"] = "Rejected Successfully";
                    break;
                }
                default:
                {
                    TempData["ErrorMsg"] = "Nothing Heppens";
                    break;
                }
            }

            return RedirectToAction("Index");
        }
    }
}