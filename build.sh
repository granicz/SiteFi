#!/bin/bash

echo "Copy legal/site-docs/if.com/ under src/Hosted/legal/"

cp -r legal/site-docs/intellifactory.com/ src/Hosted/legal/

echo "Installing dotnet-serve"

dotnet tool install dotnet-serve --tool-path .tools

echo "Running npm install"

pushd src/Hosted
npm install
popd

echo "Running dotnet build"

dotnet build SiteFi.sln

