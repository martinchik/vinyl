FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 20081
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Vinyl.GetDataJob.dll"]
