# Use SDK image to build the app
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["../MyRestApi/rest_api.csproj", "MyRestApi/"]
RUN dotnet restore "MyRestApi/rest_api.csproj"
COPY . .
WORKDIR "/src/MyRestApi"
RUN dotnet build "rest_api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "rest_api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "rest_api.dll"]
