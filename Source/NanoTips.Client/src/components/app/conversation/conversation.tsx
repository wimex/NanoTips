import {useGetConversationQuery, useReqConversationMutation} from "@/redux/api.ts";
import {useEffect} from "react";

export default function Conversation({ conversationId }: { conversationId: string | null }) {
    const getConversation = useGetConversationQuery();
    const [reqConversation] = useReqConversationMutation();

    useEffect(() => {
        if (!conversationId) 
            return;
        
        (async () => {
            await reqConversation(conversationId);
        })();
    }, [conversationId]);
    
    return (
        <div>
            <h1 className="text-2xl font-bold mb-4">{getConversation.data?.conversationId}</h1>
            <h2 className="text-lg font-semibold mb-2">{getConversation.data?.subject}</h2>
        </div>
    )
}