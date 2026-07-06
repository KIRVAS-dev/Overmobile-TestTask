const cacheName = "Overmobile-TestTask-0.1.0";
const contentToCache = [
    "Build/7bb6c500633545531263a45658e36699.loader.js",
    "Build/310f7208a9f8648db0d5693bac719860.framework.js.unityweb",
    "Build/a25efdd0fac4a3623cbf8300b2d94018.data.unityweb",
    "Build/789f91e70322703af72f604f4cdcfadd.wasm.unityweb",
    "TemplateData/style.css"

];

self.addEventListener('install', function (e) {
    console.log('[Service Worker] Install');
    
    e.waitUntil((async function () {
      const cache = await caches.open(cacheName);
      console.log('[Service Worker] Caching all: app shell and content');
      await cache.addAll(contentToCache);
    })());
});

self.addEventListener('fetch', function (e) {
    e.respondWith((async function () {
      let response = await caches.match(e.request);
      console.log(`[Service Worker] Fetching resource: ${e.request.url}`);
      if (response) { return response; }

      response = await fetch(e.request);
      const cache = await caches.open(cacheName);
      console.log(`[Service Worker] Caching new resource: ${e.request.url}`);
      cache.put(e.request, response.clone());
      return response;
    })());
});
