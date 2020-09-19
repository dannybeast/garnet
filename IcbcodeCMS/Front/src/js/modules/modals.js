export default class Modal {
  constructor() {
    this.triggers = document.querySelectorAll(".js-modal");
    this.close = document.querySelectorAll(".js-close-modal");
    this.modals = document.querySelectorAll(".modal");
    this.modalInners = document.querySelectorAll(".modal-inner");

    this.listeners();
  }

  listeners() {
    window.addEventListener("keydown", this.keyDown);

    this.triggers.forEach((el) => {
      el.addEventListener("click", this.openModal, false);
    });

    this.modals.forEach((el) => {
      el.addEventListener("transitionend", this.revealModal, false);
      el.addEventListener("click", this.backdropClose, false);
    });

    this.close.forEach((el) => {
      el.addEventListener("click", Modal.hide, false);
    });

    this.modalInners.forEach((el) => {
      el.addEventListener("transitionend", this.closeModal, false);
    });
  }

  keyDown(e) {
    if (27 === e.keyCode && document.body.classList.contains("modal-body")) {
      Modal.hide();
    }
  }

  backdropClose(el) {
    if (!el.target.classList.contains("modal-visible")) {
      return;
    }

    let backdrop =
      el.currentTarget.dataset.backdrop !== undefined
        ? el.currentTarget.dataset.backdrop
        : true;

    if (backdrop === true) {
      Modal.hide();
    }
  }

  static hide() {
    let modalOpen = document.querySelector(".modal.modal-visible");
    modalOpen.querySelector(".modal-inner").classList.remove("modal-reveal");
    document
      .querySelector(".modal-body")
      .addEventListener("transitionend", Modal.modalBody, false);
    document.body.classList.add("modal-fadeOut");
  }
  hideModal() {
    Modal.hide();
  }
  closeModal(el) {
    if (
      "opacity" === el.propertyName &&
      !el.target.classList.contains("modal-reveal")
    ) {
      document.querySelectorAll(".modal").forEach((item) => {
        console.log("q");
        item.classList.remove("modal-visible");
      });
    }
  }

  openModal(el) {
    // Если это не событие а например строка для работы в коде
    if (!el.currentTarget) {
      var modalID = el;
    } else {
      // Если это событие ищем в дата атрибуте название окна
      var modalID = el.currentTarget.dataset.modal;
    }
    let modal = document.getElementById(modalID);

    document.body.classList.add("modal-body");
    modal.classList.add("modal-visible");

    var targetDiv = $(".chat__messages");
    if (targetDiv.length) {
      targetDiv.scrollTop(
        $(".chat__messages-wrapper").prop("scrollHeight") + 60
      );
    }
  }

  revealModal(el) {
    if (
      "opacity" === el.propertyName &&
      el.target.classList.contains("modal-visible")
    ) {
      el.target.querySelector(".modal-inner").classList.add("modal-reveal");
    }
  }

  static modalBody(el) {
    if (
      "opacity" === el.propertyName &&
      el.target.classList.contains("modal") &&
      !el.target.classList.contains("modal-visible")
    ) {
      document.body.classList.remove("modal-body", "modal-fadeOut");
    }
  }
}
