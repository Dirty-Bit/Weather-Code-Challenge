# Weather-Code-Challenge
---
This project will use ASP.NET 2.0 to provide current weather data and 3 hour weather data for a desired location

## ASP.NET API

+ Get All Locations (`GET` `/api/locations`)

.Returns all known locations

+ Get Single Location (`GET` `/api/locations/{id}`)

.Returns a single location, specified by ID number

+ Delete Location (`DELETE` `/api/locations/{id}`)

.Delete the specified location

+ Create Location (`POST` `/api/locations` `mime-type: x-www-form-urlencoded`)

.Add a location to the database

## Database

The location data is stored within a Local SQL Database (.mdf). 

Table definition:

### Locations
| Column Name   | Data Type     | Allow Null  | PK   |  Identity  |
| ------------- |:-------------:| ----------- | ---- | ---------- |
| Id            | int           |  no         | yes  |  no        |
| Name          | nvachar(50)   |  no         | no   |  no        |
| Zipcode       | int           |  yes        | no   |  no        |
| State         | nvachar(2)    |  no         | no   |  no        |
| Country       | nvachar(2)    |  yes        | no   |  no        |

## OpenWeatherMap

This challenge uses the OpenWeatherMap API to obtain weather information for requested locations.
An API key can be obtained at [https://openweathermap.org/api](https://openweathermap.org/api).

The free tier is sufficient.

### OpenWeatherMap APIs Used

Requests to the OpenWeatherMap are handled within the OpenWeatherMapAPI project. 
These are not accessed directly by the client.

To obtain the data for the selected location, the following public API requests are made:

+ [Current](https://openweathermap.org/current)

.This request will get the current weather for the desired location

+ [5 Day/3 Hour](https://openweathermap.org/forecast5)

.This request will retrieve the 3 hour data for the desired location

### Adding Your API Key

First, ensure that any changes that you track your API key. This can be accomplished by running the following: `git update-index --skip-worktree "Weather WebAPI\Weather WebAPI\Web.config"`

Add your API key to the Web.config file within the Weather WebAPI ASP.NET project.

`<appSettings>
<add key="key" value="YOUR_API_KEY_HERE" />
</appSettings>`

The same is true for the App.config within the OpenWeatherMapAPI.Tests project.

Remove tracking chainges: `git update-index --skip-worktree "Weather WebAPI\OpenWeatherMapAPI.Tests\Web.config"`

Add your API key:

`<appSettings>
<add key="key" value="YOUR_API_KEY_HERE" />
</appSettings>`
