#!/bin/sh

if ! command -v dotnet >/dev/null 2>&1; then
	echo Plesae install the dotnet CLI
	exit
fi

cd c2c-backend && dotnet publish -c release && cd ../c2c-frontend && dotnet publish -c release && cd ..

if [ $? -gt 0 ]; then
	echo "Error(s) occured building"
	exit
else
	echo 'Done!'
fi
