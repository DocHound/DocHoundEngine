// Making the code snippets look pretty
$('pre code')
    .each(function(i, block) {
        hljs.highlightBlock(block);
    });