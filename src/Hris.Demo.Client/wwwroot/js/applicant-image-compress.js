// Client-side resize + re-encode for applicant profile images (Blazor WASM).
window.hrisApplicantImages = {
  /**
   * @param {string} base64Input - raw file as base64 (no data: prefix)
   * @param {string} inputContentType - e.g. image/jpeg
   * @param {{ maxEdge?: number, maxBytes?: number, maxIterations?: number }} options
   * @returns {Promise<{ base64: string, contentType: string, byteLength: number } | { error: string }>}
   */
  compressBase64: async function (base64Input, inputContentType, options) {
    const maxEdge = options?.maxEdge ?? 1536;
    const maxBytes = options?.maxBytes ?? 1048576;
    const maxIterations = options?.maxIterations ?? 22;

    const binary = atob(base64Input);
    const len = binary.length;
    if (len > 12 * 1024 * 1024) {
      return { error: "Image payload is unexpectedly large after decoding." };
    }

    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
      bytes[i] = binary.charCodeAt(i);
    }

    const blob = new Blob([bytes], { type: inputContentType || "image/jpeg" });
    let bmp;
    try {
      bmp = await createImageBitmap(blob);
    } catch {
      return { error: "This image format could not be decoded in the browser." };
    }

    let w = bmp.width;
    let h = bmp.height;
    const scale = Math.min(1, maxEdge / Math.max(w, h));
    w = Math.max(1, Math.round(w * scale));
    h = Math.max(1, Math.round(h * scale));

    const canvas = document.createElement("canvas");
    canvas.width = w;
    canvas.height = h;
    const ctx = canvas.getContext("2d");
    if (!ctx) {
      bmp.close();
      return { error: "Canvas is not available in this browser." };
    }

    ctx.drawImage(bmp, 0, 0, w, h);
    bmp.close();

    const normalized = (inputContentType || "").toLowerCase();
    const preferWebp = normalized === "image/webp";
    const mimeOrder = preferWebp ? ["image/webp", "image/jpeg"] : ["image/jpeg", "image/webp"];

    for (const outMime of mimeOrder) {
      let quality = 0.92;
      for (let iter = 0; iter < maxIterations; iter++) {
        const b = await new Promise((resolve) => canvas.toBlob(resolve, outMime, quality));
        if (!b) {
          break;
        }

        if (b.size <= maxBytes) {
          const ab = await b.arrayBuffer();
          const u8 = new Uint8Array(ab);
          let bin = "";
          const chunk = 0x8000;
          for (let i = 0; i < u8.length; i += chunk) {
            bin += String.fromCharCode.apply(null, u8.subarray(i, i + chunk));
          }
          return {
            base64: btoa(bin),
            contentType: b.type || outMime,
            byteLength: u8.length,
          };
        }

        quality -= 0.055;
        if (quality < 0.34) {
          break;
        }
      }
    }

    return {
      error:
        "Could not compress this image under 1 MB while keeping acceptable quality. Try a smaller original or a different format.",
    };
  },

  compressBase64Json: async function (base64Input, inputContentType, options) {
    const r = await window.hrisApplicantImages.compressBase64(base64Input, inputContentType, options);
    return JSON.stringify(r);
  },
};
