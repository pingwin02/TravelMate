#!/bin/bash

set -e 

REGISTRY=pingwin02

declare -A SERVICES=(
  ["TravelMateFrontend"]="travelmatefrontend"
  ["TravelMateOfferCommandService"]="travelmateoffercommandservice"
  ["TravelMateOfferQueryService"]="travelmateofferqueryservice"
  ["TravelMateAuthService"]="travelmateauthservice"
  ["TravelMateBookingService"]="travelmatebookingservice"
  ["TravelMatePaymentService"]="travelmatepaymentservice"
  ["TravelMateSagaOrchestrator"]="travelmatesagaorchestrator"
)

echo "Building Docker images..."
cd ..

for DIR in "${!SERVICES[@]}"; do
  IMAGE_NAME="${REGISTRY}/${SERVICES[$DIR]}"
  echo "Building image for ${DIR} as ${IMAGE_NAME}..."
  docker build -t "$IMAGE_NAME" -f "$DIR/Dockerfile" "."
  echo "Pushing ${IMAGE_NAME}..."
  docker push "$IMAGE_NAME"
done

cd - > /dev/null
echo "All images have been built and pushed."
