export function cleanFields(formClass) {
    document.querySelectorAll(formClass).forEach(function (form) {
        form.reset();
    });
}