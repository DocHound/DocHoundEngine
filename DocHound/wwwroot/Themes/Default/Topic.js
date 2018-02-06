﻿$(function() {

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
            userSettings.refreshTargets();
        },
        refreshTargets: function userData_refresh() {
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

    // Forward and backward navigation through the browser
    window.onpopstate = function () {
        if (history.state && history.state.URL) 
            loadTopicAjax(history.state.URL, true);
    }

    // We trap all navigations on anchor tags so we can intercept all navigation on the current domain and handle it in-place
    interceptNavigation();

    // Running everything that needs to be done when a new topic loads
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
        if (!$('body').hasClass('show-mobile-menu')) 
            showMobileMenu();
        else 
            hideMobileMenu();
    });

    // Make sure the selected topic is visible
    ensureSelectedTocEntryVisible();
    
    // Making sure the main content has a minimum height set, so we do not get visual glitches when the mobile menu opens
    $(window).on('resize', handleWindowResize); // We set it every time a resize happens
    handleWindowResize(); // We set it right away so it is right initially
});

showMobileMenu = function() {
    $('body').addClass('show-mobile-menu');
    $('.header').addClass('show-mobile-menu');
    $('.mobile-menu').addClass('show-mobile-menu');
    $('.content-container').addClass('show-mobile-menu');
    $('.footer').addClass('show-mobile-menu');
    $('.mobile-menu').css('display', 'block');
    setTimeout(function () {
        $('.mobile-menu').css('z-index', 10000);
    }, 300);
}

