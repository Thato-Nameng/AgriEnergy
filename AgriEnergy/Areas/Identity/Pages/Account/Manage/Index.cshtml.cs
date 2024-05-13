// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AgriEnergy.Data;
using AgriEnergy.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AgriEnergy.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly SignInManager<UserApp> _signInManager;
        private readonly IFileService _fileService;

        public IndexModel(
            UserManager<UserApp> userManager,
            SignInManager<UserApp> signInManager,
            IFileService fileService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._fileService = fileService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            public string FName { get; set; }
            public string LName { get; set; }
            public string Picture { get; set; }
            public IFormFile ImageFile { get; set; }
        }

        private async Task LoadAsync(UserApp user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
          /*  var fname = await _userManager.GetUserNameAsync(user);
            var lName = await _userManager.GetUserNameAsync(user);
            var picutre = await _userManager.get*/


            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                LName = user.LName,
                FName = user.FName,
                Picture = user.Picture

            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            if (Input.FName != user.FName)
            {
                user.FName = Input.FName;
                await _userManager.UpdateAsync(user);
            }
            if (Input.LName != user.LName)
            {
                user.LName = Input.LName;
                await _userManager.UpdateAsync(user);
            }

            if (Input.ImageFile != null)
            {
                var result = _fileService.SaveImage(Input.ImageFile);
                if (result.Item1 == 1)
                {
                    var oldImage = user.Picture;
                    user.Picture = result.Item2;
                    await _userManager.UpdateAsync(user);
                    var deleteResult = _fileService.DeleteImage(oldImage);
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
