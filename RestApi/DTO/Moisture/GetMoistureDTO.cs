namespace RestApi.DTO.Moisture
{
    public class GetMoistureDTO
    {
        public int Id { get; set; }
        public double MoistureLvl { get; set; }
        public DateTime DateTime { get; set; }
    }
}