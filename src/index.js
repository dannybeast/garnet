// SVG
var __svg__ = {
  path: "./assets/icons/**/*.svg",
  name: "assets/icons/sprite.svg",
};
__svg__ = {
  filename: "/assets/icons/sprite.svg",
};
require("webpack-svgstore-plugin/src/helpers/svgxhr")(__svg__);

// SCSS
import "./assets/scss/main.scss";

// Modules
import hideLoader from "./js/modules/loader";
import "lazyload";
import mobileNavigation from "./js/modules/mobileNavigation";
import Modal from "./js/modules/modals";
import animations from "./js/modules/animations";
import masks from "./js/modules/inputmasks";
import sliders from "./js/modules/sliders";
import { scrollTo } from "./js/modules/scrollTo";
import contactsMap from "./js/modules/contactsMap";

// Forms
// import "./js/forms/validation";
// import "./js/forms/request";
// import "./js/forms/price";

$(document).ready(function() {
  animations();
  lazyload();
  sliders();
  contactsMap();

  // .advantage-item
  let $ad_items = document.querySelectorAll(".advantage-item");

  $ad_items.forEach(($item, index) => {
    if (index === 0) {
      $item.classList.add("active");
    }
  });

  $(".advantage-item").hover(function() {
    $(".advantage-item").removeClass("active");
    $(this).addClass("active");
  });

  scrollTo(".js-scrollTo-price", ".get-price", "0");
  scrollTo(".js-scrollTo-products", ".products", "0");
  mobileNavigation();
  new Modal();
  masks();
  hideLoader();
});
