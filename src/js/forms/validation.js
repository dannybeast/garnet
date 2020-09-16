import $ from "jquery";
import "jquery-validation";

$.validator.setDefaults({
  highlight: function(element) {
    $(element)
      .closest(".field")
      .addClass("field--error");
  },
  ignore: [],
  unhighlight: function(element) {
    $(element)
      .closest(".field")
      .removeClass("field--error");
  },
  errorElement: "p",
  errorClass: "messages",

  errorPlacement: function(error, element) {
    if (
      element.prop("type") === "file" ||
      element.prop("type") === "checkbox" ||
      element.prop("type") === "radio"
    ) {
      error.insertAfter(element.parent());
    } else {
      error.insertAfter(element);
    }
  },
});
