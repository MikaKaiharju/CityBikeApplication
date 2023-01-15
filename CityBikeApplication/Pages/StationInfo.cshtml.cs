using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CityBikeApplication.Pages
{
    public class StationInfoModel : PageModel
    {

        // store station info
        public Station Station { get; set; }
        
        public void OnGet()
        {

        }

        public void GetStation()
        {
            Station = DataHandler.Instance.GetStation(int.Parse(Request.Query["id"]));
        }


        public void OnPost()
        {
            GetStation();
            Response.Redirect("EditStation?id=" + Station.Id + "&cameFromStationInfo=true");
        }

    }
}
