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
        [RegularExpression(@"/^[a-zA-Z]+$/", ErrorMessage = "Digite apenas letras para definir o nome do item.")]
        public string Name { get; set; }

        [MaxLength(200)]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "A descrição é obrigatória!")]
        [RegularExpression(@"/^[a-zA-Z]+$/", ErrorMessage = "Digite apenas letras para definir a descrição.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O tipo do item é obrigatório!")]
        [DataType(DataType.Text)]
        [RegularExpression(@"/^[a-zA-Z]+$/", ErrorMessage = "Digite apenas letras para definir o tipo.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "O valor do item é obrigatório!")]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Digite apenas números para o valor do item.")]
        public float Value { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateOfInsert { get; set; }

        [DataType(DataType.Currency)]
        [Required(ErrorMessage = "A quantidade em estoque é obrigatória!")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Digite apenas números para a quantidade em estoque.")]
        public int InventoryAmount { get; set; }
    }
}
