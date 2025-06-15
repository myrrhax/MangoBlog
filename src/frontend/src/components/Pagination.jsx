import {Box, Pagination} from "@mui/material";
import {articlesStore} from "../stores/articlesStore.js";

const AppPagination = ({totalPages, currentPage, handlePageChange}) => {
    return (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
            <Pagination
                count={totalPages}
                page={currentPage}
                onChange={handlePageChange}
                color="primary"
            />
        </Box>
    )
}

export default AppPagination;