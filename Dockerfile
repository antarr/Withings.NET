FROM mono:latest

# Install NuGet and XSP4 (for web example)
RUN apt-get update && \
    apt-get install -y nuget mono-xsp4 && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY . .

RUN chmod +x run_tests.sh

CMD ["./run_tests.sh"]
