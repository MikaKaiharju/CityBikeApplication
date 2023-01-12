using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CityBikeApplication.Pages
{
    public class StationListModel : PageModel
    {

        public void OnGet()
        {

        }

        public void OnPostDelete(string id)
        {
            DataHandler.Instance.DeleteStation(id);
        }

        public void OnPostSortStations(string sortStationString)
        {
            DataHandler.Instance.SortStations(sortStationString);
        }
    }
}
