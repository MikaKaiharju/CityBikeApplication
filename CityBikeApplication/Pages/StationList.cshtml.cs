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

        // index used in paging
        public int currentPageIndex = 0;

        // how many stations are shown per page
        public int stationsPerPage = 20;

        // user can select how many stations are shown per page
        public int[] Choices = new int[] { 10, 20, 50, 100 };

        public void OnGet()
        {

        }

        public void OnPostChangeStationsPerPage(int selection)
        {
            // how many stations are shown per page was changed
            currentPageIndex = 0;
            stationsPerPage = selection;
        }

        public int GetPagesCount()
        {
            int count = (int)Math.Ceiling((double)DataHandler.Instance.stations.Count / (double)stationsPerPage);
            return count;
        }

        public void OnPostChangePage(int index, int perPage)
        {
            currentPageIndex = index;
            stationsPerPage = perPage;
            GetStations();
        }

        public List<Station> GetStations()
        {
            // show only a certain amount of stations per page
            int startIndex = currentPageIndex * stationsPerPage;

            // need to know how many stations can be shown 
            int leftOver = DataHandler.Instance.stations.Count - startIndex + 1;
            if (leftOver > stationsPerPage)
            {
                return DataHandler.Instance.stations.GetRange(startIndex, stationsPerPage);
            }
            else
            {
                return DataHandler.Instance.stations.GetRange(startIndex, leftOver - 1);
            }

        }

        public void OnPostDelete(int id, int index)
        {
            DataHandler.Instance.DeleteStation(id);
            // if stations count is divisible with stationsPerPage last page is blank
            if (DataHandler.Instance.stations.Count % stationsPerPage == 0 && index == GetPagesCount())
            {
                index--;
            }
            // set showing page to index
            OnPostChangePage(index, stationsPerPage);
        }

        public void OnPostSortStations(string sortStationString, int selection)
        {
            DataHandler.Instance.SortStations(sortStationString);
            stationsPerPage = selection;
        }
    }
}
