import type {
    ConversationListModel,
    GetConversationRequest,
    GetConversationResponse
} from "@/models/conversations.model.tsx";

export type MessageTypes = 'none' | 
    'reqConversations' | 'getConversations' |
    'reqConversation' | 'getConversation';

export type WebsocketEnvelopeModel = {
    type: MessageTypes;
}

export type WebsocketEnvelopeModelTyped<TType extends MessageTypes> = WebsocketEnvelopeModel & {
    data: EnvelopeTypes<TType>;
}

export type EnvelopeTypes<T extends MessageTypes> =
    T extends 'none' ? never :
        T extends 'reqConversations' ? {} :
            T extends 'getConversations' ? ConversationListModel[] :
                T extends 'reqConversation' ? GetConversationRequest :
                    T extends 'getConversation' ? GetConversationResponse :
                never;

export type ListenerTypes<T extends MessageTypes> = (data: EnvelopeTypes<T>) => void;