// Activating nomnoml
var graphs = document.getElementsByClassName('nomnoml');
for (var nomCounter = 0; nomCounter < graphs.length; nomCounter++) {
    var doc = new DOMParser().parseFromString(graphs[nomCounter].innerHTML, "text/html");
    graphs[nomCounter].innerHTML = nomnoml.renderSvg(doc.documentElement.textContent);
}
