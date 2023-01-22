using Microsoft.VisualStudio.TestTools.UnitTesting;
using CityBikeApplication;
using System;

namespace CityBikeApplicationTests
{
    [TestClass]
    public class UnitTest1
    {
        Random random = new Random();


        [TestMethod]
        public void TestGetStation()
        {
            DataHandler dataHandler = DataHandler.Instance;

            // create a bunch of test stations
            Station testStation = CreateTestStations(20);

            // since station id must be > 0 first two should return null
            Station station1 = dataHandler.GetStation(-1);
            Assert.IsNull(station1);

            Station station2 = dataHandler.GetStation(0);
            Assert.IsNull(station2);

            // this should return a non null since we created one with id=12 in CreateTestStations
            Station station4 = dataHandler.GetStation(12);
            Assert.AreEqual(testStation, station4);
        }

        private Station CreateTestStations(int stationCount)
        {
            DataHandler dataHandler = DataHandler.Instance;

            Station testStation = new Station();
            testStation.Id = 12;
            testStation.Name = "testName";
            testStation.Operator = "testOperator";
            testStation.Address = "testAddress";
            testStation.Capacity = 12;
            testStation.City = "testCity";
            testStation.X = "" + 12.34567;
            testStation.Y = "" + 12.34567;

            dataHandler.Stations.Add(testStation);

            for(int i = 0; i < stationCount - 1; i++)
            {
                Station testStation1 = new Station();
                testStation1.Id = GetRandomInt(1, 1000);
                testStation1.Name = GetRandomString(10);
                testStation1.Operator = GetRandomString(12);
                testStation1.Address = GetRandomString(14);
                testStation1.Capacity = GetRandomInt(0, 20);
                testStation1.City = GetRandomString(8);
                testStation1.X = "" + GetRandomDouble(10);
                testStation1.Y = "" + GetRandomDouble(10);

                dataHandler.Stations.Add(testStation1);
            }

            // return the first one
            return testStation;
        }

        private int GetRandomInt(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        private double GetRandomDouble(int factor)
        {
            return (double)factor * random.NextDouble();
        }

        private string GetRandomString(int stringLength)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var charArray = new char[stringLength];
            for(int i = 0; i < stringLength; i++)
            {
                charArray[i] = chars[random.Next(chars.Length)];
            }
            return new string(charArray);
        }
    }
}
