using Castle.Core.Internal;
using ConsoleTools;
using JY24WV_HSZF_2024251.Application;
using JY24WV_HSZF_2024251.Model;
using JY24WV_HSZF_2024251.Persistence.MsSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NAudio.Wave;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JY24WV_HSZF_2024251
{
    internal class Program
    {
        private static void Data_waterMax()
        {
            Console.WriteLine("This is the highest water level at the point!");
        }

        static void FileHandling(IPointService point, IDataService data)
        {
            Console.Write("File path: ");
            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                var root = JsonConvert.DeserializeObject<MeasurementRoot>(File.ReadAllText(path));
                var points = root.MeasurementPoints;
                foreach (var p in points)
                {
                    var existingPoint = point.Read().FirstOrDefault(x => x.PointNumber == p.PointNumber);
                    if (existingPoint == null)
                    {
                        point.Create(p);
                    }
                    else
                    {
                        foreach (var d in p.Measurements)
                        {
                            var existingData = existingPoint.Measurements.FirstOrDefault(m => m.Date == d.Date);
                            if (existingData == null)
                            {
                                data.Create(d, p.PointNumber);
                            }
                        }
                    }
                }
                Console.WriteLine("File loaded successfully!");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("File not found!");
                Console.ReadLine();
            }
        }

        static void ReadPoint(IPointService point)
        {
            var list = point.Read();
            foreach (var item in list)
            {
                Console.WriteLine(item.PointNumber + " - " + item.PointName);
                foreach (var data in item.Measurements)
                {
                    Console.WriteLine("\t- " + data.Date + "\n\t\t- Temperature: " + data.Temperature + " °C" + "\n\t\t- Precipitation: " + data.Precipitation + " mm" + "\n\t\t- Measurement: " + data.Measurement + " cm");
                    Console.WriteLine();
                }
            }
            Console.ReadLine();
        }

        static void CreatePoint(IPointService point)
        {
            Type t = typeof(MeasurementPoint);
            var number = t.GetProperty("PointNumber").GetCustomAttribute<StringLengthAttribute>();
            var name = t.GetProperty("PointName").GetCustomAttribute<StringLengthAttribute>();

            Console.Write("Point Number: ");
            string pointNumber = Console.ReadLine();

            if (pointNumber.Length == number.MinimumLength && pointNumber.Length == number.MaximumLength)
            {
                var existPoint = point.Read().FirstOrDefault(s => s.PointNumber == pointNumber);
                if (existPoint == null)
                {
                    Console.Write("Point Name: ");
                    string pointName = Console.ReadLine();
                    if (pointName.Length >= name.MinimumLength && pointName.Length <= name.MaximumLength)
                    {
                        MeasurementPoint p = new MeasurementPoint(pointNumber, pointName);
                        point.Create(p);
                        Console.WriteLine(pointNumber + " - " + pointName + " created successfully!");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Point Name must be between 3 and 50 characters!");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Point already exist!");
                    Console.ReadLine();
                }

            }
            else
            {
                Console.WriteLine("Point Number must be 3 characters long!");
                Console.ReadLine();
            }
        }

        static void UpdatePoint(IPointService point)
        {
            Type t = typeof(MeasurementPoint);
            var number = t.GetProperty("PointNumber").GetCustomAttribute<StringLengthAttribute>();
            var name = t.GetProperty("PointName").GetCustomAttribute<StringLengthAttribute>();

            Console.Write("Point Number: ");
            string pointNumber = Console.ReadLine();
            if (pointNumber.Length == number.MinimumLength && pointNumber.Length == number.MaximumLength)
            {
                var existPoint = point.Read().FirstOrDefault(s => s.PointNumber == pointNumber);
                if (existPoint != null)
                {
                    Console.Write("New Point Name: ");
                    string pointNameNew = Console.ReadLine();
                    if (pointNameNew.Length >= name.MinimumLength && pointNameNew.Length <= name.MaximumLength)
                    {
                        MeasurementPoint p = new MeasurementPoint(pointNumber, pointNameNew);
                        point.Update(p);
                        Console.WriteLine(pointNumber + " - " + pointNameNew + " modified successfully!");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Point Name must be between 3 and 50 characters!");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Point does not exist!");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Point Number must be 3 characters long!");
                Console.ReadLine();
            }
        }

        static void DeletePoint(IPointService point)
        {
            Type t = typeof(MeasurementPoint);
            var number = t.GetProperty("PointNumber").GetCustomAttribute<StringLengthAttribute>();

            Console.Write("Point Number: ");
            string pointNumber = Console.ReadLine();
            if (pointNumber.Length == number.MinimumLength && pointNumber.Length == number.MaximumLength)
            {
                var existPoint = point.Read().FirstOrDefault(s => s.PointNumber == pointNumber);
                if (existPoint != null)
                {
                    point.Delete(pointNumber);
                    Console.WriteLine(pointNumber + " - " + existPoint.PointName + " deleted successfully!");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Point does not exist!");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Point Number must be 3 characters long!");
                Console.ReadLine();
            }
        }

        static void ReadData(IDataService data, IPointService point)
        {
            Type tnumber = typeof(MeasurementPoint);
            var number = tnumber.GetProperty("PointNumber").GetCustomAttribute<StringLengthAttribute>();
            Console.Write("Point Number: ");
            string pointNumber = Console.ReadLine();
            if (pointNumber.Length == number.MinimumLength && pointNumber.Length == number.MaximumLength)
            {
                var pointExist = point.Read().FirstOrDefault(s => s.PointNumber == pointNumber);
                if (pointExist != null)
                {
                    var list = data.Read(pointNumber);
                    foreach (var item in list)
                    {
                        Console.WriteLine(item.PointNumber + " - " + item.PointName);
                        foreach (var d in item.Measurements)
                        {
                            Console.WriteLine("\t- " + d.Date + "\n\t\t- Temperature: " + d.Temperature + " °C" + "\n\t\t- Precipitation: " + d.Precipitation + " mm" + "\n\t\t- Measurement: " + d.Measurement + " cm");
                            Console.WriteLine();
                        }
                    }
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Point does not exist!");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Point Number must be 3 characters long!");
                Console.ReadLine();
            }

        }

        static void CreateData(IDataService data, IPointService point)
        {
            Type tnumber = typeof(MeasurementPoint);
            var number = tnumber.GetProperty("PointNumber").GetCustomAttribute<StringLengthAttribute>();
            data.waterMax -= Data_waterMax;

            Console.Write("Point Number: ");
            string pointNumber = Console.ReadLine();
            if (pointNumber.Length == number.MinimumLength && pointNumber.Length == number.MaximumLength)
            {
                var pointExist = point.Read().FirstOrDefault(s => s.PointNumber == pointNumber);
                if (pointExist != null)
                {
                    Console.Write("Date [2024. 12. 31. 00:00:00]: ");
                    string date = Console.ReadLine();
                    var validdate = DateTime.TryParse(date, out DateTime result);
                    if (validdate)
                    {
                        var existDate = data.ReadOnlyData().FirstOrDefault(s => s.Date == result);
                        if (existDate == null)
                        {
                            Type t = typeof(MeasurementData);
                            var attrtemp = t.GetProperty("Temperature").GetAttribute<RangeAttribute>();
                            Console.Write("Temperature [0.0 °C]: ");
                            double temp = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                            if (temp >= Convert.ToDouble(attrtemp.Minimum) && temp <= Convert.ToDouble(attrtemp.Maximum))
                            {
                                var attrprec = t.GetProperty("Precipitation").GetAttribute<RangeAttribute>();
                                Console.Write("Precipitation [0.0 mm]: ");
                                double prec = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                                if (prec >= Convert.ToDouble(attrprec.Minimum) && prec <= Convert.ToDouble(attrprec.Maximum))
                                {
                                    var attrme = t.GetProperty("Measurement").GetAttribute<RangeAttribute>();
                                    Console.Write("Measurement [0 cm]: ");
                                    int measurement = int.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                                    if (measurement >= Convert.ToDouble(attrme.Minimum) && measurement <= Convert.ToDouble(attrme.Maximum))
                                    {
                                        data.waterMax += Data_waterMax;
                                        MeasurementData d = new MeasurementData(result, temp, prec, measurement);
                                        data.Create(d, pointNumber);
                                        Console.WriteLine("Data created successfully!");
                                        Console.ReadLine();

                                    }
                                    else
                                    {
                                        Console.WriteLine("Wrong data! Measurement must be between 0 and 1500!");
                                        Console.ReadLine();
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Worng data! Precipitation must be between 0 and 5!");
                                    Console.ReadLine();
                                }
                            }
                            else
                            {
                                Console.WriteLine("Wrong data! Temperature must be between -100 and 100!");
                                Console.ReadLine();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Data already exist!");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong date format!");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Point does not exist!");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Point Number must be 3 characters long!");
                Console.ReadLine();
            }
        }

        static void UpdateData(IDataService data)
        {
            Console.Write("Date [2024. 12. 31. 00:00:00]: ");
            string date = Console.ReadLine();
            var validdate = DateTime.TryParse(date, out DateTime result);
            if (validdate)
            {
                var existDate = data.ReadOnlyData().FirstOrDefault(s => s.Date == result);
                if (existDate != null)
                {
                    Type t = typeof(MeasurementData);
                    var attrtemp = t.GetProperty("Temperature").GetAttribute<RangeAttribute>();
                    Console.Write("New Temperature [0.0 °C]: ");
                    double temp = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                    if (temp >= Convert.ToDouble(attrtemp.Minimum) && temp <= Convert.ToDouble(attrtemp.Maximum))
                    {
                        var attrprec = t.GetProperty("Precipitation").GetAttribute<RangeAttribute>();
                        Console.Write("New Precipitation [0.0 mm]: ");
                        double prec = double.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                        if (prec >= Convert.ToDouble(attrprec.Minimum) && prec <= Convert.ToDouble(attrprec.Maximum))
                        {
                            var attrme = t.GetProperty("Measurement").GetAttribute<RangeAttribute>();
                            Console.Write("New Measurement [0 cm]: ");
                            int measurement = int.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
                            if (measurement >= Convert.ToDouble(attrme.Minimum) && measurement <= Convert.ToDouble(attrme.Maximum))
                            {
                                MeasurementData d = new MeasurementData(result, temp, prec, measurement);
                                data.Update(d);
                                Console.WriteLine(result + " data updated successfully!");
                                Console.ReadLine();
                            }
                            else
                            {
                                Console.WriteLine("Wrong data! Measurement must be between 0 and 1500!");
                                Console.ReadLine();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Worng data! Precipitation must be between 0 and 5!");
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong data! Temperature must be between -100 and 100!");
                        Console.ReadLine();
                    }
                }
                else
                {
                    Console.WriteLine("Data does not exist!");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Wrong date format!");
                Console.ReadLine();
            }
        }

        static void DeleteData(IDataService data)
        {
            Console.Write("Date [2024. 12. 31. 00:00:00]: ");
            string date = Console.ReadLine();
            var validdate = DateTime.TryParse(date, out DateTime result);
            if (validdate)
            {
                var existDate = data.ReadOnlyData().FirstOrDefault(s => s.Date == result);
                if (existDate != null)
                {
                    data.Delete(result);
                    Console.WriteLine(result + " data deleted successfully!");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Data does not exist!");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Wrong date format!");
                Console.ReadLine();
            }
        }

        static void Statistics(IPointService point)
        {
            //Mérőpontonként az adott mérőponthoz tartozó mérések száma.
            var q1 = point.MeasurementCount();

            //Mérőpontonként az átlagos vízállás mértéke, a legmagasabb és legalacsonyabb mérések adataival.
            var q2 = point.AvgMeasurement();

            //Mérőpontonként azok a mérések, ahol a csapadék mennyisége több mint az átlag.
            var q3 = point.MoreThanAvgPrecipitation();

            Console.Write("File path [.txt]: ");
            string path = Console.ReadLine();
            if (path.Length > 4 && path.Substring(path.Length - 4, 4) == ".txt")
            {
                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine("--------Statistics--------\n");
                sw.WriteLine("Number of measurements per measurement point:");
                foreach (var item in q1)
                {
                    sw.WriteLine(" -" + item.PointNumber + " - " + item.PointName + ": " + item.MeasurementCount + " measurements");
                }
                sw.WriteLine("\nThe average water level per measuring point, with the data of the highest and lowest measurements:");
                foreach (var item in q2)
                {
                    sw.WriteLine(" -" + item.PointNumber + " - " + item.PointName + ":");
                    sw.WriteLine("\tAverage Water Level: " + item.AvgWaterLevel + " cm\n\tHighest Water Level: " + item.HighestWaterLevel + " cm\n\tLowest Water Level: " + item.LowestWaterLevel + " cm\n");
                }
                sw.WriteLine("Measurements per measuring point where the amount of precipitation is more than average:");
                foreach (var item in q3)
                {
                    sw.WriteLine(" -" + item.PointNumber + " - " + item.PointName + ":");
                    sw.WriteLine("\tAverage Precipitation: " + item.AvgPrecipitation + " mm");
                    if (item.AboveAvgPrecipitations.Count > 0)
                    {
                        sw.WriteLine("\tAbove this:\n");
                        foreach (var i in item.AboveAvgPrecipitations)
                        {
                            sw.WriteLine("\t -" + i.Date + "\n\t -Temperature: " + i.Temperature + " °C" + "\n\t -Precipitation: " + i.Precipitation + " mm" + "\n\t -Measurement: " + i.Measurement + " cm\n");
                        }
                    }
                    else
                    {
                        sw.WriteLine("\tAbove this: none\n");
                    }
                }
                sw.Close();
                Console.WriteLine("Statistics generated successfully!");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Wrong output format!");
                Console.ReadLine();
            }

        }

        static void Search(IPointService point)
        {
            Console.WriteLine("Order: PointNumber PointName Date (*)Temperature (*)Precipitation (*)Measurement");
            Console.WriteLine("Split with SPACE.");
            Console.WriteLine("If you want to skip a parameter, add (-).");
            Console.WriteLine("You can use (<, >) operators at the marked (*) parameters.");
            Console.Write("Search: ");
            string[] input = Console.ReadLine().Split(' ');
            if (input.Length == 6)
            {
                string number = input[0];
                string name = input[1];
                string date = input[2];
                string temp = input[3];
                string prec = input[4];
                string mes = input[5];
                var list = point.Search(number, name, date, temp, prec, mes);
                var exist = list.ToList();
                if (exist.Count > 0)
                {
                    foreach (var item in list)
                    {
                        Console.WriteLine(item.PointNumber + " - " + item.PointName);
                        foreach (var d in item.Measurements)
                        {
                            Console.WriteLine("\t- " + d.Date + "\n\t\t- Temperature: " + d.Temperature + " °C" + "\n\t\t- Precipitation: " + d.Precipitation + " mm" + "\n\t\t- Measurement: " + d.Measurement + " cm");
                            Console.WriteLine();
                        }
                    }
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Data not found!");
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("Wrong input format!");
                Console.ReadLine();
            }
        }

        static void Flood()
        {
            string musicFilePath = "music.wav";
            using (var audioFile = new AudioFileReader(musicFilePath))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                int width = Console.WindowWidth;
                int height = Console.WindowHeight;

                double baseAmplitude = height / 6;
                double frequency = 0.3;
                double waveOffset = 0;

                char waveChar = '~';

                ConsoleColor[] blueColors = { ConsoleColor.DarkBlue, ConsoleColor.Blue, ConsoleColor.Cyan, ConsoleColor.DarkCyan };
                Random random = new Random();

                int waveStartY = height - 1;
                char[,] waveBuffer = new char[height, width];
                while (waveStartY > 0)
                {
                    for (int x = 0; x < width; x++)
                    {
                        double radians = x * frequency + waveOffset;
                        double sineValue = Math.Sin(radians);
                        double amplitude = baseAmplitude * (1 - ((waveStartY) / (double)height));
                        int y = (int)(waveStartY - amplitude + sineValue * amplitude);
                        y = Math.Clamp(y, 0, height - 1);
                        Console.ForegroundColor = blueColors[random.Next(blueColors.Length)];
                        waveBuffer[y, x] = waveChar;
                    }
                    Console.Clear();
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            if (waveBuffer[i, j] != '\0')
                            {
                                Console.SetCursorPosition(j, i);
                                Console.Write(waveBuffer[i, j]);
                            }
                        }
                    }
                    waveStartY--;
                    waveOffset += 0.15;

                    Thread.Sleep(100);
                }
                string message = "DROWNED";
                int messageLength = message.Length;
                int messageX = (width - messageLength) / 2;
                int messageY = height / 2;

                Console.SetCursorPosition(messageX, messageY);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(message);

                Console.SetCursorPosition(0, height - 1);
                Console.ForegroundColor = blueColors[random.Next(blueColors.Length)];
                for (int i = 0; i < width; i++)
                {
                    Console.Write(waveChar);
                }

                Console.ResetColor();
                Console.ReadLine();
                outputDevice.Stop();
            }
        }

        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                            .ConfigureServices((hostContext, services) =>
                            {
                                services.AddScoped<AppDbContext>();
                                services.AddSingleton<IMeasurmentDataProvider, MeasurementDataProvider>();
                                services.AddSingleton<IDataService, DataService>();
                                services.AddSingleton<IMeasurmentPointProvider, MeasurementPointProvider>();
                                services.AddSingleton<IPointService, PointService>();
                            })
                            .Build();
            host.Start();

            using IServiceScope serviceScope = host.Services.CreateScope();
            var pointService = host.Services.GetRequiredService<IPointService>();
            var dataService = host.Services.GetRequiredService<IDataService>();

            var subPoints = new ConsoleMenu(args, level: 1)
            .Add("List Points", () => ReadPoint(pointService))
            .Add("Add Point", () => CreatePoint(pointService))
            .Add("Update Point", () => UpdatePoint(pointService))
            .Add("Delete Point", () => DeletePoint(pointService))
            .Add("Back", ConsoleMenu.Close);

            var subData = new ConsoleMenu(args, level: 1)
            .Add("List Data", () => ReadData(dataService, pointService))
            .Add("Add Data", () => CreateData(dataService, pointService))
            .Add("Update Data", () => UpdateData(dataService))
            .Add("Delete Data", () => DeleteData(dataService))
            .Add("Back", ConsoleMenu.Close);

            var menu = new ConsoleMenu(args, level: 0)
              .Add("Measurement Points", () => subPoints.Show())
              .Add("Measurement Data", () => subData.Show())
              .Add("Search", () => Search(pointService))
              .Add("Generate statistics", () => Statistics(pointService))
              .Add("Load JSON", () => FileHandling(pointService, dataService))
              .Add("Let's check the water level", () => Flood())
              .Add("Exit", () => Environment.Exit(0));

            menu.Show();
        }
    }
}
