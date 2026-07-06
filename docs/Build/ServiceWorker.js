const cacheName = "Overmobile-TestTask-0.1.0";
const contentToCache = [
    "Build/3a9012e1ea458689124c0fe117fd47ca.loader.js",
    "Build/ccd45f6119611cdbcf48ca9dcb22135b.framework.js.gz",
    "Build/8a85a926f751efe2414e5884338ee3f1.data.gz",
    "Build/3e5b81ea486c9f4f1122399f7ec349eb.wasm.gz",
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
