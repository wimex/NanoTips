import {
    type EnvelopeTypes,
    type GetConversationsMessage,
    type ListenerTypes,
    type MessageTypes
} from "@/models/socket.models";

class WebSocketClass
{
    private url: string = 'http://localhost:5245/ws';
    private ws: WebSocket | null = null;
    
    private listeners = {
        open: [] as Array<() => void>,
        message: {
            none: [] as Array<(message: GetConversationsMessage) => void>,
            getConversations: [] as Array<(message: GetConversationsMessage) => void>,
        } as Record<MessageTypes, Array<ListenerTypes<MessageTypes>>>,
        close: [] as Array<() => void>,
        error: [] as Array<(error: Event) => void>,
    }
    
    public sendMessage<T extends MessageTypes>(message: EnvelopeTypes<T>): void {
        const json = JSON.stringify(message);
        if (this.ws && this.ws.readyState === WebSocket.OPEN) {
            this.ws.send(json);
            console.log('WebSocket message sent:', json);
        } else {
            console.error('WebSocket is not open. Cannot send message:', json);
            setTimeout(() => {
                this.sendMessage(message);
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
            
            const message: GetConversationsMessage = JSON.parse(event.data);
            if (message && message.type) {
                for (const listener of this.listeners.message[message.type]) {
                    listener(message);
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