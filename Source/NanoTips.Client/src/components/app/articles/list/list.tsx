import {useGetArticlesQuery} from "@/redux/api.ts";

export default function List({onArticleSelected}: {onArticleSelected: (articleId: string) => void}) {
    const createArticle = 'create-article';
    const getArticles = useGetArticlesQuery(undefined, {refetchOnMountOrArgChange: true});
    
    return (
        <div>
            {getArticles.data?.map((article) => (
                <div onClick={() => onArticleSelected(article.articleId)} key={article.articleId} className="p-2 border-b border-gray-200 hover:bg-gray-100 cursor-pointer">
                    <h3 className="text-sm font-semibold">{article.title}</h3>
                    <p className="text-xs text-gray-500">{article.slug}</p>
                </div>
            ))}

            <div onClick={() => onArticleSelected(createArticle)} key={createArticle} className="p-2 border-b border-gray-200 hover:bg-gray-100 cursor-pointer">
                <h3 className="text-sm font-semibold">Create New Article</h3>
            </div>
        </div>
    );
}