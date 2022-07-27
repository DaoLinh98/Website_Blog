using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using WebBlog.Areas.Admin.Models;
using WebBlog.Extension;
using WebBlog.Helpers;
using WebBlog.Models;

namespace WebBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
   [Authorize(Roles ="Admin")]
    public class AccountsController : Controller
    {
        private readonly DbBlogContext _context;

        public AccountsController(DbBlogContext context)
        {
            _context = context;
        }

        // GET: Admin/Accounts
      
       
        public IActionResult Index(int? page)
        {
            // TẠO PHÂN TRANG CHO INDEX
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE;//20
            var IsAccount = _context.Accounts.Include(a => a.Role).OrderByDescending(x => x.CreateDate);

            PagedList<Account> models = new PagedList<Account>(IsAccount, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);

        }



        //Get : Admin/Login
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap.html",Name ="Login")]
        
        public IActionResult Login(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID != null) return RedirectToAction("Index", "Home", new { Area = "Admin" });
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "Login")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnURL = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Account kh = _context.Accounts.Include(p => p.Role)
                        .SingleOrDefault(p => p.Email.ToLower() == model.Email.ToLower().Trim());
                    if(kh == null)
                    {
                        ViewBag.Error = "Thông tin người dùng không chính xác vui lòng nhạp lại!.";
                        return View(model);
                    }
                    string pass = (model.Password.Trim());
                    if(kh.Password.Trim() != pass)
                    {
                        ViewBag.Error = "Thông tin người dùng không chính xác vui lòng nhạp lại!.";
                        return View(model);
                    }

                    //Dn thanh cong
                    kh.LastLogin = DateTime.Now;
                    _context.Update(kh);
                    await _context.SaveChangesAsync();
                    //
                    var taikhoanID = HttpContext.Session.GetString("AccountId");
                    //identity
                    //luu session ma kh
                    HttpContext.Session.SetString("AccountId", kh.AccountId.ToString());

                    //identity
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, kh.FullName),
                        new Claim(ClaimTypes.Email, kh.Email),
                        new Claim("AccountId",kh.AccountId.ToString()),
                        new Claim("RoleId",kh.RoleId.ToString()),
                        new Claim(ClaimTypes.Role,kh.Role.RoleName)

                    };

                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                    await HttpContext.SignInAsync(userPrincipal);

                    //
                    //if (Url.IsLocalUrl(returnURL))
                    //{
                    //    return Redirect(returnURL);
                    //}

                    return RedirectToAction("Index", "Home", new { Area = "Admin" });


                }
            }
            catch
            {
                return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            }

                return RedirectToAction("Login", "Accounts", new { Area = "Admin" });


        }


        //Dang xuat
        [AllowAnonymous]
        [Route("dang-xuat.html", Name = "Logout")]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("AccountId");
                return RedirectToAction("Index", "Home");

            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        //doi mat khau
        [HttpGet]
        [AllowAnonymous]
        [Route("doi-mat-khau.html",Name = "ChangePassword")]
        public IActionResult ChangePassword()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if (taikhoanID == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            return View();
        }


        //changes password
        [Route("doi-mat-khau.html",Name ="ChangePassword")]
        [Authorize,HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap.html");
            var taikhoanID = HttpContext.Session.GetString("AccountId");
            if(taikhoanID == null) return RedirectToAction("Login","Accounts", new {Area = "Admin"});
            if (ModelState.IsValid)
            {
                var account = _context.Accounts.AsNoTracking().FirstOrDefault(x => x.AccountId == int.Parse(taikhoanID));
                if (account == null) return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
                try
                {
                    //kiem tra mk hien tai co dung khong
                    string passnow = model.PasswordNow;
                    if(passnow == account.Password.Trim())
                    {
                        //tao mk moi
                        account.Password = (model.Password);
                        _context.Update(account);
                        _context.SaveChanges();
                        return RedirectToAction("Profile", "Accounts", new { Area = "Admin" });

                    }
                    else
                    {
                        return View();

                    }
                }
                catch
                {
                    return View();
                }
                
            }

            return View();

        }







        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,FullName,Email,Phone,Password,Salt,Active,CreateDate,RoleId,LastLogin")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,FullName,Email,Phone,Password,Salt,Active,CreateDate,RoleId,LastLogin")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
