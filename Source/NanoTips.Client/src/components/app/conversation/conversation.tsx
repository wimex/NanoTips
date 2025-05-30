import {useGetConversationQuery, useReqConversationMutation} from "@/redux/api.ts";
import {useEffect} from "react";
import {ArrowBottomRightIcon, ArrowTopRightIcon} from "@radix-ui/react-icons";

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
            <div>
                {getConversation.data?.messages?.map((message) => (
                    <div key={message.messageId} className="mb-4 p-4 border rounded">
                        <p className="text-sm text-gray-500">{message.direction === 'incoming' ? <ArrowBottomRightIcon /> : <ArrowTopRightIcon />}</p>
                        <p className="text-sm text-gray-500">{new Date(message.created).toLocaleString()}</p>
                        <p className="font-semibold">{message.sender} to {message.recipient}</p>
                        <p className="mt-2">{message.body}</p>
                        {message.categoryId && (
                            <p className="mt-2 text-sm text-blue-600">Category: {message.categoryId}</p>
                        )}
                    </div>
                ))}
            </div>
        </div>
    )
}