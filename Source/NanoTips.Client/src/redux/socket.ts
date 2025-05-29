import {
    type EnvelopeTypes,
    type ListenerTypes,
    type MessageTypes, type WebsocketEnvelopeModel, type WebsocketEnvelopeModelTyped
} from "@/models/socket.models";

class WebSocketClass
{
    private url: string = 'http://localhost:5245/ws';
    private ws: WebSocket | null = null;
    
    private listeners = {
        open: [] as Array<() => void>,
        message: {} as Record<MessageTypes, Array<ListenerTypes<MessageTypes>>>,
        close: [] as Array<() => void>,
        error: [] as Array<(error: Event) => void>,
    }
    
    public sendMessage<T extends MessageTypes>(type: MessageTypes, message: EnvelopeTypes<T>): void {
        const data: WebsocketEnvelopeModelTyped<T> = { type: type, data: message };
        const json = JSON.stringify(data);
        
        if (this.ws && this.ws.readyState === WebSocket.OPEN) {
            this.ws.send(json);
            console.log('WebSocket message sent:', json);
        } else {
            console.error('WebSocket is not open. Cannot send message:', json);
            setTimeout(() => {
                this.sendMessage(type, message);
            }, 1000);
        }
    }
    
    public addMessageListener<T extends MessageTypes>(type: MessageTypes, listener: ListenerTypes<T>): void {
        this.listeners.message[type].push(listener);
    }
    
    public removeMessageListener<T extends MessageTypes>(type: MessageTypes, listener: ListenerTypes<T>): void {
        const index = this.listeners.message[type].indexOf(listener);
        if (index !== -1) {
            this.listeners.message[type].splice(index, 1);
        }
    }
    
    public initialize(): void {
        this.ws = new WebSocket(this.url);
        
        this.ws.addEventListener('open', () => {
            console.log('WebSocket connection opened');
            for (const listener of this.listeners.open) {
                listener();
            }
        });
        
        this.ws.addEventListener('message', (event) => {
            console.log('WebSocket message received:', event.data);
            
            const typecheck = JSON.parse(event.data) as WebsocketEnvelopeModel;
            if (!typecheck || !typecheck.type) {
                console.warn('Received message with no type:', event.data);
                return;
            }
            
            const message = JSON.parse(event.data) as WebsocketEnvelopeModelTyped<typeof typecheck.type>;
            if (message && message.type) {
                const listeners = this.listeners.message[message.type] as ListenerTypes<typeof message.type>[];
                for (const listener of listeners) {
                    listener(message.data);
                }
            } else {
                console.warn('Received message with unknown type:', message);
            }
        });
        
        this.ws.addEventListener('close', () => {
            for (const listener of this.listeners.close) {
                listener();
            }
            
            this.ws = null;
        });
        
        this.ws.addEventListener('error', (error) => {
            for (const listener of this.listeners.error) {
                listener(error);
            }
            
            console.error('WebSocket error:', error);
            this.ws?.close();
            this.initialize();
        });
    }
}

export const ws = new WebSocketClass();
ws.initialize();