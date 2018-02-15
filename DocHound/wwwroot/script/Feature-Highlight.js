// Making the code snippets look pretty
$('pre code')
    .each(function(i, block) {
        hljs.highlightBlock(block);
    });

if ($('pre code').length > 0) {
    var themeSelectorContainer = $('#syntaxThemeSelectorContainer');
    if (themeSelectorContainer.length > 0) {
        themeSelectorContainer.css('display', '');

        // If there is a syntax-theme-color selector (which there now should be, but we check anyway, just in case), we also put it into the topic head
        var $syntaxThemeColorSelector = $('#syntaxThemeSelector');
        if ($syntaxThemeColorSelector.length > 0) {
            var syntaxThemeColorSelectorHtml = $syntaxThemeColorSelector[0].outerHTML;
            syntaxThemeColorSelectorHtml = syntaxThemeColorSelectorHtml.replace('id="syntaxThemeSelector"', 'id="syntaxThemeSelector2"')
            appendToSettingsContainer('<span class="option-label">Syntax Highlight Theme:' + syntaxThemeColorSelectorHtml + "</span>");
        }
    }
    
    $('#syntaxThemeSelector').change(function () {
        if (handleSyntaxThemeChange('#syntaxThemeSelector'))
            userSettings.refreshSyntaxThemeSelector('#syntaxThemeSelector2'); // We also refresh the "other" same selector
    });

    $('#syntaxThemeSelector2').change(function () {
        if (handleSyntaxThemeChange('#syntaxThemeSelector2'))
            userSettings.refreshSyntaxThemeSelector('#syntaxThemeSelector'); // We also refresh the "other" same selector
    });

}

handleSyntaxThemeChange = function(id) {
    // First, we disable all color CSS links
    var selectedValue = $(id + ' option:selected').val();

    // Before we do anything, we make sure we are not already on the right CSS file
    var alreadySelectedValue = $(id + ' option:selected').val();
    var alreadyLoadedLinks = $("link[href='" + alreadySelectedValue + "']");
    if (alreadyLoadedLinks.length > 0) {
        if (!alreadyLoadedLinks[0].disabled)
            return false; // The right style sheet is already loaded
    }

    userSettings.syntaxHighlightCss = selectedValue;
    userSettings.save();
    $(id + ' option').each(function () {
        var cssUrl = $(this).val();
        var existingLinks = $("link[href='" + cssUrl + "']");
        if (existingLinks.length > 0) {
            existingLinks[0].disabled = selectedValue != cssUrl;
        }
    });

    // Then, we either load or enable the selected one
    $(id + ' option:selected').each(function () {
        var cssUrl = $(this).val();
        var existingLinks = $("link[href='" + cssUrl + "']");
        if (existingLinks.length == 0) {
            $('head').append('<link rel="stylesheet" href="' + cssUrl + '" type="text/css" />');
        }
    });

    return true;
}