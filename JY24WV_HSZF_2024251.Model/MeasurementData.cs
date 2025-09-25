using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JY24WV_HSZF_2024251.Model
{
    public class MeasurementData
    {
        [Required]
        [Range(-100, 100)]
        public double Temperature { get; set; }

        [Required]
        [Range(0, 5)]
        public double Precipitation { get; set; }

        [Required]
        [Range(0, 1500)]
        public int Measurement { get; set; }

        [Key]
        public DateTime Date { get; set; }

        public string PointNumber { get; set; }

        public virtual MeasurementPoint MeasurementPoint { get; set; }

        public MeasurementData(DateTime date, double temperature, double precipitation, int measurement)
        {
            Date = date;
            Temperature = temperature;
            Precipitation = precipitation;
            Measurement = measurement;
        }

    }
}
