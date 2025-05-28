import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react";
import {ws} from "@/redux/socket.ts";

export const api = createApi({
    baseQuery: fetchBaseQuery({baseUrl: '/'}),
    endpoints: (builder) => ({
        getConversations: builder.query<string[], string>({
            queryFn() {
                return { data: [] };
            },
            async onCacheEntryAdded(arg, {cacheDataLoaded, updateCachedData, cacheEntryRemoved}) {
                console.log(`Running getConversations: ${arg}`);
                
                function onMessageReceived() {
                    console.log(`Message received: ${arg}`);
                    
                    updateCachedData((draft) => {
                        draft.push(arg);
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
} = api;