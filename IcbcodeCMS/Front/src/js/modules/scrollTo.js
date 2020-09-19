import $ from "jquery";
export function scrollTo(element, scrollBlock, el_offset) {
  var offset = el_offset || 0;

  var destination = $(scrollBlock).offset().top;

  $("body").delegate(element, "click", function(e) {
    e.preventDefault();
    if ($(window).width() > 768) {
      $("html").animate(
        {
          scrollTop: destination - 100,
        },
        100
      );
    } else {
      $(".header .hamburger").removeClass("is-active");
      $(".mobile-menu").removeClass("open");
      $("body").removeClass("overflow-transparent");

      $("html").animate(
        {
          scrollTop: destination - 100,
        },
        0
      );
    }
    return false;
  });
}
