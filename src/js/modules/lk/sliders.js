import Swiper from "swiper/js/swiper.min";
export default function() {
  var swiperExchange = new Swiper(".js-swiper-exchange", {
    loop: false,
    spaceBetween: 0,
    noSwiping: false,
    // speed:1000,
    // autoplay:
    // {
    //   delay: 2000,
    // },
    breakpoints: {
      0: {
        slidesPerView: "auto",
        freeMode: true,
      },
      1024: {
        slidesPerView: 5,
      },
      1260: {
        slidesPerView: 6,
        noSwiping: true,
      },
    },
  });

  var swiperAccount = new Swiper(".js-swiper-account", {
    loop: false,
    noSwiping: true,
    breakpoints: {
      0: {
        slidesPerView: "auto",
        spaceBetween: 0,
        freeMode: true,
        noSwiping: false,
      },
      992: {
        slidesPerView: 7,
        spaceBetween: 0,
      },
    },
  });

  var swiperLicenses = new Swiper(".js-swiper-licenses", {
    loop: false,
    noSwiping: true,
    spaceBetween: 0,
    pagination: {
      el: ".js-swiper-licenses .slider-dots",
      clickable: true,
      dynamicBullets: true,
    },
    breakpoints: {
      0: {
        slidesPerView: "auto",
        freeMode: true,
        centeredSlides: true,
        noSwiping: false,
      },
      768: {
        slidesPerView: 2.5,
        freeMode: true,
        centeredSlides: false,
        noSwiping: false,
      },

      1024: {
        slidesPerView: 3,
      },
      1280: {
        slidesPerView: 4,
      },
    },
  });

  document.querySelectorAll(".slider-dots").forEach((dots) => {
    let bullets = dots.querySelectorAll(".swiper-pagination-bullet");
    bullets.length === 1 || bullets.length === 0 ? dots.remove() : null;
  });
}
