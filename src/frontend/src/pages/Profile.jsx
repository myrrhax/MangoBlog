import {useEffect, useRef, useState} from 'react';
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
    Button,
    Tabs,
    Tab
} from '@mui/material';
import EmailIcon from '@mui/icons-material/Email';
import PersonIcon from '@mui/icons-material/Person';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import TelegramIcon from '@mui/icons-material/Telegram';
import { authStore } from '../stores/authStore';
import { mediaService } from '../services/mediaService';
import ProfileGridComponent from '../components/ProfileGridComponent';
import parseDateTime from "../utils/parseDateTime.js";
import useSecretText from "../hooks/useSecretText.jsx";
import NewspaperIcon from '@mui/icons-material/Newspaper';
import { profileStore } from '../stores/profileStore';
import {articlesStore} from "../stores/articlesStore.js";
import ArticlesWithFilters from "../components/articles/ArticlesWithFilters";
import PublicationsList from "../components/publications/PublicationsList";
import {publicationsStore} from "../stores/publicationsStore.js";
import ArticlesList from "../components/articles/ArticlesList.jsx";

const Profile = observer(() => {
    const { userId } = useParams();
    const { isAuthenticated, user } = authStore;
    const [currentTab, setCurrentTab] = useState('articles');
    const [showIntegrationCodeStatus, showCode] = useSecretText(3000);
    const tabNames = {'articles': 'Посты', 'publications': 'Публикации', 'ratedPosts': 'Оценки постов'};

    const tabsContent = {
        'articles': <ArticlesWithFilters isCurrent={profileStore.isCurrentUser} />,
        'publications': <PublicationsList publications={publicationsStore.publications} />,
        'ratedPosts': <ArticlesList articles={articlesStore.articles} />
    }

    const addIntegration = async () => {
        const isSuccess = await profileStore.addIntegration();
        if (isSuccess) {
            await authStore.fetchUser();
            profileStore.setUser(authStore.user, true);
        }
    }

    const isDisabled = (type) => {
        return type === 'publications'
            ? (!profileStore.isCurrentUser
                || authStore.user.integration?.telegram === null
                || !authStore.user.integration?.telegram.isConnected)
            : false;
    }

    useEffect(() => {
        articlesStore.clearFilters();
    }, []);

    useEffect(() => {
        // Основная загрузка профиля и установка фильтра authorId
        const fetchData = async () => {
            if (userId === 'me' || userId === authStore.user?.id) {
                await profileStore.setUser(authStore.user, true);
            } else {
                await profileStore.fetchUser(userId);
            }

            await articlesStore.setAuthorId(userId === 'me' ? authStore.user.id : userId); // Устанавливает authorId в фильтры → триггерит другой useEffect
        };

        fetchData();
    }, [userId, isAuthenticated, user]);

    useEffect(() => {
        const loadData = async () => {
            if (currentTab === 'publications') {
                await publicationsStore.fetchMy();
            } else if (currentTab === 'ratedPosts') {
                await articlesStore.fetchMyRatedArticles();
            }
        }

        loadData();
    }, [currentTab]);

    useEffect(() => {
        if (currentTab !== 'articles') return;
        if (!articlesStore.filters.authorId) return;
        articlesStore.fetchArticles();
    }, [articlesStore.currentPage, articlesStore.filters, currentTab]);


    if (profileStore.isLoading) {
        return (
            <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
                <CircularProgress />
            </Box>
        );
    }

    if (profileStore.loadingError) {
        return (
            <Alert severity="error" sx={{ my: 2 }}>
                {profileStore.loadingError}
            </Alert>
        );
    }

    if (!profileStore.user) {
        return null;
    }

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
                        src={profileStore.user.avatarId ? mediaService.makeImageUrl(profileStore.user.avatarId) : null}
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
                                {profileStore.user.displayedName}
                            </Typography>
                            <Chip
                                label={profileStore.user.role}
                            />
                        </Box>
                        <Box>
                            <Typography
                                variant="body1"
                            >
                                {profileStore.user.firstName} {profileStore.user.lastName}
                            </Typography>
                        </Box>
                    </Box>
                </Box>
                <Divider sx={{my:3}} />
                <Box
                    sx={{display: 'flex', gap: 1}}
                >
                    <CalendarTodayIcon/>
                    <Box>{profileStore.user.birthDate}</Box>
                </Box>

                {profileStore.isCurrentUser && (
                    <Box
                        sx={{ mt: 1, display: 'flex', flexDirection: 'column', gap: 1}}
                    >
                        <Box sx={{gap: 2, display:'flex', alignItems: 'center'}}>
                            <ProfileGridComponent caption={profileStore.user.login} Icon={PersonIcon} />
                            <ProfileGridComponent caption={profileStore.user.email} Icon={EmailIcon} />
                        </Box>
                        <Box sx={{gap: 1, display:'flex', alignItems: 'center'}}>
                            <Typography variant="body1">
                                Дата регистрации:
                            </Typography>
                            <Typography variant="body1">
                                {parseDateTime(profileStore.user.registrationTime)}
                            </Typography>
                        </Box>
                        <Divider sx={{my:3}} />
                        <Typography
                            variant={"h5"}
                        >
                            Интеграции:
                        </Typography>
                        {profileStore.user.integration?.telegram
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
                                            checked={profileStore.user.integration.telegram.isConnected}
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
                                            <Link href={"https://t.me/mango_blog_dev_bot?start=" + profileStore.user.integration.telegram.integrationCode}>
                                                {profileStore.user.integration.telegram.integrationCode}
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
                                            {profileStore.user.integration.telegram.channels.length > 0
                                                ? (
                                                    <List>
                                                        {profileStore.user.integration.telegram.channels.map((channel) => (
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
                                                )
                                                : <Typography variant={'body1'}>Здесь пока ничего нет!</Typography>
                                            }
                                    </Box>
                                </Paper>
                            )
                            : (
                                <Box sx={{display: 'flex', flexDirection: 'column', gap: 2}}>
                                    <Typography variant={"body1"}>
                                        Здесь пока ничего нет. Добавьте новую интеграцию
                                    </Typography>
                                    <Button
                                        onClick={() => addIntegration()}
                                    >
                                        Добавить новую интеграцию
                                    </Button>
                                </Box>
                            )
                        }
                    </Box>
                )}
            </Paper>
            <Divider/>
            {profileStore.isCurrentUser && (
                <Tabs
                    value={currentTab}
                    onChange={(e, value) => setCurrentTab(value)}
                    textColor="secondary"
                >
                    {Object.keys(tabNames).map((key) => (
                        <Tab key={key} value={key} label={tabNames[key]} disabled={isDisabled(key)} />
                    ))}
                </Tabs>
            )}
            <Paper
                sx={{ width: '75%', m: 3, p: 3 }}
                elevation={3}
            >
                <Typography
                    variant={"h6"}>
                    {tabNames[currentTab]}:
                </Typography>
                {tabsContent[currentTab]}
            </Paper>
        </Container>
    );
});

export default Profile; 