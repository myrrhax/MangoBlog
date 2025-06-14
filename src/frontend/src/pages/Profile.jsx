import React, { useEffect, useState } from 'react';
import { observer } from 'mobx-react-lite';
import { useParams } from 'react-router-dom';
import {
    Box,
    Typography,
    Avatar,
    Divider,
    Chip,
    CircularProgress,
    Alert,
    Link,
    Container,
    Checkbox,
    Paper,
    List,
    ListItem,
} from '@mui/material';
import EmailIcon from '@mui/icons-material/Email';
import PersonIcon from '@mui/icons-material/Person';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import TelegramIcon from '@mui/icons-material/Telegram';
import { authStore } from '../stores/authStore';
import { mediaService } from '../services/mediaService';
import api from '../services/api';
import ProfileGridComponent from '../components/ProfileGridComponent';
import parseDateTime from "../utils/parseDateTime.js";
import useSecretText from "../hooks/useSecretText.jsx";
import NewspaperIcon from '@mui/icons-material/Newspaper';

const Profile = observer(() => {
    const { userId } = useParams();
    const [isMyProfile, setIsMyProfile] = useState(false);
    const [profile, setProfile] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const { isAuthenticated, user } = authStore;
    const [showIntegrationCodeStatus, showCode] = useSecretText(3000);

    useEffect(() => {
        const fetchProfile = async () => {
            try {
                setIsLoading(true);
                setError(null);
                
                if (userId === 'me' || (isAuthenticated && userId === user?.id)) {
                    setIsMyProfile(true);
                    setProfile(user);
                } else {
                    setIsMyProfile(false);
                    const response = await api.get(`/users/${userId}`);
                    setProfile(response.data);
                    console.log(response.data);
                }
            } catch (error) {
                setError(error.response?.data?.message || 'Failed to fetch profile');
            } finally {
                setIsLoading(false);
            }
        };

        fetchProfile();
    }, [userId, isAuthenticated, user]);

    if (isLoading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Alert severity="error" sx={{ my: 2 }}>
                {error}
            </Alert>
        );
    }

    if (!profile) return null;

    return (
        <Container
            sx={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center' }}
        >
            <Paper
                sx={{ width: '75%', m: 3, p: 3 }}
                elevation={3}
            >
                <Box
                    sx={{ display: 'flex', flexDirection: 'row', alignItems: 'center', gap: 2 }}
                >
                    <Avatar
                        src={profile.avatarId ? mediaService.makeImageUrl(profile.avatarId) : null}
                        sx={{width: 64, height: 64}}
                    />
                    <Box
                        sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}
                    >
                        <Box
                            sx={{ display: 'flex', flexDirection: 'row', gap: 1 }}
                        >
                            <Typography
                                variant="h5"
                            >
                                {profile.displayedName}
                            </Typography>
                            <Chip
                                label={profile.role}
                            />
                        </Box>
                        <Box>
                            <Typography
                                variant="body1"
                            >
                                {profile.firstName} {profile.lastName}
                            </Typography>
                        </Box>
                    </Box>
                </Box>
                <Divider sx={{my:3}} />
                <Box
                    sx={{display: 'flex', gap: 1}}
                >
                    <CalendarTodayIcon/>
                    <Box>{profile.birthDate}</Box>
                </Box>

                {isMyProfile && (
                    <Box
                        sx={{ mt: 1, display: 'flex', flexDirection: 'column', gap: 1}}
                    >
                        <Box sx={{gap: 2, display:'flex', alignItems: 'center'}}>
                            <ProfileGridComponent caption={profile.login} Icon={PersonIcon} />
                            <ProfileGridComponent caption={profile.email} Icon={EmailIcon} />
                        </Box>
                        <Box sx={{gap: 1, display:'flex', alignItems: 'center'}}>
                            <Typography variant="body1">
                                Дата регистрации:
                            </Typography>
                            <Typography variant="body1">
                                {parseDateTime(profile.registrationTime)}
                            </Typography>
                        </Box>
                        <Divider sx={{my:3}} />
                        <Typography
                            variant={"h5"}
                        >
                            Интеграции:
                        </Typography>
                        {profile.integration?.telegram
                            ? (
                                <Paper sx={{display: 'flex', flexDirection: 'column', gap: 1, p: 2}}>
                                    <Box sx={{gap: 1, display:'flex', alignItems: 'center'}}>
                                        <Typography variant="h5" sx={{color: 'blue'}}>Telegram</Typography>
                                        <TelegramIcon sx={{width: '24px', height: '24px'}} />
                                    </Box>
                                    <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
                                        <Typography variant="body1">
                                            Статус:
                                        </Typography>
                                        <Checkbox
                                            checked={profile.integration.telegram.isConnected}
                                            readOnly
                                            sx={{ width: '16px', height: '16px' }}
                                        />
                                    </Box>
                                    <Box sx={{ display: 'flex', gap: 1 }}>
                                        <Typography variant="body1">
                                            Ваш код интеграции (никому его не сообщайте):
                                        </Typography>
                                        {showIntegrationCodeStatus
                                        ? (
                                            <Link href={"https://t.me/mango_blog_dev_bot?start=" + profile.integration.telegram.integrationCode}>
                                                {profile.integration.telegram.integrationCode}
                                            </Link>
                                        )
                                        : (
                                            <Typography
                                                sx={{cursor: 'pointer'}}
                                                color="textSecondary"
                                                onClick={() => showCode()}>
                                                Показать код
                                            </Typography>
                                        )}
                                    </Box>
                                    <Box sx={{display: 'flex', gap: 1, flexDirection: 'column'}}>
                                        <Typography variant={"h6"}>
                                            Подключенные каналы:
                                        </Typography>
                                        <List>
                                            {profile.integration.telegram.channels.map((channel) => (
                                                <ListItem key={channel.channelId}
                                                    sx={{display: 'flex', gap: 1}}
                                                >
                                                    <NewspaperIcon sx={{width: '24px', height: '24px'}}/>
                                                    <Typography variant={"body1"} sx={{color: 'blue', cursor: 'pointer'}}>
                                                        {channel.channelName}
                                                    </Typography>
                                                </ListItem>
                                            ))}
                                        </List>
                                    </Box>
                                </Paper>
                            )
                            : (
                                <Box>
                                    <Typography variant={"body1"}>
                                        Здесь пока ничего нет. Добавьте новую интеграцию
                                    </Typography>
                                </Box>
                            )
                        }
                    </Box>
                )}
            </Paper>
        </Container>
    );
});

export default Profile; 