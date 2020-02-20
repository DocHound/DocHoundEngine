$(function() {

    $.expr[":"].contains = $.expr.createPseudo(function(arg) {
        return function( elem ) {
            return $(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0;
        };
    });

    // This object handles everything related to switching themes, theme colors, and syntax highlight themes
    themeHandler = {
        setSelectedThemeForElement: function(id) {
            if (!id) id = '#themeColorSelector';

            // First, we disable all color CSS links
            var selectedCssUrl = $(id + ' option:selected').val();
            var allAvailableUrls = themeHandler.getAllCssUrls(id);

            // We save the theme setting away in the user settings in local storage
            if (userSettings.themeColorCss != selectedCssUrl) {
                userSettings.themeColorCss = selectedCssUrl;
                userSettings.save();
            }

            // Before we do anything, we make sure we are not already on the right CSS file
            var currentlyActiveCssUrl = themeHandler.getActiveCssUrl(allAvailableUrls);
            if (selectedCssUrl == currentlyActiveCssUrl
            ) return false; // We are already on that theme, so we do not need to change anything

            // We trigger an update to get the appropriate URL set
            themeHandler.setActiveLinkUrl(selectedCssUrl, allAvailableUrls);
        },

        getAllCssUrls: function(id) {
            var allCssUrls = [];
            $(id + ' option').each(function() {
                allCssUrls.push($(this).val());
            });
            return allCssUrls;
        },

        setActiveLinkUrl: function(cssUrl, allCssUrls) {
            // We look for all currently existing links and see if they should be enabled or disabled
            for (var counter = 0; counter < allCssUrls.length; counter++) {
                var existingUrl = allCssUrls[counter];
                var existingLinks = $("link[href='" + existingUrl + "']");
                if (existingLinks.length > 0)
                    existingLinks[0].disabled = existingUrl != cssUrl;
            }

            // Then, we check whether the style sheet is already loaded (it may not have been part of the loop above the first time around) and if not, we load it in
            var existingLinks = $("link[href='" + cssUrl + "']");
            if (existingLinks.length == 0)
                $('head').append('<link rel="stylesheet" href="' + cssUrl + '" type="text/css" />');
        },

        getActiveCssUrl: function(allCssUrls) {
            for (var counter = 0; counter < allCssUrls.length; counter++) {
                var existingUrl = allCssUrls[counter];
                var existingLinks = $("link[href='" + existingUrl + "']");
                if (existingLinks.length > 0)
                    if (!existingLinks[0].disabled)
                        return existingUrl;
            }
            return '';
        },

        refreshThemeSelector: function(id) { // Makes sure the right option is selected in the drop-down
            if (!id) id = '#themeColorSelector';
            if (userSettings.themeColorCss.length > 0) {
                $(id + ' option').removeAttr('selected');
                var $selectedOption = $(id + ' option[value="' + userSettings.themeColorCss + '"]');
                if ($selectedOption.length > 0)
                    $selectedOption.attr('selected', '');
            }
        },

        setSelectedSyntaxThemeForElement: function(id) {
            if (!id) id = '#syntaxThemeSelectorContainer';

            // First, we disable all color CSS links
            var selectedCssUrl = $(id + ' option:selected').val();
            var allAvailableUrls =
                themeHandler
                    .getAllCssUrls(
                        id); // We can reuse the same method here that we call for the general page theme URLs

            // We save the theme setting away in the user settings in local storage
            if (userSettings.syntaxHighlightCss != selectedCssUrl) {
                userSettings.syntaxHighlightCss = selectedCssUrl;
                userSettings.save();
            }

            // Before we do anything, we make sure we are not already on the right CSS file
            var currentlyActiveCssUrl = themeHandler.getActiveCssUrl(allAvailableUrls);
            if (selectedCssUrl == currentlyActiveCssUrl
            ) return false; // We are already on that theme, so we do not need to change anything

            // We trigger an update to get the appropriate URL set
            themeHandler.setActiveLinkUrl(selectedCssUrl, allAvailableUrls);
            return true;
        },

        refreshSyntaxThemeSelector: function(id) { // Makes sure the right option is selected in the drop-down
            if (!id) id = '#themeColorSelector';
            if (userSettings.syntaxHighlightCss.length > 0) {
                $(id + ' option').removeAttr('selected');
                var $selectedOption = $(id + ' option[value="' + userSettings.syntaxHighlightCss + '"]');
                if ($selectedOption.length > 0)
                    $selectedOption.attr('selected', '');
            }
        },

    };


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
            if (themeHandler) {
                themeHandler.refreshThemeSelector('#themeColorSelector');
                themeHandler.refreshThemeSelector('#themeColorSelector2');
                themeHandler.setSelectedThemeForElement('#themeColorSelector');

                themeHandler.refreshSyntaxThemeSelector('#syntaxThemeSelector');
                themeHandler.refreshSyntaxThemeSelector('#syntaxThemeSelector2');
                themeHandler.setSelectedSyntaxThemeForElement('#syntaxThemeSelector');
            }
        }
    };
    userSettings.load();

    // Forward and backward navigation through the browser
    window.onpopstate = function() {
        if (history.state && history.state.URL)
            loadTopicAjax(history.state.URL, true);
    };

    // We trap all navigations on anchor tags so we can intercept all navigation on the current domain and handle it in-place
    interceptNavigation();

    // Running everything that needs to be done when a new topic loads
    processTopicLoad();

    // Making sure the mobile menu doesn't stay open when people tap on the main topic
    $(document).on('click', '.content-container', function() {
        // Regardless of anything else, we can now close the mobile menu
        if ($('body').hasClass('show-mobile-menu')) hideMobileMenu();
    });
    
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
            $('.topic-list li').show();
        } else {
            $('.topic-list li').hide();
            var $matches = $('.topic-list li:contains("' + filter + '")');
            var $caretMatches = $('.caret', $matches);
            $matches.parent().removeClass('topic-collapsed');
            $caretMatches.removeClass('caret-collapsed');
            $matches.addClass('topic-expanded');
            $matches.parent().addClass('topic-expanded');
            $caretMatches.addClass('caret-expanded');
            $matches.show();
        }
    });

    // Reacting to various things when the page scrolls
    $(document).on('scroll', function () {
        //We assign a scrolled class to all kinds of elements so they can react when the page isn't at the very top
        if ($(document).scrollTop() > 50) 
            $('.header, .toc, .sidebar, .logo, .footer, .repositoryTitle').addClass('scrolled');
        else 
            $('.header, .toc, .sidebar, .logo, .footer, .repositoryTitle').removeClass('scrolled');
        highlightActiveOutlineHeading();
    });
    window.highlightNextOutlineScroll = true;

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

