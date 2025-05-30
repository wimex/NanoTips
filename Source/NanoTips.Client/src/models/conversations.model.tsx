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

export type ConversationViewModel = {
    conversationId: string;
    subject: string;
    messages: ConversationMessageViewModel[];
}

export type ConversationMessageViewModel = {
    messageId: string;
    created: Date;
    processed: Date | null;
    direction: MessageDirection;
    sender: string;
    recipient: string;
    subject: string;
    body: string;
    categorySuggestions: Record<string, number>;
    categoryId?: string;
    sent: boolean;
};

export type MessageDirection = 'incoming' | 'outgoing';