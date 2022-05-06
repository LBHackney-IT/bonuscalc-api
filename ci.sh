#!/bin/bash
set -e
set -o pipefail

dotnet sonarscanner begin \
    /k:"LBHackney-IT_bonuscalc-api" \
    /o:"lbhackney-it" \
    /d:sonar.host.url=https://sonarcloud.io \
    /d:sonar.cs.vscoveragexml.reportsPaths=coverage.xml \
    /d:sonar.login="${SONAR_TOKEN}"

dotnet-coverage collect 'dotnet test' \
    -f xml  -o 'coverage.xml'

dotnet sonarscanner end \
    /d:sonar.login="${SONAR_TOKEN}"
