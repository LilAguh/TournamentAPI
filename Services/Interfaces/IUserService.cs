using Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> Register(UserDto userDto);
        Task<string> Login(UserDto userDto);
        Task<UserResponseDto> GetUserById(int id);
        Task UpdateUser(int id, UserDto userDto);
        Task DeactivateUser(int id);
    }
}
