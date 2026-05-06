FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SpendSmart.Notification.API/SpendSmart.Notification.API.csproj", "SpendSmart.Notification.API/"]
COPY ["SpendSmart.Shared/SpendSmart.Shared.csproj", "SpendSmart.Shared/"]
RUN dotnet restore "SpendSmart.Notification.API/SpendSmart.Notification.API.csproj"
COPY . .
WORKDIR "/src/SpendSmart.Notification.API"
RUN dotnet build "SpendSmart.Notification.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpendSmart.Notification.API.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpendSmart.Notification.API.dll"]
