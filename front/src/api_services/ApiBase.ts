const DEFAULT_API_URL = "http://localhost:5062/";

function ensureTrailingSlash(url: string): string {
    return url.endsWith("/") ? url : `${url}/`;
}

export function buildApiUrl(path: string): string {
    const baseUrl = import.meta.env.VITE_SERVER
        ? ensureTrailingSlash(import.meta.env.VITE_SERVER as string)
        : DEFAULT_API_URL;

    const normalizedPath = path.startsWith("/") ? path.slice(1) : path;
    return `${baseUrl}${normalizedPath}`;
}
