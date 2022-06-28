#!/bin/bash

args=(
  /k:LBHackney-IT_bonuscalc-api
  /o:lbhackney-it
  /d:sonar.host.url=https://sonarcloud.io
  /d:sonar.cs.opencover.reportsPaths=coverage.xml
  /d:sonar.login="${SONAR_TOKEN}"
)

if [ "$CIRCLE_PULL_REQUEST" != "" ]; then
  args+=(/d:sonar.pullrequest.key="$(basename $CIRCLE_PULL_REQUEST)")
  args+=(/d:sonar.pullrequest.branch="${CIRCLE_BRANCH}")
elif [ "$CIRCLE_BRANCH" != "" ]; then
  args+=(/d:sonar.branch.name="${CIRCLE_BRANCH}")
fi

dotnet sonarscanner begin "${args[@]}"

dotnet build --no-incremental

coverlet ./BonusCalcApi.Tests/bin/Debug/net6.0/BonusCalcApi.Tests.dll \
  --target "dotnet" \
  --targetargs "test --no-build" \
  -f=opencover \
  -o="coverage.xml"

status=$?

dotnet sonarscanner end \
  /d:sonar.login="${SONAR_TOKEN}"

exit $status
