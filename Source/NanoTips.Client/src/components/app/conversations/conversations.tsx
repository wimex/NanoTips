import {useGetConversationsQuery, useReqConversationsMutation} from "@/redux/api.ts";
import {useEffect, useState} from "react";
import type {ConversationListModel} from "@/models/conversations.model.tsx";

export default function Conversations() {
    const [ conversations, setConversations ] = useState<ConversationListModel[]>([]);
    
    const [reqConversations] = useReqConversationsMutation();
    const getConversations = useGetConversationsQuery();
    console.log('RENDER conversations', getConversations.data);

    useEffect(() => {
        (async () => {
            await reqConversations();
        })();
    }, []);
    
    return (<div>
        {getConversations.data?.map((conversation) => (
            <div key={conversation.conversationId} className="p-2 border-b border-gray-200 hover:bg-gray-100 cursor-pointer">
                <h3 className="text-lg font-semibold">{conversation.subject}</h3>
            </div>
        ))}
    </div>);
}