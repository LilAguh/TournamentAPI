using DataAccess.DAOs.Interfaces;
using DataAccess.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAOs.Implementations
{
    public class UserDao : IUserDao
    {
        private readonly IDatabaseConnection _databaseConnection;

        public UserDao (IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }
    }
}
