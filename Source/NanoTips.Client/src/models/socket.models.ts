export type MessageTypes = 'none' | 
    'reqConversations' | 'getConversations' |
    'reqConversation' | 'getConversation';

export type WebsocketEnvelopeModel = {
    type: MessageTypes;
}

export type WebsocketEnvelopeModelTyped<TType extends MessageTypes> = WebsocketEnvelopeModel & {
    data: EnvelopeTypes<TType>;
}

export type GetConversationsMessage = {
    conversations: string[];
}

export type GetConversationRequest = {
    conversationId: string;
}

export type GetConversationResponse = {
    conversationId: string;
    messages: string[];
}

export type EnvelopeTypes<T extends MessageTypes> =
    T extends 'none' ? never :
        T extends 'reqConversations' ? {} :
            T extends 'getConversations' ? GetConversationsMessage :
                T extends 'reqConversation' ? GetConversationRequest :
                    T extends 'getConversation' ? GetConversationResponse :
                never;

export type ListenerTypes<T extends MessageTypes> = (data: EnvelopeTypes<T>) => void;