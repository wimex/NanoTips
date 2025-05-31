import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react";
import {ws} from "@/redux/socket.ts";
import type {ConversationListModel, ConversationViewModel} from "@/models/conversations.model.tsx";
import {messageTypes} from "@/models/socket.models.ts";

export const api = createApi({
    baseQuery: fetchBaseQuery({baseUrl: '/'}),
    endpoints: (builder) => ({
        getConversation: builder.query<ConversationViewModel, string>({
            queryFn(conversationId) {
                ws.sendMessage(messageTypes.getConversation, conversationId);
                return { data: {} as ConversationViewModel };
            },
            async onCacheEntryAdded(arg, {cacheDataLoaded, cacheEntryRemoved, dispatch}) {
                function onMessageReceived(data: ConversationViewModel) {
                    dispatch(api.util.updateQueryData('getConversation', data.conversationId, () => {
                        return data;
                    }));
                }
                
                try {
                    await cacheDataLoaded;
                    console.log(`getConversation cache data loaded: ${arg}`);
                    
                    ws.addMessageListener('getConversation', onMessageReceived);
                } finally {
                    await cacheEntryRemoved;
                    ws.removeMessageListener('getConversation', onMessageReceived);
                    
                    console.log(`getConversation cache entry removed: ${arg}`);
                }
            },
        }),
        getConversations: builder.query<ConversationListModel[], void>({
            queryFn() {
                ws.sendMessage(messageTypes.getConversations, {});
                return { data: [] };
            },
            async onCacheEntryAdded(arg, {cacheDataLoaded, updateCachedData, cacheEntryRemoved, getCacheEntry}) {
                function onMessageReceived(data: ConversationListModel[]) {
                    console.log(`Message received: ${arg}`);
                    
                    updateCachedData(() => {
                        const entries = getCacheEntry()?.data || [];
                        const items = [...data, ...entries];
                        const uniques = items.filter((x, i, a) => a.findIndex(y => x.conversationId === y.conversationId) === i);
                        return uniques.sort((a, b) => new Date(b.lastMessageDate).getTime() - new Date(a.lastMessageDate).getTime());
                    });
                }
                
                try {
                    await cacheDataLoaded;
                    console.log(`getConversations cache data loaded: ${arg}`);
                    
                    ws.addMessageListener('getConversations', onMessageReceived);
                } finally {
                    await cacheEntryRemoved;
                    ws.removeMessageListener('getConversations', onMessageReceived);
                    
                    console.log(`getConversations cache entry removed: ${arg}`);
                }
            },
        }),
    }),
});

export const {
    useGetConversationsQuery,
    useGetConversationQuery,
} = api;