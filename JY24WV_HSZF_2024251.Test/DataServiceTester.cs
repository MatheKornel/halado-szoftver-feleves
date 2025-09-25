using JY24WV_HSZF_2024251.Application;
using JY24WV_HSZF_2024251.Model;
using JY24WV_HSZF_2024251.Persistence.MsSql;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JY24WV_HSZF_2024251.Test
{
    [TestFixture]
    public class DataServiceTester
    {
        DataService service;
        Mock<IMeasurmentDataProvider> mockDataProvider;

        [SetUp]
        public void Init()
        {
            mockDataProvider = new Mock<IMeasurmentDataProvider>();
            mockDataProvider.Setup(p => p.ReadOnlyData()).Returns(new List<MeasurementData>()
            {
                new MeasurementData(DateTime.Parse("2024-01-15T00:00:00Z"),10,1,100),
                new MeasurementData(DateTime.Parse("2024-02-15T00:00:00Z"),20, 2, 200),
                new MeasurementData(DateTime.Parse("2024-03-15T00:00:00Z"), 30, 3, 300),
                new MeasurementData(DateTime.Parse("2024-04-15T00:00:00Z"), 40, 4, 400)
            }.AsEnumerable());
            service = new DataService(mockDataProvider.Object);
        }

        [TestCase("101", "2024-01-05T00:00:00Z", 11, 0.5, 110)]
        [TestCase("102", "2024-02-05T00:00:00Z", 21, 0.6, 210)]
        [TestCase("103", "2024-03-05T00:00:00Z", 31, 0.7, 310)]
        [TestCase("104", "2024-04-05T00:00:00Z", 41, 0.8, 410)]
        public void CreateDataTestCorrect(string number, string date, double temp, double prec, int mes)
        {
            DateTime.TryParse(date, out var dateTime);
            var data = new MeasurementData(dateTime, temp, prec, mes);
            service.Create(data, number);
            mockDataProvider.Verify(s => s.Create(data, number), Times.Once);
        }

        [TestCase("101", "2024-01-05T00:00:00Z", 150, 0.5, 110)]
        [TestCase("102", "2024-02-05T00:00:00Z", 21, 9.9, 210)]
        [TestCase("103", "2024-03-05T00:00:00Z", 31, 0.7, 1900)]
        [TestCase("104", "2024-04-05T00:00:00Z", -111, -0.9, -100)]
        public void CreateDataTestInCorrect(string number, string date, double temp, double prec, int mes)
        {
            DateTime.TryParse(date, out var dateTime);
            var data = new MeasurementData(dateTime, temp, prec, mes);
            try
            {
                service.Create(data, number);
            }
            catch { }
            mockDataProvider.Verify(s => s.Create(data, number), Times.Never);
        }

        [TestCase("2024-01-15T00:00:00Z", 11, 0.5, 110)]
        [TestCase("2024-02-15T00:00:00Z", 21, 0.6, 210)]
        [TestCase("2024-03-15T00:00:00Z", 31, 0.7, 310)]
        [TestCase("2024-04-15T00:00:00Z", 41, 0.8, 410)]
        public void UpdateDataTestCorrect(string date, double temp, double prec, int mes)
        {
            DateTime.TryParse(date, out var dateTime);
            var data = new MeasurementData(dateTime, temp, prec, mes);
            service.Update(data);
            mockDataProvider.Verify(s => s.Update(data), Times.Once);
        }

        [TestCase("2024-01-05T00:00:00Z", 150, 0.5, 110)]
        [TestCase("2024-02-05T00:00:00Z", 21, 9.9, 210)]
        [TestCase("2024-03-05T00:00:00Z", 31, 0.7, 1900)]
        [TestCase("2024-05-05T00:00:00Z", -111, -0.9, -100)]
        public void UpdateDataTestInCorrect(string date, double temp, double prec, int mes)
        {
            DateTime.TryParse(date, out var dateTime);
            var data = new MeasurementData(dateTime, temp, prec, mes);
            try
            {
                service.Update(data);
            }
            catch { }
            mockDataProvider.Verify(s => s.Update(data), Times.Never);
        }

        [TestCase("2024-01-15T00:00:00Z")]
        [TestCase("2024-02-15T00:00:00Z")]
        [TestCase("2024-03-15T00:00:00Z")]
        [TestCase("2024-04-15T00:00:00Z")]
        public void DeleteDataTestCorrect(string date)
        {
            DateTime.TryParse(date, out var dateTime);
            service.Delete(dateTime);
            mockDataProvider.Verify(s => s.Delete(dateTime), Times.Once);
        }

        [TestCase("2024-11-05T00:00:00Z")]
        [TestCase("2024-10-05T00:00:00Z")]
        [TestCase("2024-09-05T00:00:00Z")]
        [TestCase("2024-08-05T00:00:00Z")]
        public void DeleteDataTestInCorrect(string date)
        {
            DateTime.TryParse(date, out var dateTime);
            try
            {
                service.Delete(dateTime);
            }
            catch { }
            mockDataProvider.Verify(s => s.Delete(dateTime), Times.Never);
        }

        [TestCase("101")]
        [TestCase("102")]
        [TestCase("103")]
        [TestCase("104")]
        public void ReadDataTest(string number)
        {
            service.Read(number);
            mockDataProvider.Verify(s => s.Read(number), Times.Once);
        }

        [Test]
        public void ReadOnlyDataTest()
        {
            service.ReadOnlyData();
            mockDataProvider.Verify(s => s.ReadOnlyData(), Times.Once);
        }
    }
}
