using PloomesAPI.Models;
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
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [MaxLength(200)]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "A descrição é obrigatória!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O valor do item é obrigatório!")]
        [DataType(DataType.Currency)]
        public float Value { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateOfInsert { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "A quantidade em estoque é obrigatória!")]
        public int InventoryAmount { get; set; }

        [Required]
        public int ItemCategoryId { get; set; }

        public virtual ItemCategory ItemCategory { get; set; }
    }
}
