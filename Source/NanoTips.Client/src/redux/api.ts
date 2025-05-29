import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react";
import {ws} from "@/redux/socket.ts";
import type {GetConversationRequest} from "@/models/socket.models.ts";

export const api = createApi({
    baseQuery: fetchBaseQuery({baseUrl: '/'}),
    endpoints: (builder) => ({
        reqConversation: builder.mutation<void, GetConversationRequest>({
            queryFn(request) {
                console.log(`Running reqConversation: ${request.conversationId}`);
                ws.sendMessage('reqConversation', request);
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
        getConversations: builder.query<string[], void>({
            queryFn() {
                return { data: [] };
            },
            async onCacheEntryAdded(arg, {cacheDataLoaded, updateCachedData, cacheEntryRemoved}) {
                console.log(`Running getConversations: ${arg}`);
                
                function onMessageReceived() {
                    console.log(`Message received: ${arg}`);
                    
                    updateCachedData((_) => {
                        //TODO: Update data in the cache
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