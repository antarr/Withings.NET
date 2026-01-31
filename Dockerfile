FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY . .

RUN chmod +x run_tests.sh

CMD ["./run_tests.sh"]
