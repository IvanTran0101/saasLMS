// Development service worker — no caching, everything passes through to network.
// In production publish, this file is replaced by service-worker.published.js
// via the <ServiceWorker> MSBuild item in the .csproj.
self.addEventListener('install', event => event.waitUntil(self.skipWaiting()));
self.addEventListener('activate', event => event.waitUntil(self.clients.claim()));
