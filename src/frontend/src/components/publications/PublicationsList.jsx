import Publication from "./Publication.jsx";
import {Box} from "@mui/material";

const PublicationsList = ({publications}) => {
    return (
        <Box sx={{display: 'flex', flexDirection: 'column', gap: 2}}>
            {publications.map((publication) => (
                <Publication key={publication.id} {...publication} />
            ))}
        </Box>

    )
}

export default PublicationsList;