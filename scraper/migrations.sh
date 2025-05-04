#!/bin/bash

for dir in ../TravelMate*/; do
    if [ -d "$dir" ]; then
        echo "Processing directory: $dir"
        cd "$dir" || exit

        rm -rf bin Migrations obj

        echo "Adding migration InitialCreate..."
        dotnet-ef migrations add InitialCreate

        echo "Updating database..."
        dotnet-ef database update

        cd - || exit
    fi
done