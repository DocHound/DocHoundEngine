// Activating nomnoml
var graphs = document.getElementsByClassName('nomnoml');
for (var i = 0; i < graphs.length; i++) {
    var doc = new DOMParser().parseFromString(graphs[i].innerHTML, "text/html");
    graphs[i].innerHTML = nomnoml.renderSvg(doc.documentElement.textContent);
}
