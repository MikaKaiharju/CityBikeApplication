using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace CityBikeApplication.Pages
{
    public class JourneyListModel : PageModel
    {
        // connection string to be used in sql database queries
        string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CityBikeDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public void OnGet()
        {
            


        }

        public List<Journey> GetJourneys()
        {
            // read journeys from database

            List<Journey> journeys = new List<Journey>();            

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();

                    string commandString = "SELECT * FROM journeys";
                    using (SqlCommand command = new SqlCommand(commandString, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Journey journey = new Journey();
                                journey.departureTime = reader.GetString(1);
                                journey.departureStationId = reader.GetString(2);
                                journey.departureStationName = reader.GetString(3);
                                journey.returnStationId = reader.GetString(4);
                                journey.returnStationName = reader.GetString(5);
                                journey.coveredDistance = reader.GetInt32(6);
                                journey.duration = reader.GetInt32(7);
                                journeys.Add(journey);
                            }
                        }

                    }

                    connection.Close();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error in reading: " + e);
            }

            return journeys;
        }

    }
}
