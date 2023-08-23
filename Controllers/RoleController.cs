using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PloomesAPI.Data.Dtos;
using PloomesAPI.Models;
using StoreAPI.Data;
using StoreAPI.Model;
using System.Data;

namespace PloomesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private DataContext _context;
        private IMapper _mapper;

        public RoleController(DataContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Método para cadastrar novas roles para usuários;
        /// </summary>
        /// <param name="roleDTO">DTO de criação do objeto ROLE</param>
        /// <returns>ActionResult e uma mensagem para o usuário.</returns>
        [HttpPost]
        [Authorize(Roles = "administrador")]
        public ActionResult RegisterRole([FromBody] CreateRoleDTO roleDTO)
        {
           Role role = _mapper.Map<Role>(roleDTO);
            try
            {
                _context.Roles.Add(role);
                _context.SaveChanges();

                return Ok("Função adicionada com sucesso!");
            }
            catch
            {
                return BadRequest("Erro ao cadastrar a função. Por favor, tente novamente mais tarde.");
            }
        }

        /// <summary>
        /// Método que atualiza o nome de uma role.
        /// </summary>
        /// <param name="id">ID da role alvo</param>
        /// <param name="roleDTO">DTO de uma role com as informações.</param>
        /// <returns>ActionResult</returns>
        /// <response code="204">Caso a atualização seja feita com sucesso</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "administrador")]
        public ActionResult UpdateRole(int id, [FromBody] UpdateRoleDTO roleDTO)
        {
            try
            {
                var targetRole = _context.Roles.FirstOrDefault(i => i.Id == id);

                if (targetRole == null) return NotFound("Função não encontrada.");

                _mapper.Map(roleDTO, targetRole);
                _context.SaveChanges();

                return NoContent();
            }
            catch
            {
                return BadRequest("Erro ao atualizar a função. Tente novamente mais tarde.");
            }
        }

        /// <summary>
        /// Método que retorna a coleção de funções cadastradas.
        /// </summary>
        /// <returns>ActionResult com a coleção de roles.</returns>
        [HttpGet]
        [Authorize(Roles = "administrador")]
        public ActionResult<IEnumerable<Role>>GetRoles()
        {
            try
            {
                var roles = _mapper.Map<List<ReadRoleDTO>>(_context.Roles.ToList());
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao buscar as funções.", error = ex.Message });
            }
        }

        /// <summary>
        /// Método para visualizar as informações de uma role por ID
        /// </summary>
        /// <param name="id">ID da role</param>
        /// <returns>Action result</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "administrador")]
        public ActionResult GetRoleById(int id)
        {
            Role role = _context.Roles.FirstOrDefault(i => i.Id == id);
            if (role != null)
            {
                ReadRoleDTO roleDTO = _mapper.Map<ReadRoleDTO>(role);
                return Ok(roleDTO);
            }
            return NotFound();
        }

        /// <summary>
        /// Método para deletar uma função
        /// </summary>
        /// <param name="id">ID da função que será deletada</param>
        /// <returns>ActionResult</returns>
        /// <response code="204">Caso o delete seja feito com sucesso</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "administrador")]
        public ActionResult DeleteRole(int id)
        {
            try
            {
                var targetRole = _context.Roles.FirstOrDefault(i => i.Id == id);

                if (targetRole == null)
                    return NotFound("Função não encontrada.");

                _context.Roles.Remove(targetRole);
                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao deletar a fuñção.", error = ex.Message });
            }
        }

    }
}
