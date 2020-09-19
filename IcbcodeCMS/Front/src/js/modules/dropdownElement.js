export default function () {
    const dropdownElement = document.querySelectorAll('.js-dropdown');

    dropdownElement.forEach(element => {
        element.addEventListener('click', function (e) {
            this.classList.toggle('open');
        })
    });

    document.addEventListener('click', e => {
        dropdownElement.forEach(element => {
            if (!element.contains(e.target)) {
                element.classList.remove('open');
            }
        })
    });
}