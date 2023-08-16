using System.ComponentModel.DataAnnotations;

namespace PloomesAPI.Data.Dtos
{
    public class ReadItemDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public float Value { get; set; }

        public DateTime DateOfInsert { get; set; }

        public int InventoryAmount { get; set; }

        public DateTime TimeOfRead { get; set; } = DateTime.Now;
    }
}
