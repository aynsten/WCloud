FROM node:lts-alpine as build-stage
COPY . /app
WORKDIR /app

#ENV API_GATEWAY_BASE_URL=https://api-xx.domain.com

#RUN npm config set registry https://registry.npm.taobao.org
RUN npm install --registry=https://registry.npm.taobao.org
RUN npm run build

#production stage
FROM nginx:1.15 as production-stage

WORKDIR /usr/share/nginx/html
COPY --from=build-stage /app/dist .

ENTRYPOINT ["nginx", "-g", "daemon off;"]