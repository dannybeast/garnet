export default function() {
	let nav = document.querySelector('.header__nav')
	let socials = document.querySelector('.header .follow-header')
	let headerButtons = document.querySelector('.header__buttons')
	if (nav) {
		let navCloned = nav.cloneNode(true)
		let socialsCloned = socials.cloneNode(true)
		//let headerButtonsCloned = headerButtons.cloneNode(true);

		document.querySelector('.header__row').insertAdjacentHTML(
			'beforeend',
			`
        <button class="nav-button">
            <div class="hamburger">
                <span class="line"></span>
                <span class="line"></span>
                <span class="line"></span>
            </div>
        </button>`
		)

		let navBtn = document.querySelector('.header .nav-button')
		let mobileMenu = document.createElement('div')
		mobileMenu.classList.add('mobile-menu')
		let menuContainer = document.createElement('div')
		menuContainer.classList.add('container')
		mobileMenu.appendChild(menuContainer)
		//menuContainer.appendChild(headerButtonsCloned);
		menuContainer.appendChild(navCloned)
		menuContainer.appendChild(socialsCloned)

		// Append menu
		document.body.appendChild(mobileMenu)

		navBtn.addEventListener('click', function(e) {
			document.querySelector('.header .hamburger').classList.toggle('is-active')
			mobileMenu.classList.toggle('open')
			document.body.classList.toggle('overflow-transparent')
		})
	}
}

