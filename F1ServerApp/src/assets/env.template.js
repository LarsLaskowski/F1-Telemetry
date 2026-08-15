// Source template for `env.js`, rendered by `docker_entrypoint.sh` (via `envsubst`)
// into `env.js` at container startup. Not served directly.
(function (window) {
  window["env"] = window["env"] || {};

  // Environment variables
  window["env"]["apiUrl"] = "${F1SERVER_URL}/";
  window["env"]["debug"] = false;
})(this);
