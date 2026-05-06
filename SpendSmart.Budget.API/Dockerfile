FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SpendSmart.Budget.API/SpendSmart.Budget.API.csproj", "SpendSmart.Budget.API/"]
COPY ["SpendSmart.Shared/SpendSmart.Shared.csproj", "SpendSmart.Shared/"]
RUN dotnet restore "SpendSmart.Budget.API/SpendSmart.Budget.API.csproj"
COPY . .
WORKDIR "/src/SpendSmart.Budget.API"
RUN dotnet build "SpendSmart.Budget.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpendSmart.Budget.API.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpendSmart.Budget.API.dll"]
