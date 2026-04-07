let dotNetReference = null;
const thumbmarkOptions = {};
const thumbmarks = {};
const observers = {};
const results = {};

function parseOptions(options) {
    if (!options)
        return {};

    try {
        const opt = typeof options === "string" ? JSON.parse(options) : options;

        return {
            api_key: typeof opt.api_key === "string" ? opt.api_key : undefined,
            api_endpoint: typeof opt.api_endpoint === "string" ? opt.api_endpoint : undefined,
            include: Array.isArray(opt.include) ? opt.include : undefined,
            exclude: Array.isArray(opt.exclude) ? opt.exclude : undefined,
            permissions_to_check: Array.isArray(opt.permissions_to_check) ? opt.permissions_to_check : undefined,
            stabilize: Array.isArray(opt.stabilize) ? opt.stabilize : undefined,
            timeout: typeof opt.timeout === "number" ? opt.timeout : undefined,
            logging: typeof opt.logging === "boolean" ? opt.logging : undefined,
            cache_api_call: typeof opt.cache_api_call === "boolean" ? opt.cache_api_call : undefined,
            cache_lifetime_in_ms: typeof opt.cache_lifetime_in_ms === "number" ? opt.cache_lifetime_in_ms : undefined,
            performance: typeof opt.performance === "boolean" ? opt.performance : undefined,
            metadata: opt.metadata ?? undefined
        };
    } catch (err) {
        console.error("Failed to parse ThumbmarkJS options.", err);
        return {};
    }
}

function createThumbmark(options) {
    const ThumbmarkCtor =
        globalThis.Thumbmark ??
        globalThis.ThumbmarkJS?.Thumbmark;

    if (!ThumbmarkCtor) {
        console.error("ThumbmarkJS constructor could not be found.");
        return null;
    }

    return new ThumbmarkCtor(options ?? {});
}

function ensureThumbmark(elementId) {
    let inst = thumbmarks[elementId];
    if (inst)
        return inst;

    const opts = thumbmarkOptions[elementId] ?? {};
    inst = createThumbmark(opts);
    thumbmarks[elementId] = inst;
    return inst;
}

async function getCachedResult(elementId) {
    let data = results[elementId];
    if (data)
        return data;

    const thumbmark = ensureThumbmark(elementId);
    if (!thumbmark)
        return null;

    data = await thumbmark.get();
    results[elementId] = data;
    return data;
}

function clearResult(elementId) {
    delete results[elementId];
}

export function initialize(ref) {
    dotNetReference = ref;
}

export function setOptions(elementId, options) {
    const parsed = parseOptions(options);
    thumbmarkOptions[elementId] = parsed;
    thumbmarks[elementId] = createThumbmark(parsed);
    clearResult(elementId);
}

export async function get(elementId) {
    const data = await getCachedResult(elementId);
    if (!data)
        return null;

    const value = data.thumbmark ?? null;

    if (dotNetReference) {
        await dotNetReference.invokeMethodAsync("OnGenerated", value);
    }

    return value;
}

export async function getData(elementId) {
    const data = await getCachedResult(elementId);
    if (!data)
        return null;

    if (dotNetReference) {
        await dotNetReference.invokeMethodAsync("OnGenerated", data.thumbmark ?? null);
        await dotNetReference.invokeMethodAsync("OnDataGenerated", data);
    }

    return data;
}

export function createObserver(elementId) {
    const target = document.getElementById(elementId);
    if (!target?.parentNode)
        return;

    disposeObserver(elementId);

    const observer = new MutationObserver((mutations) => {
        const removed = mutations.some((mutation) =>
            Array.from(mutation.removedNodes).includes(target)
        );

        if (removed) {
            disposeObserver(elementId);
            delete thumbmarks[elementId];
            delete thumbmarkOptions[elementId];
            delete results[elementId];
        }
    });

    observer.observe(target.parentNode, { childList: true });
    observers[elementId] = observer;
}

function disposeObserver(elementId) {
    const observer = observers[elementId];
    if (observer) {
        observer.disconnect();
        delete observers[elementId];
    }
}

export function dispose(elementId) {
    if (elementId) {
        disposeObserver(elementId);
        delete thumbmarks[elementId];
        delete thumbmarkOptions[elementId];
        delete results[elementId];
        return;
    }

    for (const key of Object.keys(observers)) {
        disposeObserver(key);
    }

    for (const k of Object.keys(thumbmarks)) delete thumbmarks[k];
    for (const k of Object.keys(thumbmarkOptions)) delete thumbmarkOptions[k];
    for (const k of Object.keys(results)) delete results[k];
    dotNetReference = null;
}
