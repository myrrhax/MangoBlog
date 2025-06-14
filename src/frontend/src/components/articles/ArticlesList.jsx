import {Box, Pagination} from "@mui/material";
import {articlesStore} from "../../stores/articlesStore.js";
import React, {useEffect} from "react";
import {observer} from "mobx-react-lite";
import Article from "./Article";

const ArticlesList = observer(({autoCenter = true}) => {
    const handlePageChange = (event, value) => {
        articlesStore.setCurrentPage(value);
    };

    return (
        <>
            <Box sx={{ maxWidth: 800, mx: autoCenter ? 'auto' : '0px' }}>
                {articlesStore.articles.map((article) => (
                    <Article article={article} />
                ))}
            </Box>

            <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
                <Pagination
                    count={articlesStore.totalPages}
                    page={articlesStore.currentPage}
                    onChange={handlePageChange}
                    color="primary"
                />
            </Box>
        </>
    )
})

export default ArticlesList;