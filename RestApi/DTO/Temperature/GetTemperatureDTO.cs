
namespace RestApi.DTO.Temperature
{
    public class GetTemperatureDTO
    {
        public int Id { get; set; }
        public double Temp { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}