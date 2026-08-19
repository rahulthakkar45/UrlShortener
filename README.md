# UrlShortener

## Project Overview 

A UrlShortener API is build with ASP.NET Core 8, Entity Framework Core, SQL Server, and also have JWT authentication.

This allows authenticated users to create and manage short URLs, redirect user to the original URLs from the short code, track click and it's counts and view the statistics of clicks.

The Flow: Initially if user has an account then he can log in or if not so then register itself and once they do they will be provided a token which needs to be provided in the Application Header which also known as the Authentication Header and then they can able to create the Short URLs and after that based on the given short code which is provided after creating short URLs you can able to redirect by proving the https://localhost:{port}/{shortcode} to the original URLs and you need to write those ugly and long URLs after that. Also it helps to keep the statics of the click that had been hit on the URLs and provide the data for reporting or analytics purpose.

---

## Technologies Used

.NET 8
ASP.NET Core Web API
Entity Framework Core
SQL Server
JWT Authentication
Swagger
xUnit

---

## Prerequisites
Before running the code you at least have this much in your machine or install it.
.NET SDK 8
SQL Server(Microsoft SQL Server Management Studio 18)
Visual Studio 2022/2026

## Local Setup

1. Clone the Repository
  git clone <GITHUB_REPO_URL>
Navigate to the the project directory 
  cd UrlShortener

2. Restore NuGet Packages
  dotnet restore

## Configuration

Configuration is located in:
 UrlShortener.API/appsetting.json

## Database Configuration

"ConnectionStrings": {
    "UrlShortenerDatabase": "Server=RAHUL;Database=UrlShortenerDB;Trusted_Connection=True;TrustServerCertificate=True"
}
Please change your connection string according to your local SQL Server Configuration.


## JWT Configuration 

As we have implemented the JWT config for JWT authentication and for that we have implemented the following configuration:

"Jwt": {
    "Key": "Your_SECERET_KEY",
    "Issuer": "UrlShortener",
    "Audience": "UrlShortenerAPI",
    "ExpiryMinutes": 120
}

## Database Setup

As we uses the Entity Framework Core migration.

Make sure SQL Server is running and the connection string is configured correctly.

If you have not installed the EF Core CLI then install it using:
  dotnet tool install --global dotnet-ef

and then apply the existing migration:
  dotnet ef database update

## Run the Application

For running the application uses:
  dotnet run --project UrlShortener.API
