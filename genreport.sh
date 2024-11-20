#!/bin/bash
# Get the current directory name
CURRENT_DIR_NAME=$(basename "$PWD")
# Check if the current directory is "PSInzinerija"
if [ "$CURRENT_DIR_NAME" != "PSInzinerija" ]; then
    echo "Error: This script must be run from the 'PSInzinerija' directory!"
    exit 1
fi
rm -r */TestResults/* coveragereport/*
dotnet test --settings coverlet.runsettings.xml
reportgenerator -reports:*/TestResults/*/coverage.cobertura.xml -targetdir:coveragereport