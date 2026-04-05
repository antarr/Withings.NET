FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

COPY . .

RUN dotnet restore Withings.NET.sln
RUN dotnet build Withings.NET.sln --no-restore

CMD ["dotnet", "test", "Withings.NET.sln", "--no-build", "--verbosity", "normal", "--filter", "TestCategory!=E2E"]
