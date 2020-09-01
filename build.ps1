echo "Copy legal files"

Copy-Item -Path legal/site-docs/intellifactory.com/* -Destination src/Hosted/legal -Recurse

echo "Running npm install" 

pushd src/Hosted
npm install
popd

echo "Running dotnet build"

dotnet build SiteFi.sln
