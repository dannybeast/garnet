import Swiper from "swiper/js/swiper.min";
export default function() {
  // Swiper cards
  var swiperInfo = new Swiper(".js-swiper-info", {
    loop: true,
    noSwiping: true,
    pagination: {
      el: ".js-swiper-info .slider-dots",
      clickable: true,
      dynamicBullets: true,
    },
    breakpoints: {
      0: {
        slidesPerView: 1,
        spaceBetween: 15,
        noSwiping: false,
      },
      768: {
        slidesPerView: 2,
        noSwiping: false,
      },
      992: {
        slidesPerView: 3,
        spaceBetween: 30,
      },
    },
  });
  var swiperInfo = new Swiper(".js-swiper-info-autoplay", {
    loop: true,
    speed: 1000,
    autoplay: {
      delay: 2000,
    },
    pagination: {
      el: ".js-swiper-info-autoplay .slider-dots",
      clickable: true,
      dynamicBullets: true,
    },
    breakpoints: {
      0: {
        slidesPerView: 1,
        spaceBetween: 15,
      },
      768: {
        slidesPerView: 2,
      },
      992: {
        slidesPerView: 3,
        spaceBetween: 30,
      },
    },
  });
  // -

  //
  var swiperOperations = new Swiper(".js-swiper-operations", {
    loop: false,
    speed: 1000,
    autoplay: {
      delay: 2000,
    },
    breakpoints: {
      0: {
        slidesPerView: 3,
      },
      768: {
        slidesPerView: 4,
      },
      992: {
        slidesPerView: 6,
      },
      1280: {
        slidesPerView: 7,
      },
      1368: {
        slidesPerView: 8,
      },
    },
  });
  //
  var swiperExchange = new Swiper(".exchange-slider", {
    loop: false,
    noSwiping: true,
    pagination: {
      el: ".exchange-slider .slider-dots",
      clickable: true,
      dynamicBullets: true,
    },
    breakpoints: {
      0: {
        slidesPerView: 2,
        spaceBetween: 15,
        noSwiping: false,
      },
      768: {
        slidesPerView: 3,
        noSwiping: false,
      },
      992: {
        slidesPerView: 4,
        noSwiping: false,
      },
      1100: {
        slidesPerView: 5,
        spaceBetween: 20,
      },
    },
  });
}
