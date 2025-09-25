using JY24WV_HSZF_2024251.Model;
using JY24WV_HSZF_2024251.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JY24WV_HSZF_2024251.Application
{
    public interface IPointService
    {
        void Create(MeasurementPoint p);
        IEnumerable<MeasurementPoint> Read();
        void Update(MeasurementPoint p);
        void Delete(string id);
        IEnumerable<MeasurementPoint> Search(string point, string name, string date, string temp, string prec, string mes);
        IEnumerable<PointService.Q1Info> MeasurementCount();
        IEnumerable<PointService.Q2Info> AvgMeasurement();
        IEnumerable<PointService.Q3Info> MoreThanAvgPrecipitation();

    }
    public class PointService : IPointService
    {
        private readonly IMeasurmentPointProvider measurmentPointProvider;

        public PointService(IMeasurmentPointProvider measurmentPointProvider)
        {
            this.measurmentPointProvider = measurmentPointProvider;
        }

        public void Create(MeasurementPoint p)
        {
            if (p.PointNumber.Length != 3 || p.PointName.Length < 3 || p.PointName.Length > 50)
            {
                throw new ArgumentException("Wrong input!");
            }
            measurmentPointProvider.Create(p);
        }

        public void Delete(string id)
        {
            if (id.Length != 3)
            {
                throw new ArgumentException("Wrong input!");
            }
            else if (this.measurmentPointProvider.Read().FirstOrDefault(s => s.PointNumber == id) == null)
            {
                throw new ArgumentException("Point does not exist!");
            }
            measurmentPointProvider.Delete(id);
        }

        public IEnumerable<MeasurementPoint> Read()
        {
            return measurmentPointProvider.Read();
        }

        public void Update(MeasurementPoint p)
        {
            if (p.PointNumber.Length != 3 || p.PointName.Length < 3 || p.PointName.Length > 50)
            {
                throw new ArgumentException("Wrong input!");
            }
            measurmentPointProvider.Update(p);
        }
        public IEnumerable<MeasurementPoint> Search(string point, string name, string date, string temp, string prec, string mes)
        {
            return measurmentPointProvider.Search(point, name, date, temp, prec, mes);
        }

        public IEnumerable<Q1Info> MeasurementCount()
        {
            //Mérőpontonként az adott mérőponthoz tartozó mérések száma.
            var q1 = this.measurmentPointProvider.Read().Select(s => new Q1Info
            {
                PointNumber = s.PointNumber,
                PointName = s.PointName,
                MeasurementCount = s.Measurements.Count()
            });
            return q1;
        }

        public IEnumerable<Q2Info> AvgMeasurement()
        {
            //Mérőpontonként az átlagos vízállás mértéke, a legmagasabb és legalacsonyabb mérések adataival.
            var q2 = this.measurmentPointProvider.Read().Select(t => new Q2Info
            {
                PointNumber = t.PointNumber,
                PointName = t.PointName,
                AvgWaterLevel = Math.Round(t.Measurements.Average(m => m.Measurement), 2),
                HighestWaterLevel = t.Measurements.Max(x => x.Measurement),
                LowestWaterLevel = t.Measurements.Min(f => f.Measurement)
            });
            return q2;
        }

        public IEnumerable<Q3Info> MoreThanAvgPrecipitation()
        {
            //Mérőpontonként azok a mérések, ahol a csapadék mennyisége több mint az átlag.
            var q3 = this.measurmentPointProvider.Read().Select(e => new Q3Info
            {
                PointNumber = e.PointNumber,
                PointName = e.PointName,
                AvgPrecipitation = Math.Round(e.Measurements.Average(r => r.Precipitation), 2),
                AboveAvgPrecipitations = e.Measurements.Where(k => k.Precipitation > e.Measurements.Average(l => l.Precipitation)).ToList()
            });
            return q3;
        }

        public class Q1Info()
        {
            public string PointNumber { get; set; }
            public string PointName { get; set; }
            public int MeasurementCount { get; set; }

            public override bool Equals(object? obj)
            {
                Q1Info temp = obj as Q1Info;
                if (temp == null) return false;
                else
                {
                    return this.PointNumber == temp.PointNumber
                        && this.PointName == temp.PointName
                        && this.MeasurementCount.Equals(temp.MeasurementCount);
                }
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(this.PointNumber, this.PointName, this.MeasurementCount);
            }
        }

        public class Q2Info()
        {
            public string PointNumber { get; set; }
            public string PointName { get; set; }
            public double AvgWaterLevel { get; set; }
            public int HighestWaterLevel { get; set; }
            public int LowestWaterLevel { get; set; }

            public override bool Equals(object? obj)
            {
                if (obj is Q2Info temp)
                {
                    return this.PointNumber == temp.PointNumber
                        && this.PointName == temp.PointName
                        && this.AvgWaterLevel == temp.AvgWaterLevel
                        && this.HighestWaterLevel == temp.HighestWaterLevel
                        && this.LowestWaterLevel == temp.LowestWaterLevel;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(this.PointNumber, this.PointName, this.AvgWaterLevel, this, HighestWaterLevel, this.LowestWaterLevel);
            }
        }

        public class Q3Info()
        {
            public string PointNumber { get; set; }
            public string PointName { get; set; }
            public double AvgPrecipitation { get; set; }
            public List<MeasurementData> AboveAvgPrecipitations { get; set; }

            public override bool Equals(object? obj)
            {
                Q3Info temp = obj as Q3Info;
                if (temp == null) return false;
                else
                {
                    return this.PointNumber == temp.PointNumber
                        && this.PointName == temp.PointName
                        && this.AvgPrecipitation.Equals(temp.AvgPrecipitation)
                        && this.AboveAvgPrecipitations.Equals(temp.AboveAvgPrecipitations);
                }
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(this.PointNumber, this.PointName, this.AvgPrecipitation, this.AboveAvgPrecipitations);
            }
        }
    }
}
