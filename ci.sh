#!/bin/bash

dotnet sonarscanner begin \
    /k:"LBHackney-IT_bonuscalc-api" \
    /o:"lbhackney-it" \
    /d:sonar.host.url=https://sonarcloud.io \
    /d:sonar.cs.opencover.reportsPaths=coverage.xml \
    /d:sonar.login="${SONAR_TOKEN}"

dotnet build --no-incremental

coverlet ./BonusCalcApi.Tests/bin/Debug/netcoreapp3.1/BonusCalcApi.Tests.dll \
    --target "dotnet" \
    --targetargs "test --no-build" \
    -f=opencover \
    -o="coverage.xml"

dotnet sonarscanner end \
    /d:sonar.login="${SONAR_TOKEN}"
