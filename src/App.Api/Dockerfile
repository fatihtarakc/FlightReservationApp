FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_HTTP_PORTS=5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/App.Api/App.Api.csproj", "src/App.Api/"]
COPY ["src/App.BackgroundJobs/App.BackgroundJobs.csproj", "src/App.BackgroundJobs/"]
COPY ["src/App.Business/App.Business.csproj", "src/App.Business/"]
COPY ["src/App.Cache/App.Cache.csproj", "src/App.Cache/"]
COPY ["src/App.Core/App.Core.csproj", "src/App.Core/"]
COPY ["src/App.DataAccess/App.DataAccess.csproj", "src/App.DataAccess/"]
COPY ["src/App.DataAccess.Abstract/App.DataAccess.Abstract.csproj", "src/App.DataAccess.Abstract/"]
COPY ["src/App.DataAccess.Concrete/App.DataAccess.Concrete.csproj", "src/App.DataAccess.Concrete/"]
COPY ["src/App.Dtos/App.Dtos.csproj", "src/App.Dtos/"]
COPY ["src/App.Entity/App.Entity.csproj", "src/App.Entity/"]
COPY ["src/App.Entities.Configurations/App.Entities.Configurations.csproj", "src/App.Entities.Configurations/"]
COPY ["src/App.Queue/App.Queue.csproj", "src/App.Queue/"]

RUN dotnet restore "src/App.Api/App.Api.csproj"

COPY . .
WORKDIR "/src/src/App.Api"
RUN dotnet build "App.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "App.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "App.Api.dll"]
