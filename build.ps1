echo "Copy legal files"

cp legal/site-docs/intellifactory.com src/Hosted/legal

echo "Running npm install" 

pushd src/Hosted
npm install
popd

echo "Running dotnet build"

dotnet build SiteFi.sln
