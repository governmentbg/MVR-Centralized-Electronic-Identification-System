# LogsViewer\eID.PJS.LogsViewer workspace contains the ngx-eid-logs-viewer library

# Guide installation in nexus

## Configure package.json of the ngx-eid-logs-viewer library and add "publishConfig" with "registry" inside

"registry": "http://nexus.mvreid.local:8081/repository/npm/"

## Login to npm registry

npm login --registry http://nexus.mvreid.local:8081/repository/npm/

## Build

Run `ng build ngx-eid-logs-viewer` to build the library. The build artifacts will be stored in the `dist/` directory.

## Open the dist folder

cd dist/ngx-eid-logs-viewer

## Publish in private nexus registry

npm publish

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.

# Usage of the library

Create a .npmrc file: [external project path]/.npmrc 

and put inside

registry=http://nexus.mvreid.local:8081/repository/npm/

## Install the library in the external project

npm config set legacy-peer-deps true

npm i ngx-eid-logs-viewer

## Configure library dependency in the external project

Update the file [external project path]/src/app/app.module.ts

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { EidPjsModule } from 'ngx-eid-logs-viewer';
import { AppConfigService as LibConfigService } from 'ngx-eid-logs-viewer';

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

# Include as component

<lib-ngx-eid-logs-viewer></lib-ngx-eid-logs-viewer>

# Include directly in the AppRoutingModule

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
{
path: '',
children: [
{
path: '',
pathMatch: 'full',
redirectTo: 'channels',
},
{
path: 'journals',
loadChildren: () => import('ngx-eid-logs-viewer').then(m => m.EidPjsModule),
},
],
},

];

@NgModule({
imports: [RouterModule.forRoot(routes)],
exports: [RouterModule],
})
export class AppRoutingModule {}
