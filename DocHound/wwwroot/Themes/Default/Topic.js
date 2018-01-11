$(function () {
    // Make sure the selected topic is visible
    var selectedTopics = $('.selectedTopic');
    if (selectedTopics.length > 0) {
        selectedTopics[0].scrollIntoView();
    }

    // Making the code snippets look pretty
    $('pre').addClass('prettyprint');

    // Add click behavior to tree "arrows"
    $('.caret').click(function () {
        var parent = $(this).parent();
        if ($(this).hasClass('caretExpanded')) {
            $(this).removeClass('caretExpanded');
            $(this).addClass('caretCollapsed');
            var ul2 = $('>ul', parent);
            ul2.removeClass('topicExpanded');
            ul2.addClass('topicCollapsed');
        }
        else {
            $(this).removeClass('caretCollapsed');
            $(this).addClass('caretExpanded');
            var ul1 = $('>ul', parent);
            ul1.removeClass('topicCollapsed');
            ul1.addClass('topicExpanded');
        }
    });

    // Reacting to various things when the page scrolls
    $(document).on('scroll', function () {
        if ($(document).scrollTop() > 60) {
            $('.header').addClass('scrolled');
            $('.toc').addClass('scrolled');
            $('.sidebar').addClass('scrolled');
            $('.logo').addClass('scrolled');
            $('.footer').addClass('scrolled');
        } else {
            $('.header').removeClass('scrolled');
            $('.toc').removeClass('scrolled');
            $('.sidebar').removeClass('scrolled');
            $('.logo').removeClass('scrolled');
            $('.footer').removeClass('scrolled');
        }
    });
});

// Closing the menu when the header areas is clicked
$('.header').on('click', function() {
    $('.subMenu').removeClass('visibleSubMenu');
});
$('.menu>ul>li').on('mouseenter', function () {
    $('.subMenu').removeClass('visibleSubMenu');
    $(this).find('.subMenu').addClass('visibleSubMenu');
});
$('body').on('mouseenter', function () {
    $('.subMenu').removeClass('visibleSubMenu');
});

// Opening the hamburger/mobile menu
$('.mobileMenuIcon').on('click', function () {
    if (!$('body').hasClass('showMobileMenu')) {
        $('body').addClass('showMobileMenu');
        $('.header').addClass('showMobileMenu');
        $('.mobileMenu').addClass('showMobileMenu');
        $('.content-container').addClass('showMobileMenu');
        $('.footer').addClass('showMobileMenu');
        $('.mobileMenu').css('display', 'block');
        setTimeout(function () {
            $('.mobileMenu').css('z-index', 10000);
        }, 300);
    } else {
        $('body').removeClass('showMobileMenu');
        $('.header').removeClass('showMobileMenu');
        $('.content-container').removeClass('showMobileMenu');
        $('.footer').removeClass('showMobileMenu');
        $('.mobileMenu').removeClass('showMobileMenu');
        $('.mobileMenu').css('z-index', -10000);
        setTimeout(function () {
            $('.mobileMenu').css('display', 'none');
        }, 300);
    }
});
