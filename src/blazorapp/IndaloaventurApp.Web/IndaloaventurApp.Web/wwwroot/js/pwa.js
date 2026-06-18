window.indaloPwa = (() => {
    async function registerServiceWorker() {
        if (!window.isSecureContext || !("serviceWorker" in navigator)) {
            return false;
        }

        try {
            await navigator.serviceWorker.register("/service-worker.js", {
                scope: "/"
            });

            return true;
        } catch (error) {
            console.warn("service_worker_registration_failed", error);
            return false;
        }
    }

    registerServiceWorker();

    return {
        registerServiceWorker
    };
})();
