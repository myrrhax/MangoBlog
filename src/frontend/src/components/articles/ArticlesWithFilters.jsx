import ArticlesFilters from "./ArticlesFilters.jsx";
import {profileStore} from "../../stores/profileStore.js";
import ArticlesList from "./ArticlesList.jsx";
import {Box, Pagination} from "@mui/material";
import {articlesStore} from "../../stores/articlesStore.js";
import {observer} from "mobx-react-lite";

const ArticlesWithFilters = observer(({isCurrent}) => {
    const handlePageChange = (event, value) => {
        articlesStore.setCurrentPage(value);
    };

    return (
        <Box sx={{display: 'flex', flexDirection: 'column', gap: 1}}>
            <ArticlesFilters showAddPost={isCurrent} />
            <ArticlesList articles={articlesStore.articles} autoCenter={false} />
            <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
                <Pagination
                    count={articlesStore.totalPages}
                    page={articlesStore.currentPage}
                    onChange={handlePageChange}
                    color="primary"
                />
            </Box>
        </Box>
    )
})

export default ArticlesWithFilters;