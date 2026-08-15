// Local dev default, loaded by `ng serve`/`ng build` and by `index.html` directly.
// In containers, `docker_entrypoint.sh` overwrites this file at startup by rendering
// `env.template.js` with the actual `F1SERVER_URL`, so this checked-in default is
// never used at runtime there.
(function (window) {
  window["env"] = window["env"] || {};

  // Environment variables
  window["env"]["apiUrl"] = "http://localhost/";
  window["env"]["debug"] = false;
})(this);
