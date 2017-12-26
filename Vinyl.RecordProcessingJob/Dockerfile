FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 20082/tcp
COPY . .
ENV ASPNETCORE_URLS http://*:20082
ENTRYPOINT ["dotnet", "RecordProcessingJob/Vinyl.RecordProcessingJob.dll", "--server.urls", "http://*:20082"]
