echo "Running npm install" 

pushd src/Hosted
npm install
popd

echo "Running dotnet build"

dotnet build SiteFi.sln