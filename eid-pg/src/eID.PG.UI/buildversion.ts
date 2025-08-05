const { version } = require('./package.json');
const { resolve, relative } = require('path');
const { writeFileSync } = require('fs-extra');

const file = resolve('./src/assets/js/version.ts');
writeFileSync(
    file,
    `
// IMPORTANT: THIS FILE IS AUTO GENERATED! DO NOT MANUALLY EDIT!
export const VERSION = "${version}";
    `,
    { encoding: 'utf-8' }
);

console.log(`Wrote version info ${version} to ${relative(resolve(__dirname, '..'), file)}`);
