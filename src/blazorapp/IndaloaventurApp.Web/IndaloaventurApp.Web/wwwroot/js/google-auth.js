window.indaloGoogleAuth = (() => {
    let googleScriptPromise;

    function loadGoogleScript() {
        if (window.google?.accounts?.id) {
            return Promise.resolve();
        }

        if (googleScriptPromise) {
            return googleScriptPromise;
        }

        googleScriptPromise = new Promise((resolve, reject) => {
            const existingScript = document.querySelector('script[data-google-identity="true"]');
            if (existingScript) {
                existingScript.addEventListener("load", () => resolve(), { once: true });
                existingScript.addEventListener("error", () => reject(new Error("google_not_loaded")), { once: true });
                return;
            }

            const script = document.createElement("script");
            script.src = "https://accounts.google.com/gsi/client";
            script.async = true;
            script.defer = true;
            script.dataset.googleIdentity = "true";
            script.onload = () => resolve();
            script.onerror = () => reject(new Error("google_not_loaded"));
            document.head.appendChild(script);
        });

        return googleScriptPromise;
    }

    async function initializeButton(elementId, clientId, dotNetReference) {
        if (!clientId) {
            return false;
        }

        try {
            await loadGoogleScript();

            if (!window.google?.accounts?.id) {
                await dotNetReference.invokeMethodAsync("HandleGoogleSignInErrorAsync", "google_not_loaded");
                return false;
            }

            const host = document.getElementById(elementId);
            if (!host) {
                return false;
            }

            host.innerHTML = "";

            google.accounts.id.initialize({
                client_id: clientId,
                callback: async (response) => {
                    if (response?.credential) {
                        await dotNetReference.invokeMethodAsync("HandleGoogleCredentialAsync", response.credential);
                        return;
                    }

                    await dotNetReference.invokeMethodAsync("HandleGoogleSignInErrorAsync", "google_missing_credential");
                }
            });

            google.accounts.id.renderButton(host, {
                theme: "outline",
                size: "large",
                shape: "pill",
                text: "signin_with",
                width: Math.max(host.clientWidth, 260)
            });

            return true;
        } catch (error) {
            const errorCode = error instanceof Error ? error.message : "google_not_loaded";
            await dotNetReference.invokeMethodAsync("HandleGoogleSignInErrorAsync", errorCode);
            return false;
        }
    }

    function reset() {
        if (!window.google?.accounts?.id) {
            return;
        }

        google.accounts.id.cancel();
        google.accounts.id.disableAutoSelect();
    }

    return {
        initializeButton,
        reset
    };
})();
