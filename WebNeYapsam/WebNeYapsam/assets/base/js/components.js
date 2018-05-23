"use strict";
/**
Core layout handlers and component wrappers
**/

// BEGIN: Layout Brand
var LayoutBrand = function() {

    return {
        //main function to initiate the module
        init: function() {
            $('body').on('click', '.c-hor-nav-toggler', function() {
                var target = $(this).data('target');
                $(target).toggleClass("c-shown");
            });
        }

    };
}();
// END

// BEGIN: Layout Header
var LayoutHeader = function() {

    var _handleHeaderOnScroll = function() {
        if ($(window).scrollTop() > 60) {
            $("body").addClass("c-page-on-scroll");
        } else {
            $("body").removeClass("c-page-on-scroll");
        }
    }

    return {
        //main function to initiate the module
        init: function() {
            if ($('body').hasClass('c-layout-header-fixed-non-minimized')) {
                return; 
            }

            _handleHeaderOnScroll();

            $(window).scroll(function() {
                _handleHeaderOnScroll();
            });
        }
    };
}();
// END

// BEGIN: Layout Mega Menu
var LayoutMegaMenu = function() {

    return {
        //main function to initiate the module
        init: function() {
            $('.c-mega-menu').on('click', 'a.dropdown-toggle, .dropdown-submenu > a', function(e) {
                if (App.getViewPort().width < App.getBreakpoint('md')) {
                    e.preventDefault();
                    $(this).parent("li").toggleClass("c-open");
                }
            });
        }
    };
}();
// END

// BEGIN: Layout Mega Menu
var LayoutQuickSearch = function() {

    return {
        //main function to initiate the module
        init: function() {
            // desktop mode
            $('.c-layout-header').on('click', '.c-mega-menu .c-search-toggler', function(e) {
                e.preventDefault();

                $('body').addClass('c-layout-quick-search-shown');

                if (App.isIE() === false) {
                    $('.c-quick-search > .form-control').focus();
                }
            });

            // mobile mode
            $('.c-layout-header').on('click', '.c-brand .c-search-toggler', function(e) {
                e.preventDefault();

                $('body').addClass('c-layout-quick-search-shown');

                if (App.isIE() === false) {
                    $('.c-quick-search > .form-control').focus();
                }
            });

            // handle close icon for mobile and desktop
            $('.c-quick-search').on('click', '> span', function(e) {
                e.preventDefault();
                $('body').removeClass('c-layout-quick-search-shown');
            });
        }
    };
}();
// END

// BEGIN: Layout Mega Menu
var LayoutQuickSidebar = function() {

    return {
        //main function to initiate the module
        init: function() {
            // desktop mode
            $('.c-layout-header').on('click', '.c-mega-menu .c-quick-sidebar-toggler', function(e) {
                e.preventDefault();
                e.stopPropagation();

                if ($('body').hasClass("c-layout-quick-sidebar-shown")) {
                    $('body').removeClass("c-layout-quick-sidebar-shown");
                } else {
                    $('body').addClass("c-layout-quick-sidebar-shown");
                }
            });

            $('.c-layout-quick-sidebar').on('click', '.c-close', function(e) {
                e.preventDefault();

                $('body').removeClass("c-layout-quick-sidebar-shown");
            });

            $('.c-layout-quick-sidebar').on('click', function(e) {
                e.stopPropagation();
            });

            $(document).on('click', '.c-layout-quick-sidebar-shown', function(e) {
                $(this).removeClass("c-layout-quick-sidebar-shown");
            });
        }
    };
}();
// END

