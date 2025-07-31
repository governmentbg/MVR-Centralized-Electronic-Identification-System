# Guide
## Installation in nexus
### Add "publishConfig" with "registry" inside package.json (in the library not in the workspace)

"publishConfig": {
        "@eid:registry": "http://nexus.mvreid.local:8081/repository/npm/"
},

### Login to npm registry

npm login --registry http://nexus.mvreid.local:8081/repository/npm/

### Build

Run `ng build @eid/ngx-eid-logs-viewer` to build the library. The build artifacts will be stored in the `dist/` directory.

### Open the dist folder

cd dist/@eid/ngx-eid-logs-viewer

### Publish in private nexus registry

npm publish

### Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

### Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.


## Usage of the library in the external project
Create a .npmrc file: [external project path]/.npmrc

and add this row in it:

@eid:registry=http://nexus.mvreid.local:8081/repository/npm/

## Install the library in the external project

npm config set legacy-peer-deps true

### For remote production installation

npm i @eid/ngx-eid-logs-viewer

### For local build and debugging the project

1. In the ngx-eid-logs-viewer/src/lib, you need to run 'ng build --watch' command so on every change 
in the library codebase to be reloaded in the external project. After execution of the command if the 
project is build successfully tou should see something like this:

------------------------------------------------------------------------------
Built Angular Package
 - from: C:\eid-pjs\src\LogsViewer\eID.PJS.LogsViewer\projects\ngx-eid-logs-viewer
 - to:   C:\eid-pjs\src\LogsViewer\eID.PJS.LogsViewer\dist\ngx-eid-logs-viewer
------------------------------------------------------------------------------

2. Copy the - to: path and execute this command in the external project 

npm install C:\eid-pjs\src\LogsViewer\eID.PJS.LogsViewer\dist\ngx-eid-logs-viewer
 
## Configure library dependency in the external project

Update the file [external project path]/src/app/app.module.ts

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { EidPjsModule } from '@eid/ngx-eid-logs-viewer';
import { AppConfigService as LibConfigService } from '@eid/ngx-eid-logs-viewer';

export function initConfig(appConfig: AppConfigService) {
    return () => appConfig.loadConfig();
}

@NgModule({
    declarations: [
        AppComponent
    ],
    imports: [
        BrowserModule,
        EidPjsModule.forRoot(new EidPjsLibConfig('This is the key'))
    ],
    providers: [
        {
        provide: APP_INITIALIZER,
        multi: true,
        deps: [AppConfigService],
        useFactory: initConfig,
        },
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }

## Use the library

### Include as component

<lib-ngx-eid-logs-viewer></lib-ngx-eid-logs-viewer>

### Include directly in the AppRoutingModule

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
    {
        path: '',
        children: [
            {
                path: '',
                pathMatch: 'full',
                redirectTo: 'default',
            },
            {
                path: 'logs-viewer',
                loadChildren: () => import('@eid/ngx-eid-logs-viewer').then(m => m.EidPjsModule),
            },
        ],
    },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}

## For developers only, new functionality and updating the version

When new functionality is added to the codebase, the version of the library should be incremented with command:
 
   npm version X.X.X (for example if version is 0.0.1 -> 0.0.2) 
   the lowest integer of each section can be 0 and the highest 9

X.X.X should be the next valid version please refer to the official documentation for proper versioning: 
https://docs.npmjs.com/about-semantic-versioning
