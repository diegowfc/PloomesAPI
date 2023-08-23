using System.ComponentModel.DataAnnotations;

namespace PloomesAPI.Data.Dtos
{
    public class UpdateCategoryDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "O nome do tipo do item deve ter pelo menos 2 caracteres", MinimumLength = 2)]
        public string Name { get; set; }
    }
}
