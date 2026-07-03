ARG BASE_SDK="node:26-alpine"
ARG BASE_NGINX="nginx:1-alpine"
ARG BASE_NGINX_DIGEST

FROM ${BASE_SDK} AS build
WORKDIR /app

ENV PATH /app/node_modules/.bin:$PATH

COPY ./package.json ./package-lock.json /app/

RUN npm install -g npm@latest

RUN echo "NODE Version: " && node --version
RUN echo "NPM  Version: " && npm --version

RUN npm ci --force

COPY . /app

RUN npm run build

FROM ${BASE_NGINX} AS runtime
ARG BASE_NGINX
ARG BASE_NGINX_DIGEST

LABEL org.opencontainers.image.base.name="${BASE_NGINX}"
LABEL org.opencontainers.image.base.digest="${BASE_NGINX_DIGEST}"

COPY ["./conf/default.conf","/etc/nginx/conf.d/default.conf.template"]
COPY ["docker_entrypoint.sh", "/docker_entrypoint.sh"]
RUN ["chmod", "+x", "/docker_entrypoint.sh"]

RUN apk add curl
HEALTHCHECK --interval=5m --timeout=3s --retries=5 CMD curl --fail http://localhost/api/health || exit 1

WORKDIR /usr/share/nginx/html

# Remove default nginx static assets
RUN rm -rf ./*

COPY --from=build /app/dist/f1-server-app/browser /usr/share/nginx/html

EXPOSE 80

ENV F1SERVER_URL=
ENV F1SERVER_HOST=
ENV F1SERVER_PORT 4811

ENTRYPOINT ["/docker_entrypoint.sh"]

CMD ["nginx", "-g", "daemon off;"]
