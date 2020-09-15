import 'sticky-sidebar-v2';
import '../libs/resizeSensor';
if (document.querySelector('.js-sticky-sidebar')) {
    window.sidebar = new StickySidebar('.js-sticky-sidebar', {
        resizeSensor: true,
        topSpacing: 0,
        bottomSpacing: 0,
        innerWrapperSelector: '.sidebar__inner',
        minWidth: 1100
    });
}