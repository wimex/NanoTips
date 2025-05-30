export type ConversationListModel = {
    conversationId: string;
    subject: string;
    lastMessageDate: string;
    messageCount: number;
    answered: boolean;
};

export type GetConversationRequest = {
    conversationId: string;
}

export type GetConversationResponse = {
    conversationId: string;
    messages: string[];
}