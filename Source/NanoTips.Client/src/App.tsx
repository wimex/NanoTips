import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs"
import styles from './App.module.scss';
import {useEffect, useState} from "react";
import {useReqConversationMutation, useReqConversationsMutation} from "@/redux/api.ts";

function App() {
    const [tab, setTab] = useState<string>('messages');
    const [reqConversations] = useReqConversationsMutation();
    const [reqConversation] = useReqConversationMutation();
    
    useEffect(() => {
        (async () => {
            await reqConversations();
            await reqConversation({ conversationId: 'test-conversation' });
        })();
    }, []);
    
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
                    {tab === 'messages' ? (<div>messages</div>) : (<div>articles</div>)}
                </div>
                <div className={styles.main}>
                    {tab === 'messages' ? (<div>messages</div>) : (<div>articles</div>)}
                </div>
            </div>
        </div>
    )
}

export default App