highlightActiveOutlineHeading = function() {
    if (!window.highlightNextOutlineScroll) return;

    // Looking for the closest heading to the top, or the one that is the bottom most one that has scrolled off
    var contentHeadings = $('.content-container h1, .content-container h2, .content-container h3');
    var $activeHeading = null;
    for (var counter = 0; counter < contentHeadings.length; counter++) {
        if (!$activeHeading) $activeHeading = $(contentHeadings[counter]);
        var clientRect = contentHeadings[counter].getBoundingClientRect();
        if (clientRect.bottom < 50) $activeHeading = $(contentHeadings[counter]);
        if (clientRect.top > 50) break;
    }
    if ($activeHeading && $activeHeading.length > 0) {
        var headingId = $activeHeading[0].id;
        $('#outline-content li').removeClass('selected');
        $('#outline-content li[data-id="' + headingId + '"]').addClass('selected');
    }
};

showMobileMenu = function() {
    $('body, .header, .content-container, .footer, .toc, #tree-filter').addClass('show-mobile-menu');
    $('.toc').addClass('mobile-styles-set');
    $('.toc').css('display', 'block');
    setTimeout(function () {
        $('.toc').css('z-index', 10000);
    }, 300);
}

hideMobileMenu = function() {
    $('body, .header, .content-container, .footer, #tree-filter').removeClass('show-mobile-menu');
    $('.toc').addClass('mobile-styles-set');
    $('.toc').css('z-index', -1);
    setTimeout(function() {
            $('.toc').removeClass('show-mobile-menu');
            $('.toc').css('display', 'none');
        },
        300);
};

