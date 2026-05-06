FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SpendSmart.Report.API/SpendSmart.Report.API.csproj", "SpendSmart.Report.API/"]
COPY ["SpendSmart.Shared/SpendSmart.Shared.csproj", "SpendSmart.Shared/"]
RUN dotnet restore "SpendSmart.Report.API/SpendSmart.Report.API.csproj"
COPY . .
WORKDIR "/src/SpendSmart.Report.API"
RUN dotnet build "SpendSmart.Report.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpendSmart.Report.API.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpendSmart.Report.API.dll"]
