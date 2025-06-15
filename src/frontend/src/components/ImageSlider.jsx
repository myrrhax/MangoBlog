import {Box, Card, CardMedia} from "@mui/material";
import Slider from 'react-slick';
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

const ImageSlider = ({images}) => {
    const settings = {
        dots: true,
        infinite: true,
        speed: 500,
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: true,
        autoplay: true,
        autoplaySpeed: 3000,
    };

    return (
        <Box sx={{ maxWidth: 800, mx: 'auto', mt: 4, display: 'flex', flexDirection: 'row' }}>
            <Slider {...settings}>
                {images.map((img, index) => (
                    <Card key={index} sx={{ borderRadius: 2, overflow: 'hidden' }}>
                        <CardMedia
                            component="img"
                            image={img}
                            alt={`Slide ${index + 1}`}
                            sx={{ height: 300, objectFit: 'cover' }}
                        />
                    </Card>
                ))}
            </Slider>
        </Box>
    )
}

export default ImageSlider;