﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebBlog.Helpers;
using WebBlog.Models;

namespace WebBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly DbBlogContext _context;

        public CategoriesController(DbBlogContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories Anh paging
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Categories.ToListAsync());
        //}

        public IActionResult Index(int? page)
        {
            // TẠO PHÂN TRANG CHO INDEX
            var pageNumber = page == null || page <= 0 ? 1 : page.Value ;
          //  var pageSize = Utilities.PAGE_SIZE;//20
            var pageSize =1;//20
            var IsCategories = _context.Categories.OrderByDescending(x => x.CatId);
            
            PagedList<Category> models = new PagedList<Category>(IsCategories.AsEnumerable(), pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);

        }



        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            ViewData["DanhMucGoc"] = new SelectList(_context.Categories.Where(x => x.Levers ==1), "CatId", "CatName");
            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CatName,Title,Alias,MetaDesc,MetaKey,Thumb,Published,Oedering,Parents,Levers,Icon,Cover,Description")] Category category, Microsoft.AspNetCore.Http.IFormFile fThumb, Microsoft.AspNetCore.Http.IFormFile fIcon, Microsoft.AspNetCore.Http.IFormFile fCover)
        {
            if (ModelState.IsValid)
            {
                category.Alias = category.CatName;
                if(category.Parents == null)
                {
                    category.Levers = 1;
                }
                else
                {
                    category.Levers = category.Parents == 0 ? 1 : 2;
                }
                //
                if(fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string Newname = category.CatName + "preview_" + extension;
                    category.Thumb = await Utilities.UploadFile(fThumb, @"categories\", Newname.ToLower());
                }
                //
                if (fIcon != null)
                {
                    string extension = Path.GetExtension(fIcon.FileName);
                    string Newname = category.CatName + "preview_" + extension;
                    category.Thumb = await Utilities.UploadFile(fIcon, @"icons\", Newname.ToLower());
                }
                //
                if (fCover != null)
                {
                    string extension = Path.GetExtension(fCover.FileName);
                    string Newname = category.CatName + "preview_" + extension;
                    category.Thumb = await Utilities.UploadFile(fCover, @"covers\", Newname.ToLower());
                }
                //
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatId,CatName,Title,Alias,MetaDesc,MetaKey,Thumb,Published,Oedering,Parents,Levers,Icon,Cover,Description")] Category category,
            Microsoft.AspNetCore.Http.IFormFile fThumb, Microsoft.AspNetCore.Http.IFormFile fIcon, Microsoft.AspNetCore.Http.IFormFile fCover)
        {
            if (id != category.CatId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //
                    category.Alias = category.CatName;
                    if (category.Parents == null)
                    {
                        category.Levers = 1;
                    }
                    else
                    {
                        category.Levers = category.Parents == 0 ? 1 : 2;
                    }
                    //
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string Newname = category.CatName + "preview_" + extension;
                        category.Thumb = await Utilities.UploadFile(fThumb, @"categories\", Newname.ToLower());
                    }
                    //
                    if (fIcon != null)
                    {
                        string extension = Path.GetExtension(fIcon.FileName);
                        string Newname = category.CatName + "preview_" + extension;
                        category.Thumb = await Utilities.UploadFile(fIcon, @"icons\", Newname.ToLower());
                    }
                    //
                    if (fCover != null)
                    {
                        string extension = Path.GetExtension(fCover.FileName);
                        string Newname = category.CatName + "preview_" + extension;
                        category.Thumb = await Utilities.UploadFile(fCover, @"covers\", Newname.ToLower());
                    }
                    //

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CatId))
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
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CatId == id);
        }
    }
}
