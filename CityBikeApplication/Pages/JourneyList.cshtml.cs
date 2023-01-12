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
        public int currentPageIndex = 0;

        // how many journeys are shown per page
        public int journeysPerPage = 20;

        // user can select how many journeys are shown per page
        public int[] Choices = new int[] { 10, 20, 50, 100 };

        public void OnGet()
        {

        }

        public void OnPostChangeJourneysPerPage(int selection)
        {
            // how many journeys are shown per page was changed
            currentPageIndex = 0;
            journeysPerPage = selection;
        }

        public int GetPagesCount()
        {
            int count = (int)Math.Ceiling((double)DataHandler.Instance.journeys.Count / (double)journeysPerPage);
            return count;
        }

        public void OnPostChangePage(int index, int perPage)
        {
            currentPageIndex = index;
            journeysPerPage = perPage;
            GetJourneys();
        }

        public void OnPostSortJourneys(string sortJourneyString)
        {
            DataHandler.Instance.SortJourneys(sortJourneyString);
        }


        public void OnPostDelete(string id, int index)
        {
            DataHandler.Instance.DeleteJourney(id);
            // if journeys count is divisible with journeysPerPage last page is blank
            if (DataHandler.Instance.journeys.Count % journeysPerPage == 0 && index == GetPagesCount()) 
            {
                index--;
            }
            // set showing page to index
            OnPostChangePage(index, journeysPerPage);
        }

        public List<Journey> GetJourneys()
        {
            // show only a certain amount of journeys per page
            int startIndex = currentPageIndex * journeysPerPage;

            // need to know how many journeys can be shown 
            int leftOver = DataHandler.Instance.journeys.Count - startIndex + 1;
            if(leftOver > journeysPerPage)
            {
                return DataHandler.Instance.journeys.GetRange(startIndex, journeysPerPage);
            }
            else
            {
                return DataHandler.Instance.journeys.GetRange(startIndex, leftOver - 1);
            }
            
        }

        private void p(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

    }
}
