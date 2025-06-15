import { Box, Typography }  from "@mui/material";

const ProfileGridComponent = ({caption, Icon, otherBoxValues}) => {
    return (
        <Box
            sx={{display: 'flex', flexDirection: 'row', gap: 1, alignItems: 'center'}}
            {...otherBoxValues}
        >
            <Icon/>
            <Typography variant="body1">
                {caption}
            </Typography>
        </Box>
    );
}

export default ProfileGridComponent;