import styles from './messaging.module.scss';
import Conversation from "@/components/app/messaging/conversation/conversation.tsx";
import Conversations from "./conversations/conversations";
import {useState} from "react";

export default function Messaging({ onCategorySelected }: { onCategorySelected: (categoryId: string) => void }) {
    const [conversationId, setConversationId] = useState<string | null>(null);
    
    return (
        <div className={styles.content}>
            <div className={styles.sidebar}>
                <Conversations onConversationIdChanged={setConversationId}/>
            </div>
            <div className={styles.main}>
                {conversationId && <Conversation onCategorySelected={onCategorySelected} conversationId={conversationId}></Conversation>}
            </div>
        </div>
    );
}