# Weather-Code-Challenge
---
This project will use ASP.NET 2.0 to provide current weather data and 3 hour weather data for a desired location


## OpenWeatherMap

This challenge uses the OpenWeatherMap API to obtain weather information for requested locations.
An API key can be obtained at [https://openweathermap.org/api](https://openweathermap.org/api).

The free tier is sufficient.

### OpenWeatherMap APIs Used

Requests to the OpenWeatherMap are handled within the OpenWeatherMapAPI project. 
These are not accessed directly by the client.

To obtain the data for the selected location, the following public API requests are made:
+ [Current](https://openweathermap.org/current)
...This request will get the current weather for the desired location

+ [5 Day/3 Hour](https://openweathermap.org/forecast5)
...This request will retrieve the 3 hour data for the desired location