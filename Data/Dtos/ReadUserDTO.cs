namespace PloomesAPI.Data.Dtos
{
    public class ReadUserDTO
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public ReadRoleDTO Role { get; set; }
    }
}
