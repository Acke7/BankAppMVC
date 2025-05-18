using AutoMapper;
using BankAppMVC.Models.ViewModels.UsersVm;
using DatabaseLayer.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Users;

namespace BankAppMVC.Controllers
{

    [Authorize(Roles = "Admin")]
    public class UserController : Controller
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public UserController(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }
        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var users = await _userService.GetAllUsersAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users
                    .Where(u => u.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            var viewModels = _mapper.Map<List<UserViewModel>>(users);
                return View(viewModels);
            }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
            {
                var userDto = await _userService.GetUserByIdAsync(id);
                if (userDto == null) return NotFound();

                var viewModel = _mapper.Map<EditUserViewModel>(userDto);
                return View(viewModel);
            }

            [HttpPost]
            public async Task<IActionResult> Edit(EditUserViewModel viewModel)
            {
                if (!ModelState.IsValid)
                    return View(viewModel);

                var userDto = _mapper.Map<EditUserDto>(viewModel);
                var success = await _userService.UpdateUserAsync(userDto);
                if (!success) return BadRequest();

                return RedirectToAction("Index");
            }

            public async Task<IActionResult> Delete(string id)
            {
                await _userService.DeleteUserAsync(id);
                return RedirectToAction("Index");
            }
        }
    }