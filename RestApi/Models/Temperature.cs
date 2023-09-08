namespace RestApi.Models
{
    public class Temperature
    {
        public int Id { get; set; }
        public double Temp { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime CreatedAt { get; set; } // Add this line
    }
}