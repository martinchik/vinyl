FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 20081/tcp
COPY . .
ENV ASPNETCORE_URLS http://*:20081
ENTRYPOINT ["dotnet", "Vinyl.ParsingJob.dll", "--server.urls", "http://*:20081"]
