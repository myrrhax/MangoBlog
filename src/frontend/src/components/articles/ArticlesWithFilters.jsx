import ArticlesFilters from "./ArticlesFilters.jsx";
import {profileStore} from "../../stores/profileStore.js";
import ArticlesList from "./ArticlesList.jsx";
import {Box} from "@mui/material";

const ArticlesWithFilters = ({isCurrent}) => {
    return (
        <Box sx={{display: 'flex', flexDirection: 'column', gap: 1}}>
            <ArticlesFilters showAddPost={isCurrent} />
            <ArticlesList autoCenter={false} />
        </Box>
    )
}

export default ArticlesWithFilters;