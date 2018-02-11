var existingFeaturesElement = $('article.content-container .features-container');
if (existingFeaturesElement.length < 1) {
    var immediateContentElements = $('article.content-container>*');
    if (immediateContentElements.length > 0) {
        if (immediateContentElements[0].nodeName == 'H1' || immediateContentElements[0].nodeName == 'H2' || immediateContentElements[0].nodeName == 'H3') {
            // The doc starts with a heading, so we inserts right after the heading
            $('<div class="features-container"></div>').insertAfter(immediateContentElements[0]);
        } else {
            // THe doc starts with some regular content, so we add the features before that
            $('<div class="features-container"></div>').insertBefore(immediateContentElements[0]);
        }
    }
}

var existingFeaturesElement = $('article.content-container .features-container');
if (existingFeaturesElement.length == 1) { // This should now always be there
    var wordsPerMinute = 300;
    var content = $('article.content-container').text();
    while (content.includes('  ')) content = content.replace('  ',' ');
    var words = content.split(' ');
    var wordCount = words.length;
    var readingTimeText = "less than 1 minute to read";
    if (wordCount > wordsPerMinute) {
        var minutes = wordCount / wordsPerMinute;
        var minutes = minutes.toFixed(0);
        if (minutes == 1) readingTimeText = "1 minute to read";
        else readingTimeText = minutes + " minutes to read";
    }
    existingFeaturesElement.append('<span><i class="far fa-clock"></i> '+ readingTimeText +'</span>');
}