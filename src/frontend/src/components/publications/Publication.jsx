import {Box, Divider, Paper, Typography} from "@mui/material";
import ImageSlider from "../ImageSlider.jsx";
import {mediaService} from "../../services/mediaService.js";

const Publication = ({content, medias}) => {
    const imageUrls = medias.map((media) => mediaService.makeImageUrl(media.id));
    return (
        <Paper
            sx={{display: 'flex', flexDirection: 'column'}}
        >
            <Typography variant="h6">
                {content}
            </Typography>
            <Divider />
            <Box>
                {imageUrls.length > 0 && (
                    <ImageSlider images={imageUrls} />
                )}
            </Box>


        </Paper>
    )
}

export default Publication;