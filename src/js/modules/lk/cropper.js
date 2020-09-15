import $ from "jquery";
import "croppie/croppie";

$.ajaxSetup({
  headers: {
    "X-CSRF-TOKEN": $('meta[name="csrf-token"]').attr("content"),
  },
});

var $uploadCrop = $("#upload-demo").croppie({
  enableExif: true,
  setZoom: 2,
  viewport: {
    width: 200,
    height: 200,
    type: "circle",
  },
});

$("#upload").on("change", function() {
  var reader = new FileReader();
  reader.onload = function(e) {
    $uploadCrop
      .croppie("bind", {
        url: e.target.result,
      })
      .then(function() {
        $("#upload-demo").removeClass("hide");
        $(".avatar-upload__image >img").hide();
        $(".js-remove-photo").hide();
        $(".upload-result").show();
      });
  };
  reader.readAsDataURL(this.files[0]);
});

$(".upload-result").hide();

$(".upload-result").on("click", function(e) {
  e.preventDefault();
  $uploadCrop
    .croppie("result", {
      type: "base64",
      size: "viewport",
    })
    .then(function(resp) {
      $.ajax({
        url: "/image-crop",
        type: "POST",
        data: { image: resp },
        success: function(data) {
          $(".upload-result").hide();
          $("#upload-demo").addClass("hide");
          $(".js-remove-photo").show();
          $(".avatar-upload__image >img").show();
          $(".js-save-crop-image").attr("src", resp);
        },
      });
    });
});
