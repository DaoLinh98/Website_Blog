using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebBlog.Models;

namespace WebBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize()]
    public class PostsController : Controller
    {
        private readonly DbBlogContext _context;

        public PostsController(DbBlogContext context)
        {
            _context = context;
        }

        // GET: Admin/Posts
    

         public IActionResult Index(int? page)
        {
            //Kiểm tra quyên tuy cập
            //if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            //var taikhoanID = HttpContext.Session.GetString("AccountId");
            //if (taikhoanID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            //var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId ==
            //int.Parse(taikhoanID));
            //if (account == null) return NotFound();


            // TẠO PHÂN TRANG CHO INDEX
            var pageNumber = page == null || page <= 0 ? 1 : page.Value ;
          //  var pageSize = Utilities.PAGE_SIZE;//20
            var pageSize =1;//20
            var IsCategories = _context.Posts.Include(p => p.Account).Include(p => p.Cat).OrderByDescending(x => x.CreateDate);
            PagedList<Post> models = new PagedList<Post>(IsCategories, pageNumber, pageSize);

          ViewBag.CurrentPage = pageNumber;
            return View(models);

        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId");
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatId");
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,ShortContents,Contents,Thumb,Published,Alias,CreateDate,Author,AccountId,Tags,CatId,IsHot,IsNewfeed")] Post post)
        {
            //Kiểm tra quyên tuy cập
            //if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            //var taikhoanID = HttpContext.Session.GetString("AccountId");
            //if (taikhoanID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            //var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId ==
            //int.Parse(taikhoanID));
            //if (account == null) return NotFound();
            //

            //Create
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatId", post.CatId);
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatId", post.CatId);
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,ShortContents,Contents,Thumb,Published,Alias,CreateDate,Author,AccountId,Tags,CatId,IsHot,IsNewfeed")] Post post)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

      //Kiểm tra quyên tuy cập
            //if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            //var taikhoanID = HttpContext.Session.GetString("AccountId");
            //if (taikhoanID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            //var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId ==
            //int.Parse(taikhoanID));
            //if (account == null) return NotFound();
            //
    //Kiểm tra dúng thông tin bài đăng
            //if (account.RoleId != 1)
            //{
            //    if (post.AccountId != account.AccountId) return RedirectToAction(nameof(Index));
            //}




            //
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "AccountId", post.AccountId);
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatId", post.CatId);
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
