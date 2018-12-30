$(function () {
    if (('ontouchstart' in window) || window.DocumentTouch && document instanceof DocumentTouch) {
        return;
    }

    $('.navbar-popover').tooltip({
        trigger: 'hover'
    });
});