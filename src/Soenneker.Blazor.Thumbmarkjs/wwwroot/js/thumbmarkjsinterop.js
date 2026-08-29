const dotNetReferences = {};
const thumbmarkOptions = {};
const thumbmarks = {};
const observers = {};
const results = {};

function parseOptions(options) {
    if (!options)
        return { logging: false };

    try {
        const opt = typeof options === "string" ? JSON.parse(options) : options;

        return {
            api_key: typeof (opt.api_key ?? opt.apiKey) === "string" ? (opt.api_key ?? opt.apiKey) : undefined,
            api_endpoint: typeof (opt.api_endpoint ?? opt.apiEndpoint) === "string" ? (opt.api_endpoint ?? opt.apiEndpoint) : undefined,
            include: Array.isArray(opt.include) ? opt.include : undefined,
            exclude: Array.isArray(opt.exclude) ? opt.exclude : undefined,
            permissions_to_check: Array.isArray(opt.permissions_to_check ?? opt.permissionsToCheck) ? (opt.permissions_to_check ?? opt.permissionsToCheck) : undefined,
            stabilize: Array.isArray(opt.stabilize) ? opt.stabilize : undefined,
            timeout: typeof opt.timeout === "number" ? opt.timeout : undefined,
            logging: typeof opt.logging === "boolean" ? opt.logging : false,
            cache_api_call: typeof (opt.cache_api_call ?? opt.cacheApiCall) === "boolean" ? (opt.cache_api_call ?? opt.cacheApiCall) : undefined,
            cache_lifetime_in_ms: typeof (opt.cache_lifetime_in_ms ?? opt.cacheLifetimeInMs) === "number" ? (opt.cache_lifetime_in_ms ?? opt.cacheLifetimeInMs) : undefined,
            performance: typeof opt.performance === "boolean" ? opt.performance : undefined,
            metadata: opt.metadata ?? undefined
        };
    } catch (error) {
        throw new Error("Failed to parse ThumbmarkJS options.", { cause: error });
    }
}

function createThumbmark(options) {
    const ThumbmarkCtor = globalThis.Thumbmark ?? globalThis.ThumbmarkJS?.Thumbmark;

    if (!ThumbmarkCtor)
        throw new Error("ThumbmarkJS constructor could not be found.");

    return new ThumbmarkCtor(options ?? { logging: false });
}

function ensureThumbmark(elementId) {
    let instance = thumbmarks[elementId];
    if (instance)
        return instance;

    const options = thumbmarkOptions[elementId] ?? { logging: false };
    instance = createThumbmark(options);
    thumbmarks[elementId] = instance;
    return instance;
}

async function getCachedResult(elementId) {
    let data = results[elementId];
    if (data)
        return data;

    data = await ensureThumbmark(elementId).get();
    results[elementId] = data;
    return data;
}

function clearResult(elementId) {
    delete results[elementId];
}

export function initialize(elementId, reference) {
    dotNetReferences[elementId] = reference;
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
    await invokeDotNet(elementId, "OnGenerated", value);
    return value;
}

export async function getData(elementId) {
    const data = await getCachedResult(elementId);
    if (!data)
        return null;

    await invokeDotNet(elementId, "OnGenerated", data.thumbmark ?? null);
    await invokeDotNet(elementId, "OnDataGenerated", data);
    return data;
}

export function createObserver(elementId) {
    const target = document.getElementById(elementId);
    if (!target)
        return;

    disposeObserver(elementId);

    const observer = new MutationObserver(() => {
        if (!target.isConnected)
            dispose(elementId);
    });

    const observationRoot = document.body ?? document.documentElement;
    if (observationRoot) {
        observer.observe(observationRoot, { childList: true, subtree: true });
        observers[elementId] = observer;
    }
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
        delete dotNetReferences[elementId];
        return;
    }

    for (const key of Object.keys(observers))
        disposeObserver(key);

    for (const key of Object.keys(thumbmarks)) delete thumbmarks[key];
    for (const key of Object.keys(thumbmarkOptions)) delete thumbmarkOptions[key];
    for (const key of Object.keys(results)) delete results[key];
    for (const key of Object.keys(dotNetReferences)) delete dotNetReferences[key];
}

async function invokeDotNet(elementId, methodName, ...args) {
    const reference = dotNetReferences[elementId];
    if (!reference)
        return;

    try {
        await reference.invokeMethodAsync(methodName, ...args);
    } catch (error) {
        console.debug(`Unable to invoke .NET callback '${methodName}'.`, error);
    }
}
