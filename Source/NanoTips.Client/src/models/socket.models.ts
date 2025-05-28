export type MessageTypes = 'none' | 'getConversations';

export type SocketMessage = {
    type: MessageTypes;
}

export type GetConversationsMessage = SocketMessage & {
    conversations: string[];
}

export type EnvelopeTypes<T extends MessageTypes> =
    T extends 'none' ? SocketMessage : 
        T extends 'getConversations' ? GetConversationsMessage : 
        never;

export type ListenerTypes<T extends MessageTypes> =
    T extends 'none' ? (message: SocketMessage) => void : 
        T extends 'getConversations' ? (message: GetConversationsMessage) => void : 
        never;