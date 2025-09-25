using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JY24WV_HSZF_2024251.Model
{
    public class MeasurementPoint
    {
        [Key]
        [StringLength(3, MinimumLength = 3)]
        public string PointNumber { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string PointName { get; set; }

        public virtual ICollection<MeasurementData> Measurements { get; set; }
        public MeasurementPoint()
        {
            Measurements = new HashSet<MeasurementData>();
        }

        public MeasurementPoint(string pnum, string name)
        {
            PointNumber = pnum;
            PointName = name;
            Measurements = new HashSet<MeasurementData>();
        }
    }

}
