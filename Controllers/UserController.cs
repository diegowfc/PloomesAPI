using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PloomesAPI.Services;
using StoreAPI.Data;
using StoreAPI.Model;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PloomesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext context;
        private readonly IConfiguration configuration;

        public UserController(DataContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }


        private bool ValidateUser(User user)
        {
            if (user == null)
                return false;

            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                return false;

            return true;
        }

        private byte[] GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[32]; // Adjust the size based on your preference
                rng.GetBytes(saltBytes);
                return saltBytes;
            }
        }

        private byte[] HashPasswordWithSalt(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                byte[] combinedBytes = new byte[passwordBytes.Length + salt.Length];
                Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, combinedBytes, passwordBytes.Length, salt.Length);
                return sha256.ComputeHash(combinedBytes);
            }
        }

        [HttpGet]
        [Authorize(Roles = "administrador")]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            try
            {
                var users = context.Users.ToList();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao buscar os usuários.", error = ex.Message });
            }
        }


        [HttpPost]
        public ActionResult RegisterUser([FromBody] User user)
        {
            try
            {
                if (!ValidateUser(user))
                    return BadRequest("Falha no cadastro do usuário. Revise as informações inseridas");

                byte[] salt = GenerateSalt();
                byte[] hashedPassword = HashPasswordWithSalt(user.Password, salt);

                user.Password = Convert.ToBase64String(hashedPassword);
                user.Salt = Convert.ToBase64String(salt);

                if (user.Role == null) user.Role = "Usuário comum";

                context.Users.Add(user);
                context.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest("Erro ao cadastrar usuário. Por favor, tente novamente mais tarde.");
            }
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] User userLogin)
        {
            var user = context.Users.FirstOrDefault(u => u.Username == userLogin.Username);

            if (user == null)
            {
                return BadRequest("Usuário ou senha inválidos.");
            }

            var hashedPassword = HashPasswordWithSalt(userLogin.Password, Convert.FromBase64String(user.Salt));

            if (!hashedPassword.SequenceEqual(Convert.FromBase64String(user.Password)))
            {
                return BadRequest("Usuário ou senha inválidos.");
            }

            var token = TokenService.GenerateToken(user, configuration);


            return Ok(new { Token = token });
        }
    }
}
