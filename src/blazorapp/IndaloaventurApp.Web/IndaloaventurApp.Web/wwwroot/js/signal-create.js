window.indaloSignals = window.indaloSignals || {};

window.indaloSignals.getCurrentPosition = function () {
    return new Promise(function (resolve, reject) {
        if (!navigator.geolocation) {
            reject(new Error("Geolocation unavailable."));
            return;
        }

        navigator.geolocation.getCurrentPosition(
            function (position) {
                resolve({
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude
                });
            },
            function () {
                reject(new Error("Geolocation denied."));
            },
            {
                enableHighAccuracy: true,
                timeout: 10000,
                maximumAge: 0
            });
    });
};

window.indaloSignals.triggerFileInput = function (id) {
    var element = document.getElementById(id);
    if (element) {
        element.click();
    }
};

window.indaloSignals.loadAndNormalizeImage = async function (id, maxBytes, maxDimension, minDimension, initialQuality, minimumQuality) {
    var input = document.getElementById(id);
    if (!input || !input.files || input.files.length === 0) {
        return null;
    }

    var file = input.files[0];

    try {
        if (!file.type || file.type.indexOf("image/") !== 0) {
            return { success: false, errorKey: "signal_create_photo_invalid_type" };
        }

        if (file.size <= maxBytes) {
            return {
                success: true,
                contentBase64: await fileToBase64(file),
                contentType: file.type || "image/jpeg",
                fileName: file.name,
                previewUrl: await fileToDataUrl(file)
            };
        }

        var sourceImage = await loadImage(file);
        var width = sourceImage.naturalWidth || sourceImage.width;
        var height = sourceImage.naturalHeight || sourceImage.height;
        var currentMaxDimension = Math.min(maxDimension, Math.max(width, height));
        var quality = Number(initialQuality);
        var canvas = document.createElement("canvas");
        var context = canvas.getContext("2d");

        if (!context) {
            return { success: false, errorKey: "signal_create_photo_processing_error" };
        }

        while (currentMaxDimension >= minDimension) {
            var scaled = scaleToFit(width, height, currentMaxDimension);
            canvas.width = scaled.width;
            canvas.height = scaled.height;
            context.clearRect(0, 0, scaled.width, scaled.height);
            context.drawImage(sourceImage, 0, 0, scaled.width, scaled.height);

            while (quality >= minimumQuality) {
                var blob = await canvasToBlob(canvas, "image/jpeg", quality);
                if (blob && blob.size <= maxBytes) {
                    return {
                        success: true,
                        contentBase64: await blobToBase64(blob),
                        contentType: "image/jpeg",
                        fileName: normalizeFileName(file.name),
                        previewUrl: await blobToDataUrl(blob)
                    };
                }

                quality = roundQuality(quality - 0.08);
            }

            currentMaxDimension = Math.floor(currentMaxDimension * 0.8);
            quality = Number(initialQuality);
        }

        return { success: false, errorKey: "signal_create_photo_cannot_optimize" };
    } finally {
        input.value = "";
    }
};

function roundQuality(value) {
    return Math.round(value * 100) / 100;
}

function scaleToFit(width, height, maxDimension) {
    if (width <= maxDimension && height <= maxDimension) {
        return { width: width, height: height };
    }

    if (width >= height) {
        return {
            width: maxDimension,
            height: Math.max(1, Math.round((height / width) * maxDimension))
        };
    }

    return {
        width: Math.max(1, Math.round((width / height) * maxDimension)),
        height: maxDimension
    };
}

function normalizeFileName(fileName) {
    var extensionIndex = fileName.lastIndexOf(".");
    if (extensionIndex === -1) {
        return fileName + ".jpg";
    }

    return fileName.substring(0, extensionIndex) + ".jpg";
}

function canvasToBlob(canvas, contentType, quality) {
    return new Promise(function (resolve) {
        canvas.toBlob(function (blob) {
            resolve(blob);
        }, contentType, quality);
    });
}

function loadImage(file) {
    return new Promise(function (resolve, reject) {
        var reader = new FileReader();
        reader.onload = function () {
            var image = new Image();
            image.onload = function () {
                resolve(image);
            };
            image.onerror = function () {
                reject(new Error("Image load failed."));
            };
            image.src = reader.result;
        };
        reader.onerror = function () {
            reject(new Error("File read failed."));
        };
        reader.readAsDataURL(file);
    });
}

function fileToDataUrl(file) {
    return new Promise(function (resolve, reject) {
        var reader = new FileReader();
        reader.onload = function () {
            resolve(reader.result);
        };
        reader.onerror = function () {
            reject(new Error("File read failed."));
        };
        reader.readAsDataURL(file);
    });
}

function blobToDataUrl(blob) {
    return new Promise(function (resolve, reject) {
        var reader = new FileReader();
        reader.onload = function () {
            resolve(reader.result);
        };
        reader.onerror = function () {
            reject(new Error("Blob read failed."));
        };
        reader.readAsDataURL(blob);
    });
}

async function fileToBase64(file) {
    var buffer = await file.arrayBuffer();
    return arrayBufferToBase64(buffer);
}

async function blobToBase64(blob) {
    var buffer = await blob.arrayBuffer();
    return arrayBufferToBase64(buffer);
}

function arrayBufferToBase64(buffer) {
    var bytes = new Uint8Array(buffer);
    var chunkSize = 0x8000;
    var binary = "";

    for (var index = 0; index < bytes.length; index += chunkSize) {
        var chunk = bytes.subarray(index, Math.min(index + chunkSize, bytes.length));
        binary += String.fromCharCode.apply(null, chunk);
    }

    return btoa(binary);
}
