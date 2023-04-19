<h1 align="center">MyBikeApp</h1>

This project is Solita Dev Academy pre-assignment.
https://github.com/solita/dev-academy-2023-exercise

Tested with Microsoft Visual Studio 2022 & Firefox

<h2> How to run? </h2>

Click the "Play"-button in Visual Studio and App should start in default browser.

<h2> What does it do? </h2>

Data:
- Import csv files from /csv folder. Parses everyline. Journey has to be greater than 10 seconds and greater than 10 meter to be validated.
- Journeys are importet to local SQL database. For easy and fast use for the user there is no need to start external SQL database to run the project.
- In console you can see how many journeys were included and excluded.

Database:
- upload statio and journey csv files(Might take few minutes)
- clears the database from stations and journeys

Journey list view: 
- Shows 200 journeys per page. Possibility to go to next page or the last one.
- each journey shows departure and return stations, covered distance in kilometers and duration in minutes.
- Order journeys by column. Orders by descending.
- Search Journeys by departure or return station.
- Create/delete journey

Station list:
- Pagination and Search field
- Click "Show Station Info" to see more information about selected station
- Create/delete/edit station

Single station view:
- Shows Station name, Station address.
- Total number of journeys starting from the station.
- Top 5 most popular return stations for journeys starting from the station.
- Top 5 most popular departure stations for journeys ending at the station.
- Total number of journeys ending at the station.
- Station location on google maps.
- The average distance of a journey starting from the station.
- The average distance of a journey ending at the station.


<h2> Build with </h2>
Visual Studio ASP.NET MVC, SQL

<h2> Future updates </h2>
Login & roles,  Implement E2E tests, Ability to filter all Single station view calculations per month
