import {useGetConversationsQuery} from "@/redux/api.ts";

export default function Conversations({ onConversationIdChanged }: { onConversationIdChanged: (conversationId: string) => void }) {
    const getConversations = useGetConversationsQuery(undefined, { refetchOnMountOrArgChange: true });
    const mailbox = window.location.pathname.split('/')[1];

    return (<div>
        {getConversations.data?.map((conversation) => (
            <div key={conversation.conversationId} className="p-2 border-b border-gray-200 hover:bg-gray-100 cursor-pointer" onClick={() => onConversationIdChanged(conversation.conversationId)}>
                <h3 className="text-sm font-semibold">{conversation.subject}</h3>
            </div>
        ))}
        
        <div key="conversation-info" className="p-2 border-b border-gray-200 hover:bg-gray-100">
            <p>To talk to the bot, setup Postmark to call the webhook at <em>https://pmc-nanotips-be.azurewebsites.net/webhook/{mailbox}</em></p>
        </div>
    </div>);
}