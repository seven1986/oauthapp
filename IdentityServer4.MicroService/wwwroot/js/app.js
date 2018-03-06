var app = angular.module('app', []);
$(function () {
    var ydSwiper = new Swiper('#youdaoBanner', {
        centeredSlides: true,
        slidesPerView: 'auto',
        spaceBetween: 40,
        initialSlide: 3,
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
    })
});