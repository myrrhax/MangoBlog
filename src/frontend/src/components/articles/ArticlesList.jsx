import {Box, Pagination, Typography} from "@mui/material";
import {articlesStore} from "../../stores/articlesStore.js";
import {observer} from "mobx-react-lite";
import Article from "./Article";

const ArticlesList = ({articles, autoCenter = true}) => {
    return (
        <>
            <Box sx={{ maxWidth: 800, mx: autoCenter ? 'auto' : '0px' }}>
                {articles.length > 0
                    ? (
                        articles.map((article) => (
                            <Article key={article.id} article={article} />
                        ))
                    ) : (
                        <Typography variant={'h6'}>
                            Здесь пока ничего нет
                        </Typography>
                )}
            </Box>
        </>
    )
}

export default ArticlesList;