FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:5.0 AS build
LABEL maintainer="stunislaf.mail@gmail.com"
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o ./release ./LogServer.csproj
# -----------------------
FROM mcr.microsoft.com/dotnet/aspnet:5.0
LABEL maintainer="stunislaf.mail@gmail.com"
WORKDIR /app
COPY --from=build /app/release .
EXPOSE 8080/tcp
ENTRYPOINT ["dotnet", "LogServer.dll"]
