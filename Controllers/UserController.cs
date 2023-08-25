using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PloomesAPI.Data.Dtos;
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
        private IMapper _mapper;

        public UserController(DataContext context, IConfiguration configuration, IMapper mapper)
        {
            this.context = context;
            this.configuration = configuration;
            this._mapper = mapper;
        }

        /// <summary>
        /// Valida as informações de cadastro.
        /// </summary>
        /// <param name="user">Objeto do tipo USER.</param>
        /// <returns>Retorna true se as informações forem válidas e false caso não seja</returns>
        private bool ValidateUser(User user)
        {
            if (user == null)
                return false;

            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Password))
                return false;

            return true;
        }

        /// <summary>
        /// Método que gera um salt para incrementar o hash da senha.
        /// </summary>
        /// <returns>Retorna o valor do salt em bytes</returns>
        private byte[] GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] saltBytes = new byte[32]; // Adjust the size based on your preference
                rng.GetBytes(saltBytes);
                return saltBytes;
            }
        }

        /// <summary>
        /// Método que combina o hash gerado para a senha com o salt criado anteriormente.
        /// </summary>
        /// <param name="password">Senha criada pelo usuário</param>
        /// <param name="salt">Salt gerado aleatoriamente pelo sistema</param>
        /// <returns>Retorna um hash de senha 256</returns>
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

        /// <summary>
        /// Retorna uma coleção de usuários do sistema.
        /// </summary>
        /// <returns>Retorna um código de confirmação 200 em caso de sucesso e bad request em caso de erro</returns>
        [HttpGet]
        [Authorize(Roles = "administrador")]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            try
            {
                var users = _mapper.Map<List<ReadUserDTO>>(context.Users.ToList());
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao buscar os usuários.", error = ex.Message });
            }
        }

        /// <summary>
        /// Método para registrar um novo usuário no sistema
        /// </summary>
        /// <param name="user">Objeto do tipo usuário com as informações fornecidas</param>
        /// <returns>Retorna 200 em caso de sucesso e bad request em caso de erro</returns>
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

                context.Users.Add(user);
                context.SaveChanges();
                return Ok("Usuário cadastrado com sucesso");
            }
            catch
            {
                return BadRequest("Erro ao cadastrar usuário. Por favor, tente novamente mais tarde.");
            }
        }

        /// <summary>
        /// Método para realizar login no sistema
        /// </summary>
        /// <param name="userLogin">Objeto do tipo USER</param>
        /// <returns>Retorna um token para o usuário autenticar no sistema</returns>
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
