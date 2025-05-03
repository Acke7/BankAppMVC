using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Users
{
    using AutoMapper;
 
    using DatabaseLayer.DTOs.User;
    using Microsoft.AspNetCore.Identity;

    namespace Services
    {
        public class UserService : IUserService
        {
            private readonly UserManager<IdentityUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly IMapper _mapper;

            public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
            {
                _userManager = userManager;
                _roleManager = roleManager;
                _mapper = mapper;
            }

            public async Task<List<UserDto>> GetAllUsersAsync()
            {
                var users = _userManager.Users.ToList();
                var userDtos = _mapper.Map<List<UserDto>>(users);

                for (int i = 0; i < users.Count; i++)
                {
                    var roles = await _userManager.GetRolesAsync(users[i]);
                    userDtos[i].Roles = roles;
                }

                return userDtos;
            }

            public async Task<EditUserDto> GetUserByIdAsync(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return null;

                var dto = _mapper.Map<EditUserDto>(user);
                dto.SelectedRoles = (await _userManager.GetRolesAsync(user)).ToList();
                dto.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();

                return dto;
            }

            public async Task<bool> UpdateUserAsync(EditUserDto userDto)
            {
                var user = await _userManager.FindByIdAsync(userDto.Id);
                if (user == null) return false;

                _mapper.Map(userDto, user);

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return false;

                var currentRoles = await _userManager.GetRolesAsync(user);
                var removeRoles = currentRoles.Except(userDto.SelectedRoles);
                var addRoles = userDto.SelectedRoles.Except(currentRoles);

                await _userManager.RemoveFromRolesAsync(user, removeRoles);
                await _userManager.AddToRolesAsync(user, addRoles);

                return true;
            }

            public async Task<bool> DeleteUserAsync(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return false;

                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }
        }
    }

}
