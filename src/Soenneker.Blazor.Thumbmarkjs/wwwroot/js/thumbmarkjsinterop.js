const interop = (() => {
    const instance = {};
    instance.initialize = function(dotNetReference) {
        this.dotNetReference = dotNetReference;
    };

    instance.setOptions = function(elementId, options) {
        const parsed = this.#parseOptions(options);
        this.options[elementId] = parsed;
        this.thumbmarks[elementId] = this.#createThumbmark(parsed);
        delete this.results[elementId];
    };

    instance.get = async function(elementId) {
        const data = await this.#getResult(elementId);
        if (!data)
            return null;

        const value = data.thumbmark ?? null;

        if (this.dotNetReference) {
            await this.dotNetReference.invokeMethodAsync("OnGenerated", value);
        }

        return value;
    };

    instance.getData = async function(elementId) {
        const data = await this.#getResult(elementId);
        if (!data)
            return null;

        if (this.dotNetReference) {
            await this.dotNetReference.invokeMethodAsync("OnGenerated", data.thumbmark ?? null);
            await this.dotNetReference.invokeMethodAsync("OnDataGenerated", data);
        }

        return data;
    };

    instance.clearResult = function(elementId) {
        delete this.results[elementId];
    };

    instance.createObserver = function(elementId) {
        const target = document.getElementById(elementId);
        if (!target?.parentNode)
            return;

        this.disposeObserver(elementId);

        const observer = new MutationObserver((mutations) => {
            const removed = mutations.some((mutation) =>
                Array.from(mutation.removedNodes).includes(target)
            );

            if (removed) {
                this.disposeObserver(elementId);
                delete this.thumbmarks[elementId];
                delete this.options[elementId];
                delete this.results[elementId];
            }
        });

        observer.observe(target.parentNode, { childList: true });
        this.observers[elementId] = observer;
    };

    instance.disposeObserver = function(elementId) {
        const observer = this.observers[elementId];
        if (observer) {
            observer.disconnect();
            delete this.observers[elementId];
        }
    };

    instance.dispose = function(elementId) {
        if (elementId) {
            this.disposeObserver(elementId);
            delete this.thumbmarks[elementId];
            delete this.options[elementId];
            delete this.results[elementId];
            return;
        }

        for (const key of Object.keys(this.observers)) {
            this.disposeObserver(key);
        }

        this.thumbmarks = {};
        this.options = {};
        this.results = {};
        this.dotNetReference = null;
    };

    instance.getResult = async function(elementId) {
        let data = this.results[elementId];
        if (data)
            return data;

        const thumbmark = this.ensureThumbmark(elementId);
        if (!thumbmark)
            return null;

        data = await thumbmark.get();
        this.results[elementId] = data;
        return data;
    };

    instance.ensureThumbmark = function(elementId) {
        let instance = this.thumbmarks[elementId];
        if (instance)
            return instance;

        const options = this.options[elementId] ?? {};
        instance = this.createThumbmark(options);
        this.thumbmarks[elementId] = instance;
        return instance;
    };

    instance.createThumbmark = function(options) {
        const ThumbmarkCtor =
            globalThis.Thumbmark ??
            globalThis.ThumbmarkJS?.Thumbmark;

        if (!ThumbmarkCtor) {
            console.error("ThumbmarkJS constructor could not be found.");
            return null;
        }

        return new ThumbmarkCtor(options ?? {});
    };

    instance.parseOptions = function(options) {
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
    };

        instance.options = {};
        instance.dotNetReference = null;
        instance.thumbmarks = {};
        instance.observers = {};
        instance.results = {};
    

    return instance;
})();
export function initialize(dotNetReference) {
    return interop.initialize(dotNetReference);
}

export function setOptions(elementId, options) {
    return interop.setOptions(elementId, options);
}

export function get(elementId) {
    return interop.get(elementId);
}

export function getData(elementId) {
    return interop.getData(elementId);
}

export function createObserver(elementId) {
    return interop.createObserver(elementId);
}

export function dispose(elementId) {
    return interop.dispose(elementId);
}