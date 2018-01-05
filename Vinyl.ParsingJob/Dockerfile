FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app

# copy csproj and restore as distinct layers
FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY ./Vinyl.ParsingJob/*.csproj ./Vinyl.ParsingJob/
COPY ./Vinyl.Common/*.csproj ./Vinyl.Common/
COPY ./Vinyl.DbLayer/*.csproj ./Vinyl.DbLayer/
COPY ./Vinyl.Kafka.Lib/*.csproj ./Vinyl.Kafka.Lib/
COPY ./Vinyl.Metadata/*.csproj ./Vinyl.Metadata/
RUN dotnet restore

# copy everything else and build
COPY . .
WORKDIR /src/Vinyl.ParsingJob
RUN dotnet build -c Release -o /app

# build runtime image
FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
EXPOSE 20081
ENTRYPOINT ["dotnet", "Vinyl.ParsingJob.dll"]
