using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
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

        public ItemController(DataContext context)
        {
            this.context = context;
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
        /// <returns>Código 200 em caso de sucesso e bad request em caso de erro</returns>
        [HttpPost]
        [Authorize(Roles = "administrador")]
        public ActionResult SaveNewItem([FromBody] Item item)
        {
            try
            {
                if (!ValidateItem(item))
                    return BadRequest("Falha no cadastro. Verifique as informações inseridas.");

                context.Itens.Add(item);
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
        /// <returns>Retorna um código 200 em caso de sucesso e bad request em caso de erro</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "administrador")]
        public ActionResult UpdateItem(int id, [FromBody] Item item)
        {
            try
            {
                var targetItem = context.Itens.FirstOrDefault(i => i.Id == id);

                if (targetItem == null)
                    return NotFound("Item não encontrado.");

                if (!ValidateItem(item))
                    return BadRequest("Falha na atualização. Verifique as informações inseridas.");

                targetItem.Name = item.Name;
                targetItem.Description = item.Description;
                targetItem.Type = item.Type;
                targetItem.Value = item.Value;
                targetItem.DateOfInsert = item.DateOfInsert;

                context.SaveChanges();
                return Ok("Item atualizado com sucesso.");
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
        /// <returns>Código 200 em caso de sucesso e bad request em caso de erro</returns>
        [HttpPatch("{id}/AtualizaInventario/{amount}")]
        [Authorize(Roles = "administrador")]
        public ActionResult UpdateInventoryAmount(int id, int amount)
        {
            try
            {
                var targetItem = context.Itens.FirstOrDefault(i => i.Id == id);

                if (targetItem == null)
                    return NotFound("Item não encontrado.");

                if (amount < 0)
                    return BadRequest("O estoque não pode ser menor que 0.");

                targetItem.InventoryAmount = amount;

                context.SaveChanges();
                return Ok("Número em estoque atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao atualizar o invetário. Tente novamente mais tarde.", error = ex.Message });
            }
        }

        /// <summary>
        /// Método para deletar um item
        /// </summary>
        /// <param name="id">ID do item que será deletado</param>
        /// <returns>Código 200 em caso de sucesso e bad request em caso de erro</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "administrador")]
        public ActionResult DeleteItem(int id)
        {
            try
            {
                var targetItem = context.Itens.FirstOrDefault(i => i.Id == id);

                if (targetItem == null)
                    return NotFound("Item não encontrado.");

                context.Itens.Remove(targetItem);
                context.SaveChanges();

                return Ok("Item removido.");
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
        /// <param name="pageSize">Numero de itens retornados por página</param>
        /// <returns>Retorna uma coleção de itens ou uma mensagem de erro.</returns>
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

                var items = context.Itens
                    .Skip((currentPage - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

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
        /// <returns>Returna um código 200 de confirmação, além de retornar também as informações do item específico. Ou uma mensagem de erro em caso de falha.</returns>
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Item> GetItemById(int id)
        {
            try
            {
                var item = context.Itens.FirstOrDefault(i => i.Id == id);

                if (item == null)
                {
                    return NotFound("Item não encontrado."); 
                }

                return Ok(item); 
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao buscar o item.", error = ex.Message });
            }
        }


    }
}
