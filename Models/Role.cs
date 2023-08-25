using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using StoreAPI.Model;

namespace PloomesAPI.Models
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da função é obrigatório!")]
        public string AccountRole { get; set; }

        public virtual ICollection<User>? User { get; set; }
    }
}
