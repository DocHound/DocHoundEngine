var wordsPerMinute = 300;
var content = $('article.content-container').text();

while (content.includes('  ')) 
    content = content.replace('  ',' ');

var words = content.split(' ');
var wordCount = words.length;
var readingTimeText = '';

if (wordCount > wordsPerMinute) {
    var minutes = wordCount / wordsPerMinute;
    minutes = minutes.toFixed(0);
    if (minutes === 1) readingTimeText = 'About 1 minute to read';
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
    appendToFeaturesContainer('<span><i class="far fa-clock"></i> '+ readingTimeText +'</span>');
