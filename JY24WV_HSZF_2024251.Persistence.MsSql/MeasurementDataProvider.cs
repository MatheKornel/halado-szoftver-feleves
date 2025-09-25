using JY24WV_HSZF_2024251.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JY24WV_HSZF_2024251.Persistence.MsSql
{
    public interface IMeasurmentDataProvider
    {
        void Create(MeasurementData d, string id);
        IEnumerable<MeasurementPoint> Read(string id);
        IEnumerable<MeasurementData> ReadOnlyData();
        void Update(MeasurementData d);
        void Delete(DateTime m);
    }
    public class MeasurementDataProvider : IMeasurmentDataProvider
    {
        private readonly AppDbContext ctx;

        public MeasurementDataProvider(AppDbContext context)
        {
            this.ctx = context;
        }

        public void Create(MeasurementData d, string id)
        {
            ctx.Points.Include(t => t.Measurements).FirstOrDefault(s => s.PointNumber == id).Measurements.Add(d);
            ctx.SaveChanges();
        }

        public void Delete(DateTime m)
        {
            var dataToDelete = ctx.Data.FirstOrDefault(s => s.Date == m);
            if (dataToDelete != null)
            {
                ctx.Data.Remove(dataToDelete);
                ctx.SaveChanges();
            }
        }

        public IEnumerable<MeasurementPoint> Read(string id)
        {
            return ctx.Points.Include(t => t.Measurements).Where(s => s.PointNumber == id);
        }

        public IEnumerable<MeasurementData> ReadOnlyData()
        {
            return ctx.Data;
        }

        public void Update(MeasurementData d)
        {
            var dataToUpdate = ctx.Data.FirstOrDefault(s => s.Date == d.Date);
            if (dataToUpdate != null)
            {
                dataToUpdate.Temperature = d.Temperature;
                dataToUpdate.Precipitation = d.Precipitation;
                dataToUpdate.Measurement = d.Measurement;
                ctx.SaveChanges();
            }
        }
    }
}
