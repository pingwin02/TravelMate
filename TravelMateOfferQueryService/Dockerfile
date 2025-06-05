ARG PROJECT_NAME=TravelMateOfferQueryService

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

RUN apt-get update && \
    apt-get install -y tzdata && \
    ln -snf /usr/share/zoneinfo/Europe/Warsaw /etc/localtime && \
    echo "Europe/Warsaw" > /etc/timezone && \
    apt-get clean

ENV TZ=Europe/Warsaw

USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG PROJECT_NAME
WORKDIR /src
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
COPY TravelMateMessages ../TravelMateMessages
COPY ${PROJECT_NAME} .
RUN dotnet restore "${PROJECT_NAME}.csproj"
RUN dotnet build "${PROJECT_NAME}.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG PROJECT_NAME
RUN dotnet publish "${PROJECT_NAME}.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ARG PROJECT_NAME
ENV PROJECT_NAME=${PROJECT_NAME}
ENTRYPOINT ["sh", "-c", "dotnet ${PROJECT_NAME}.dll"]
