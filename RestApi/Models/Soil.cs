using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestApi.Models {
    [Table("soil")]
    public class Soil {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("soil_moisture")]
        public double Moisture { get; set; }

        [Column("soil_temperature")]
        public double Temperature { get; set; }

        // [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}