
class WebSocketClass
{
    private url: string = 'http://localhost:3000';
    private ws: WebSocket | null = null;
    
    private listeners = {
        open: [] as Array<() => void>,
        message: [] as Array<(event: MessageEvent) => void>,
        close: [] as Array<() => void>,
        error: [] as Array<(error: Event) => void>,
    }
    
    public addEventListener(event: 'open' | 'message' | 'close' | 'error', listener: any): void {
        this.listeners[event].push(listener);
        console.log(`Listener added for event: ${event} - Total listeners: ${this.listeners[event].length}`);
    }
    
    public removeEventListener(event: 'open' | 'message' | 'close' | 'error', listener: any): void {
        this.listeners[event].splice(this.listeners[event].indexOf(listener), 1);
        console.log(`Listener removed for event: ${event} - Remaining listeners: ${this.listeners[event].length}`);
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
            for (const listener of this.listeners.message) {
                listener(event);
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