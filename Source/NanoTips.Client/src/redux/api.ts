import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react";

export const api = createApi({
    baseQuery: fetchBaseQuery({baseUrl: '/'}),
    endpoints: (builder) => ({
        getMessages: builder.query<string[], string>({
            queryFn() {
                return { data: [] };
            },
            async onCacheEntryAdded(arg, {cacheDataLoaded, updateCachedData, cacheEntryRemoved}) {
                console.log(`Running getMessages: ${arg}`);
                
                try {
                    await cacheDataLoaded;
                    console.log(`getMessages cache data loaded: ${arg}`);
                    
                    // Simulate a delay to mimic fetching data
                    await new Promise(resolve => setTimeout(resolve, 3000));
                    updateCachedData(draft => {
                        console.log(`Updating cached data for getMessages: ${arg}`);
                        draft.push(`Message ${draft.length + 1} for ${arg}`);
                    });
                } finally {
                    await cacheEntryRemoved;
                    console.log(`getMessages cache entry removed: ${arg}`);
                }
            },
        }),
    }),
});

export const {
    useGetMessagesQuery,
} = api;