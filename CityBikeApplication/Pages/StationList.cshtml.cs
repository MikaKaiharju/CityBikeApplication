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
        public int CurrentPageIndex { get; set; } = 0;

        // how many stations are shown per page
        public int StationsPerPage { get; set; } = 20;

        // user can select how many stations are shown per page
        public int[] Choices = new int[] { 10, 20, 50, 100 };

        public void OnGet()
        {

        }

        public void OnPostChangeStationsPerPage(int selection)
        {
            // how many stations are shown per page was changed
            CurrentPageIndex = 0;
            StationsPerPage = selection;
        }

        public int GetPagesCount()
        {
            int count = (int)Math.Ceiling((double)DataHandler.Instance.Stations.Count / (double)StationsPerPage);
            return count;
        }

        public void OnPostChangePage(int index, int perPage)
        {
            CurrentPageIndex = index;
            StationsPerPage = perPage;
            GetStations();
        }

        public List<Station> GetStations()
        {
            // show only a certain amount of stations per page
            int startIndex = CurrentPageIndex * StationsPerPage;

            // need to know how many stations can be shown 
            int leftOver = DataHandler.Instance.Stations.Count - startIndex + 1;
            if (leftOver > StationsPerPage)
            {
                return DataHandler.Instance.Stations.GetRange(startIndex, StationsPerPage);
            }
            else
            {
                return DataHandler.Instance.Stations.GetRange(startIndex, leftOver - 1);
            }

        }

        public void OnPostDelete(int id, int index, int perPage)
        {
            DataHandler.Instance.DeleteStation(id);

            // remember how many journeys are on page
            StationsPerPage = perPage;

            // if stations count is divisible with stationsPerPage last page is blank
            if (DataHandler.Instance.Stations.Count % StationsPerPage == 0 && index == GetPagesCount())
            {
                index--;
            }
            // set showing page to index
            OnPostChangePage(index, StationsPerPage);
        }

        public void OnPostSortStations(DataHandler.SortOrder sortStationString, int selection)
        {
            DataHandler.Instance.SortStations(sortStationString);
            StationsPerPage = selection;
        }
    }
}
