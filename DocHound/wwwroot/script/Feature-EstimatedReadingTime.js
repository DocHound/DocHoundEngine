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
    var readingTimeText = '';
    if (wordCount > wordsPerMinute) {
        var minutes = wordCount / wordsPerMinute;
        var minutes = minutes.toFixed(0);
        if (minutes == 1) readingTimeText = 'About 1 minute to read';
        else if (minutes < 9) readingTimeText = 'About ' + minutes + ' minutes to read';
        else if (minutes < 13) readingTimeText = 'About 10 minutes to read';
        else if (minutes < 18) readingTimeText = 'About 15 minutes to read';
        else if (minutes < 23) readingTimeText = 'About 20 minutes to read';
        else if (minutes < 28) readingTimeText = 'About 25 minutes to read';
        else if (minutes < 38) readingTimeText = 'About a half hour to read';
        else if (minutes < 50) readingTimeText = 'About 45 minutes to read';
        else if (minutes < 70) readingTimeText = 'About one hour to read';
        else if (minutes < 80) readingTimeText = 'About an hour and 15 minutes to read';
        else if (minutes < 100) readingTimeText = 'About an hour and a half to read';
        else if (minutes < 130) readingTimeText = 'About two hours to read';
        else if (minutes < 160) readingTimeText = 'About two and a half hours to read';
        else if (minutes < 190) readingTimeText = 'About three hours to read';
        else readingTimeText = 'More than three hours to read';
    }
    if (readingTimeText.length > 0)
        existingFeaturesElement.append('<span><i class="far fa-clock"></i> '+ readingTimeText +'</span>');
}