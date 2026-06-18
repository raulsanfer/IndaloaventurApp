const CACHE_NAME = "indaloaventurapp-shell-v2";
const SHELL_ASSETS = [
    "/",
    "/manifest.webmanifest",
    "/favicon.png",
    "/apple-touch-icon.png",
    "/pwa-192x192.png",
    "/pwa-512x512.png",
    "/pwa-maskable-512x512.png",
    "/js/google-auth.js",
    "/js/pwa.js",
    "/app.css"
];

self.addEventListener("install", (event) => {
    event.waitUntil(
        caches.open(CACHE_NAME).then((cache) => cache.addAll(SHELL_ASSETS)).then(() => self.skipWaiting())
    );
});

self.addEventListener("activate", (event) => {
    event.waitUntil(
        caches.keys().then((keys) =>
            Promise.all(keys.filter((key) => key !== CACHE_NAME).map((key) => caches.delete(key)))
        ).then(() => self.clients.claim())
    );
});

self.addEventListener("fetch", (event) => {
    const request = event.request;

    if (request.method !== "GET") {
        return;
    }

    const url = new URL(request.url);
    if (url.origin !== self.location.origin) {
        return;
    }

    if (request.mode === "navigate") {
        event.respondWith(
            fetch(request).then((response) => {
                const responseClone = response.clone();
                caches.open(CACHE_NAME).then((cache) => cache.put("/", responseClone));
                return response;
            }).catch(() => caches.match("/"))
        );
        return;
    }

    if (!shouldCache(request)) {
        return;
    }

    event.respondWith(
        caches.match(request).then((cachedResponse) => {
            if (cachedResponse) {
                return cachedResponse;
            }

            return fetch(request).then((response) => {
                if (!response || response.status !== 200 || response.type === "opaque") {
                    return response;
                }

                const responseClone = response.clone();
                caches.open(CACHE_NAME).then((cache) => cache.put(request, responseClone));
                return response;
            });
        })
    );
});

function shouldCache(request) {
    const destination = request.destination;
    return destination === "style"
        || destination === "script"
        || destination === "image"
        || destination === "font"
        || destination === "manifest";
}
