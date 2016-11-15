FROM microsoft/dotnet:1.0.1-runtime
EXPOSE 80

WORKDIR /app
COPY ./bin .

CMD ["dotnet", "service-b.dll"]