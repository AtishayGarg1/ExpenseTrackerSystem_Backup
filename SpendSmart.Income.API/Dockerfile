FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SpendSmart.Income.API/SpendSmart.Income.API.csproj", "SpendSmart.Income.API/"]
COPY ["SpendSmart.Shared/SpendSmart.Shared.csproj", "SpendSmart.Shared/"]
RUN dotnet restore "SpendSmart.Income.API/SpendSmart.Income.API.csproj"
COPY . .
WORKDIR "/src/SpendSmart.Income.API"
RUN dotnet build "SpendSmart.Income.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpendSmart.Income.API.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpendSmart.Income.API.dll"]
