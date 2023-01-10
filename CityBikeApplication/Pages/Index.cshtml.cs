using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Globalization;

namespace CityBikeApplication.Pages
{
    public class IndexModel : PageModel
    {

        public void OnGet()
        {
            DataHandler dataHandler = DataHandler.Instance;

            if (dataHandler.ready)
            {
                UpdateProgress("Datahandler on ready");
            }
            else
            {
                UpdateProgress("Datahandler ei oo ready");
            }

        }



        public void UpdateProgress(string s)
        {
            System.Diagnostics.Debug.WriteLine(s);
        }

    }
}
