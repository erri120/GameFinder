import { defineConfig } from 'vite';
import dts from 'vite-plugin-dts';
import { resolve } from 'path';
import { builtinModules } from 'module';

export default defineConfig({
  build: {
    lib: {
      entry: {
        index: resolve(__dirname, 'src/index.ts'),
        cli: resolve(__dirname, 'src/cli.ts'),
      },
      formats: ['es'],
      fileName: (format, entryName) => `${entryName}.js`,
    },
    rollupOptions: {
      external: [
        // Node.js built-ins (with and without node: prefix)
        ...builtinModules,
        ...builtinModules.map((m) => `node:${m}`),
        // Dependencies (don't bundle them)
        'fast-xml-parser',
        'js-sha3',
        'neverthrow',
        'vdf-parser',
      ],
      output: {
        preserveModules: true,
        preserveModulesRoot: 'src',
      },
    },
    target: 'node18',
    outDir: 'dist',
    emptyOutDir: true,
    ssr: true,
  },
  plugins: [
    dts({
      include: ['src/**/*.ts'],
      exclude: ['src/**/*.test.ts'],
    }),
  ],
});
