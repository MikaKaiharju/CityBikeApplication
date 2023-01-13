using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CityBikeApplication.Pages
{
    public class CreateNewStationModel : PageModel
    {
        public List<string> errorMessages = new List<string>();

        // station that user is editing
        // if there were errors, information given by the user is stored in oldStation
        public Station oldStation { get { return (oldStation == null ? new Station() : oldStation); } set { } }

        public void OnGet()
        {
            
        }

        public void GetOldStation()
        {

        }

        public void OnPost()
        {

            // remove previous errorMessages
            errorMessages.Clear();

            Station newStation = new Station();
            string idString = Sanitize(Request.Form["id"]);
            newStation.nimi = Sanitize(Request.Form["nimi"]);
            newStation.namn = Sanitize(Request.Form["namn"]);
            newStation.name = Sanitize(Request.Form["name"]);
            newStation.osoite = Sanitize(Request.Form["osoite"]);
            newStation.address = Sanitize(Request.Form["address"]);
            newStation.kaupunki = Sanitize(Request.Form["kaupunki"]);
            newStation.stad = Sanitize(Request.Form["stad"]);
            newStation.operaattori = Sanitize(Request.Form["operaattori"]);
            string kapasiteettiString = Sanitize(Request.Form["kapasiteetti"]);
            string xString = Sanitize(Request.Form["x"]);
            string yString = Sanitize(Request.Form["y"]);

            if (idString.Length > 0)
            {
                try
                {
                    int id = int.Parse(idString);

                    if (id < 0)
                    {
                        errorMessages.Add("Id needs to be integer that is >= 0");
                    }
                    else
                    {
                        // check if there is already a station with this new id and its not the old
                        Station station = DataHandler.Instance.GetStation(id);
                        if (station != null)
                        {
                            errorMessages.Add("There is already a station with this id: id=" + station.id + " name="+ station.name);
                        }
                        
                        newStation.id = id;
                    }

                }
                catch (Exception e)
                {
                    errorMessages.Add("Id needs to be integer that is >= 0");
                }

            }
            else
            {
                errorMessages.Add("Id is required and needs to be integer that is >= 0");
            }

            if (kapasiteettiString.Length > 0)
            {
                try
                {
                    int kapasiteetti = int.Parse(kapasiteettiString);

                    if (kapasiteetti < 0)
                    {
                        newStation.kapasiteetti = kapasiteetti;
                        errorMessages.Add("Capacity needs to be >= 0");
                    }
                    else
                    {
                        newStation.kapasiteetti = kapasiteetti;
                    }
                }
                catch (Exception e)
                {
                    errorMessages.Add("Capacity needs to be integer that is >= 0");
                }
            }

            // double parse needs number to be in comma form
            xString = xString.Replace(".", ",");
            yString = yString.Replace(".", ",");

            if (xString.Length > 0)
            {
                try
                {
                    // store in dot form
                    newStation.x = ("" + double.Parse(xString)).Replace(",", "."); // longitude
                }
                catch (Exception e)
                {
                    errorMessages.Add("Longitude must be a decimal number with either . or , as a separator");
                    return;
                }
            }
            else
            {
                newStation.x = "";
            }
            if (yString.Length > 0)
            {
                try
                {
                    // store in dot form
                    newStation.y = ("" + double.Parse(yString)).Replace(",", "."); // latitude
                }
                catch (Exception e)
                {
                    errorMessages.Add("Latitude must be a decimal number with either . or , as a separator");
                    return;
                }
            }
            else
            {
                newStation.y = "";
            }

            // if there were errors remember what data was given
            oldStation = newStation;

            if (errorMessages.Count == 0)
            {
                DataHandler.Instance.CreateNewStation(newStation);
                Response.Redirect("StationList");
            }
        }

        private string Sanitize(string str)
        {
            // remove special characters
            if(str != null)
            {
                return Regex.Replace(str, "[^a-öA-Ö0-9_ .,()]", "", RegexOptions.Compiled);
            }
            else { return ""; }
        }

        private void p(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }
    }
}

