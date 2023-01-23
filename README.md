**City Bike Application**

This web app is the pre-assignment for Solita Dev Academy

This project was done by using Microsoft Visual Studio 2019 which used ***.NET 5*** which is also required to run this application.

**Things to know**

Application downloads data from url and imports data asyncronously in the background during startup. When all data is downloaded and imported, data appears in the station list and journey list views. Amount of data can be limited in the DataHandler class by changing "_limit" value to a smaller value. "_limit" value represents how many lines of data are imported from each csv-file. Source of the data can also be changed to local harddrive by changing path1, path2, path3 and path4 values in the DataHandler class. Paths 1, 2 and 3 must be the journey data and path4 must be station data. If user wants to import data from harddrive paths must be in a form such as

"C:\\Users\\[user]\\Desktop\\2021-05.csv".

Journey data used in this project are imported from
* [https://dev.hsl.fi/citybikes/od-trips-2021/2021-05.csv](https://dev.hsl.fi/citybikes/od-trips-2021/2021-05.csv)
* [https://dev.hsl.fi/citybikes/od-trips-2021/2021-06.csv](https://dev.hsl.fi/citybikes/od-trips-2021/2021-06.csv)
* [https://dev.hsl.fi/citybikes/od-trips-2021/2021-07.csv](https://dev.hsl.fi/citybikes/od-trips-2021/2021-07.csv)

Station data used in this project are imported from
* [https://opendata.arcgis.com/datasets/726277c507ef4914b0aec3cbcfcbfafc_0.csv](https://opendata.arcgis.com/datasets/726277c507ef4914b0aec3cbcfcbfafc_0.csv)

**Classes**
* DataHandler class handles data import and sorting of journeys and stations.
* Imported data are stored in the Journey and Station classes.

**View contents and functionality**

*Index page*

Shows a welcoming message and tells user what is happening in the background. When importing is done, data appears in the station list and journey list views.

*Station list page*

Each station imported are shown in a table. Tables sorting buttons are on top of the table (header). Data can be sorted by id, name, address, city, operator and capacity. Pressing sorting once sorts the data in descending order and pressing again sorts in ascending order. In addition of the data each row also has 4 buttons for easy access. These buttons are info, edit, delete and map which shows location of the station on Google Maps if location is in the data. Info button brings user to the station info page, edit to the edit station page and delete button deletes station without confirmation. Station list page also has buttons for creating a new station, dropdown list for changing how many elements are shown on the page, button to update page after changing the elements per page and finally paging at the bottom of the page which change data shown on the table. Paging shows current page showing by filling it blue. Paging shows the current page +- 5 pages when possible and in edge cases so that theres always 10 pages + current page. The first and last pages are also accessible by using buttons labeled as such.

*Journey list page*

Journey list page works pretty much as the station list page but departure and return station info pages can be accessed easily by pressing links in the columns.

*Station info page*

This page shows some of the data imported but also some calculated data such as Total number of journeys starting from this station, Average journey distance to this station, The 5 most popular return stations from this station and The 5 most popular departure stations to this station. The last two are shown in descending order and the popularity is shown in parentheses. Info page also lets user to quickly access edit station page and returning to previous page.

*New station/edit station page*

New station and edit station pages give user the possibility to change data on the selected station. These two pages work pretty much the same. The difference is that edit station already has information on the page unlike new station page. After submit button is pressed data given by the user is validated and error messages pop on top of the page if the are any. Validation is done by trying to parse user given info into right format and by using common logic such as return date can't be before departure date. Before validation info given by the user is sanitized to remove unwanted code injection. 

*New journey/edit journey page*

Edit journey and new journey pages use date time picker functionality to let user input date and time. This is done by pressing icon on the right side of the bar. Departure and return stations can be selected from the dropdown lists or new stations can be created. As user presses the submit button selections are valuated and error messages are shown if there are any. Something to note is that creating new station at these pages don't get deleted even if user decides to cancel the journey editing or creating new journey.

**Unit testing**

Solution also includes simple unit tests for station and journey data imports. This is done in another project which is included called CityBikeApplicationTests. Two text files with test data are included and used to test data import. Test files include one line that has correct formatting and else are wrong in some way. Importing test data should result in returning only one element per file in a list.

**Future improvements**
* date and time could be limited to some event such as when city bikes came to Finland
* more thorough import validation
* non local database
* more ways to interpret data such as plots










