# Employee Manager

Employee and department management app built for the Luftborn technical test.

## Tech stack
- **Backend:** ASP.NET Core Web API (.NET 8), EF Core, SQL Server
- **Frontend:** Angular (TypeScript + SCSS)
- **SSO:** Google sign-in (OpenID Connect), validated in the API with JwtBearer

## Architecture (onion)
`Api -> Infrastructure -> Application -> Domain`
- **Domain:** entities and repository interfaces
- **Application:** services, DTOs, exceptions
- **Infrastructure:** EF Core DbContext, configurations, repositories
- **API:** thin controllers, error-handling middleware

Errors are thrown as `NotFoundException` / `ConflictException` in services and mapped to 404 / 409 by one middleware.

## Features
- CRUD for employees and departments, paging, search, department filter
- Google SSO: every API endpoint requires a valid token

## Run it
**API**
```
dotnet ef database update -p EmployeeManager.Infrastructure -s EmployeeManager.API
dotnet run --project EmployeeManager.API
```
**Frontend**
```
cd EmployeeManager.Web
npm install
ng serve
```
Open http://localhost:4200. Set `apiUrl` in `src/environments/environment.ts` to the API URL.

## SSO
Implemented with Google over OpenID Connect. The Google client ID is in `appsettings.json` and `environment.ts` (it is not a secret). The app is in Google's Testing mode, so only registered test users can sign in; send me the email to add. Switching to ADFS only means changing the Authority and ClientId.

## Libraries and why
- EF Core: data access
- Microsoft.AspNetCore.Authentication.JwtBearer: validate the Google ID token
- Angular (no UI library): required frontend framework, plain SCSS for styling
