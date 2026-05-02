// Production service worker for saasLMS PWA
// Strategy:
//   _framework/ + _content/ -> cache-first  (Blazor WASM assemblies & ABP assets)
//   CSS / JS / fonts / images              -> stale-while-revalidate
//   HTML navigation + API calls            -> network-only (never cached)
//
// Cache invalidation: on activate, fetches /_framework/blazor.web.js with no-store
// and compares a hash to the previously stored value. If it changed (new deploy),
// both framework and static caches are cleared so fresh assets are downloaded.

const FRAMEWORK_CACHE = 'saasLMS-framework';
const STATIC_CACHE    = 'saasLMS-static';
const VERSION_CACHE   = 'saasLMS-version';

// ─── Lifecycle ───────────────────────────────────────────────────────────────

self.addEventListener('install', event => {
    event.waitUntil(self.skipWaiting());
});

self.addEventListener('activate', event => {
    event.waitUntil(handleActivate());
});

async function handleActivate() {
    await self.clients.claim();
    await checkAndInvalidateOnUpdate();
    await cleanupStaleCaches();
}

// ─── Version / cache-busting ─────────────────────────────────────────────────

async function checkAndInvalidateOnUpdate() {
    try {
        const response = await fetch('/_framework/blazor.web.js', { cache: 'no-store' });
        if (!response.ok) return;

        const text = await response.text();
        const currentHash = simpleHash(text).toString();

        const versionCache = await caches.open(VERSION_CACHE);
        const storedResponse = await versionCache.match('boot-hash');
        const storedHash = storedResponse ? await storedResponse.text() : null;

        if (storedHash !== null && storedHash !== currentHash) {
            // New deployment detected — wipe caches so fresh assets are fetched
            await Promise.all([
                caches.delete(FRAMEWORK_CACHE),
                caches.delete(STATIC_CACHE),
            ]);
            console.log('[SW] New app version detected. Caches cleared.');
        }

        await versionCache.put('boot-hash', new Response(currentHash));
    } catch {
        // Offline or boot entrypoint unavailable — keep existing cache as-is
    }
}

async function cleanupStaleCaches() {
    const valid = new Set([FRAMEWORK_CACHE, STATIC_CACHE, VERSION_CACHE]);
    const keys  = await caches.keys();
    await Promise.all(keys.filter(k => !valid.has(k)).map(k => caches.delete(k)));
}

// ─── Fetch interception ──────────────────────────────────────────────────────

self.addEventListener('fetch', event => {
    const req = event.request;
    const url = new URL(req.url);

    // Only handle same-origin GET requests
    if (req.method !== 'GET' || url.origin !== self.location.origin) return;

    // Blazor framework assemblies and ABP content assets — cache first
    // (large files; version check above handles invalidation on redeploy)
    if (url.pathname.startsWith('/_framework/') || url.pathname.startsWith('/_content/')) {
        event.respondWith(cacheFirst(req, FRAMEWORK_CACHE));
        return;
    }

    // Static file assets — stale-while-revalidate
    // (serve cached copy instantly, update in background)
    if (/\.(css|js|woff2?|ttf|eot|png|svg|ico|gif|jpg|jpeg|webp)$/i.test(url.pathname)) {
        event.respondWith(staleWhileRevalidate(req, STATIC_CACHE));
        return;
    }

    // HTML navigation, OIDC callbacks, appsettings.json — network only
    // Ensures: latest HTML shell, auth flow not intercepted, config always fresh
});

// ─── Cache strategies ────────────────────────────────────────────────────────

async function cacheFirst(request, cacheName) {
    const cache  = await caches.open(cacheName);
    const cached = await cache.match(request);
    if (cached) return cached;

    const response = await fetch(request);
    if (response.ok) cache.put(request, response.clone());
    return response;
}

async function staleWhileRevalidate(request, cacheName) {
    const cache  = await caches.open(cacheName);
    const cached = await cache.match(request);

    const networkFetch = fetch(request)
        .then(response => {
            if (response.ok) cache.put(request, response.clone());
            return response;
        })
        .catch(() => cached);

    return cached ?? networkFetch;
}

// ─── Utility ─────────────────────────────────────────────────────────────────

function simpleHash(str) {
    let h = 0;
    for (let i = 0; i < str.length; i++) {
        h = (Math.imul(31, h) + str.charCodeAt(i)) | 0;
    }
    return Math.abs(h);
}
