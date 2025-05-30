import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react";
import {ws} from "@/redux/socket.ts";
import type {ConversationListModel} from "@/models/conversations.model.tsx";

export const api = createApi({
    baseQuery: fetchBaseQuery({baseUrl: '/'}),
    endpoints: (builder) => ({
        reqConversation: builder.mutation<void, void>({
            queryFn() {
                console.log(`Running reqConversation`);
                ws.sendMessage('reqConversation', {});
                return { data: undefined };
            }
        }),
        reqConversations: builder.mutation<void, void>({
            queryFn() {
                console.log('Running reqConversations');
                ws.sendMessage('reqConversations', {});
                return { data: undefined };
            },
        }),
        getConversations: builder.query<ConversationListModel[], void>({
            queryFn() {
                return { data: [] };
            },
            async onCacheEntryAdded(arg, {cacheDataLoaded, updateCachedData, cacheEntryRemoved}) {
                console.log(`Running getConversations: ${arg}`);
                
                function onMessageReceived(data: ConversationListModel[]) {
                    console.log(`Message received: ${arg}`);
                    
                    updateCachedData((draft) => {
                        draft.push(...data);
                        console.log(`Updated cache with data`, draft);
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
    useReqConversationsMutation,
    useReqConversationMutation,
} = api;