// Makes sure that the specified topic is visible (all the parents are expanded) and that it is scrolled into view
ensureTopicIsExpandedAndVisible = function($topic) {
    var filter = $topic.text();
    var matches = $('.topic-list li:contains("' + filter + '")');
    matches.parent().removeClass('topic-collapsed');
    matches.addClass('topic-expanded');
    matches.parent().addClass('topic-expanded');
    matches.show();
    var carets = $('.topic-expanded>.caret.caret-collapsed');
    carets.removeClass('caret-collapsed');
    carets.addClass('caret-expanded');
    ensureSelectedTocEntryVisible();
};

// Scrolls the selected topic into view if need be
ensureSelectedTocEntryVisible = function() {
    var selectedTopics = $('.selected-topic');
    if (selectedTopics.length > 0) {
        selectedTopics[0].scrollIntoView();
    }
};

// Everything that needs to happen the first time the page loads, as well as every time a topic is loaded dynamiclly
processTopicLoad = function() {
    // If there is a theme-color selector, we also put it into the topic head
    var $themeColorSelector = $('#themeColorSelector');
    if ($themeColorSelector.length > 0) {
        var themeColorSelectorHtml = $themeColorSelector[0].outerHTML;
        themeColorSelectorHtml = themeColorSelectorHtml.replace('id="themeColorSelector"', 'id="themeColorSelector2"')
        removeFromSettingsContainer('.theme-color-selector-setting');
        appendToSettingsContainer('<div class="option-label theme-color-selector-setting"><div>Theme:</div>' + themeColorSelectorHtml + '</div>', '.syntax-theme-selector-setting', true);
    }

    // Hooking up various options (if they are present)
    $('#themeColorSelector').change(function () {
        if (themeHandler) {
            themeHandler.setSelectedThemeForElement('#themeColorSelector');
            themeHandler.refreshThemeSelector('#themeColorSelector2'); // Making sure the second selector is in sync with this one
        }
    });
    $('#themeColorSelector2').change(function () {
        if (themeHandler) {
            themeHandler.setSelectedThemeForElement('#themeColorSelector2');
            themeHandler.refreshThemeSelector('#themeColorSelector'); // Making sure the second selector is in sync with this one
        }
    });

    // Creating a document outline for the local document content
    var headers = $('h1, h2, h3');
    if (headers.length > 1) {
        var select = '<select id="outlineSelect">';
        var outline = '<ul>';
        for (var headerCounter = 0; headerCounter < headers.length; headerCounter++) {
            var header = headers[headerCounter];
            if (header.id) {
                var localOutline = '<li data-id="' + header.id + '" ';
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
                localOutline = localOutline + '><a data-id="' + header.id + '" class="local-outline-jump">' + header.innerText + '</a></li>';
                outline = outline + localOutline;
            }
        }
        outline = outline + '</ul>';
        select = select + '</select>';
        $('#outline-content').html(select + outline);
        $('#outline').show();
        $('#outline-content').on('click', 'li>a',
            function() {
                window.highlightNextOutlineScroll = false; // All of this will also cause a scroll operation. We do not want that scroll to trigger a different heading to be selected
                
                headerId = $.trim($(this).data('id'));
                var target = $('#' + headerId)[0];
                target.scrollIntoView();
                var doc = $(document);
                doc.scrollTop(doc.scrollTop() - $('header').height() - 10);
                var href = '#' + headerId;
                window.history.pushState({ title: '', URL: href }, "", href);
                
                $('#outline-content li').removeClass('selected');
                $(this).parent().addClass('selected');

                setTimeout(function() {
                    window.highlightNextOutlineScroll = true; // A half a second later, we should be able to process regular scrolling again
                },500);
            });
    }
    highlightActiveOutlineHeading();
};

