$(function () {
    $.expr[":"].contains = $.expr.createPseudo(function(arg) {
        return function( elem ) {
            return $(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0;
        };
    });

    // Make sure the selected topic is visible
    var selectedTopics = $('.selected-topic');
    if (selectedTopics.length > 0) {
        selectedTopics[0].scrollIntoView();
    }

    // Add click behavior to tree "arrows"
    $('.caret').click(function () {
        var parent = $(this).parent();
        if ($(this).hasClass('caret-expanded')) {
            $(this).removeClass('caret-expanded');
            $(this).addClass('caret-collapsed');
            var ul2 = $('>ul', parent);
            ul2.removeClass('topic-expanded');
            ul2.addClass('topic-collapsed');
        }
        else {
            $(this).removeClass('caret-collapsed');
            $(this).addClass('caret-expanded');
            var ul1 = $('>ul', parent);
            ul1.removeClass('topic-collapsed');
            ul1.addClass('topic-expanded');
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
            matches.parent().removeClass('topic-collapsed');
            matches.addClass('topic-expanded');
            matches.parent().addClass('topic-expanded');
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
            matches.parent().removeClass('topic-collapsed');
            matches.addClass('topic-expanded');
            matches.parent().addClass('topic-expanded');
            matches.show();
        }
    });

    // Hooking up various options (if they are present)
    $('#themeColorSelector').change(function () {
        // First, we disable all color CSS links
        var selectedValue = $('#themeColorSelector option:selected').val();
        $('#themeColorSelector option').each(function () {
            var cssUrl = $(this).val();
            var existingLinks = $("link[href='" + cssUrl + "']");
            if (existingLinks.length > 0) {
                existingLinks[0].disabled = selectedValue != cssUrl;
            }
        });

        // Then, we either load or enable the selected one
        $('#themeColorSelector option:selected').each(function () {
            var cssUrl = $(this).val();
            var existingLinks = $("link[href='" + cssUrl + "']");
            if (existingLinks.length == 0) {
                $('head').append('<link rel="stylesheet" href="' + cssUrl + '" type="text/css" />');
            }
        });
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
                    localOutline = localOutline + 'class="outline-level-1"';
                } else if (header.tagName == 'H2') {
                    localOutline = localOutline + 'class="outline-level-2"';
                } else if (header.tagName == 'H3') {
                    localOutline = localOutline + 'class="outline-level-3"';
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

    // Closing the menu when the header areas is clicked
    $('.header').on('click', function() {
        $('.sub-menu').removeClass('visible-sub-menu');
    });
    $('.menu>ul>li').on('mouseenter', function () {
        $('.sub-menu').removeClass('visible-sub-menu');
        $(this).find('.sub-menu').addClass('visible-sub-menu');
    });
    $('body').on('mouseenter', function () {
        $('.sub-menu').removeClass('visible-sub-menu');
    });

    // Opening the hamburger/mobile menu
    $('.mobile-menu-icon').on('click', function () {
        if (!$('body').hasClass('show-mobile-menu')) {
            $('body').addClass('show-mobile-menu');
            $('.header').addClass('show-mobile-menu');
            $('.mobile-menu').addClass('show-mobile-menu');
            $('.content-container').addClass('show-mobile-menu');
            $('.footer').addClass('show-mobile-menu');
            $('.mobile-menu').css('display', 'block');
            setTimeout(function () {
                $('.mobile-menu').css('z-index', 10000);
            }, 300);
        } else {
            $('body').removeClass('show-mobile-menu');
            $('.header').removeClass('show-mobile-menu');
            $('.content-container').removeClass('show-mobile-menu');
            $('.footer').removeClass('show-mobile-menu');
            $('.mobile-menu').removeClass('show-mobile-menu');
            $('.mobile-menu').css('z-index', -10000);
            setTimeout(function () {
                $('.mobile-menu').css('display', 'none');
            }, 300);
        }
    });

    // Making sure the main content has a minimum height set, so we do not get visual glitches when the mobile menu opens
    $(window).on('resize', function(){
        var paddingTop = $('.content-container').css('padding-top').replace('px','');
        var paddingBottom = $('.content-container').css('padding-bottom').replace('px','');
        $('.content-container').css('min-height', ($(window).height() - paddingTop - paddingBottom) + 'px');
    });
    var paddingTop = $('.content-container').css('padding-top').replace('px','');
    var paddingBottom = $('.content-container').css('padding-bottom').replace('px','');
    $('.content-container').css('min-height', ($(window).height() - paddingTop - paddingBottom) + 'px');
});
