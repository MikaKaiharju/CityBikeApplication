using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CityBikeApplication.Pages
{
    public class EditStationModel : PageModel
    {

        public string errorMessage = "";

        public Station oldStation;

        public void OnGet()
        {
            oldStation = DataHandler.Instance.GetStation(Request.Query["id"]);
        }

        public void OnPost()
        {
            Station newStation = new Station();
            newStation.id = Request.Form["id"];
            newStation.nimi = Request.Form["nimi"];
            newStation.namn = Request.Form["namn"];
            newStation.name = Request.Form["name"];
            newStation.osoite = Request.Form["osoite"];
            newStation.address = Request.Form["address"];
            newStation.kaupunki = Request.Form["kaupunki"];
            newStation.stad = Request.Form["stad"];
            newStation.operaattori = Request.Form["operaattori"];
            newStation.kapasiteetti = Request.Form["kapasiteetti"];

            string xString = Request.Form["x"];
            string yString = Request.Form["y"];

            // this makes it possible to user use dot instead of comma
            xString = xString.Replace(".", ",");
            yString = yString.Replace(".", ",");

            if (xString.Length > 0)
            {
                try
                {
                    // double is stored in comma form
                    newStation.x = ("" + double.Parse(xString)).Replace(",", "."); // longitude
                }
                catch (Exception e)
                {
                    errorMessage = "Something wrong with the longitude";
                    return;
                }
            }
            else
            {
                newStation.x = "";
            }
            if(yString.Length > 0)
            {
                try
                {
                    // double is stored in comma form
                    newStation.y = ("" + double.Parse(yString)).Replace(",", "."); // latitude
                }
                catch(Exception e)
                {
                    errorMessage = "Something wrong with the latitude";
                    return;
                }
            }
            else
            {
                newStation.y = "";
            }

            oldStation = newStation;
            DataHandler.Instance.ReplaceStation(newStation);
            
            Response.Redirect("StationList");

        }
    }
}
