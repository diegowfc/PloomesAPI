using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PloomesAPI.Data.Dtos;
using PloomesAPI.Models;
using StoreAPI.Data;

namespace PloomesAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private DataContext _context;
        private IMapper _mapper;

        public CategoryController(DataContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }

        /// <summary>
        /// Método para cadastrar novas categorias de itens.
        /// </summary>
        /// <param name="categoryDTO">DTO para mapeamento e criação.</param>
        /// <returns>ActionResult e uma mensagem para o usuário.</returns>
        [HttpPost]
        [Authorize(Roles = "administrador")]
        public ActionResult RegisterCategory([FromBody] CreateCategoryDTO categoryDTO)
        {
            ItemCategory category = _mapper.Map<ItemCategory>(categoryDTO);
            try
            {
                _context.ItemCategories.Add(category);
                _context.SaveChanges();

                return Ok("Categoria de item adicionada com sucesso!");
            }
            catch
            {
                return BadRequest("Erro ao cadastrar a categoria. Por favor, tente novamente mais tarde.");
            }
        }

        /// <summary>
        /// Método que retorna a coleção de categorias de itens cadastradas.
        /// </summary>
        /// <returns>ActionResult com a coleção de categorias.</returns>
        [HttpGet]
        [Authorize(Roles = "administrador")]
        public ActionResult<IEnumerable<Role>> GetCategory()
        {
            try
            {
                var categories = _mapper.Map<List<ReadCategoryDTO>>(_context.ItemCategories.ToList());
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao buscar as categorias.", error = ex.Message });
            }
        }

        /// <summary>
        /// Método que atualiza o nome de uma categoria.
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <param name="roleDTO">DTO de uma categoria com as informações.</param>
        /// <returns>ActionResult</returns>
        /// <response code="204">Caso a atualização seja feita com sucesso</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "administrador")]
        public ActionResult UpdateCategory(int id, [FromBody] UpdateCategoryDTO categoryDTO)
        {
            try
            {
                var targetCategory = _context.ItemCategories.FirstOrDefault(i => i.Id == id);

                if (targetCategory == null) return NotFound("Categoria não encontrada.");

                _mapper.Map(categoryDTO, targetCategory);
                _context.SaveChanges();

                return NoContent();
            }
            catch
            {
                return BadRequest("Erro ao atualizar a categoria. Tente novamente mais tarde.");
            }
        }

        /// <summary>
        /// Método para visualizar as informações de uma categoria por ID
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <returns>Action result</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "administrador")]
        public ActionResult GetCategoryById(int id)
        {
            ItemCategory category = _context.ItemCategories.FirstOrDefault(i => i.Id == id);
            if (category != null)
            {
                ReadCategoryDTO categoryDTO = _mapper.Map<ReadCategoryDTO>(category);
                return Ok(category);
            }
            return NotFound();
        }

        /// <summary>
        /// Método para deletar uma categoria
        /// </summary>
        /// <param name="id">ID da categoria que será deletada</param>
        /// <returns>ActionResult</returns>
        /// <response code="204">Caso o delete seja feito com sucesso</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "administrador")]
        public ActionResult DeleteCategory(int id)
        {
            try
            {
                var targetCategory = _context.ItemCategories.FirstOrDefault(i => i.Id == id);

                if (targetCategory == null)
                    return NotFound("Categoria não encontrada.");

                _context.ItemCategories.Remove(targetCategory);
                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao deletar a categoria.", error = ex.Message });
            }
        }
    }
}
