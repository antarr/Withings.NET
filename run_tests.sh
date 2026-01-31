#!/bin/bash
set -e

echo "Restoring NuGet packages..."
nuget restore Withings.NET.sln

echo "Building Withings.Specifications..."
msbuild Withings.Specifications/Withings.Net.Tests.csproj /p:Configuration=Debug

echo "Running tests..."
mono ./packages/NUnit.ConsoleRunner.3.6.1/tools/nunit3-console.exe Withings.Specifications/bin/Debug/Withings.Specifications.dll --noresult
