FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 20081/tcp
COPY ${source:-obj/Docker/publish} /ParsingJob
ENV ASPNETCORE_URLS http://*:20081
ENTRYPOINT ["dotnet", "ParsingJob/Vinyl.ParsingJob.dll", "--server.urls", "http://*:20081"]
