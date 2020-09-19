import Notice from "../modules/notifications";
import $ from "jquery";
import Modal from "../modules/modals";
const modal = new Modal();
let form = ".js-request-form";

$(form).validate({
  rules: {
    name: {
      required: true,
      minlength: 2,
    },
    phone: {
      required: true,
      minlength: 18,
    },
  },
  messages: {
    name: {
      required: "Введите имя",
      minlength: "Слишком короткое имя",
    },
    phone: {
      required: "Введите телефон",
      minlength: "Слишком короткий номер",
    },
  },
  submitHandler: function(form) {
    let $form = $(form);
    let formData = $form.serializeArray();

    formData.push({ name: "type", value: "bid" });

    $.ajax({
      type: "POST",
      url: "utility/send-email",
      data: formData,
      success: function(response) {
        modal.hideModal();
        Notice.openSuccess("Заявка отправлена!");
        $form.get(0).reset();
      },
    });

    return false;
  },
});
