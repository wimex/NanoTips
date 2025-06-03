import styles from "@/components/app/messaging/messaging.module.scss";
import {useEffect, useState} from "react";
import List from "@/components/app/articles/list/list.tsx";
import Editor from "@/components/app/articles/editor/editor.tsx";

export default function Articles({ selectedArticleSlug }: { selectedArticleSlug: string | null }) {
    const [articleId, setArticleId] = useState<string | null>(null);
    const [createFromSlug, setCreateFromSlug] = useState<string | null>(null);
    
    useEffect(() => {
        if (selectedArticleSlug) {
            setCreateFromSlug(selectedArticleSlug);
            setArticleId('create-article');
        }
    }, [selectedArticleSlug]);
    
    return (
        <div className={styles.content}>
            <div className={styles.sidebar}>
                <List onArticleSelected={setArticleId} />
            </div>
            <div className={styles.main}>
                {articleId && <Editor onArticleSelected={setArticleId} articleId={articleId} createFromSlug={createFromSlug} />}
            </div>
        </div>
    );
}