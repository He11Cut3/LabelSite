using LabelSite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LabelSite.Models;

namespace VihoTask.ViewComponents
{
    public class UserViewComponent : ViewComponent
    {
        private readonly IUserRepository _userRepository;

        public UserViewComponent(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize]
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal user = User as ClaimsPrincipal;
            string userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            UserModel userModel = GetUserModel(userId);

            if (userModel != null)
            {
                return View(userModel);
            }
            return View();
        }

        [Authorize]
        public async Task<IViewComponentResult> UserInfoAsync()
        {
            ClaimsPrincipal user = User as ClaimsPrincipal;
            string userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            UserModel userModel = GetUserModel(userId);

            return View("UserInfo", userModel);
        }

        private UserModel GetUserModel(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                User user = _userRepository.GetUserById(userId);

                if (user != null)
                {
                    return new UserModel
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        UserPhoto = user.UserPhoto,
                    };
                }
            }

            // Если пользователь не найден, вернем пустую модель
            return new UserModel();
        }
    }
}
