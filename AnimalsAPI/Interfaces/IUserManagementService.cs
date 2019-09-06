using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalsAPI.Interfaces
{
    public interface IUserManagementService
    {
        bool IsValidUser(string connectionString, string username, string password);
    }
}
