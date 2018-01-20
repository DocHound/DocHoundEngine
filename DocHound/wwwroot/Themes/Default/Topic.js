$(function () {
    $.expr[":"].contains = $.expr.createPseudo(function(arg) {
        return function( elem ) {
            return $(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0;
        };
    });

    // Make sure the selected topic is visible
    var selectedTopics = $('.selectedTopic');
    if (selectedTopics.length > 0) {
        selectedTopics[0].scrollIntoView();
    }

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

    // Wiring up the tree filter text element
    $('#tree-filter').keyup(function() {
        var filter = $('#tree-filter').val();

        if (filter == '') {
            $('.topicList li').show();
        } else {
            $('.topicList li').hide();
            var matches = $('.topicList li:contains("' + filter + '")');
            matches.parent().removeClass('topicCollapsed');
            matches.addClass('topicExpanded');
            matches.parent().addClass('topicExpanded');
            matches.show();
        }
    });
    $('#tree-filter-mobile').keyup(function() {
        var filter = $('#tree-filter-mobile').val();

        if (filter == '') {
            $('.topicList li').show();
        } else {
            $('.topicList li').hide();
            var matches = $('.topicList li:contains("' + filter + '")');
            matches.parent().removeClass('topicCollapsed');
            matches.addClass('topicExpanded');
            matches.parent().addClass('topicExpanded');
            matches.show();
        }
    });

    // Creating a document outline for the local document content
    var headers = $('h1, h2, h3');
    if (headers.length > 1) {
        var outline = '<ul>';
        for (var headerCounter = 0; headerCounter < headers.length; headerCounter++) {
            var header = headers[headerCounter];
            if (header.id) {
                var localOutline = '<li ';
                if (header.tagName == 'H1') {
                    localOutline = localOutline + 'class="outlineLevel1"';
                } else if (header.tagName == 'H2') {
                    localOutline = localOutline + 'class="outlineLevel2"';
                } else if (header.tagName == 'H3') {
                    localOutline = localOutline + 'class="outlineLevel3"';
                }
                localOutline = localOutline + '><a data-id="' + header.id + '">' + header.innerText + '</a></li>';
                outline = outline + localOutline;
            }
        }
        outline = outline + '</ul>';
        $('#outlineContent').html(outline);
        $('#outline').show();
        $('#outlineContent').on('click',
            'li>a',
            function() {
                headerId = $.trim($(this).data('id'));
                var target = $('#' + headerId)[0];
                target.scrollIntoView();
                var doc = $(document);
                doc.scrollTop(doc.scrollTop() - $('header').height() - 10);
                var href = '#' + headerId;
                window.history.pushState({ title: '', URL: href }, "", href);
            });
    }

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
