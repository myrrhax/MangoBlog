import {articlesStore} from "../../stores/articlesStore.js";
import {observer} from "mobx-react-lite";
import {
    Box,
    Button,
    FormControl,
    Grid,
    InputLabel,
    MenuItem,
    Select,
    TextField
} from "@mui/material";
import {useNavigate} from "react-router-dom";
import AddIcon from '@mui/icons-material/Add';

const ArticlesFilters = observer(({showAddPost = true}) => {
    const navigate = useNavigate();

    const handleQueryChange = (event) => {
        articlesStore.setFilters({ query: event.target.value });
    };

    const handleSortByDateChange = (event) => {
        articlesStore.setFilters({ sortByDate: event.target.value });
    };

    const handleSortByPopularityChange = (event) => {
        articlesStore.setFilters({ sortByPopularity: event.target.value });
    };

    return (
        <Box sx={{ mb: 4 }}>
            <Grid container spacing={2} alignItems="center">
                <Grid item xs={12} md={3}>
                    <TextField
                        fullWidth
                        label="Search"
                        value={articlesStore.filters.query}
                        onChange={handleQueryChange}
                    />
                </Grid>
                <Grid item xs={12} md={3}>
                    <FormControl fullWidth>
                        <InputLabel>Sort by Date</InputLabel>
                        <Select
                            value={articlesStore.filters.sortByDate}
                            label="Sort by Date"
                            onChange={handleSortByDateChange}
                        >
                            <MenuItem value="none">None</MenuItem>
                            <MenuItem value="asc">Oldest First</MenuItem>
                            <MenuItem value="desc">Newest First</MenuItem>
                        </Select>
                    </FormControl>
                </Grid>
                <Grid item xs={12} md={3}>
                    <FormControl fullWidth>
                        <InputLabel>Sort by Popularity</InputLabel>
                        <Select
                            value={articlesStore.filters.sortByPopularity}
                            label="Sort by Popularity"
                            onChange={handleSortByPopularityChange}
                        >
                            <MenuItem value="none">None</MenuItem>
                            <MenuItem value="asc">Least Popular</MenuItem>
                            <MenuItem value="desc">Most Popular</MenuItem>
                        </Select>
                    </FormControl>
                </Grid>
                {showAddPost && (
                    <Grid item xs={12} md={3} sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                        <Button
                            variant="contained"
                            color="success"
                            startIcon={<AddIcon />}
                            onClick={() => navigate('/article/new')}
                        >
                            Add Post
                        </Button>
                    </Grid>
                )}
            </Grid>
        </Box>
    );
})

export default ArticlesFilters;