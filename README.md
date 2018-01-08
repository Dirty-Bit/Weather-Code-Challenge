# Weather-Code-Challenge
---
This project will use ASP.NET 2.0 to provide current weather data and 3 hour weather data for a desired location.

## ASP.NET Web API

+ Get All Locations (`GET` `/api/locations`)

  Returns all known locations

+ Get Single Location (`GET` `/api/locations/{id}`)

  Returns a single location, specified by ID number

+ Delete Location (`DELETE` `/api/locations/{id}`)

  Delete the specified location

+ Create Location (`POST` `/api/locations` `mime-type: x-www-form-urlencoded`)

  Add a location to the database

## Response Objects

### Location

A single location within the database.

`{ 
	"Id": 5308655,
	"Name": "Phoenix",
	"Zipcode": 85044,
	"State": "AZ",
	"Country": "US"
}`

### Weather

The current weather and a 3 hour forecast of the weather.

`{
   "current":{
      "coord":{
         "lon":-112.07,
         "lat":33.45
      },
      "weather":[
         {
            "id":803,
            "main":"Clouds",
            "description":"broken clouds",
            "icon":"04d"
         }
      ],
      "base":"stations",
      "main":{
         "temp":285.48,
         "pressure":1018.0,
         "humidity":41.0,
         "temp_min":283.15,
         "temp_max":287.15,
         "sea_level":0.0,
         "grnd_level":0.0,
         "temp_kf":0.0
      },
      "visibility":16093,
      "wind":{
         "speed":2.1,
         "deg":170.0
      },
      "clouds":{
         "all":75
      },
      "dt":1515424500,
      "sys":{
         "type":1,
         "id":291,
         "message":0.0044,
         "country":"US",
         "sunrise":1515421983,
         "sunset":1515458272,
         "pod":null
      },
      "id":5308655,
      "name":"Phoenix",
      "cod":200
   },
   "forecast":{
      "cod":"200",
      "message":0.004,
      "cnt":40,
      "list":[
         {
            "dt":1515434400,
            "main":{
               "temp":291.26,
               "pressure":977.67,
               "humidity":41.0,
               "temp_min":288.272,
               "temp_max":291.26,
               "sea_level":1031.03,
               "grnd_level":977.67,
               "temp_kf":2.99
            },
            "weather":[
               {
                  "id":804,
                  "main":"Clouds",
                  "description":"overcast clouds",
                  "icon":"04d"
               }
            ],
            "clouds":{
               "all":92
            },
            "wind":{
               "speed":2.36,
               "deg":40.5004
            },
            "sys":{
               "type":0,
               "id":0,
               "message":0.0,
               "country":null,
               "sunrise":0,
               "sunset":0,
               "pod":"d"
            },
            "dt_txt":"2018-01-08 18:00:00"
         }
      ],
      "city":{
         "id":5308655,
         "name":"Phoenix",
         "coord":{
            "lon":-112.0741,
            "lat":33.4484
         },
         "country":"US"
      }
   }
}`

### Action Response 

An object that gives the client a response as to whether or not their action was successful. This is used in DELETE and POST

`{
    "success": true
}`

  
## Database

The location data is stored within a Local SQL Database (.mdf). The database is created automatically in the Temp directory. It is automatically populated with two entries.

Table definition:

### Locations
| Column Name   | Data Type     | Allow Null  | PK   |  Identity  |
| ------------- | ------------- | ----------- | ---- | ---------- |
| Id            | int           |  no         | yes  |  no        |
| Name          | nvachar(50)   |  no         | no   |  no        |
| Zipcode       | int           |  yes        | no   |  no        |
| State         | nvachar(2)    |  no         | no   |  no        |
| Country       | nvachar(2)    |  yes        | no   |  no        |

## Caching

Request caching is implemented using Redis 3.2.100 for Windows, available [here](https://github.com/MicrosoftArchive/redis/releases).

Nuget package [StackExchange.Redis 1.2.6 by Marc Gravell](https://www.nuget.org/packages/StackExchange.Redis/) is a dependency required for using Redis.

Redis will be used where possible to eliminate calls to the SQL Server.

### Redis Database Keys

Keys have a prefix and then will be followed by the Location ID, as defined by OpenWeatherMap API.

| Prefix	    | TTL (min)     | Model                       | 
| ------------- | ------------- | --------------------------- | 
| loc_          | 60            |  Location                   | 
| current_      | 5             |  Current_Weather            | 
| forecast_     | 5             |  Forecast                   | 


## OpenWeatherMap

This challenge uses the OpenWeatherMap API to obtain weather information for requested locations.
An API key can be obtained at [https://openweathermap.org/api](https://openweathermap.org/api).

The free tier is sufficient.

### OpenWeatherMap APIs Used

Requests to the OpenWeatherMap are handled within the OpenWeatherMapAPI project. 
These are not accessed directly by the client.

To obtain the data for the selected location, the following public API requests are made:

+ [Current](https://openweathermap.org/current)

  This request will get the current weather for the desired location

+ [5 Day/3 Hour](https://openweathermap.org/forecast5)

  This request will retrieve the 3 hour data for the desired location

### Adding Your API Key

First, ensure that any changes that you track your API key. This can be accomplished by running the following: `git update-index --skip-worktree "Weather WebAPI\Weather WebAPI\Web.config"`

Add your API key to the Web.config file within the Weather WebAPI ASP.NET project.

`<appSettings>
<add key="key" value="YOUR_API_KEY_HERE" />
</appSettings>`

The same is true for the App.config within the OpenWeatherMapAPI.Tests project.

Remove tracking changes: `git update-index --skip-worktree "Weather WebAPI\OpenWeatherMapAPI.Tests\Web.config"`

Add your API key:

`<appSettings>
<add key="key" value="YOUR_API_KEY_HERE" />
</appSettings>`
