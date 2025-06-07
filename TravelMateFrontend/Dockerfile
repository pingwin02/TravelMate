FROM node:20 AS build

WORKDIR /app

COPY TravelMateFrontend/package*.json ./
RUN npm install

COPY TravelMateFrontend/. ./
RUN npm run build

FROM nginx:alpine

COPY --from=build /app/dist/TravelMateFrontend /usr/share/nginx/html
COPY TravelMateFrontend/default.conf.template /etc/nginx/templates/default.conf.template

CMD ["nginx", "-g", "daemon off;"]
