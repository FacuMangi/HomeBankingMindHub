using HomeBankingMindHub.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using HomeBankingMindHub.Models;
using HomeBankingMindHub.dtos;

namespace HomeBankingMindHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IClientRepository _clientRepository;
        public AuthController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Client client) //metodo Login asincronico que devuelve una task del tipo IActionResult, donde este metodo recibe el parametro client del tipo CLient con los datos del cuerpo de la solicitud HTTP (el json de la rquest)
        {
            try
            {
                Client user = _clientRepository.FindByEmail(client.Email); //se crea un user del tipo client igual al cliente de la DB que se encuentre con el metodo FindByEmail
                if (user == null || !String.Equals(user.Password, client.Password)) 
                    return Unauthorized(); //si user devuelve null o su dato Password no es igual al dato Password sacado de la request devuelve Unauthorized

                var claims = new List<Claim>
                {
                    new Claim("Client", user.Email),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                    );

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
