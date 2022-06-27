.PHONY: setup
setup:
	docker-compose build

.PHONY: build
build:
	docker-compose build bonuscalc-api

.PHONY: build-test
build-test: build
	docker-compose build bonuscalc-api-test

.PHONY: serve
serve: build migrate-dev-db
	docker-compose up -d bonuscalc-api

.PHONY: shell
shell: build
	docker-compose run --rm bonuscalc-api bash

.PHONY: test
test: test-db build-test migrate-test-db
	docker-compose run --rm bonuscalc-api-test

.PHONY: lint
lint:
	dotnet format

.PHONY: migrate-dev-db
migrate-dev-db: dev-db
	docker-compose run --rm bonuscalc-api dotnet ef database update -p BonusCalcApi -c BonusCalcApi.V1.Infrastructure.BonusCalcContext

.PHONY: migrate-test-db
migrate-test-db: test-db
	docker-compose run --rm bonuscalc-api-test dotnet ef database update -p BonusCalcApi -c BonusCalcApi.V1.Infrastructure.BonusCalcContext

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
