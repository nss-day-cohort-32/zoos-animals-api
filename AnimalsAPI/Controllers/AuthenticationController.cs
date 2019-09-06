using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AnimalsAPI.Interfaces;
using AnimalsAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AnimalsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthenticateService _authService;
        public AuthenticationController(IConfiguration config, IAuthenticateService authService)
        {
            _config = config;
            _authService = authService;
        }


        [AllowAnonymous]
        [HttpPost, Route("request")]
        public IActionResult RequestToken([FromBody] AuthenticationRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string token;

            if (_authService.IsAuthenticated(_config.GetConnectionString("DefaultConnection"), request, out token))
            {
                return Ok(token);
            }

            return BadRequest("Invalid Request");
        }
    }
}