.PHONY: test build web shell clean

test:
	docker-compose run --rm tests

build:
	docker-compose run --rm tests dotnet build Withings.NET.sln

web:
	docker-compose up web

shell:
	docker-compose run --rm tests bash

clean:
	docker-compose down
