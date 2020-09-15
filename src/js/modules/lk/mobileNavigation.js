export default function () {

    let headerNav = document.querySelector('.header-lk__nav');
    let nav = document.querySelector('.navigation-icons');
    let courseModules = document.querySelectorAll('.header-lk .course-module');

    if (headerNav) {
        let headerNavCloned = headerNav.cloneNode(true);
        let navCloned = nav.cloneNode(true);


        var $header = document.querySelector('.header-lk');
        let $lkMessage = document.querySelector('.lk__message');
        let headerHeight;
        let lkMessageHeight = 0;
        if($header) {
           var rect = $header.getBoundingClientRect();
           headerHeight = rect.height;
        }
        if($lkMessage){
            var rectMessage = $lkMessage.getBoundingClientRect();
            lkMessageHeight = rectMessage.height;
        }
        
        document.querySelector('.header-lk .container').insertAdjacentHTML('beforeend', `
        <button class="nav-button">
            <div class="hamburger">
                <span class="line"></span>
                <span class="line"></span>
                <span class="line"></span>
            </div>
        </button>`);

        let navBtn = document.querySelector('.header-lk .nav-button');
        
        let mobileMenu = document.createElement('div')
        mobileMenu.classList.add('mobile-menu');
        let menuContainer = document.createElement('div')
        menuContainer.classList.add('container');
        mobileMenu.appendChild(menuContainer);


       
        courseModules.forEach(module =>{
            let moduleCloned = module.cloneNode(true);
            menuContainer.appendChild(moduleCloned);
            
        }) 
        
        menuContainer.appendChild(navCloned);
        menuContainer.appendChild(headerNavCloned);
    

        // Append menu
        document.body.appendChild(mobileMenu);


        mobileMenu.style.top = `${headerHeight+lkMessageHeight}px`;
        mobileMenu.style.height = `calc(100% - ${headerHeight+lkMessageHeight}px)`;

        navBtn.addEventListener('click', function (e) {
            document.querySelector('.header-lk .hamburger').classList.toggle('is-active');
            mobileMenu.classList.toggle('open');
            document.body.classList.toggle('overflow-transparent');
        });

    }


}