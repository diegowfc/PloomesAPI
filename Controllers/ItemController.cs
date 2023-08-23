using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PloomesAPI.Data.Dtos;
using StoreAPI.Data;
using StoreAPI.Model;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace StoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemController : ControllerBase
    {

        private readonly DataContext context;
        private readonly IMapper mapper;

        public ItemController(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        /// <summary>
        /// Valida as informações que o usuário forneceu para inserir um novo item
        /// </summary>
        /// <param name="item">Objeto do tipo item</param>
        /// <returns>Retorna true se estiver dentro das validãções e false caso não esteja</returns>
        private bool ValidateItem(Item item)
        {
            if (item == null)
                return false;

            if (string.IsNullOrWhiteSpace(item.Name) || string.IsNullOrWhiteSpace(item.Type))
                return false;

            if (item.Value <= 0)
                return false;

            return true;
        }

        /// <summary>
        /// Método para registrar um item novo no sistema.
        /// </summary>
        /// <param name="item">Objeto do tipo ITEM</param>
        /// <returns>ActionResult</returns>
        /// <response code="201">Caso a inserção seja feito com sucesso</response>
        [HttpPost]
        [Authorize(Roles = "administrador")]
        public ActionResult SaveNewItem([FromBody] CreateItemDTO itemDTO)
        {
            Item item = mapper.Map<Item>(itemDTO);

            try
            {
                if (!ValidateItem(item))
                    return BadRequest("Falha no cadastro. Verifique as informações inseridas.");

                context.Items.Add(item);
                context.SaveChanges();
                return Ok("Item cadastrado com sucesso.");
            }
            catch
            {
                return BadRequest("Erro ao salvar o novo item. Por favor, tente novamente mais tarde.");
            }
        }

        /// <summary>
        /// Método que atualiza todas as informações de um item.
        /// </summary>
        /// <param name="id">ID do item alvo</param>
        /// <param name="item">Objeto do tipo ITEM com as informações novas.</param>
        /// <returns>ActionResult</returns>
        ///<response code="204">Caso a atualização seja feita com sucesso</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "administrador")]
        public ActionResult UpdateItem(int id, [FromBody] UpdateItemDTO itemDTO)
        {
            try
            {
                var targetItem = context.Items.FirstOrDefault(i => i.Id == id);

                if (targetItem == null) return NotFound("Item não encontrado.");

                mapper.Map(itemDTO, targetItem);

                if (!ValidateItem(targetItem))
                    return BadRequest("Falha na atualização. Verifique as informações inseridas.");

                context.SaveChanges();
                return NoContent();
            }
            catch
            {
                return BadRequest("Erro ao atualizar o item. Tente novamente mais tarde.");
            }
        }

        /// <summary>
        /// Método que atualiza apenas o número do estoque de determinado item.
        /// </summary>
        /// <param name="id">ID do item alvo</param>
        /// <param name="amount">Valor do qual a quantidade será atualizada</param>
        /// <returns>ActionResult</returns>
        ///<response code="204">Caso a atualização seja feita com sucesso</response>
        [HttpPatch("{id}")]
        [Authorize(Roles = "administrador")]
        public ActionResult ParcialUpdate(int id, int amount, JsonPatchDocument<UpdateItemDTO> patch)
        {
            try
            {
                var targetItem = context.Items.FirstOrDefault(i => i.Id == id);

                var itemToUpdate = mapper.Map<UpdateItemDTO>(targetItem);

                patch.ApplyTo(itemToUpdate, ModelState);

                if (!TryValidateModel(itemToUpdate)) return ValidationProblem(ModelState);

                mapper.Map(itemToUpdate, targetItem);
                context.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao atualizar o item. Tente novamente mais tarde.", error = ex.Message });
            }
        }

        /// <summary>
        /// Método para deletar um item
        /// </summary>
        /// <param name="id">ID do item que será deletado</param>
        /// <returns>ActionResult</returns>
        /// <response code="204">Caso o delete seja feito com sucesso</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "administrador")]
        public ActionResult DeleteItem(int id)
        {
            try
            {
                var targetItem = context.Items.FirstOrDefault(i => i.Id == id);

                if (targetItem == null)
                    return NotFound("Item não encontrado.");

                context.Items.Remove(targetItem);
                context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao deletar item.", error = ex.Message });
            }
        }

        /// <summary>
        /// Método que retorna uma coleção de items salvos no banco.
        /// </summary>
        /// <param name="page">Número da página</param>
        /// <param name="pageSize">Numero de Items retornados por página</param>
        /// <returns>Retorna uma coleção de Items ou uma mensagem de erro.</returns>
        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<Item>> GetItems(int? page = null, int pageSize = 5)
        {
            try
            {
                if (page.HasValue && page <= 0)
                    return BadRequest("Página inválida.");

                if (pageSize <= 0)
                    return BadRequest("Página inválida.");

                int currentPage = page ?? 1;

                var items = mapper.Map<List<ReadItemDTO>>(context.Items
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList());

                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao buscar o item.", error = ex.Message });
            }
        }

        /// <summary>
        /// Método que retorna as informações de um item específico
        /// </summary>
        /// <param name="id">ID do item</param>
        /// <returns>Retorna as informações do item específico ou uma mensagem de erro em caso de falha.</returns>
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Item> GetItemById(int id)
        {
            try
            {
                var item = context.Items.FirstOrDefault(i => i.Id == id);

                if (item == null) return NotFound("Item não encontrado.");
                var itemDTO = mapper.Map<ReadItemDTO>(item);
                return Ok(itemDTO); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao buscar o item.", error = ex.Message });
            }
        }


    }
}
