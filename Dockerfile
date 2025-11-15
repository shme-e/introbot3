FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /build

COPY . ./

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0

RUN apt-get update && apt-get install libopus0 libopus-dev libsodium23 libsodium-dev ffmpeg -y
WORKDIR /app

COPY --from=build-env /build/out /app

ENTRYPOINT ["dotnet", "IntroBot3.dll"]