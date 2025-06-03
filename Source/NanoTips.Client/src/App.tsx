import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs"
import styles from './App.module.scss';
import {useState} from "react";
import Messaging from "@/components/app/messaging/messaging.tsx";
import Articles from "@/components/app/articles/articles.tsx";

function App() {
    const [tab, setTab] = useState<string>('messages');
    const [slug, setSlug] = useState<string | null>(null);
    
    function onCategorySelected(categoryId: string): void {
        setTab('articles');
        setSlug(categoryId);
    }
    
    return (
        <div className={styles.container}>
            <div className={styles.header}>
                <div className={styles.logo}>
                    <h1>NanoTips</h1>
                </div>
                <div className={styles.menu}>
                    <div className={styles.tabs}>
                        <Tabs defaultValue="messages" value={tab} onValueChange={(v: string) => setTab(v)}>
                            <TabsList>
                                <TabsTrigger value="messages" className="text-lg">Messages</TabsTrigger>
                                <TabsTrigger value="articles" className="text-lg">Articles</TabsTrigger>
                            </TabsList>
                        </Tabs>
                    </div>
                </div>
            </div>
            <div className={styles.content}>
                {tab === 'messages' && (<Messaging onCategorySelected={onCategorySelected} />)}
                {tab === 'articles' && (<Articles selectedArticleSlug={slug} />)}
            </div>
        </div>
    )
}

export default App
