using System.ComponentModel.DataAnnotations;

namespace PloomesAPI.Data.Dtos
{
    public class UpdateRoleDTO
    {
        [Required(ErrorMessage = "O nome da função é obrigatório!")]
        public string AccountRole { get; set; }
    }
}
