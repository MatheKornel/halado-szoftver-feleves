using JY24WV_HSZF_2024251.Application;
using JY24WV_HSZF_2024251.Model;
using JY24WV_HSZF_2024251.Persistence.MsSql;
using Moq;
using NUnit.Framework;

namespace JY24WV_HSZF_2024251.Test
{
    [TestFixture]
    public class PointServiceTester
    {
        PointService service;
        Mock<IMeasurmentPointProvider> mockPointProvider;

        [SetUp]
        public void Init()
        {
            mockPointProvider = new Mock<IMeasurmentPointProvider>();
            mockPointProvider.Setup(p => p.Read()).Returns(new List<MeasurementPoint>()
            {
                new MeasurementPoint("101", "Point1")
                {
                    Measurements = new List<MeasurementData>()
                    {
                        new MeasurementData(DateTime.Parse("2024-01-15T00:00:00Z"),10,1,100)
                    }
                },
                new MeasurementPoint("102", "Point2")
                {
                    Measurements= new List<MeasurementData>()
                    {
                        new MeasurementData(DateTime.Parse("2024-02-15T00:00:00Z"),20, 2, 200),
                        new MeasurementData(DateTime.Parse("2024-03-15T00:00:00Z"), 30, 3, 300),
                    }
                },
                new MeasurementPoint("103", "Point3")
                {
                    Measurements=new List<MeasurementData>()
                    {
                        new MeasurementData(DateTime.Parse("2024-04-15T00:00:00Z"), 40, 4, 400),
                        new MeasurementData(DateTime.Parse("2024-04-15T00:00:00Z"), 50, 5, 500),
                        new MeasurementData(DateTime.Parse("2024-04-15T00:00:00Z"), 60, 0.6, 600),
                    }
                },
                new MeasurementPoint("104", "Point4")
                {
                    Measurements=new List<MeasurementData>()
                    {
                        new MeasurementData(DateTime.Parse("2024-04-15T00:00:00Z"), 70, 0.7, 700),
                        new MeasurementData(DateTime.Parse("2024-04-15T00:00:00Z"), 80, 0.8, 800)
                    }
                }
            }.AsEnumerable());
            service = new PointService(mockPointProvider.Object);
        }

        [TestCase("123", "szügy")]
        [TestCase("456", "bgy")]
        [TestCase("789", "pest")]
        public void CreatePointTestCorrect(string number, string name)
        {
            var point = new MeasurementPoint(number, name);
            service.Create(point);
            mockPointProvider.Verify(s => s.Create(point), Times.Once);
        }

        [TestCase("12", "")]
        [TestCase("1234", "szügy")]
        [TestCase("123", "bg")]
        [TestCase("123", "bggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg")]
        public void CreatePointTestInCorrect(string number, string name)
        {
            var point = new MeasurementPoint(number, name);
            try
            {
                service.Create(point);
            }
            catch { }
            mockPointProvider.Verify(s => s.Create(point), Times.Never);
        }

        [TestCase("123", "szügy")]
        [TestCase("456", "bgy")]
        [TestCase("789", "pest")]
        public void UpdatePointTestCorrect(string number, string name)
        {
            var point = new MeasurementPoint(number, name);
            service.Update(point);
            mockPointProvider.Verify(s => s.Update(point), Times.Once);
        }

        [TestCase("12", "")]
        [TestCase("1234", "szügy")]
        [TestCase("123", "bg")]
        [TestCase("123", "bggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg")]
        public void UpdatePointTestInCorrect(string number, string name)
        {
            var point = new MeasurementPoint(number, name);
            try
            {
                service.Update(point);
            }
            catch { }
            mockPointProvider.Verify(s => s.Update(point), Times.Never);
        }

        [TestCase("101")]
        [TestCase("102")]
        [TestCase("103")]
        [TestCase("104")]
        public void DeletePointTestCorrect(string number)
        {
            var point = number;
            service.Delete(point);
            mockPointProvider.Verify(s => s.Delete(point), Times.Once);
        }

        [TestCase("234")]
        [TestCase("211")]
        [TestCase("555")]
        [TestCase("789")]
        public void DeletePointTestInCorrect(string number)
        {
            try
            {
                service.Delete(number);
            }
            catch { }
            mockPointProvider.Verify(s => s.Delete(number), Times.Never);
        }

        [Test]
        public void ReadPointTest()
        {
            service.Read();
            mockPointProvider.Verify(s => s.Read(), Times.Once);
        }

        [TestCase("101", "Point1", "-", "10", "-", "100")]
        [TestCase("102", "Point2", "-", ">20", "2", "-")]
        [TestCase("103", "Point3", "-", "-", "-", "-")]
        [TestCase("104", "Point4", "-", "-", "0.7", "<750")]
        [TestCase("101", "-", "-", "-", "-", "-")]
        [TestCase("102", "-", "-", "-", "<3", "-")]
        [TestCase("103", "-", "-", "40", "4", "400")]
        [TestCase("104", "-", "-", "-", "<1", "-")]
        [TestCase("-", "Point1", "-", "-", "-", "-")]
        [TestCase("103", "Point3", "2024-04-15T00:00:00Z", "50", "5", "500")]
        [TestCase("-", "-", "-", "-", "-", "-")]
        public void SearchTest(string point, string name, string date, string temp, string prec, string mes)
        {
            service.Search(point, name, date, temp, prec, mes);
            mockPointProvider.Verify(s => s.Search(point, name, date, temp, prec, mes), Times.Once);
        }

        [Test]
        public void MeasurementCountTest()
        {
            var result = service.MeasurementCount().ToList();
            var expected = new List<PointService.Q1Info>()
            {
                new PointService.Q1Info()
                {
                    PointNumber = "101",
                    PointName = "Point1",
                    MeasurementCount = 1
                },
                new PointService.Q1Info()
                {
                    PointNumber = "102",
                    PointName = "Point2",
                    MeasurementCount = 2
                },
                new PointService.Q1Info()
                {
                    PointNumber = "103",
                    PointName = "Point3",
                    MeasurementCount = 3
                },
                new PointService.Q1Info()
                {
                    PointNumber = "104",
                    PointName = "Point4",
                    MeasurementCount = 2
                }
            };
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
