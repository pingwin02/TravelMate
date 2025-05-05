#!/bin/bash

set -e 

REGISTRY=pingwin02

declare -A SERVICES=(
  ["travelmate-userapp"]="travelmatefrontend"
  ["TravelMateOfferService"]="travelmateofferservice"
  ["TravelMateAuthService"]="travelmateauthservice"
  ["TravelMateBookingService"]="travelmatebookingservice"
  ["TravelMatePaymentService"]="travelmatepaymentservice"
)

echo "Building Docker images..."

for DIR in "${!SERVICES[@]}"; do
  IMAGE_NAME="${REGISTRY}/${SERVICES[$DIR]}"
  echo "Building image for ${DIR} as ${IMAGE_NAME}..."
  docker build -t "$IMAGE_NAME" "../$DIR"
  echo "Pushing ${IMAGE_NAME}..."
  docker push "$IMAGE_NAME"
done

echo "All images have been built and pushed."
