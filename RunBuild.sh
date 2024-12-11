./RunTest.sh

if [ "$?" -eq 0 ]; then
    cd DarkAgesRPG
    dotnet run build --project DarkAgesRPG.csproj
    cd ..
fi 
