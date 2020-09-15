export default function () {
    let $aside = document.querySelector('.aside-navigation');
    if ($aside) {
        let $li = $aside.querySelectorAll('li');

        const toggleActive = li => {
            $li.forEach(li => {
                li.classList.remove('active');
            });
            li.classList.toggle('active');
        };

        $li.forEach(li => {
            li.addEventListener('click', (e) => {
                if (e.target.closest('li').querySelector('ul')) {
                    e.preventDefault();
                    toggleActive(li);
                }
            });
        })
    };


}