// SVG
var __svg__ = {
	path: './assets/icons/**/*.svg',
	name: 'assets/icons/sprite.svg',
}
__svg__ = {
	filename: '/Front/dist/assets/icons/sprite.svg',
}
require('webpack-svgstore-plugin/src/helpers/svgxhr')(__svg__)

// SCSS
import './assets/scss/main.scss'

// Modules
import hideLoader from './js/modules/loader'
import 'lazyload'
import mobileNavigation from './js/modules/mobileNavigation'
import Modal from './js/modules/modals'
import animations from './js/modules/animations'
import masks from './js/modules/inputmasks'
import sliders from './js/modules/sliders'
import contactsMap from './js/modules/contactsMap'
import { Fancybox } from '@fancyapps/ui'

// Forms
import './js/forms/validation'
import './js/forms/request'
import './js/forms/price'

/**
 * Модальные окна
 */
const initFancybox = () => {
	window.Fancybox = Fancybox
	window.Fancybox.bind("[data-fancybox='dialog']", {
		autoFocus: false,
		type: 'inline',
		hideScrollbar: false,
		groupAttr: false,
		dragToClose: false,
	})

	window.Fancybox.bind('[data-fancybox]', {
		Toolbar: {
			display: false,
		},
		hideScrollbar: false,
	})
}

$(document).ready(function() {
	animations()
	lazyload()
	sliders()
	contactsMap()
	initFancybox()

	// .advantage-item
	let $ad_items = document.querySelectorAll('.advantage-item')

	$ad_items.forEach(($item, index) => {
		if (index === 0) {
			$item.classList.add('active')
		}
	})

	$('.advantage-item').hover(function() {
		$('.advantage-item').removeClass('active')
		$(this).addClass('active')
	})

	mobileNavigation()
	new Modal()
	masks()
	hideLoader()

	$('[data-video-play]').on('click', function() {
		$(this).toggleClass('is-active')
		$('[data-video]')[0].paused
			? $('[data-video]')[0].play()
			: $('[data-video]')[0].pause()
	})
})

