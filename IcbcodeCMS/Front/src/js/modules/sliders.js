import Swiper from "swiper/js/swiper.min";
export default function() {
  // Swiper cards
  var swiperAdvantages = new Swiper(".js-swiper-advantages", {
    loop: false,
    spaceBetween: 30,
    noSwiping: true,
    breakpoints: {
      0: {
        slidesPerView: 1.2,
        freeMode: true,
        noSwiping: false,
      },
      768: {
        slidesPerView: 2.2,
        freeMode: true,
        noSwiping: false,
      },
      992: {
        slidesPerView: 4,
      },
    },
  });
new Swiper(".js-swiper-news", {
    loop: false,
    spaceBetween: 10,
    noSwiping: true,
    breakpoints: {
      0: {
        slidesPerView: 1.2,
        freeMode: true,
        noSwiping: false,
      },
      768: {
        slidesPerView: 2,
        freeMode: true,
        noSwiping: false,
      },
      1100: {
        slidesPerView: 'auto',
      },
    },
  });
  var swiperApple = new Swiper(".js-swiper-apple", {
    loop: false,
    spaceBetween: 0,
    navigation: {
      nextEl: ".apple-slider .swiper-next",
      prevEl: ".apple-slider .swiper-prev",
    },
    pagination: {
      el: ".apple-slider .slider-dots",
      clickable: true,
      dynamicBullets: true,
    },
    breakpoints: {
      992: {
        slidesPerView: 1,
      },
    },
  });
}
