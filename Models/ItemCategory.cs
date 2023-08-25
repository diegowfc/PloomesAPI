using StoreAPI.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PloomesAPI.Models
{
    public class ItemCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "O nome do tipo do item deve ter pelo menos 2 caracteres", MinimumLength = 2)]
        public string Name { get; set; }

        public virtual ICollection<Item>? Item { get; set; }

    }
}
