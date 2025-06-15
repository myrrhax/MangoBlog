import {Box, Card, Divider, List, ListItem, Paper, Typography, Checkbox} from "@mui/material";
import ImageSlider from "../ImageSlider.jsx";
import {mediaService} from "../../services/mediaService.js";
import CalendarTodayIcon from "@mui/icons-material/CalendarToday";
import parseDateTime from "../../utils/parseDateTime.js";
import TelegramIcon from "@mui/icons-material/Telegram";

const Publication = ({content, medias, creationDate, integrations}) => {
    const imageUrls = medias.map((media) => mediaService.makeImageUrl(media.id));
    const integrationTypeIcons = {
        'Telegram': <TelegramIcon sx={{width: 32, height: 32}} />,
    }
    return (
        <Paper
            sx={{display: 'flex', flexDirection: 'column', my: 2}}
            elevation={3}
        >
            <Box>
                {imageUrls.length > 0 && (
                    <ImageSlider images={imageUrls} />
                )}
            </Box>
            <Box sx={{p: 2}}>
                <Typography variant="h6">
                    {content}
                </Typography>
                <Divider sx={{my: 2}} />
                <Box sx={{display: 'flex', alignItems: 'center', gap: 1}}>
                    <CalendarTodayIcon />
                    <Typography variant="body1">
                        {parseDateTime(creationDate)}
                    </Typography>
                </Box>
                <Card sx={{my: 2, p: 2}}>
                    {integrations.map((integration) => (
                        <Box sx={{display: 'flex', flexDirection: 'column', justifyContent: 'flex-start'}}>
                            <Box key={integration.integrationType} sx={{display: 'flex', alignItems: 'center', gap: 1}}>
                                {integrationTypeIcons[integration.integrationType]}
                                <Typography sx={{fontWeight: 'bold', fontSize: 24}}>
                                    {integration.integrationType}
                                </Typography>
                            </Box>
                            <Divider sx={{my:2}}/>
                            <List>
                                {integration.roomStatuses.map(roomStatus => (
                                    <ListItem key={roomStatus.roomId} sx={{display: 'flex', flexDirection: 'row', alignItems: 'center', gap: 3}}>
                                        <Typography sx={{fontWeight: 'bold', fontSize: 18}}>
                                            {roomStatus.channelName}
                                        </Typography>
                                        <Box sx={{display: 'flex', alignItems: 'center'}}>
                                            <Typography>
                                                Статус публикации:
                                            </Typography>
                                            <Checkbox checked={roomStatus.isPublished} readOnly />
                                        </Box>

                                    </ListItem>
                                ))}
                            </List>
                        </Box>
                    ))}
                </Card>
            </Box>

        </Paper>
    )
}

export default Publication;