// BEGIN: Layout Go To Top
var LayoutGo2Top = function() {
    
    var handle = function() {
        var currentWindowPosition = $(window).scrollTop(); // current vertical position
        if (currentWindowPosition > 300) {
            $(".c-layout-go2top").show();
        } else {
            $(".c-layout-go2top").hide();
        }
    };

    return {

         //main function to initiate the module
        init: function() {
            
            handle(); // call headerFix() when the page was loaded

            if (navigator.userAgent.match(/iPhone|iPad|iPod/i)) {
                $(window).bind("touchend touchcancel touchleave", function(e) {
                    handle();
                });
            } else {
                $(window).scroll(function() {
                    handle();
                });
            }

            $(".c-layout-go2top").on('click', function(e) {
                e.preventDefault();
                $("html, body").animate({
                    scrollTop: 0
                }, 600);
            });
        }

    };
}();
// END: Layout Go To Top

// BEGIN: Handle Theme Settings
var LayoutThemeSettings = function() {
    
    var handle = function() {

        $('.c-settings .c-color').on('click', function(){
            var val = $(this).attr('data-color');
            $('#style_theme').attr('href', 'assets/base/css/themes/' + val + '.css');

            $('.c-settings .c-color').removeClass('c-active');
            $(this).addClass('c-active');
        });

        $('.c-setting_header-type').on('click', function(){
            var val = $(this).attr('data-value');
            if (val == 'fluid') {
                $('.c-layout-header .c-topbar > .container').removeClass('container').addClass('container-fluid');
                $('.c-layout-header .c-navbar > .container').removeClass('container').addClass('container-fluid');
            } else {
                $('.c-layout-header .c-topbar > .container-fluid').removeClass('container-fluid').addClass('container');
                $('.c-layout-header .c-navbar > .container-fluid').removeClass('container-fluid').addClass('container');
            }   
            $('.c-setting_header-type').removeClass('active');
            $(this).addClass('active');
        });

        $('.c-setting_header-mode').on('click', function(){
            var val = $(this).attr('data-value');
            if (val == 'static') {
                $('body').removeClass('c-layout-header-fixed').addClass('c-layout-header-static');
            } else {
                $('body').removeClass('c-layout-header-static').addClass('c-layout-header-fixed');
            }   
            $('.c-setting_header-mode').removeClass('active');
            $(this).addClass('active');
        });

        $('.c-setting_font-style').on('click', function(){
            var val = $(this).attr('data-value');

            if (val == 'light') {
                $('.c-font-uppercase').addClass('c-font-uppercase-reset').removeClass('c-font-uppercase');
                $('.c-font-bold').addClass('c-font-bold-reset').removeClass('c-font-bold');

                $('.c-fonts-uppercase').addClass('c-fonts-uppercase-reset').removeClass('c-fonts-uppercase');
                $('.c-fonts-bold').addClass('c-fonts-bold-reset').removeClass('c-fonts-bold');
            } else {
                $('.c-font-uppercase-reset').addClass('c-font-uppercase').removeClass('c-font-uppercase-reset');
                $('.c-font-bold-reset').addClass('c-font-bold').removeClass('c-font-bold-reset');

                $('.c-fonts-uppercase-reset').addClass('c-fonts-uppercase').removeClass('c-fonts-uppercase-reset');
                $('.c-fonts-bold-reset').addClass('c-fonts-bold').removeClass('c-fonts-bold-reset');
            } 

            $('.c-setting_font-style').removeClass('active');
            $(this).addClass('active');
        });

        $('.c-setting_megamenu-style').on('click', function(){
            var val = $(this).attr('data-value');
            if (val == 'dark') {
                $('.c-mega-menu').removeClass('c-mega-menu-light').addClass('c-mega-menu-dark');
            } else {
                $('.c-mega-menu').removeClass('c-mega-menu-dark').addClass('c-mega-menu-light');
            } 
            $('.c-setting_megamenu-style').removeClass('active');
            $(this).addClass('active');
        });
    
    };

    return {

         //main function to initiate the module
        init: function() {
            
            handle();
        }

    };
}();
// END: Handle Theme Settings

// Main theme initialization
$(document).ready(function() {
    // init layout handlers 
    LayoutBrand.init();
    LayoutHeader.init();
    LayoutMegaMenu.init();
    LayoutQuickSearch.init();
    LayoutQuickSidebar.init();
    LayoutGo2Top.init();
    LayoutThemeSettings.init();
});