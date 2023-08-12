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
    public class StoreController : ControllerBase
    {

        private readonly DataContext context;

        public StoreController(DataContext context)
        {
            this.context = context;
        }

        private bool ValidateItem(Item item)
        {
            if (item == null)
                return false;

            if (string.IsNullOrWhiteSpace(item.Name))
                return false;

            if (string.IsNullOrWhiteSpace(item.Type))
                return false;

            if (item.Value <= 0)
                return false;

            return true;
        }

        [HttpPost]
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
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public ActionResult UpdateItem(int id, [FromBody] Item item)
        {
            try
            {
                var targetItem = context.Itens.FirstOrDefault(i => i.Id == id);

                if (targetItem == null)
                    return NotFound();

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
                return BadRequest();
            }
        }


        [HttpPatch("{id}/UpdateInventory/{amount}")]
        public ActionResult UpdateInventoryAmount(int id, int amount)
        {
            try
            {
                var targetItem = context.Itens.FirstOrDefault(i => i.Id == id);

                if (targetItem == null)
                    return NotFound();

                if (amount < 0)
                    return BadRequest("O estoque não pode ser menor que 0.");

                targetItem.InventoryAmount = amount;

                context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error updating inventory amount.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteItem(int id)
        {
            try
            {
                var targetItem = context.Itens.FirstOrDefault(i => i.Id == id);

                if (targetItem == null)
                    return NotFound();

                context.Itens.Remove(targetItem);
                context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error deleting item.", error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Item>> GetItems(int? page = null, int pageSize = 20)
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
        public ActionResult<Item> GetItemById(int id)
        {
            try
            {
                var item = context.Itens.FirstOrDefault(i => i.Id == id);

                if (item == null)
                {
                    return NotFound(); 
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
