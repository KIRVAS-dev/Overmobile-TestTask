const cacheName = "Overmobile-TestTask-0.1.0";
const contentToCache = [
    "Build/82c8b9dd86b97a9b8155a6d67bdcf9da.loader.js",
    "Build/404a236c302daeef2e64dd109cfb2f6b.framework.js.unityweb",
    "Build/712a4ce34116d95f1d77473bb58c0c42.data.unityweb",
    "Build/2796e4240432135f03ac7919fab11974.wasm.unityweb",
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
