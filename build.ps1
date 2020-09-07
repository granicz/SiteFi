echo "Copy legal files"

xcopy .\legal\site-docs\intellifactory.com\* .\src\Hosted\legal\ /s /e

echo "Copy blog posts files"

xcopy .\blogs\user\* .\src\Hosted\posts\ /s /e

echo "Running npm install" 

pushd src/Hosted
npm install
popd

echo "Running dotnet build"

dotnet build SiteFi.sln
