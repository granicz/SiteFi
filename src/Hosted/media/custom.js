/*
 * ===========================================================
 * CUSTOM.JS 
 * ===========================================================
 * This is a custom file and it's used only on this template.
 *
*/

'use strict';
(function ($) {
    $('.title h2, [class^=col-] > h2,.container  > h2,.container  > h1,.cnt-call h2, [class^=col-] > h1, .col h2, header h1,.fixed-area > h2').each(function () {
        let text = $(this).html().trim();
        if (text.charAt(text.length - 1) == '.') {
            $(this).html(text.substr(0, text.length - 1) + '<span class="dot">.</span>');
        }
    });
}(jQuery)); 
