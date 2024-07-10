module.exports = {
    '**/*.{ts,html,css}': 'npx prettier --write',
    'src/**/*.{ts,html}': files => {
        return `ng lint ${files.map(file => `--lint-file-patterns ${file}`).join(' ')}`;
    },
};
