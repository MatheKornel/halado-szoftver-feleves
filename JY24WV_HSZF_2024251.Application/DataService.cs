using JY24WV_HSZF_2024251.Model;
using JY24WV_HSZF_2024251.Persistence.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JY24WV_HSZF_2024251.Application
{
    public interface IDataService
    {
        void Create(MeasurementData d, string id);
        IEnumerable<MeasurementPoint> Read(string id);
        public IEnumerable<MeasurementData> ReadOnlyData();
        void Update(MeasurementData d);
        void Delete(DateTime m);
        public delegate void WaterLevel();
        public event WaterLevel waterMax;
    }
    public class DataService : IDataService
    {
        private readonly IMeasurmentDataProvider measurmentDataProvider;

        public DataService(IMeasurmentDataProvider measurmentDataProvider)
        {
            this.measurmentDataProvider = measurmentDataProvider;
        }

        public event IDataService.WaterLevel waterMax;

        public void Create(MeasurementData d, string id)
        {
            if (d.Temperature < -100 || d.Temperature > 100 || d.Precipitation < 0 || d.Precipitation > 5 || d.Measurement < 0 || d.Measurement > 1500)
            {
                throw new ArgumentException("Wrong input!");
            }
            var max = ReadOnlyData().Max(s => s?.Measurement);
            if (d.Measurement > max)
            {
                waterMax?.Invoke();
            }
            measurmentDataProvider.Create(d, id);
        }

        public void Delete(DateTime m)
        {
            if (this.measurmentDataProvider.ReadOnlyData().FirstOrDefault(s => s.Date == m) == null)
            {
                throw new ArgumentException("Data does not exist!");
            }
            measurmentDataProvider.Delete(m);
        }

        public IEnumerable<MeasurementPoint> Read(string id)
        {
            return measurmentDataProvider.Read(id);
        }

        public IEnumerable<MeasurementData> ReadOnlyData()
        {
            return measurmentDataProvider.ReadOnlyData();
        }

        public void Update(MeasurementData d)
        {
            if (this.measurmentDataProvider.ReadOnlyData().FirstOrDefault(s => s.Date == d.Date) == null)
            {
                throw new ArgumentException("Data does not exist!");
            }
            else if (d.Temperature < -100 || d.Temperature > 100 || d.Precipitation < 0 || d.Precipitation > 5 || d.Measurement < 0 || d.Measurement > 1500)
            {
                throw new ArgumentException("Wrong input!");
            }
            measurmentDataProvider.Update(d);
        }
    }
}
