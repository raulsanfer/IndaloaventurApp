window.indaloSessionStorage = (() => {
    function resolveStorage(storageKind) {
        return storageKind === "local" ? window.localStorage : window.sessionStorage;
    }

    function read(storageKind, key) {
        try {
            return resolveStorage(storageKind).getItem(key);
        } catch (error) {
            console.warn("session_storage_read_failed", storageKind, error);
            return null;
        }
    }

    function write(storageKind, key, value) {
        try {
            resolveStorage(storageKind).setItem(key, value);
        } catch (error) {
            console.warn("session_storage_write_failed", storageKind, error);
        }
    }

    function remove(storageKind, key) {
        try {
            resolveStorage(storageKind).removeItem(key);
        } catch (error) {
            console.warn("session_storage_remove_failed", storageKind, error);
        }
    }

    return {
        read,
        write,
        remove
    };
})();
