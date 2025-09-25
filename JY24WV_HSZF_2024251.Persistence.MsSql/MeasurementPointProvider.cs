using JY24WV_HSZF_2024251.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JY24WV_HSZF_2024251.Persistence.MsSql
{
    public interface IMeasurmentPointProvider
    {
        void Create(MeasurementPoint p);
        IEnumerable<MeasurementPoint> Read();
        void Update(MeasurementPoint p);
        void Delete(string id);
        IEnumerable<MeasurementPoint> Search(string point, string name, string date, string temp, string prec, string mes);
    }
    public class MeasurementPointProvider : IMeasurmentPointProvider
    {
        private readonly AppDbContext ctx;

        public MeasurementPointProvider(AppDbContext context)
        {
            this.ctx = context;
        }

        public void Create(MeasurementPoint p)
        {
            ctx.Points.Add(p);
            ctx.SaveChanges();
        }

        public void Delete(string id)
        {
            var pointToDelete = ctx.Points.Include(s => s.Measurements).FirstOrDefault(t => t.PointNumber == id);
            if (pointToDelete != null)
            {
                ctx.Data.RemoveRange(pointToDelete.Measurements);
                ctx.Points.Remove(pointToDelete);
                ctx.SaveChanges();
            }
        }

        public IEnumerable<MeasurementPoint> Read()
        {
            return ctx.Points.Include(t => t.Measurements);
        }

        public void Update(MeasurementPoint p)
        {
            var pointToUpdate = ctx.Points.FirstOrDefault(s => s.PointNumber == p.PointNumber);
            pointToUpdate.PointName = p.PointName;
            ctx.SaveChanges();
        }

        public IEnumerable<MeasurementPoint> Search(string point, string name, string date, string temp, string prec, string mes)
        {
            var list = Read();
            if (point != "-")
            {
                list = list.Where(s => s.PointNumber == point);
            }
            if (name != "-")
            {
                list = list.Where(s => s.PointName == name);
            }
            if (date != "-")
            {
                DateTime.TryParse(date, out DateTime result);
                list = list.Select(s => new MeasurementPoint
                {
                    PointNumber = s.PointNumber,
                    PointName = s.PointName,
                    Measurements = s.Measurements.Where(f => f.Date == result).ToList()
                }).Where(m => m.Measurements.Any());
            }
            if (temp != "-")
            {
                if (temp[0] == '<')
                {
                    temp = temp.Replace("<", "");
                    list = list.Select(s => new MeasurementPoint
                    {
                        PointNumber = s.PointNumber,
                        PointName = s.PointName,
                        Measurements = s.Measurements.Where(f => f.Temperature < double.Parse(temp, CultureInfo.InvariantCulture)).ToList()
                    }).Where(m => m.Measurements.Any());
                }
                else if (temp[0] == '>')
                {
                    temp = temp.Replace(">", "");
                    list = list.Select(s => new MeasurementPoint
                    {
                        PointNumber = s.PointNumber,
                        PointName = s.PointName,
                        Measurements = s.Measurements.Where(f => f.Temperature > double.Parse(temp, CultureInfo.InvariantCulture)).ToList()
                    }).Where(m => m.Measurements.Any());
                }
                else
                {
                    list = list.Select(s => new MeasurementPoint
                    {
                        PointNumber = s.PointNumber,
                        PointName = s.PointName,
                        Measurements = s.Measurements.Where(f => f.Temperature == double.Parse(temp, CultureInfo.InvariantCulture)).ToList()
                    }).Where(m => m.Measurements.Any());
                }
            }
            if (prec != "-")
            {
                if (prec[0] == '<')
                {
                    prec = prec.Replace("<", "");
                    list = list.Select(s => new MeasurementPoint
                    {
                        PointNumber = s.PointNumber,
                        PointName = s.PointName,
                        Measurements = s.Measurements.Where(f => f.Precipitation < double.Parse(prec, CultureInfo.InvariantCulture)).ToList()
                    }).Where(m => m.Measurements.Any());
                }
                else if (prec[0] == '>')
                {
                    prec = prec.Replace(">", "");
                    list = list.Select(s => new MeasurementPoint
                    {
                        PointNumber = s.PointNumber,
                        PointName = s.PointName,
                        Measurements = s.Measurements.Where(f => f.Precipitation > double.Parse(prec, CultureInfo.InvariantCulture)).ToList()
                    }).Where(m => m.Measurements.Any());
                }
                else
                {
                    list = list.Select(s => new MeasurementPoint
                    {
                        PointNumber = s.PointNumber,
                        PointName = s.PointName,
                        Measurements = s.Measurements.Where(f => f.Precipitation == double.Parse(prec, CultureInfo.InvariantCulture)).ToList()
                    }).Where(m => m.Measurements.Any());
                }
            }
            if (mes != "-")
            {
                if (mes[0] == '<')
                {
                    mes = mes.Replace("<", "");
                    list = list.Select(s => new MeasurementPoint
                    {
                        PointNumber = s.PointNumber,
                        PointName = s.PointName,
                        Measurements = s.Measurements.Where(f => f.Measurement < double.Parse(mes, CultureInfo.InvariantCulture)).ToList()
                    }).Where(m => m.Measurements.Any());
                }
                else if (mes[0] == '>')
                {
                    mes = mes.Replace(">", "");
                    list = list.Select(s => new MeasurementPoint
                    {
                        PointNumber = s.PointNumber,
                        PointName = s.PointName,
                        Measurements = s.Measurements.Where(f => f.Measurement > double.Parse(mes, CultureInfo.InvariantCulture)).ToList()
                    }).Where(m => m.Measurements.Any());
                }
                else
                {
                    list = list.Select(s => new MeasurementPoint
                    {
                        PointNumber = s.PointNumber,
                        PointName = s.PointName,
                        Measurements = s.Measurements.Where(f => f.Measurement == double.Parse(mes, CultureInfo.InvariantCulture)).ToList()
                    }).Where(m => m.Measurements.Any());
                }
            }
            return list;
        }
    }
}
