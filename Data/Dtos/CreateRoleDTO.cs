using System.ComponentModel.DataAnnotations;

namespace PloomesAPI.Data.Dtos
{
    public class CreateRoleDTO
    {
        [Required(ErrorMessage = "O nome da função é obrigatório!")]
        public string AccountRole { get; set; }
    }
}
