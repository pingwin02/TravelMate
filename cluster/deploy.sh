#!/bin/bash

echo "Deploying the app..."
docker stack deploy -c docker-compose.yml RSWD_188597 --with-registry-auth

echo "Status of the stack..."
watch -n 1 -c -d "docker stack ps RSWD_188597 && docker stack services RSWD_188597"

# docker service logs <id>
# docker stack rm RSWD_188597