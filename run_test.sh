#!/bin/bash

# Run Test first[Threshold: 90% or more] - Use %2c instead of comma
# See https://github.com/coverlet-coverage/coverlet/issues/182
dotnet test -p:CoverletOutputFormat=cobertura -p:CollectCoverage=true -p:Threshold=90  #-p:ExcludeByFile="**/Migrations/*.cs%2c**/Model/**/*.cs"

# now in root folder, file name called coverage.cobertura.xml should exists.
reportgenerator -reports:"MoneyManagerTest/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
