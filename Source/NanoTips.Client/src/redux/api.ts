import {createApi, fetchBaseQuery} from "@reduxjs/toolkit/query/react";
import {ws} from "@/redux/socket.ts";
import type {ConversationListModel, ConversationViewModel} from "@/models/conversations.model.tsx";
import {messageTypes} from "@/models/socket.models.ts";
import type {ArticleEditorModel, ArticleListViewModel, ArticleViewModel} from "@/models/articles.model.tsx";

export const api = createApi({
    baseQuery: fetchBaseQuery({baseUrl: '/'}),
    endpoints: (builder) => ({
        editArticle: builder.mutation<{}, ArticleEditorModel>({
            queryFn(article) {
                ws.sendMessage(messageTypes.editArticle, article);
                return { data: {} };
            },
        }),
        getArticle: builder.query<ArticleViewModel, string>({
            queryFn(articleId) {
                ws.sendMessage(messageTypes.getArticle, {articleId, slug: '', title: '', content: ''});
                return { data: {} as ArticleViewModel };
            },
            async onCacheEntryAdded(arg, {cacheDataLoaded, cacheEntryRemoved, dispatch}) {
                function onMessageReceived(data: ArticleViewModel) {
                    dispatch(api.util.updateQueryData(messageTypes.getArticle, data.articleId, () => {
                        return data;
                    }));
                }
                
                try {
                    await cacheDataLoaded;
                    console.log(`getArticle cache data loaded: ${arg}`);
                    
                    ws.addMessageListener(messageTypes.getArticle, onMessageReceived);
                } finally {
                    await cacheEntryRemoved;
                    ws.removeMessageListener(messageTypes.getArticle, onMessageReceived);
                    
                    console.log(`getArticle cache entry removed: ${arg}`);
                }
            },
        }),
        getArticles: builder.query<ArticleListViewModel[], void>({
            queryFn() {
                ws.sendMessage(messageTypes.getArticles, []);
                return { data: [] };
            },
            async onCacheEntryAdded(arg, {cacheDataLoaded, updateCachedData, cacheEntryRemoved, getCacheEntry}) {
                function onMessageReceived(data: ArticleListViewModel[]) {
                    updateCachedData(() => {
                        const entries = getCacheEntry()?.data || [];
                        const items = [...data, ...entries];
                        const uniques = items.filter((x, i, a) => a.findIndex(y => x.articleId === y.articleId) === i);
                        return uniques.sort((a, b) => a.title.localeCompare(b.title));
                    });
                }
                
                try {
                    await cacheDataLoaded;
                    console.log(`getArticles cache data loaded: ${arg}`);
                    
                    ws.addMessageListener(messageTypes.getArticles, onMessageReceived);
                } finally {
                    await cacheEntryRemoved;
                    ws.removeMessageListener(messageTypes.getArticles, onMessageReceived);
                    
                    console.log(`getArticles cache entry removed: ${arg}`);
                }
            },
        }),
        getConversation: builder.query<ConversationViewModel, string>({
            queryFn(conversationId) {
                ws.sendMessage(messageTypes.getConversation, {conversationId, subject: '', messages: []});
                return { data: {} as ConversationViewModel };
            },
            async onCacheEntryAdded(arg, {cacheDataLoaded, cacheEntryRemoved, dispatch}) {
                function onMessageReceived(data: ConversationViewModel) {
                    dispatch(api.util.updateQueryData(messageTypes.getConversation, data.conversationId, () => {
                        return data;
                    }));
                }
                
                try {
                    await cacheDataLoaded;
                    console.log(`getConversation cache data loaded: ${arg}`);
                    
                    ws.addMessageListener(messageTypes.getConversation, onMessageReceived);
                } finally {
                    await cacheEntryRemoved;
                    ws.removeMessageListener(messageTypes.getConversation, onMessageReceived);
                    
                    console.log(`getConversation cache entry removed: ${arg}`);
                }
            },
        }),
        getConversations: builder.query<ConversationListModel[], void>({
            queryFn() {
                ws.sendMessage(messageTypes.getConversations, []);
                return { data: [] };
            },
            async onCacheEntryAdded(arg, {cacheDataLoaded, updateCachedData, cacheEntryRemoved, getCacheEntry}) {
                function onMessageReceived(data: ConversationListModel[]) {
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
                    
                    ws.addMessageListener(messageTypes.getConversations, onMessageReceived);
                } finally {
                    await cacheEntryRemoved;
                    ws.removeMessageListener(messageTypes.getConversations, onMessageReceived);
                    
                    console.log(`getConversations cache entry removed: ${arg}`);
                }
            },
        }),
    }),
});

export const {
    useGetConversationsQuery,
    useGetConversationQuery,
    useGetArticlesQuery,
    useGetArticleQuery,
    useEditArticleMutation,
} = api;