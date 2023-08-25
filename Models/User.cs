using PloomesAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace StoreAPI.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do usuário é obrigatório!")]
        [StringLength(100, ErrorMessage = "O nome de usuário deve ter pelo menos 2 letras", MinimumLength = 2)]
        public string Username { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória!")]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z]).{5,}$", ErrorMessage = "A senha deve ter no mínimo 5 caracteres, uma letra maíscula e uma minúscula.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? Salt { get; set; }

        [Required]
        public int RoleId { get; set; }

        public virtual Role? Role { get; set; }
    }
}
