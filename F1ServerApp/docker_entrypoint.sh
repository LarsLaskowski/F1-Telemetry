#!/usr/bin/env sh
set -eu

envsubst '${F1SERVER_HOST} ${F1SERVER_PORT}' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf
envsubst '${F1SERVER_URL}' < /usr/share/nginx/html/assets/env.template.js > /usr/share/nginx/html/assets/env.js

exec "$@"
