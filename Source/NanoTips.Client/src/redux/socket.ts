import {
    type CallbackType,
    type DataType,
    type MessageType, type WebsocketEnvelope, type WebsocketEnvelopeTyped
} from "@/models/socket.models";

class WebSocketClass
{
    private keepAlive = {
        checkInterval: 5000,
        backoffPeriods: [1000, 1000, 2000, 5000, 10000],
        currentPeriod: 0,
        retryDelay: 3000,
    };
    
    private url: string = import.meta.env.VITE_BACKEND_URL + '/ws';
    private ws: WebSocket | null = null;
    
    private listeners = {
        open: [] as Array<() => void>,
        message: {} as Record<MessageType, Array<CallbackType<MessageType>>>,
        close: [] as Array<() => void>,
        error: [] as Array<(error: Event) => void>,
    }
    
    public sendMessage<T extends MessageType>(type: MessageType, message: DataType<T>): void {
        const data: WebsocketEnvelopeTyped<T> = { type: type, data: message };
        const json = JSON.stringify(data);
        
        if (this.ws && this.ws.readyState === WebSocket.OPEN) {
            this.ws.send(json);
        } else {
            setTimeout(() => {
                this.sendMessage(type, message);
            }, this.keepAlive.retryDelay);
        }
    }
    
    public addMessageListener<T extends MessageType>(type: T, listener: CallbackType<T>): void {
        this.listeners.message[type] = this.listeners.message[type] || [];
        this.listeners.message[type].push(listener);
    }
    
    public removeMessageListener<T extends MessageType>(type: T, listener: CallbackType<T>): void {
        this.listeners.message[type] = this.listeners.message[type] || [];
        const index = this.listeners.message[type].indexOf(listener);
        if (index !== -1) {
            this.listeners.message[type].splice(index, 1);
        }
    }
    
    private watchdog(): void {
        console.log('WebSocket watchdog triggered');
        if (!this.ws) //Connection is closed, no need to keep alive
            return;
        
        console.log('WebSocket watchdog triggered, current state:', this.ws.readyState);
        // Connection is in a healthy state
        if (this.ws.readyState === WebSocket.OPEN) {
            this.keepAlive.currentPeriod = 0;
            setTimeout(() => this.watchdog(), this.keepAlive.checkInterval);
            return;
        }
        if( this.ws.readyState === WebSocket.CONNECTING) {
            const period = Math.min(this.keepAlive.currentPeriod, this.keepAlive.backoffPeriods.length - 1);
            const timeout = this.keepAlive.backoffPeriods[period];
            setTimeout(() => this.watchdog(), timeout);
            return;
        }
        
        try {
            console.warn('WebSocket connection is not open, attempting to reinitialize...');
            this.initialize(true);
        } finally {
            const period = Math.min(this.keepAlive.currentPeriod, this.keepAlive.backoffPeriods.length - 1);
            const timeout = this.keepAlive.backoffPeriods[period];
            this.keepAlive.currentPeriod++;

            console.log(`Rescheduling watchdog: ${timeout}ms, current period: ${this.keepAlive.currentPeriod}`);
            setTimeout(() => this.watchdog(), timeout);
        }
    }
    
    private callEventHandlers(type: 'open' | 'close' | 'message' | 'error', event?: Event): void {
        if (type === 'open' || type === 'close') {
            this.listeners[type].forEach(handler => handler());
        } else if (type === 'message' && event) {
            const parameter = event as MessageEvent;
            if (!parameter || !parameter.data) {
                console.warn('Received message with no data:', event);
                return;
            }

            const typecheck = JSON.parse(parameter.data) as WebsocketEnvelope;
            if (!typecheck || !typecheck.type) {
                console.warn('Received message with no type:', parameter.data);
                return;
            }

            const message = JSON.parse(parameter.data) as WebsocketEnvelopeTyped<typeof typecheck.type>;
            if (message && message.type) {
                const listeners = this.listeners.message[message.type];
                if (!Array.isArray(listeners))
                    return;

                listeners.forEach(handler => handler(message.data));
            } else {
                console.warn('Received message with unknown type:', message);
            }
        } else if (type === 'error') {
            const error = event ?? new Event('Unknown error during WebSocket operation');
            this.listeners[type].forEach(handler => handler(error));
        }
    }
    
    public initialize(watchdog: boolean): void {
        const mailbox = window.location.pathname.split('/')[1];
        if(!mailbox) {
            console.warn('No mailbox found in URL, cannot initialize WebSocket');
            return;
        }
        
        try {
            this.ws = new WebSocket(`${this.url}/${mailbox}`);

            this.ws.addEventListener('open', () => {
                console.log('WebSocket connection opened');
                this.callEventHandlers('open');
            });

            this.ws.addEventListener('message', (event) => {
                console.log('WebSocket message received:', event.data);
                this.callEventHandlers('message', event);
            });

            this.ws.addEventListener('close', () => {
                this.callEventHandlers('close');
            });

            this.ws.addEventListener('error', (error) => {
                this.callEventHandlers('error', error);
                this.ws?.close();
            });
        } catch (error) {
            console.error('Error initializing WebSocket:', error);
        }
        
        if(!watchdog)
            this.watchdog();
    }
}

export const ws = new WebSocketClass();
ws.initialize(false);