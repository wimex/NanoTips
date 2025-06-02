import {useGetArticleQuery} from "@/redux/api.ts";
import type {FormEvent} from "react";

export default function Editor({ articleId }: { articleId: string }) {
    const getArticleQuery = useGetArticleQuery(articleId, { skip: articleId === 'create-article' });
    
    function onTitleChanged(e: React.ChangeEvent<HTMLInputElement>) {
        const title = e.currentTarget.value;
        if (!title)
            return;
        
        const slug = title.toLowerCase().replace(/[^a-zA-Z0-9]+/g, '-').replace(/^-|-$/g, '');
        const input = document.querySelector('input[name="slug"]') as HTMLInputElement;
        if (input && input.value !== slug) {
            input.value = slug;
        }
    }
    
    async function onSaveArticle(e: FormEvent<HTMLFormElement>) {
        e?.preventDefault();

        const data = new FormData(e.currentTarget);
        const title = data.get('title');
        const slug = data.get('slug');
        const content = data.get('content');
        if (!title || !slug || !content) {
            alert('Title, slug and content are required');
            return;
        }
    }
    
    return (
        <div>
            <h1>Article editor</h1>
            <form className="mb-4" onSubmit={onSaveArticle}>
                <p>
                    <input type="text" placeholder="Title" name="title" onChange={onTitleChanged} className="border p-2 w-full mb-4" defaultValue={getArticleQuery.data?.title} required/>
                </p>
                <p>
                    <input type="text" placeholder="Slug" name="slug" className="border p-2 w-full mb-4" defaultValue={getArticleQuery.data?.slug} required/>
                </p>
                <p>
                    <textarea placeholder="Content" name="content" className="border p-2 w-full h-64" defaultValue={getArticleQuery.data?.title} required></textarea>
                </p>
                <p>
                    <input type="submit" value="Save" className="bg-blue-500 text-white p-2 rounded cursor-pointer" />
                </p>
            </form>
        </div>
    );
}