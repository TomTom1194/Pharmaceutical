// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function() {
    // Handle Quote FAB visibility on scroll
    var quoteFab = document.querySelector('.quote-fab');
    if (quoteFab) {
        window.addEventListener('scroll', function() {
            if (window.scrollY > 150) {
                quoteFab.classList.add('visible');
            } else {
                quoteFab.classList.remove('visible');
            }
        });
    }
});
