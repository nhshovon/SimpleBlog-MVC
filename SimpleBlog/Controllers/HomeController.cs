using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using SimpleBlog.Models;
using SimpleBlog.Models.ViewModels;

namespace SimpleBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly Blog_Db_Entities _entities = new Blog_Db_Entities();

        public ActionResult Index(string filterBy = null, int? filterTypeId = null, int? year = null, int? month = null, int? page = null)
        {
            var blogPageDetails = new BlogPageVm {BlogPostsList = new List<BlogPost>()};

            ViewBag.filterBy = filterBy;
            ViewBag.filterTypeId = filterTypeId;
            ViewBag.year = year;
            ViewBag.month = month;

            if (page == null)
            {
                page = 1;
            }
            if (string.IsNullOrEmpty(filterBy))
            {
                blogPageDetails.BlogPostsList = _entities.BlogPosts.Where(x => x.IsDeleted == false && x.IsPublished && x.IsApproved && x.PostFormat == 1).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                switch (filterBy)
                {
                    case "category":
                        {
                            if (filterTypeId != null)
                            {
                                var categoryInfo = _entities.BlogPostCategories.Find(filterTypeId);
                                if (categoryInfo != null && categoryInfo.CategoryName.Equals("Educational Video"))
                                {
                                    blogPageDetails.BlogPostsList =
                                    _entities.BlogPostsPostCategories.Where(x => x.CategoryId == filterTypeId)
                                    .Select(x => x.BlogPost).Where(x => x.IsDeleted == false && x.IsApproved && x.IsPublished && x.PostFormat == 2).OrderByDescending(x => x.Id)
                                    .ToList();
                                }
                                else
                                {
                                    blogPageDetails.BlogPostsList =
                                    _entities.BlogPostsPostCategories.Where(x => x.CategoryId == filterTypeId)
                                    .Select(x => x.BlogPost).Where(x => x.IsDeleted == false && x.IsApproved && x.IsPublished && x.PostFormat == 1).OrderByDescending(x => x.Id)
                                    .ToList();
                                }

                            }
                            else
                            {
                                TempData["ErrorMsg"] = "There is noting found in this category";
                                blogPageDetails.BlogPostsList = new List<BlogPost>();
                            }
                            break;
                        }
                    case "tag":
                        {
                            if (filterTypeId != null)
                            {
                                blogPageDetails.BlogPostsList =
                           _entities.BlogPostsPostTags.Where(x => x.TagId == filterTypeId)
                               .Select(x => x.BlogPost).Where(x => x.IsDeleted == false && x.IsApproved && x.IsPublished && x.PostFormat == 1).OrderByDescending(x => x.Id)
                               .ToList();
                            }
                            else
                            {
                                TempData["ErrorMsg"] = "There is noting found in this tags";
                                blogPageDetails.BlogPostsList = new List<BlogPost>();
                            }
                            break;
                        }
                    case "archives":
                        {
                            if (year != null && month != null)
                            {
                                var searchStartDate = new DateTime(year.Value, month.Value, 1);
                                var searchEndDate = searchStartDate.AddMonths(1).AddDays(-1);

                                blogPageDetails.BlogPostsList =
                                    _entities.BlogPosts
                                        .Include(x => x.BlogPostsPostCategories)
                                        .Include(x => x.BlogPostsPostTags)
                                    .Where(x => x.CreatedDate >= searchStartDate && x.CreatedDate <= searchEndDate && x.IsDeleted == false && x.IsApproved && x.IsPublished && x.PostFormat == 1).OrderByDescending(x => x.Id)
                                   .ToList();
                            }
                            else
                            {
                                TempData["ErrorMsg"] = "There is noting found in this archives";
                                blogPageDetails.BlogPostsList = new List<BlogPost>();
                            }
                            break;
                        }
                }
            }
            int pageNumber = ((int) page);
            return View(blogPageDetails.BlogPostsList.ToPagedList(pageNumber, 10));
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var postDetails = _entities.BlogPosts
                .Include(x => x.BlogPostsPostCategories).FirstOrDefault(x => x.Id == id && x.IsApproved && x.IsPublished);
            if (postDetails == null)
            {
                return RedirectToAction("Index");
            }
            return View(postDetails);
        }

        [HttpGet]
        public ActionResult AboutMe()
        {
            return View();
        }
    }
}