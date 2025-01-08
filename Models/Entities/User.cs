﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class User
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Alias { get; set; }
        public string? Email { get; set; }
        public string PasswordHashed { get; set; }
        public string? Country { get; set; }
        public string? Role { get; set; }
        public string? AvatarUrl { get; set; }
        public bool Active { get; set; } = true;
    }
}