hideMobileMenu = function() {
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

// Makes sure that the specified topic is visible (all the parents are expanded) and that it is scrolled into view
ensureTopicIsExpandedAndVisible = function($topic) {
    var filter = $topic.text();
    var matches = $('.topicList li:contains("' + filter + '")');
    matches.parent().removeClass('topic-collapsed');
    matches.addClass('topic-expanded');
    matches.parent().addClass('topic-expanded');
    matches.show();
    var carets = $('.topic-expanded>.caret.caret-collapsed');
    carets.removeClass('caret-collapsed');
    carets.addClass('caret-expanded');
    ensureSelectedTocEntryVisible();
}

// Scrolls the selected topic into view if need be
ensureSelectedTocEntryVisible = function() {
    var selectedTopics = $('.selected-topic');
    if (selectedTopics.length > 0) {
        selectedTopics[0].scrollIntoView();
    }
}

// Everything that needs to happen the first time the page loads, as well as every time a topic is loaded dynamiclly
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

// This method wires up the click event of anchor tags within the given context to prevent navitation and instead load topics inline if possible.
interceptNavigation = function($referenceObject) {
    if (!$referenceObject) $referenceObject = $(document);

    $referenceObject.on('click', 'a', function() {
        // Regardless of anything else, we can now close the mobile menu
        if ($('body').hasClass('show-mobile-menu')) hideMobileMenu();

        var href = $(this).attr('href');

        $('.selected-topic').removeClass('selected-topic');
        if ($(this).parent().hasClass('topic-link')) 
            $(this).parent().addClass('selected-topic');
        else {
            var found = false;
            var currentSlug = href;
            var allTocLinks = $('.topic-link a');
            for (var counter = 0; counter < allTocLinks.length; counter++){
                var $currentTopic = $(allTocLinks[counter]);
                if (slugMatchesTopic(currentSlug, $currentTopic)) {
                    $currentTopic.parent().addClass('selected-topic');
                    ensureTopicIsExpandedAndVisible($currentTopic);
                    found = true;
                }
            }
            if (!found) { // Since we haven't found anythign yet, we run a more lenient search
                for (var counter = 0; counter < allTocLinks.length; counter++){
                    var $currentTopic = $(allTocLinks[counter]);
                    if (linkMatchesTopic(currentSlug, $currentTopic)) {
                        $currentTopic.parent().addClass('selected-topic');
                        ensureTopicIsExpandedAndVisible($currentTopic);
                    }
                }
            }
        }
        ensureSelectedTocEntryVisible();

        var hrefLower = href.toLowerCase();
        if (!hrefLower.startsWith('http://') && !hrefLower.startsWith('https://') && !hrefLower.startsWith('#') && hrefLower.length > 0) {
            loadTopicAjax(href);
            return false;
        }
    });
}

// This method mirrors TopicHelper.SlugMatchesTopic() on the server. The two methods should be kept in sync functionally
slugMatchesTopic = function(slug, $topicLink)
{
    // This is a more discriminating version of linkMatchesTopic()

    if (!slug) return false;
    if (!$topicLink) return false;

    var topicSlug = $topicLink.attr('data-slug')
    if (!topicSlug) return false;

    slug = slug.toLowerCase();
    topicSlug = topicSlug.toLowerCase();

    while (slug.length > 0 && slug.startsWith('/')) slug = slug.Substring(1);
    while (topicSlug.length > 0 && topicSlug.startsWith('/')) topicSlug = topicSlug.Substring(1);

    return slug === topicSlug;
}

// This method mirrors TopicHelper.LinkMatchesTopic() on the server. The two methods should be kept in sync functionally
linkMatchesTopic = function(link, $topicLink)
{
    if (link == null) return false;
    if ($topicLink == null) return false;

    var normalizedLink = getNormalizedName(link).toLowerCase();

    // We run all kinds of things we can compare to
    var topicSlug = $topicLink.attr('data-slug');
    if (topicSlug) {
        topicSlug = getNormalizedName(topicSlug).toLowerCase();
        if (topicSlug === normalizedLink) return true;
    }

    var topicLink = $topicLink.attr('data-link');
    if (topicLink) {
        topicLink = getNormalizedName(topicLink).toLowerCase();
        if (topicLink === normalizedLink) return true;
    }

    var topicHref = $topicLink.attr('href');
    if (topicHref) {
        topicHref = getNormalizedName(topicHref).toLowerCase();
        if (topicHref === normalizedLink) return true;
    }

    var topicText = $topicLink.text();
    if (topicText) {
        topicText = getNormalizedName(topicText).toLowerCase();
        if (topicText === normalizedLink) return true;
    }

    // We run them all again, but this time we trim out potential file extensions
    var topicSlug = $topicLink.attr('data-slug');
    if (topicSlug) {
        if (topicSlug.indexOf('.') > -1) topicSlug = topicSlug.split('.')[0];
        topicSlug = getNormalizedName(topicSlug).toLowerCase();
        if (topicSlug === normalizedLink) return true;
    }

    var topicLink = $topicLink.attr('data-link');
    if (topicLink) {
        if (topicLink.indexOf('.') > -1) topicLink = topicLink.split('.')[0];
        topicLink = getNormalizedName(topicLink).toLowerCase();
        if (topicLink === normalizedLink) return true;
    }

    var topicHref = $topicLink.attr('href');
    if (topicHref) {
        if (topicHref.indexOf('.') > -1) topicHref = topicHref.split('.')[0];
        topicHref = getNormalizedName(topicHref).toLowerCase();
        if (topicHref === normalizedLink) return true;
    }

    var topicText = $topicLink.text();
    if (topicText) {
        if (topicText.indexOf('.') > -1) topicText = topicText.split('.')[0];
        topicText = getNormalizedName(topicText).toLowerCase();
        if (topicText === normalizedLink) return true;
    }
    
    return false;
}

// This method mirrors TopicHelper.GetNormalizedName() on the server. The two methods should be kept in sync functionally
getNormalizedName = function(name)
{
    if (!name) return '';

    var normalizedName = name;
    while (normalizedName.indexOf(' ') > -1) normalizedName = normalizedName.replace(" ", "-");
    while (normalizedName.indexOf('%20') > -1) normalizedName = normalizedName.replace("%20", "-");
    while (normalizedName.indexOf(',') > -1) normalizedName = normalizedName.replace(",", '');
    while (normalizedName.indexOf('(') > -1) normalizedName = normalizedName.replace("(", '');
    while (normalizedName.indexOf(')') > -1) normalizedName = normalizedName.replace(")", '');
    while (normalizedName.indexOf('?') > -1) normalizedName = normalizedName.replace("?", '');
    while (normalizedName.indexOf(':') > -1) normalizedName = normalizedName.replace(":", '');
    while (normalizedName.indexOf('#') > -1) normalizedName = normalizedName.replace("#", '');
    while (normalizedName.indexOf('&') > -1) normalizedName = normalizedName.replace("&", '');
    while (normalizedName.indexOf('/') > -1) normalizedName = normalizedName.replace("/", '');
    return normalizedName;
}

// Loads a topic dynamically through Ajax and displays it inline
loadTopicAjax = function(href, noPushState) {
    if (href.indexOf('?') !== -1) {
        href += '&notoc=true';
    } else {
        href += '?notoc=true';
    }
    $('article.content-container').css('opacity', '0');
    $('aside.sidebar').css('opacity', '0');

    $.get(href, function(data, status) {
        if (status == 'success') {
            var $html = $('<div>' + data + '</div>'); // This is kind of a hack, but we need to elevate everything within the returned concent one level so jquery actually finds everything
            
            // We merge in the main content
            var $content = $html.find('article.content-container');
            if ($content.length > 0) {
                $('article.content-container').html($content.html());
                $('article.content-container').css('opacity', '1');
            }

            // We also merge in the sidebar
            var $sidebar = $html.find('aside.sidebar');
            if ($sidebar.length > 0) {
                $('aside.sidebar').html($sidebar.html());
                $('aside.sidebar').css('opacity', '1');
            }

            // We take all the scripts from the new topic and move them into the main content
            var $scripts = $html.find('script');
            $scripts.each(function() {
                var scriptSrc = this.src;
                var scriptSrcLower = scriptSrc.toLowerCase();
                if (scriptSrcLower.endsWith('jsmath/easy/load.js')) { // jsmath doesn't work within this context, so we have to force a reload of the page.
                    window.location.href = href;
                    return;
                }
                if (!scriptSrcLower.endsWith('/topic.js')) { // Note: We can't reload this file itself, as it would trigger document-ready stuff we do not want
                    $('script[src="' + scriptSrc + '"]').remove(); // If the script already exists, we remove it, so we can add it back in. (Note: We MUST add it back in, because it may have code that needs to re-run immediately!!!)
                    try {
                        $('body').append(this); // We are adding the new script into the main document. If it contains auto-run code, it will run
                    } catch (ex) {
                        window.location.href = href; // If there is an issue with all this, we are triggering a full navigation to the current page
                    }
                }
            });

            // We now also look at all the links. If they are not yet loaded, we load them now.
            var $links = $html.find('link');
            $links.each(function() {
                var linkHref = this.href;
                if (linkHref.startsWith(window.location.origin))
                    linkHref = linkHref.substring(window.location.origin.length);
                var existingLinks = $('link[href="' + linkHref + '"]');
                if (existingLinks.length < 1) {
                    // The link doesn't yet exist, so we add it
                    $('head').append(this);
                }
            });


            // We set the browser URL. This happens on most dynamic topic loads, except when the forward or back button is pushed in the browser
            if (!noPushState && window.history.pushState) {
                var title = $html.find('title').text();
                href = href.replace('&notoc=true', '').replace('?notoc=true', '')
                window.history.pushState({ title: title, URL: href }, "", href);
                document.title = title;
            }
            // We give everything a moment to load, and then wire up the newly loaded content and intercept navigation within this content
            setTimeout(function() {
                processTopicLoad(); // Makes sure everything going on in the new topics is properly processed (such as syntax highlighting, or a table of contents,...)
                userSettings.refreshTargets(); // Makes sure that everything that got newly loaded that may need user data is refreshes
                interceptNavigation($('article.content-container')); // Wires up all anchor tag navigation within the main content
                interceptNavigation($('aside.sidebar')); // Wires up all anchor tag navigation within the sidebar content
                window.scrollTo(0, 0); // Scrolling the newly loaded content back to the very top
            });
        } else {
            // TODO: We should handle this better :-)
            alert('The requested topic is not available.');
        }
    });
}

// Handles page resize and sets the min height of the main content container, which prevents visual glitches in the mobile version when the mobile menu is open.
handleWindowResize = function() {
    var paddingTop = $('.content-container').css('padding-top').replace('px','');
    var paddingBottom = $('.content-container').css('padding-bottom').replace('px','');
    $('.content-container').css('min-height', ($(window).height() - paddingTop - paddingBottom) + 'px');

    if ($('body').hasClass('show-mobile-menu')) hideMobileMenu();
}