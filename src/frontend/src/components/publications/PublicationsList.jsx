import Publication from "./Publication.jsx";
import {Box, Button} from "@mui/material";
import CreatePublication from "./CreatePublication";
import {publicationsStore} from "../../stores/publicationsStore";
import {useState} from "react";
import AddIcon from '@mui/icons-material/Add';

const PublicationsList = ({publications}) => {
    const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);

    const handlePublicationCreated = () => {
        publicationsStore.fetchMy();
    };

    return (
        <Box sx={{display: 'flex', flexDirection: 'column', gap: 2}}>
            <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
                <Button
                    variant="contained"
                    startIcon={<AddIcon />}
                    onClick={() => setIsCreateModalOpen(true)}
                >
                    Create Publication
                </Button>
            </Box>

            <CreatePublication 
                open={isCreateModalOpen}
                onClose={() => setIsCreateModalOpen(false)}
                onSuccess={handlePublicationCreated}
            />

            {publications.map((publication) => (
                <Publication key={publication.id} {...publication} />
            ))}
        </Box>
    )
}

export default PublicationsList;