﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using Models.Entities;

namespace DataAccess.DAOs.Interfaces
{
    public interface IUserDao
    {
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmail(string email);
        Task AddUser(UserDto userRegisterDto);
        Task UpdateUser(int id, UserDto userUpdateDto);
        Task DesactivateUser(int id);
    }
}