// This method wires up the click event of anchor tags within the given context to prevent navitation and instead load topics inline if possible.
interceptNavigation = function() {
    $(document).on('click',
        'a:not(a.area-link)',
        function() {
            // Regardless of anything else, we can now close the mobile menu
            if ($('body').hasClass('show-mobile-menu')) hideMobileMenu();

            var $anchor = $(this);
            if ($anchor.hasClass('local-outline-jump')
            )
                return; // Since this click handler is not meant for local outline navigation, we do not want to handle it here
            var href = $anchor.attr('href');

            $('.selected-topic').removeClass('selected-topic');
            var $parent = $(this).parent();
            if ($parent.hasClass('topic-link')) {
                $parent.addClass('selected-topic');
                var expandCaret = $parent.find('>span.caret');
                if (expandCaret.length > 0) {
                    // This node has sub-nodes
                    expandCaret.removeClass('caret-collapsed');
                    expandCaret.addClass('caret-expanded');
                }
                var expandTopicList = $parent.find('>ul.topic-list');
                if (expandTopicList.length > 0) {
                    // This node has sub-nodes
                    expandTopicList.removeClass('topic-collapsed');
                    expandTopicList.addClass('topic-expanded');
                }
            } else {
                var found = false;
                var currentSlug = href;
                var allTocLinks = $('.topic-link a');
                for (var counter = 0; counter < allTocLinks.length; counter++) {
                    var $currentTopic = $(allTocLinks[counter]);
                    if (slugMatchesTopic(currentSlug, $currentTopic)) {
                        var currentParent = $currentTopic.parent();
                        currentParent.addClass('selected-topic');
                        ensureTopicIsExpandedAndVisible($currentTopic);
                        found = true;
                    }
                }
                if (!found) { // Since we haven't found anythign yet, we run a more lenient search
                    for (var counter = 0; counter < allTocLinks.length; counter++) {
                        var $currentTopic = $(allTocLinks[counter]);
                        if (linkMatchesTopic(currentSlug, $currentTopic)) {
                            var currentParent = $currentTopic.parent();
                            currentParent.addClass('selected-topic');
                            ensureTopicIsExpandedAndVisible($currentTopic);
                        }
                    }
                }
            }
            ensureSelectedTocEntryVisible();

            var hrefLower = href.toLowerCase();
            if (!hrefLower.startsWith('http://') &&
                !hrefLower.startsWith('https://') &&
                !hrefLower.startsWith('#') &&
                hrefLower.length > 0) {
                loadTopicAjax(href);
                return false;
            }
        });
};

// This method mirrors TopicHelper.SlugMatchesTopic() on the server. The two methods should be kept in sync functionally
slugMatchesTopic = function(slug, $topicLink) {
    // This is a more discriminating version of linkMatchesTopic()

    if (!slug) return false;
    if (!$topicLink) return false;

    var topicSlug = $topicLink.attr('data-slug')
    if (!topicSlug) return false;

    slug = slug.toLowerCase();
    topicSlug = topicSlug.toLowerCase();

    while (slug.length > 0 && slug.startsWith('/')) slug = slug.substring(1);
    while (topicSlug.length > 0 && topicSlug.startsWith('/')) topicSlug = topicSlug.substring(1);

    return slug === topicSlug;
};

// This method mirrors TopicHelper.LinkMatchesTopic() on the server. The two methods should be kept in sync functionally
linkMatchesTopic = function(link, $topicLink) {
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
};

// This method mirrors TopicHelper.GetNormalizedName() on the server. The two methods should be kept in sync functionally
getNormalizedName = function(name) {
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
};

// Loads a topic dynamically through Ajax and displays it inline

