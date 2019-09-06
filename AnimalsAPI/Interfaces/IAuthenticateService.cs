using AnimalsAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalsAPI.Interfaces
{
    
        public interface IAuthenticateService
        {
            bool IsAuthenticated(string connectionString, AuthenticationRequest request, out string token);
        }
    
}
