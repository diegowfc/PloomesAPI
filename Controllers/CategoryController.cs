using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PloomesAPI.Data.Dtos;
using PloomesAPI.Models;
using StoreAPI.Data;

namespace PloomesAPI.Controllers
{
    [Route("api/[controller]")]
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
    }
}