loadTopicAjax = function(href, noPushState) {

    if (href.indexOf('?') !== -1)
        href += '&notoc=true';
    else
        href += '?notoc=true';
    $('article.content-container').css('opacity', '0');
    $('aside.sidebar').css('opacity', '0');
    setTimeout(function() {
            if (!window.newContentLoading
            ) return; // If content loading is already done, we are not messing with this anymore
            $('article.content-container').html('');
            $('aside.sidebar').html('');
            $('article.content-container').css('opacity', '1');
            $('aside.sidebar').css('opacity', '1');
        },
        200);

    window.newContentLoading = true;
    setTimeout(function() {
            if (window.newContentLoading) $('#load-indicator').css('display', 'block');
        },
        1000);

    window.lastAjaxLoadUrl = href;

    $.get(href,
        function(data, status) {
            if (href == window.lastAjaxLoadUrl && status == 'success') {
                window.newContentLoading = false;
                $('#load-indicator').css('display', 'none');

                var $html = $('<div>' + data + '</div>'); // This is kind of a hack, but we need to elevate everything within the returned concent one level so jquery actually finds everything

                // We grab all the references we need later
                var $scripts = $html.find('script');
                var $links = $html.find('link');

                // Adding all the links that came down into the header of the current document
                $links.each(function() {
                    var $currentLink = $(this);
                    var linkHref = $currentLink.attr('href');
                    if ($('link[href="' + linkHref + '"]').length < 1) {
                        $('head').append($currentLink);
                    }
                });

                // We also make sure that we do not have links in the overall doc that are not supposed to be there for the newly downloaded topic anymore
                $('link').each(function() {
                    var $currentExistingLink = $(this);
                    var currentLinkHref = $currentExistingLink.attr('href');
                    var $foundExistingLink = $html.find('link[href="' + currentLinkHref + '"]');
                    if ($foundExistingLink.length < 1) {
                        $currentExistingLink.remove();
                    }
                });

                // We remove scripts and links from the content we merge, since we are merging them into the main content and we do not want to have these elements twice
                $html.find('script').remove();
                $html.find('link').remove();

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
                // Note that we need to add them, even if they are already there, because the scripts may need to run again
                $scripts.each(function() {
                    var scriptSrc = this.src;
                    var scriptSrcLower = scriptSrc.toLowerCase();
                    if (scriptSrcLower.endsWith('jsmath/easy/load.js')) { // jsmath doesn't work within this context, so we have to force a reload of the page.
                        window.location.href = trimNoToc(href);
                        return;
                    }
                    if (scriptSrcLower.indexOf('?') > -1)
                        scriptSrcLower = scriptSrcLower.substring(0, scriptSrcLower.indexOf('?'));
                    if (!scriptSrcLower.endsWith('/topic.js') &&
                        !scriptSrcLower.endsWith('/topic.min.js') &&
                        !scriptSrcLower.endsWith('/jquery-3.2.1.min.js')
                    ) { // Note: We can't reload this file itself, as it would trigger document-ready stuff we do not want. We also have no need to reload jquery
                        try {
                            // We look for all existing scripts and unload them. Note that we have to do this manually, because the src attribute does not necessarily come back clean if we just look by selector.
                            var $existingScripts = $('script');
                            for (var scriptCounter = 0; scriptCounter < $existingScripts.length; scriptCounter++) {
                                var existingScriptSource = $existingScripts[scriptCounter].src;
                                var scriptSrcLower = scriptSrc.toLowerCase();
                                if (existingScriptSource.toLowerCase() == scriptSrcLower)
                                    $existingScripts[scriptCounter]
                                        .remove(); // If the script already exists, we remove it, so we can add it back in. (Note: We MUST add it back in, because it may have code that needs to re-run immediately!!!)
                            }
                            $('body').append(
                                this); // We are adding the new script into the main document. If it contains auto-run code, it will run
                        } catch (ex) {
                            window.location.href =
                                href; // If there is an issue with all this, we are triggering a full navigation to the current page
                        }
                    }
                });

                // We set the browser URL. This happens on most dynamic topic loads, except when the forward or back button is pushed in the browser
                if (!noPushState && window.history.pushState) {
                    var title = $html.find('title').text();
                    window.history.pushState({ title: title, URL: href }, "", trimNoToc(href));
                    document.title = title;
                }
                // We give everything a moment to load, and then wire up the newly loaded content and intercept navigation within this content
                setTimeout(function() {
                    processTopicLoad(); // Makes sure everything going on in the new topics is properly processed (such as syntax highlighting, or a table of contents,...)
                    userSettings
                        .refreshTargets(); // Makes sure that everything that got newly loaded that may need user data is refreshes
                    // interceptNavigation($('article.content-container')); // Wires up all anchor tag navigation within the main content
                    // interceptNavigation($('aside.sidebar')); // Wires up all anchor tag navigation within the sidebar content
                    window.scrollTo(0, 0); // Scrolling the newly loaded content back to the very top
                });
            } else {
                // The requested topic does not seem to be available. Maybe there is some sort of in-place navigation problem. 
                // Therefore, we trigger a full navigation in the hopes that that may go better.
                window.location.href = trimNoToc(href);
                return;
            }
        });
};

