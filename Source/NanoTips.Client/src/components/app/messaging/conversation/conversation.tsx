import {useGetConversationQuery} from "@/redux/api.ts";
import {ArrowBottomRightIcon, ArrowTopRightIcon} from "@radix-ui/react-icons";

export default function Conversation({ conversationId }: { conversationId: string }) {
    const getConversation = useGetConversationQuery(conversationId);
    
    async function answerWithArticle(category: string) {
    }
    
    return (
        <div>
            <h1 className="text-2xl font-bold mb-4">{getConversation.data?.subject}</h1>
            <div>
                {getConversation.data?.messages?.map((message) => (
                    <div key={message.messageId} className="mb-4 p-4 border rounded">
                        <p className="text-sm text-gray-500">{message.direction === 'incoming' ? <ArrowBottomRightIcon /> : <ArrowTopRightIcon />}</p>
                        <p className="text-sm text-gray-500">{new Date(message.created).toLocaleString()}</p>
                        <p className="font-semibold">{message.sender} to {message.recipient}</p>
                        <p className="mt-2">{message.body}</p>
                        {!message.handled && (
                            message.categorySuggestions && Object.keys(message.categorySuggestions).length > 0 && (
                                <div className="mt-2">
                                    <strong>Answer Suggestions:</strong>
                                    <ul className="list-disc pl-5">
                                        {Object.entries(message.categorySuggestions).map(([id, category]) => (
                                            <li key={id} className="mt-1">
                                                <button
                                                    className="text-blue-500 hover:underline"
                                                    onClick={() => answerWithArticle(category.categoryId)}
                                                >
                                                    {category.exists ? category.title : category.categoryId} ({(category.confidence * 100).toFixed(2)} %)
                                                </button>
                                            </li>
                                        ))}
                                    </ul>
                                </div>)
                        )}
                    </div>
                ))}
            </div>
        </div>
    )
}