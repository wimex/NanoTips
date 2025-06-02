import type {
    ConversationListModel,
    ConversationViewModel
} from "@/models/conversations.model.tsx";
import type {ArticleListViewModel} from "@/models/articles.model.tsx";

// Denotes all the possible message types. Every envelope will contain two fields: type and data.
export const messageTypes = {
    none: 'none',
    getConversations: 'getConversations',
    getConversation: 'getConversation',
    getArticles: 'getArticles',
    getArticle: 'getArticle',
} as const;

// Acts as a union type for all possible message types.
export type MessageType = (typeof messageTypes)[keyof typeof messageTypes];

// Infers the data type based on the message type. When the envelope contains e.g. type = 'getConversations', 
// the data field will be of type ConversationListModel[].
export type DataType<T extends MessageType> = 
    T extends typeof messageTypes.none ? never :
    T extends typeof messageTypes.getConversations ? ConversationListModel[] :
    T extends typeof messageTypes.getConversation ? ConversationViewModel :
    T extends typeof messageTypes.getArticles ? ArticleListViewModel[] :
    never;

// Defines a callback type that takes data of the specified message type as an argument.
// When an envelope with type 'getConversations' is received, the callback will receive data of type ConversationListModel[].
export type CallbackType<T extends MessageType> = (data: DataType<T>) => void;

// Defines the structure of a WebSocket envelope. Every message sent or received must conform to this structure.
export type WebsocketEnvelope = {
    type: MessageType;
    route?: string;
}

// Defines a typed version of the WebSocket envelope. It includes the data field based on the message type.
export type WebsocketEnvelopeTyped<TType extends MessageType> = WebsocketEnvelope & {
    data: DataType<TType>;
}