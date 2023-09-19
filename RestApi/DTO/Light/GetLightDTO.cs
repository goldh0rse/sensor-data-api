namespace RestApi.DTO.Light
{
    public class GetLightDTO
    {
        public int Id { get; set; }
        public double Lux { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}