// Production service worker for saasLMS PWA
// Strategy:
//   _framework/ + _content/ -> cache-first  (Blazor WASM assemblies & ABP assets)
//   CSS / JS / fonts / images              -> stale-while-revalidate
//   HTML navigation + API calls            -> network-only (never cached)
//
// Cache invalidation: on activate, fetches /_framework/blazor.web.js with no-store
// and compares a hash to the previously stored value. If it changed (new deploy),
// both framework and static caches are cleared so fresh assets are downloaded.

const FRAMEWORK_CACHE = 'saasLMS-framework-v2';
const STATIC_CACHE    = 'saasLMS-static-v2';
const VERSION_CACHE   = 'saasLMS-version-v2';

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
        const response = await fetchBootSignature();
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

async function fetchBootSignature() {
    const candidates = [
        '/_framework/blazor.boot.json',
        '/_framework/resource-collection.js',
        '/_framework/blazor.web.js',
    ];

    for (const path of candidates) {
        const response = await fetch(path, { cache: 'no-store' });
        if (response.ok) return response;
    }

    return new Response(null, { status: 404 });
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

    // Always revalidate the boot manifest / resource manifest entrypoints.
    // If these stay stale while assemblies are content-hashed per deploy,
    // the browser can request deleted `_framework/*.wasm` files and hit 404s.
    if (
        url.pathname === '/_framework/blazor.boot.json' ||
        url.pathname === '/_framework/resource-collection.js' ||
        url.pathname === '/_framework/blazor.web.js'
    ) {
        event.respondWith(networkFirst(req, FRAMEWORK_CACHE));
        return;
    }

    // Blazor framework assemblies and ABP content assets — cache first
    // (large files; boot manifest revalidation above handles deploy churn)
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

async function networkFirst(request, cacheName) {
    const cache = await caches.open(cacheName);

    try {
        const response = await fetch(request, { cache: 'no-store' });
        if (response.ok) await cache.put(request, response.clone());
        return response;
    } catch {
        const cached = await cache.match(request);
        if (cached) return cached;
        throw new Error(`Network request failed for ${request.url}`);
    }
}

// ─── Utility ─────────────────────────────────────────────────────────────────

function simpleHash(str) {
    let h = 0;
    for (let i = 0; i < str.length; i++) {
        h = (Math.imul(31, h) + str.charCodeAt(i)) | 0;
    }
    return Math.abs(h);
}
