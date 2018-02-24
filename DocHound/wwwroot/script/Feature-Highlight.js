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
            removeFromSettingsContainer('.syntax-theme-selector-setting');
            appendToSettingsContainer('<div class="option-label syntax-theme-selector-setting"><div>Code:</div>' + syntaxThemeColorSelectorHtml + '</div>', '.theme-color-selector-setting', true);
        }
    }
    
    $('#syntaxThemeSelector').change(function () {
        if (themeHandler) {
            themeHandler.setSelectedSyntaxThemeForElement('#syntaxThemeSelector');
            themeHandler.refreshSyntaxThemeSelector('#syntaxThemeSelector2'); // Making sure the second selector is in sync with this one
        }
    });

    $('#syntaxThemeSelector2').change(function () {
        if (themeHandler) {
            themeHandler.setSelectedSyntaxThemeForElement('#syntaxThemeSelector2');
            themeHandler.refreshSyntaxThemeSelector('#syntaxThemeSelector'); // Making sure the second selector is in sync with this one
        }
    });
}