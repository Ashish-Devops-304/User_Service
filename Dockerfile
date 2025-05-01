FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

EXPOSE 443
EXPOSE 80

# copy everything into the docker directory and restore it in docker directory
COPY . .
RUN dotnet restore 

# Build the application
RUN dotnet publish -c Release -o out

# Build runtime final image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "UserService.dll"]
