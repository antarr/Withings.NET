.PHONY: test build web shell clean

test:
	docker-compose run --rm tests

build:
	docker-compose run --rm tests bash -c "nuget restore Withings.NET.sln && msbuild Withings.NET.sln /p:Configuration=Debug"

web:
	docker-compose up web

shell:
	docker-compose run --rm tests bash

clean:
	docker-compose down
