#!/bin/bash
set -e

echo "Restoring packages..."
dotnet restore Withings.NET.sln

echo "Building..."
dotnet build Withings.NET.sln --no-restore

echo "Running tests..."
dotnet test Withings.NET.sln --no-build --verbosity normal
