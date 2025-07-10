export class ThumbmarkjsInterop {
    constructor() {
        this.options = {};
        this.dotNetReference = null;
        this.thumbmark = null;
    }

    initialize(dotNetReference) {
        this.dotNetReference = dotNetReference;
        this.thumbmark = new ThumbmarkJS.Thumbmark();
    }

    setOptions(elementId, options) {
        if (!this.thumbmark) return;

        if (options) {
            const opt = JSON.parse(options);
            this.options[elementId] = opt;

            // Apply each option
            if (opt.api_key) this.thumbmark.setOption("api_key", opt.api_key);
            if (opt.exclude) this.thumbmark.setOption("exclude", opt.exclude);
            if (opt.include) this.thumbmark.setOption("include", opt.include);
            if (opt.permissions_to_check) this.thumbmark.setOption("permissions_to_check", opt.permissions_to_check);
            if (typeof opt.timeout === "number") this.thumbmark.setOption("timeout", opt.timeout);
            if (typeof opt.logging === "boolean") this.thumbmark.setOption("logging", opt.logging);
            if (typeof opt.cache_api_call === "boolean") this.thumbmark.setOption("cache_api_call", opt.cache_api_call);
            if (typeof opt.performance === "boolean") this.thumbmark.setOption("performance", opt.performance);
        }
    }

    async get(elementId) {
        if (!this.thumbmark)
            return null;

        const result = await this.thumbmark.get();

        if (this.dotNetReference) {
            this.dotNetReference.invokeMethodAsync('OnGenerated', result.thumbmark);
        }

        return result.thumbmark;
    }

    async getData(elementId) {
        if (!this.thumbmark) return null;

        const data = await this.thumbmark.get();

        if (this.dotNetReference) {
            this.dotNetReference.invokeMethodAsync('OnDataGenerated', data);
        }

        return data;
    }

    createObserver(elementId) {
        const target = document.getElementById(elementId);
        if (!target || !target.parentNode) return;

        this.observer = new MutationObserver((mutations) => {
            const targetRemoved = mutations.some(mutation =>
                Array.from(mutation.removedNodes).indexOf(target) !== -1
            );

            if (targetRemoved) {
                this.observer && this.observer.disconnect();
                delete this.observer;
            }
        });

        this.observer.observe(target.parentNode, { childList: true });
    }
}

window.ThumbmarkjsInterop = new ThumbmarkjsInterop();
