import { useEditArticleMutation, useGetArticleQuery} from "@/redux/api.ts";
import {type FormEvent, useEffect, useState} from "react";

export default function Editor({ articleId, onArticleSelected }: { articleId: string, onArticleSelected: (articleId: string | null) => void }) {
    const [editArticleMutation] = useEditArticleMutation();
    const getArticleQuery = useGetArticleQuery(articleId, { skip: articleId === 'create-article' });
    
    const [title, setTitle] = useState('');
    const [slug, setSlug] = useState('');
    const [content, setContent] = useState('');
    
    useEffect(() => {
        setTitle('');
        setSlug('');
        setContent('');
    }, [articleId]);
    
    useEffect(() => {
        if (getArticleQuery.isLoading || !getArticleQuery.data || articleId === 'create-article')
            return;
        if(!getArticleQuery.data.title || !getArticleQuery.data.slug || !getArticleQuery.data.content)
            return;
        
        setTitle(getArticleQuery.data.title);
        setSlug(getArticleQuery.data.slug);
        setContent(getArticleQuery.data.content);
    }, [getArticleQuery.isLoading, getArticleQuery.data]);
    
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
        
        const article = {
            articleId: articleId === 'create-article' ? undefined : articleId,
            title: title.toString(),
            slug: slug.toString(),
            content: content.toString(),
        }
        
        editArticleMutation(article);
        onArticleSelected(null); // Clear the editor after saving
        e.currentTarget.reset(); // Reset the form fields
    }
    
    console.log('Editor component rendered', title, slug, content, articleId);
    
    return (
        <div>
            <h1>Article editor</h1>
            <p className="text-sm text-gray-500 mb-4">Editing article: {articleId === 'create-article' ? 'New Article' : getArticleQuery.data?.title}</p>
            <form className="mb-4" onSubmit={onSaveArticle}>
                <p>
                    <input type="text" placeholder="Title" name="title" className="border p-2 w-full mb-4" value={title} onChange={(v) => { onTitleChanged(v); setTitle(v.target.value)}} required/>
                </p>
                <p>
                    <input type="text" placeholder="Slug" name="slug" className="border p-2 w-full mb-4" value={slug} onChange={v => setSlug(v.target.value)} required/>
                </p>
                <p>
                    <textarea placeholder="Content" name="content" className="border p-2 w-full h-64" value={content} onChange={v => setContent(v.target.value)} required></textarea>
                </p>
                <p>
                    <input type="submit" value="Save" className="bg-blue-500 text-white p-2 rounded cursor-pointer" />
                </p>
            </form>
        </div>
    );
}