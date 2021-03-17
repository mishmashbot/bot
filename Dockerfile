FROM mcr.microsoft.com/dotnet/sdk:6.0.100-preview.2-alpine3.13-amd64 AS build-env
WORKDIR /app
COPY ./.git ./
COPY ./src ./
RUN dotnet publish App -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:6.0.100-preview.2-alpine3.13-amd64
WORKDIR /app
COPY --from=build-env /app/Ollio/out .
VOLUME /app/config

ENTRYPOINT ["dotnet", "Ollio.dll"]