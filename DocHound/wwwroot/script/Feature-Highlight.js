// Making the code snippets look pretty
$('pre code')
    .each(function(i, block) {
        hljs.highlightBlock(block);
    });
if ($('pre code').length > 0) {
    var themeSelectorContainer = $('#syntaxThemeSelectorContainer');
    if (themeSelectorContainer.length > 0) {
        themeSelectorContainer.css('display', '');
    }
    $('#syntaxThemeSelector').change(function () {
        // First, we disable all color CSS links
        var selectedValue = $('#syntaxThemeSelector option:selected').val();
        $('#syntaxThemeSelector option').each(function () {
            var cssUrl = $(this).val();
            var existingLinks = $("link[href='" + cssUrl + "']");
            if (existingLinks.length > 0) {
                existingLinks[0].disabled = selectedValue != cssUrl;
            }
        });

        // Then, we either load or enable the selected one
        $('#syntaxThemeSelector option:selected').each(function () {
            var cssUrl = $(this).val();
            var existingLinks = $("link[href='" + cssUrl + "']");
            if (existingLinks.length == 0) {
                $('head').append('<link rel="stylesheet" href="' + cssUrl + '" type="text/css" />');
            }
        });
    });
}