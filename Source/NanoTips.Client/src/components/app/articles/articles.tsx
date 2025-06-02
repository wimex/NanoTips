import styles from "@/components/app/messaging/messaging.module.scss";
import {useState} from "react";
import List from "@/components/app/articles/list/list.tsx";
import Editor from "@/components/app/articles/editor/editor.tsx";

export default function Articles() {
    const [articleId, setArticleId] = useState<string | null>(null);
    
    return (
        <div className={styles.content}>
            <div className={styles.sidebar}>
                <List onArticleSelected={setArticleId} />
            </div>
            <div className={styles.main}>
                {articleId && <Editor articleId={articleId} />}
            </div>
        </div>
    );
}