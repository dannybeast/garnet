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
import Notice from "./js/modules/notifications";

document.addEventListener("DOMContentLoaded", function() {
  animations();
  masks();
  mobileNavigation();
  lazyload();
  new Modal();
  sliders();

  hideLoader();
});
