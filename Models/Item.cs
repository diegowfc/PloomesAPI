using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreAPI.Model
{
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do item é obrigatório!")]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Required(ErrorMessage = "O tipo do item é obrigatório!")]
        public string Type { get; set; }

        [Required(ErrorMessage = "O valor do item é obrigatório!")]
        public float Value { get; set; }

        public DateTime DateOfInsert { get; set; }
        public int InventoryAmount { get; set; }
    }
}
