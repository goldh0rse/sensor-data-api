namespace RestApi.DTO.Soil
{
    public class GetSoilDTO
    {
        public int Id { get; set; }
        public double Moisture { get; set; }
        public double Temperature { get; set; }
        public DateTime DateTime { get; set; }
    }
}