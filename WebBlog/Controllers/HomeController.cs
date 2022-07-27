using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebBlog.Models;

namespace WebBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        DbBlogContext _context = new DbBlogContext();

        public IActionResult Index(int? page)
        {
            // TẠO PHÂN TRANG CHO INDEX
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            //var pageSize = Utilities.PAGE_SIZE;//20
            var pageSize = 3;//20
            var Ispost = _context.Posts.Include(p => p.Account).Include(p => p.Cat)
                .OrderByDescending(p => p.CreateDate).ToList();
            PagedList <Post> posts = new PagedList<Post>(Ispost.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Detail(int id)
        {
            var post = _context.Posts.Where(p=> p.PostId == id).
                Include(p => p.Account).Include(p => p.Cat)
               .FirstOrDefault();
            return View(post);
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
