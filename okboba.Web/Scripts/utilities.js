/*
 *  Utitlity Functions
 * 
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