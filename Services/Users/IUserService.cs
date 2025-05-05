using DatabaseLayer.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Users
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<EditUserDto> GetUserByIdAsync(string id);
        Task<bool> UpdateUserAsync(EditUserDto userDto);
        Task<bool> DeleteUserAsync(string id);
    }
}
