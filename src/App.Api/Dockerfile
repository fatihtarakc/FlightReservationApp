FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/App.Core/App.Core.csproj src/App.Core/
COPY src/App.Entity/App.Entity.csproj src/App.Entity/
COPY src/App.Dtos/App.Dtos.csproj src/App.Dtos/
COPY src/App.Entities.Configurations/App.Entities.Configurations.csproj src/App.Entities.Configurations/
COPY src/App.DataAccess/App.DataAccess.csproj src/App.DataAccess/
COPY src/App.DataAccess.Abstract/App.DataAccess.Abstract.csproj src/App.DataAccess.Abstract/
COPY src/App.DataAccess.Concrete/App.DataAccess.Concrete.csproj src/App.DataAccess.Concrete/
COPY src/App.Cache/App.Cache.csproj src/App.Cache/
COPY src/App.Business/App.Business.csproj src/App.Business/
COPY src/App.Queue/App.Queue.csproj src/App.Queue/
COPY src/App.BackgroundJobs/App.BackgroundJobs.csproj src/App.BackgroundJobs/
COPY src/App.Api/App.Api.csproj src/App.Api/

RUN dotnet restore src/App.Api/App.Api.csproj

COPY . .

RUN dotnet publish src/App.Api/App.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "App.Api.dll"]
