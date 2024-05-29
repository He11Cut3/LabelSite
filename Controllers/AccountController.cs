using LabelSite.Infrastructure;
using LabelSite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LabelSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DataContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, DataContext dataContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = dataContext;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserModel user, IFormFile UserPhoto )
        {
            if (ModelState.IsValid)
            {
                if (UserPhoto != null && UserPhoto.Length > 0)
                {
                    
                    byte[] photoBytes;
                    using (MemoryStream photoStream = new MemoryStream())
                    {
                        await UserPhoto.CopyToAsync(photoStream);
                        photoBytes = photoStream.ToArray();
                    }


                    User newUser = new User
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        UserPhoto = photoBytes
                    };

                    IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);

                    if (result.Succeeded)
                    {
                        if (user.UserName == "Admin")
                        {
                            await _userManager.AddToRoleAsync(newUser, "Admin");
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(newUser, "User");
                        }
                        await _signInManager.SignInAsync(newUser, isPersistent: false); // Вход пользователя
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Выберите фотографии");
                }
            }

            return View(user);
        }



        public IActionResult Login(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = await _userManager.FindByNameAsync(Email);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Login", "Account");
        }




        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}
