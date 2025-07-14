# Doshka

A very simple universal game leaderboard web API. It allows to host 
multiple leaderboards with ranked entries.

It is based on ASP.NET Core Web API. It uses Microsoft SQL Server to
store leaderboard information and Redis for caching it.

**NOTE: This service was developed for educational purposes and is 
not recommended for production use "as is". It lacks comprehensive 
mechanisms to prevent cheating and has not been used in any 
production environments. It also uses cookie-based 
authentication, which may not be ideal for some games.**

## Features

- Support for multiple leaderboards on a single server
- Configurable score ranges to limit min. and max. 
  scores per leaderboard
- Ability to retrieve the top N scores from a specified 
  leaderboard (N is configurable per leaderboard)
- User account system
- Redis-based caching

## Running locally

- Install .NET 9.0 SDK
- Install and run Microsoft SQL Server (2022 is recommended; you can use Express
  edition)
- Install and run Redis
- Clone the repository
- Run `dotnet restore` command to install dependencies
- Add initial user info for seeding in Doshka.API/seed-users.json
  (this file already has example data)
- Rename Doshka.API/appsettings.Example.json to appsettings.json
- Provide connection strings for MS SQL Server (ConnectionStrings.SQLServer)
  and Redis (ConnectionStrings.Redis) inside of Doshka.API/appsettings.json
- Run `dotnet ef database update` to create the database (make sure
  that MS SQL Server is running)
- Go to Doshka.API folder (`cd Doshka.API`)
- Run `dotnet publish -c Release` to build it in Release mode
- Run the API using `dotnet Doshka.API.dll`

## Documentation

Go to Doshka.API folder and run `dotnet run`. This will run the server 
in development mode. Go to `[your server address]/swagger` in your 
browser to read the API documentation.

## License

This project is licensed under the MIT license. Read the LICENSE file
for more details.

(C) 2025 Nazar Fedorenko