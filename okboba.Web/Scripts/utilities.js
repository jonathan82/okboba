/*
 *  Utitlity Functions - !!!!!!!THESE ARE IN THE GLOBAL SCOPE!!!!!!!
 */
function encodeHtml(str) {
    str =  String(str).replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;');

    str = str.replace(/\n/mg, '<br>');

    return str;
}

function br2nl(str) {
    return str.replace(/<br\s*\/?>/mg, "\n");
}

function nl2br(str) {
    return str.replace(/\r\n/mg, "<br/>");
}

/*
 * Removes trailing spaces, newlines as well as html spaces and <br>'s from the string.
 * Useful when getting input from a contenteditable div
 */
function trimBr(str) {
    str = str.trim();
    str = str.replace(/(<br\s*\/*?>)+$/mg, "");    
    str = str.replace(/(&nbsp;\s*)+$/mg, "");
    return str;
}

function pasteHtmlAtCaret(html) {
    var sel, range;
    if (window.getSelection) {
        // IE9 and non-IE
        sel = window.getSelection();
        if (sel.getRangeAt && sel.rangeCount) {
            range = sel.getRangeAt(0);
            range.deleteContents();

            // Range.createContextualFragment() would be useful here but is
            // only relatively recently standardized and is not supported in
            // some browsers (IE9, for one)
            var el = document.createElement("div");
            el.innerHTML = html;
            var frag = document.createDocumentFragment(), node, lastNode;
            while ((node = el.firstChild)) {
                lastNode = frag.appendChild(node);
            }
            range.insertNode(frag);

            // Preserve the selection
            if (lastNode) {
                range = range.cloneRange();
                range.setStartAfter(lastNode);
                range.collapse(true);
                sel.removeAllRanges();
                sel.addRange(range);
            }
        }
    } else if (document.selection && document.selection.type != "Control") {
        // IE < 9
        document.selection.createRange().pasteHTML(html);
    }
}

/*
 *  Spinner options
 */
var spinOpts = {
    scale: 2,
    opacity: 0.3,
    color: '#FFEB3B'
}