.PHONY: setup
setup:
	docker-compose build

.PHONY: build
build:
	docker-compose build bonuscalc-api

.PHONY: build-test
build-test:
	docker-compose build bonuscalc-api-test

.PHONY: serve
serve: build
	docker-compose up -d bonuscalc-api

.PHONY: shell
shell: build
	docker-compose run bonuscalc-api bash

.PHONY: test
test: test-db build-test
	docker-compose up bonuscalc-api-test

.PHONY: lint
lint:
	dotnet tool install -v quiet -g dotnet-format || true
	dotnet format

.PHONY: scan
scan:
	dotnet tool install -v quiet -g dotnet-coverage || true
	dotnet tool install -v quiet -g dotnet-sonarscanner || true
	dotnet sonarscanner begin /k:"LBHackney-IT_bonuscalc-api" /o:"lbhackney-it" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="${SONAR_TOKEN}" /d:sonar.cs.vscoveragexml.reportsPaths=coverage.xml
	dotnet build --no-incremental
	dotnet-coverage collect 'dotnet test' -f xml  -o 'coverage.xml'
	dotnet sonarscanner end /d:sonar.login="${SONAR_TOKEN}"

.PHONY: stop
stop:
	docker-compose down

.PHONY: dev-db
dev-db:
	docker-compose up -d dev-database

.PHONY: test-db
test-db:
	docker-compose up -d test-database

.PHONY: restart-db
restart-db:
	docker stop $$(docker ps -q --filter ancestor=test-database -a)
	-docker rm $$(docker ps -q --filter ancestor=test-database -a)
	docker rmi test-database
	docker-compose up -d test-database
