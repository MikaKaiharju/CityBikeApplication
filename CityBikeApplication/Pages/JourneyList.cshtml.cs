using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CityBikeApplication.Pages
{
    public class JourneyListModel : PageModel
    {

        // index used in paging
        public int CurrentPageIndex = 0;

        // how many journeys are shown per page
        public int JourneysPerPage { get; set; } = 20;

        // user can select how many journeys are shown per page
        public int[] Choices = new int[] { 10, 20, 50, 100 };

        public void OnGet()
        {

        }

        public void OnPostChangeJourneysPerPage(int selection)
        {
            // how many journeys are shown per page was changed
            CurrentPageIndex = 0;
            JourneysPerPage = selection;
        }

        public int GetPagesCount()
        {
            int count = (int)Math.Ceiling((double)DataHandler.Instance.Journeys.Count / (double)JourneysPerPage);
            return count;
        }

        public void OnPostChangePage(int index, int perPage)
        {    
            CurrentPageIndex = index;
            JourneysPerPage = perPage;
            GetJourneys();
        }

        public void OnPostSortJourneys(DataHandler.SortOrder sortJourneyString, int selection)
        {
            DataHandler.Instance.SortJourneys(sortJourneyString);
            JourneysPerPage = selection;
        }


        public void OnPostDelete(string id, int index, int perPage)
        {
            DataHandler.Instance.DeleteJourney(id);
            
            // remember how many journeys are on page
            JourneysPerPage = perPage;

            // if journeys count is divisible with journeysPerPage last page is blank
            if (DataHandler.Instance.Journeys.Count % JourneysPerPage == 0 && index == GetPagesCount()) 
            {
                index--;
            }
            // set showing page to index
            OnPostChangePage(index, JourneysPerPage);
        }

        public List<Journey> GetJourneys()
        {
            // show only a certain amount of journeys per page
            int startIndex = CurrentPageIndex * JourneysPerPage;

            // need to know how many journeys can be shown 
            int leftOver = DataHandler.Instance.Journeys.Count - startIndex + 1;

            if(leftOver > JourneysPerPage)
            {
                return DataHandler.Instance.Journeys.GetRange(startIndex, JourneysPerPage);
            }
            else
            {
                return DataHandler.Instance.Journeys.GetRange(startIndex, leftOver - 1);
            }
        }

        private void p(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

    }
}
