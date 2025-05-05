#!/bin/bash

echo "Deploying the app..."
docker stack deploy -c docker-compose.yml RSWW_188597 --with-registry-auth

echo "Status of the stack..."
docker stack ps RSWW_188597
docker stack services RSWW_188597

# docker stack rm RSWW_188597