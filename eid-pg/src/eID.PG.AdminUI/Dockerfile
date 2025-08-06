FROM docker.io/library/nginx:stable-alpine AS base

FROM docker.io/library/node:20-bookworm AS build
WORKDIR /app
ARG HTTP_PROXY
ARG HTTPS_PROXY
ARG NO_PROXY
COPY ./package.json .
RUN npm install

COPY . .
RUN npm run build --  --output-path=dist --output-hashing=all

FROM base AS final
ARG APP_DIR=/usr/share/nginx/html
ARG SERVICE_PORT=80

# Remove default nginx website
RUN rm -rf /usr/share/nginx/html/*

# Copy nginx custom config
COPY ./nginx/conf/default.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist ${APP_DIR}

EXPOSE ${SERVICE_PORT}
CMD ["nginx", "-g", "daemon off;"]
