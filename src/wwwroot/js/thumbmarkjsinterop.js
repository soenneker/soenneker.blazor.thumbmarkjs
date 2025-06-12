export class ThumbmarkjsInterop {
    constructor() {
        this.options = {};
        this.dotNetReference = null;
    }

    initialize(dotNetReference) {
        this.dotNetReference = dotNetReference;
    }

    setOptions(elementId, options) {
        if (options) {
            const opt = JSON.parse(options);
            this.options[elementId] = opt;
            
            // Apply each option individually as per library docs
            if (opt.exclude) {
                ThumbmarkJS.setOption('exclude', opt.exclude);
            }
            if (opt.include) {
                ThumbmarkJS.setOption('include', opt.include);
            }
            if (opt.timeout) {
                ThumbmarkJS.setOption('timeout', opt.timeout);
            }
            if (opt.logging !== undefined) {
                ThumbmarkJS.setOption('logging', opt.logging);
            }
        }
    }

    async getFingerprint(elementId) {
        const fingerprint = await ThumbmarkJS.getFingerprint();

        if (this.dotNetReference) {
            this.dotNetReference.invokeMethodAsync('OnFingerprintGenerated', fingerprint);
        }

        return fingerprint;
    }

    async getFingerprintData(elementId) {
        const data = await ThumbmarkJS.getFingerprintData();

        if (this.dotNetReference) {
            this.dotNetReference.invokeMethodAsync('OnFingerprintDataGenerated', data);
        }

        return data;
    }

    createObserver(elementId) {
        const target = document.getElementById(elementId);

        this.observer = new MutationObserver((mutations) => {
            const targetRemoved = mutations.some(mutation => Array.from(mutation.removedNodes).indexOf(target) !== -1);

            if (targetRemoved) {
                this.observer && this.observer.disconnect();
                delete this.observer;
            }
        });

        this.observer.observe(target.parentNode, { childList: true });
    }
}

window.ThumbmarkjsInterop = new ThumbmarkjsInterop();
