FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["SpendSmart.Gateway/SpendSmart.Gateway.csproj", "SpendSmart.Gateway/"]
RUN dotnet restore "SpendSmart.Gateway/SpendSmart.Gateway.csproj"
COPY . .
WORKDIR "/src/SpendSmart.Gateway"
RUN dotnet build "SpendSmart.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpendSmart.Gateway.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpendSmart.Gateway.dll"]
