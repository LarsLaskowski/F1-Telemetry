# F1ServerApp

This project was generated with [Angular CLI](https://github.com/angular/angular-cli). The version
currently pinned in `package.json` is 22.0.8.

## Development server

Run `npm start` for a dev server. Navigate to `http://localhost:4810/`. The application will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.

## Updating packages

```bash
npm i -g npm-check-updates
ncu -u
npm install
```

## Line endings of docker_entrypoint.sh

`docker_entrypoint.sh` has to keep Unix line endings (`\n`), otherwise the container fails to
start. On Windows the file is easily saved with CRLF, so convert it back before committing:

```bash
sed $'s/\r$//' ./docker_entrypoint.sh > ./docker_entrypoint.unix.sh
```

Run this from a shell that leaves the bytes untouched, for example WSL (Windows key, then `wsl`),
and replace the original with the converted file afterwards.
