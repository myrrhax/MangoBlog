import {Box, Card, CardMedia, IconButton} from "@mui/material";
import Slider from 'react-slick';
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";
import ArrowBackIosNewIcon from '@mui/icons-material/ArrowBackIosNew';
import ArrowForwardIosIcon from '@mui/icons-material/ArrowForwardIos';

const NextArrow = (props) => {
    const { onClick } = props;
    return (
        <IconButton
            onClick={onClick}
            sx={{
                position: 'absolute',
                right: 10,
                top: '50%',
                transform: 'translateY(-50%)',
                zIndex: 1,
                bgcolor: 'rgba(255, 255, 255, 0.8)',
                '&:hover': {
                    bgcolor: 'rgba(255, 255, 255, 0.9)',
                },
            }}
        >
            <ArrowForwardIosIcon />
        </IconButton>
    );
};

const PrevArrow = (props) => {
    const { onClick } = props;
    return (
        <IconButton
            onClick={onClick}
            sx={{
                position: 'absolute',
                left: 10,
                top: '50%',
                transform: 'translateY(-50%)',
                zIndex: 1,
                bgcolor: 'rgba(255, 255, 255, 0.8)',
                '&:hover': {
                    bgcolor: 'rgba(255, 255, 255, 0.9)',
                },
            }}
        >
            <ArrowBackIosNewIcon />
        </IconButton>
    );
};

const ImageSlider = ({images}) => {
    const settings = {
        dots: true,
        infinite: true,
        speed: 500,
        slidesToShow: 1,
        slidesToScroll: 1,
        nextArrow: <NextArrow />,
        prevArrow: <PrevArrow />,
        autoplay: true,
        autoplaySpeed: 5000,
        pauseOnHover: true,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                }
            },
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false,
                }
            }
        ]
    };

    return (
        <Box sx={{ 
            width: '100%', 
            maxWidth: 800, 
            mx: 'auto', 
            my: 2,
            '& .slick-dots': {
                bottom: -35,
            },
            '& .slick-dots li button:before': {
                fontSize: '12px',
            },
            '& .slick-dots li.slick-active button:before': {
                color: 'primary.main',
            }
        }}>
            <Slider {...settings}>
                {images.map((img, index) => (
                    <Card key={index} sx={{ 
                        borderRadius: 2, 
                        overflow: 'hidden',
                        mx: 1,
                        boxShadow: 2
                    }}>
                        <CardMedia
                            component="img"
                            image={img}
                            alt={`Slide ${index + 1}`}
                            sx={{ 
                                height: { xs: 200, sm: 300, md: 400 },
                                objectFit: 'contain',
                                bgcolor: 'grey.100'
                            }}
                        />
                    </Card>
                ))}
            </Slider>
        </Box>
    );
};

export default ImageSlider;