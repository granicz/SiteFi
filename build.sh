#!/bin/bash

echo "Installing dotnet-serve"

dotnet tool install dotnet-serve --tool-path .tools

echo "Running npm install"

pushd src/Hosted
npm install
popd

echo "Running dotnet build"

dotnet build SiteFi.sln

