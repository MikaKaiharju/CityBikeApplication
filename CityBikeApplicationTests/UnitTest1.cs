using Microsoft.VisualStudio.TestTools.UnitTesting;
using CityBikeApplication;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace CityBikeApplicationTests
{
    [TestClass]
    public class UnitTest1
    {
      
        [TestMethod]
        public void TestStationDataImport()
        {
            // csv data incoming should consist of multiple lines and each line should have
            // 13 strings when splitted with ','

            // Importing should skip line when:
            //      * there is not 13 strings in the line when splitted with ','
            //      * id of the station is not integer
            //      * coordinate x or y can't be parsed to double
            //      * station capacity is not integer

            // Things that are allowed:
            //      * string length is 0

            DataHandler dataHandler = DataHandler.Instance;
            List<Station> stations = dataHandler.ImportStationData(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/TestData/StationTestData.txt");
            Assert.AreEqual(stations.Count, 1);
        }

        [TestMethod]
        public void TestJourneyDataImport()
        {
            // csv data incoming should consist of multiple lines and each line should have
            // 8 strings when splitted with ','

            // Importing should skip line when:
            //      * there is not 8 strings in the line when splitted with ','
            //      * departureTime or returnTime is not dateTime
            //      * departure station id or return station id is not integer
            //      * coveredDistance is not integer or distance < 10 metres
            //      * journeyDuration is not integer or duration < 10 seconds

            // Things that are allowed:
            //      * station names can have length of 0

            DataHandler dataHandler = DataHandler.Instance;
            List<Journey> journeys = dataHandler.ImportJourneyData(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/TestData/JourneyTestData.txt");
            Assert.AreEqual(journeys.Count, 1);
        }

    }
}