trimNoToc = function(href) {
    // TODO: Need to fix the querystring if there are other QS parameters
    href = href.replace('&notoc=true', '').replace('?notoc=true', '?').replace('?&', '?').replace("&&", "&");
    // TODO: Ends With not supported in IE 11
    if (href.endsWith("?") || href.endsWith("&"))
        href = href.substring(0, href.length - 1);
    return href;
};

debounce = function (func, wait, immediate) {
    var timeout;
    return function () {
        var context = this, args = arguments;
        var later = function () {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        var callNow = immediate && !timeout;
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
        if (callNow)
            func.apply(context, args);
    };
};

handleWindowResize = function() {
    //Sets the min height of the main content container, which prevents visual glitches in the mobile version when the mobile menu is open.
    var paddingTop = $('.content-container').css('padding-top').replace('px', '');
    var paddingBottom = $('.content-container').css('padding-bottom').replace('px', '');
    $('.content-container').css('min-height', ($(window).height() - paddingTop - paddingBottom) + 'px');

    // Highlighting the current heading in the outline
    highlightActiveOutlineHeading();

    // The mobile menu should never be open while resizing, since it can cause all kinds of weird behavior
    if ($('body').hasClass('show-mobile-menu')) hideMobileMenu();

    if ($(window).width() > 1024)
        setTimeout(function() {
                $('.toc').css('display', '').css('z-index', '');
            },
            500);

    var $mobileStylesSet = $('.toc.mobile-styles-set');
    if ($mobileStylesSet.length > 0) {
        $mobileStylesSet.css('display', '').css('z-index', '');
        $mobileStylesSet.removeClass('mobile-styles-set');
    }
};

// Returns (or creates) the container element that's right below the first heading which can contain information such as reading time, contributors, or other features of the content
getFeaturesContainer = function() {
    var existingFeaturesElement = $('article.content-container .features-container');
    if (existingFeaturesElement.length < 1) {
        var immediateContentElements = $('article.content-container>*');
        if (immediateContentElements.length > 0) {
            var firstElement = immediateContentElements[0];
            if ($(firstElement).hasClass('settings-container') && immediateContentElements.length > 1)
                firstElement = immediateContentElements[1];
            if (firstElement.nodeName == 'H1' || firstElement.nodeName == 'H2' || firstElement.nodeName == 'H3') {
                // The doc starts with a heading, so we inserts right after the heading
                $('<div class="features-container"></div>').insertAfter(firstElement);
            } else {
                // THe doc starts with some regular content, so we add the features before that
                $('<div class="features-container"></div>').insertBefore(firstElement);
            }
        }
    }

    return $('article.content-container .features-container');
};

// Appends to the container element that's right below the first heading which can contain information such as reading time, contributors, or other features of the content
appendToFeaturesContainer = function(html) {
    if (!html) return;
    var $existingFeaturesElement = getFeaturesContainer();
    if ($existingFeaturesElement.length > 0)
        $existingFeaturesElement.append(html);
};

// Returns (or creates) the container element that's right above the first element, which can include various settings, such as themes
getSettingsContainer = function() {
    var existingSettingsElement = $('.toc .settings-container');
    if (existingSettingsElement.length < 1) {
        var immediateContentElements = $('.toc input');
        if (immediateContentElements.length > 0) {
            $('<div class="settings-container"></div>').insertBefore(immediateContentElements[0]);
        }
    }

    return $('.toc .settings-container');
};

// Appends to the container element that's right above the first element, which can include various settings, such as themes
appendToSettingsContainer = function(html, appendSelector, insertBefore) {
    if (!html) return;
    var $existingSettingsElement = getSettingsContainer();
    if ($existingSettingsElement.length > 0) {
        if (appendSelector) {
            $referenceElement = $(appendSelector, $existingSettingsElement);
            if ($referenceElement.length > 0) {
                if (!insertBefore)
                    $(html).insertAfter($referenceElement);
                else
                    $(html).insertBefore($referenceElement);
                return;
            }
        }
        $existingSettingsElement.append(html);
    }
};

removeFromSettingsContainer = function(selector) {
    $(selector, getSettingsContainer()).remove();
};
