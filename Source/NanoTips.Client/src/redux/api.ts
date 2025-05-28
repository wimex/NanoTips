import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react";
import {ws} from "@/redux/socket.ts";

export const api = createApi({
    baseQuery: fetchBaseQuery({baseUrl: '/'}),
    endpoints: (builder) => ({
        getMessages: builder.query<string[], string>({
            queryFn() {
                return { data: [] };
            },
            async onCacheEntryAdded(arg, {cacheDataLoaded, updateCachedData, cacheEntryRemoved}) {
                console.log(`Running getMessages: ${arg}`);
                
                function onMessageReceived() {
                    console.log(`Message received: ${arg}`);
                    
                    updateCachedData((draft) => {
                        draft.push(arg);
                    });
                }
                
                try {
                    await cacheDataLoaded;
                    console.log(`getMessages cache data loaded: ${arg}`);
                    
                    ws.addEventListener('message', onMessageReceived);
                } finally {
                    await cacheEntryRemoved;
                    ws.removeEventListener('message', onMessageReceived);
                    
                    console.log(`getMessages cache entry removed: ${arg}`);
                }
            },
        }),
    }),
});

export const {
    useGetMessagesQuery,
} = api;