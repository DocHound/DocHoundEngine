$(function() {

    $.expr[":"].contains = $.expr.createPseudo(function(arg) {
        return function( elem ) {
            return $(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0;
        };
    });

    // Saving some configuration in local storage
    userSettings = {
        themeColorCss: '',
        syntaxHighlightCss: '',
        
        save: function userData_save() {                
            if (localStorage)
                localStorage.setItem('KavaDocsUserSettings', JSON.stringify(userSettings));
        },
        load: function userData_load() {
            if (!localStorage)
                return;                
            var data = localStorage.getItem('KavaDocsUserSettings');
            if (data) {
                try {
                    data = JSON.parse(data);
                    $.extend(userSettings, data);
                }
                catch (ex) { localStorage.removeItem('KavaDocsUserSettings') };
            }

            if (userSettings.themeColorCss.length > 0) {
                $('#themeColorSelector option').removeAttr('selected');
                var $selectedOption = $('#themeColorSelector option[value="'+userSettings.themeColorCss+'"]');
                if ($selectedOption.length > 0) {
                    $selectedOption.attr('selected','');
                    setTimeout(function() {
                        $selectedOption.trigger('change');
                    });
                }
            }
            if (userSettings.syntaxHighlightCss.length > 0) {
                $('#syntaxThemeSelector option').removeAttr('selected');
                var $selectedOption2 = $('#syntaxThemeSelector option[value="'+userSettings.syntaxHighlightCss+'"]');
                if ($selectedOption2.length > 0) {
                    $selectedOption2.attr('selected','');
                    setTimeout(function() {
                        $selectedOption2.trigger('change');
                    });
                }
            }
        }
    };
    userSettings.load();

    window.onpopstate = function (event) {
        if (history.state.URL)
            loadTopicAjax(history.state.URL, true);
    }

    // We trap all navigations on anchor tags so we can intercept all navigation on the current domain and handle it in-place
    interceptNavigation();

    processTopicLoad();

    // Add click behavior to tree "arrows"
    $(document.body).on('click', '.caret', function () {
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

    // Make sure the selected topic is visible
    ensureSelectedTocEntryVisible();
    
    // Making sure the main content has a minimum height set, so we do not get visual glitches when the mobile menu opens
    $(window).on('resize', setContentContainerMinHeight); // We set it every time a resize happens
    setContentContainerMinHeight(); // We set it right away so it is right initially
});

ensureSelectedTocEntryVisible = function() {
    var selectedTopics = $('.selected-topic');
    if (selectedTopics.length > 0) {
        selectedTopics[0].scrollIntoView();
    }
}

processTopicLoad = function() {
    // Hooking up various options (if they are present)
    $('#themeColorSelector').change(function () {
        // First, we disable all color CSS links
        var selectedValue = $('#themeColorSelector option:selected').val();
        userSettings.themeColorCss = selectedValue;
        userSettings.save();
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
        var select = '<select id="outlineSelect">';
        var outline = '<ul>';
        for (var headerCounter = 0; headerCounter < headers.length; headerCounter++) {
            var header = headers[headerCounter];
            if (header.id) {
                var localOutline = '<li ';
                if (header.tagName == 'H1') {
                    localOutline = localOutline + 'class="outline-level-1"';
                    select += '<option value="' + header.id + '">' + header.innerText + '</option>';
                } else if (header.tagName == 'H2') {
                    localOutline = localOutline + 'class="outline-level-2"';
                    select += '<option value="' + header.id + '">&nbsp;&nbsp;&nbsp;' + header.innerText + '</option>';
                } else if (header.tagName == 'H3') {
                    localOutline = localOutline + 'class="outline-level-3"';
                    select += '<option value="' + header.id + '">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' + header.innerText + '</option>';
                }
                localOutline = localOutline + '><a data-id="' + header.id + '">' + header.innerText + '</a></li>';
                outline = outline + localOutline;
            }
        }
        outline = outline + '</ul>';
        select = select + '</select>';
        $('#outlineContent').html(select + outline);
        $('#outline').show();
        $('#outlineContent').on('click', 'li>a',
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
};

interceptNavigation = function($referenceObject) {
    if (!$referenceObject) $referenceObject = $(document);

    $referenceObject.on('click', 'a', function() {
        $('.toc .selected-topic').removeClass('selected-topic');
        if ($(this).parent().hasClass('topic-link')) {
            $(this).parent().addClass('selected-topic');
        } else {
            // TODO: We need to try to find the item anyway
        }
        ensureSelectedTocEntryVisible();

        var href = $(this).attr('href');
        var hrefLower = href.toLowerCase();
        if (!hrefLower.startsWith('http://') && !hrefLower.startsWith('https://') && !hrefLower.startsWith('#') && hrefLower.length > 0) {
            loadTopicAjax(href);
            return false;
        }
    });
}

loadTopicAjax = function(href, noPushState) {
    $.get(href, function(data, status) {
        if (status == 'success') {
            var $html = $('<div>' + data + '</div>');
            var $content = $html.find('.content-container');
            if ($content.length > 0) {
                $('.content-container').html($content.html());
            }
            var $sidebar = $html.find('aside');
            if ($sidebar.length > 0) {
                $('aside').html($sidebar.html());
            }
            $('page-scripts').html('');
            var $scripts = $html.find('script');
            $scripts.each(function() {
                var scriptSrc = this.src;
                var scriptSrcLower = scriptSrc.toLowerCase();
                if (scriptSrcLower.endsWith('jsmath/easy/load.js')) {
                    window.location.href = href;
                    return;
                }
                if (!scriptSrcLower.endsWith('/topic.js')) { // Note: We can't reload this file itself, as it would trigger document-ready stuff we do not want
                    $('script[src="' + scriptSrc + '"]').remove();
                    try {
                        $('body').append(this);
                    } catch (ex) {
                        window.location.href = href;
                    }
                }
            });
            if (!noPushState && window.history.pushState) {
                var title = $html.find('title').text();
                window.history.pushState({ title: title, URL: href }, "", href);
                document.title = title;
            }
            setTimeout(function() {
                processTopicLoad();
                interceptNavigation($('.content-container'));
                interceptNavigation($('aside'));
                window.scrollTo(0, 0);
            });
        } else {
            // TODO: We should handle this better :-)
            alert('The requested topic is not available.');
        }
    });
}

setContentContainerMinHeight = function() {
    var paddingTop = $('.content-container').css('padding-top').replace('px','');
    var paddingBottom = $('.content-container').css('padding-bottom').replace('px','');
    $('.content-container').css('min-height', ($(window).height() - paddingTop - paddingBottom) + 'px');
}