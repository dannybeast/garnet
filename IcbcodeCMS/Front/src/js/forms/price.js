import Notice from "../modules/notifications";
import { Email, Data } from "./smtp";
import $ from "jquery";
let form = ".js-price-form";

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

    Email.send({
      SecureToken: Data.SecureToken,
      To: Data.to,
      From: Data.from,
      Subject: "Заявка на получение прайса",
      Body: `
            <p>Имя: <strong>${formData[0].value}</strong></p>
            <p>E-mail: <strong>${formData[1].value}</strong></p>
            <p>Номер телефона: <strong>${formData[2].value}</strong></p>
            
            `,
    }).then((message) => {
      Notice.openSuccess("Заявка отправлена!");
      $form.get(0).reset();
    });

    return false;
  },
});