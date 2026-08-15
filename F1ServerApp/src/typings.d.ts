// Ambient type declarations for the runtime environment variables injected via
// `assets/env.js` (see `environment.ts`, `environment.prod.ts` and `docker_entrypoint.sh`).

interface Window
{
  env: {
    apiUrl: string;
    debug: boolean;
  };
}
