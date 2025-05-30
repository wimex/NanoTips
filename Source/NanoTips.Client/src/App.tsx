import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs"
import styles from './App.module.scss';
import {useState} from "react";
import Conversations from "@/components/app/conversations/conversations.tsx";
import Conversation from "@/components/app/conversation/conversation.tsx";

function App() {
    const [conversationId, setConversationId] = useState<string | null>(null);
    const [tab, setTab] = useState<string>('messages');
    
    return (
        <div className={styles.container}>
            <div className={styles.header}>
                <div className={styles.logo}>
                    <h1>NanoTips</h1>
                </div>
                <div className={styles.menu}>
                    <div className={styles.tabs}>
                        <Tabs defaultValue="messages" onValueChange={(v: string) => setTab(v)}>
                            <TabsList>
                                <TabsTrigger value="messages" className="text-lg">Messages</TabsTrigger>
                                <TabsTrigger value="articles" className="text-lg">Articles</TabsTrigger>
                            </TabsList>
                        </Tabs>
                    </div>
                </div>
            </div>
            <div className={styles.content}>
                <div className={styles.sidebar}>
                    {tab === 'messages' ? (<Conversations onConversationIdChanged={setConversationId} />) : (<div>articles</div>)}
                </div>
                <div className={styles.main}>
                    {tab === 'messages' ? (<Conversation conversationId={conversationId}></Conversation>) : (<div>articles</div>)}
                </div>
            </div>
        </div>
    )
}

export default App
