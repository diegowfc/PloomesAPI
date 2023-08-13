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
                return Ok();
            }
            catch
            {
                return BadRequest("Erro ao salvar o novo item. Por favor, tente novamente mais tarde.");
            }
        }

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
                return Ok();
            }
            catch
            {
                return BadRequest("Erro ao atualizar o item. Tente novamente mais tarde.");
            }
        }


        [HttpPatch("{id}/AtualizaInvetario/{amount}")]
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
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao atualizar o invetário. Tente novamente mais tarde.", error = ex.Message });
            }
        }

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

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Erro ao deletar item.", error = ex.Message });
            }
        }

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
