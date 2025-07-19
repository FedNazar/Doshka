ARG DOTNET_VERSION=9.0
ARG BUILD_CONFIG=Release

FROM mcr.microsoft.com/dotnet/sdk:$DOTNET_VERSION AS sdk

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /src

COPY ["Doshka.API/", "Doshka.API/"]
COPY ["Doshka.Infrastructure/", "Doshka.Infrastructure/"]
COPY ["Doshka.Domain/", "Doshka.Domain/"]
COPY ["Doshka.sln", "Doshka.sln"]

WORKDIR /src/Doshka.API

RUN dotnet publish -c "$BUILD_CONFIG" -o "app"

RUN dotnet-ef migrations bundle -o "app/efbundle"

FROM mcr.microsoft.com/dotnet/aspnet:$DOTNET_VERSION AS base

COPY --from=sdk src/Doshka.API/app .
COPY --from=sdk src/Doshka.API/entrypoint.sh entrypoint.sh

RUN chmod +x entrypoint.sh
RUN chmod +x efbundle

ENTRYPOINT ["./entrypoint.sh"]
