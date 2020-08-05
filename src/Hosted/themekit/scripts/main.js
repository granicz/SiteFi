/*
 * ===========================================================
 * MAIN SCRIPT- THEMEKIT
 * ===========================================================
 * This script manage all the js functions and the 3r party plugins.
 *
 * ANIMATIONS
 * -------------------------------------------------------------
 * Manage all the animations on page scroll, on click, on hover
 * Manage the timeline animations
 *
 * Copyright (c) Federico Schiocchet - schiocco.com - Themekit
*/

"use strict";
function cssInit(delay, speed) {
    delay += 'ms';
    speed += 'ms';
    return {
        'animation-duration': speed,
        'transition-timing-function': 'ease',
        'transition-delay': delay
    };
}
function initAnima(obj) {
    (function ($) {
        var animaTimeout = $.fn.getGlobalVar("animaTimeout");
        var animaTimeout_2 = $.fn.getGlobalVar("animaTimeout_2");
        var da = $(obj).attr("data-anima");
        var an = $(obj).find(".anima,[data-anima]");
        var t = $(obj).attr("data-time");
        var ta = $(obj).attr("data-target");
        var tm = $(obj).attr("data-timeline");
        var tmt = $(obj).attr("data-timeline-time");
        var tr = $(obj).attr("data-trigger");
        var len = $(an).length;
        var default_anima = $.fn.getGlobalVar("default_anima");
        if (da == "default" && !isEmpty(default_anima)) da = default_anima;
        if (isEmpty(tmt)) tmt = 500;
        if (isEmpty(an)) an = obj;
        $(an).each(function (i) {
            if (!isEmpty($(this).attr("data-anima")) && i === 0) { an = obj; return false; }
        });
        if (!isEmpty(ta)) an = $(ta);
        if (isEmpty(t)) t = 500;
        var time = 0, p = 1;
        if (!isEmpty(tm) && tm === "desc") { time = (len - 1) * tmt; p = -1 };
        var cont = null;
        $(an).each(function (index) {
            var time_now = time;
            if (index === len - 1 && tm === "desc") time_now = 0;
            if (!$(this).hasClass("anima") && an != obj && isEmpty(ta)) {
                cont = this;
            } else {
                if (cont != null && !$.contains(cont, this) || cont === null) {
                    var tobj = this;
                    var pos = $(this).css("position");
                    if (pos != 'absolute' && pos != 'fixed') $(this).css("position", "relative");
                    var aid = Math.random(5) + "";
                    $(tobj).attr("aid", aid);
                    if (animaTimeout.length > 30) {
                        animaTimeout.shift();
                        animaTimeout_2.shift();
                    }
                    animaTimeout.push([aid, setTimeout(function () {
                        $(tobj).css(cssInit(0, 0));
                        var da_ = da;
                        if (!isEmpty($(tobj).attr('class')) && $(tobj).attr('class').indexOf("anima-") != -1) {
                            var arr_a = $(tobj).attr('class').split(" ");
                            for (var i = 0; i < arr_a.length; i++) {
                                if (arr_a[i].indexOf("anima-") != -1) da_ = arr_a[i].replace("anima-", "");
                            }
                        }
                        if ($(window).width() < 768 && (isEmpty(tr) || tr === "scroll" || tr === "load")) da_ = "fade-in";
                        animaTimeout_2.push([aid, setTimeout(function () { $(tobj).css(cssInit(0, t)).addClass(da_); $(tobj).css('opacity', '') }, 100)]);
                    }, time_now)]);
                    if (!isEmpty(tm)) time += tmt * p;
                }
            }
        });
        $.fn.setGlobalVar(animaTimeout, "animaTimeout");
        $.fn.setGlobalVar(animaTimeout_2, "animaTimeout_2");
    }(jQuery));
}
function outAnima(obj) {
    (function ($) {
        var animaTimeout = $.fn.getGlobalVar("animaTimeout");
        var animaTimeout_2 = $.fn.getGlobalVar("animaTimeout_2");
        var da = $(obj).attr("data-anima");
        var an = $(obj).find(".anima,[data-anima]");
        var t = $(obj).attr("data-time");
        var hidden = $(obj).attr("data-hidden");
        var ta = $(obj).attr("data-target");
        var default_anima = $.fn.getGlobalVar("default_anima");
        if (da == "default" && !isEmpty(default_anima)) da = default_anima;
        if (isEmpty(an)) an = obj;
        if (!isEmpty(ta)) an = $(ta);
        if (isEmpty(t)) t = 500;
        var cont = null;
        $(an).each(function () {
            var aid = $(this).attr("aid");
            if (!isEmpty(aid)) {
                for (var i = 0; i < animaTimeout.length; i++) {
                    if (animaTimeout[i][0] == aid) {
                        clearTimeout(animaTimeout[i][1]);
                    }
                }
                for (var i = 0; i < animaTimeout_2.length; i++) {
                    if (animaTimeout_2[i][0] == aid) {
                        clearTimeout(animaTimeout_2[i][1]);
                    }
                }
            }
            if (!$(this).hasClass("anima") && an != obj) {
                cont = this;
            } else {
                if (cont != null && !$.contains(cont, this) || cont == null) {
                    var pos = $(this).css("position");
                    if (pos != 'absolute' && pos != 'fixed') $(this).css("position", "relative");
                    var da_ = da;
                    try {
                        if ($(this).attr('class').indexOf("anima-") != -1) {
                            var arr_a = $(this).attr('class').split(" ");
                            for (var i = 0; i < arr_a.length; i++) {
                                if (arr_a[i].indexOf("anima-") != -1) da_ = arr_a[i].replace("anima-", "");
                            }
                        }
                    } catch (e) { }
                    $(this).css(cssInit(0, t)).removeClass(da_);
                    var op = parseFloat($(this).css("opacity"));
                    if (op > 0 && op < 1) $(this).css("opacity", 1);
                }
            }
        });
        if (hidden) {
            $(an).css(cssInit(0, t)).css("opacity", 0);
            setTimeout(function () { $(an).css("opacity", 0); }, 400);
        }
        $.fn.setGlobalVar(animaTimeout, "animaTimeout");
        $.fn.setGlobalVar(animaTimeout_2, "animaTimeout_2");
    }(jQuery));
}
function resetAnima(cnt) {
    (function ($) {
        if (isEmpty(cnt)) cnt = "body";
        $(cnt).find("[data-anima],[data-fullpage-anima]").each(function () {
            var animation = $(this).attr("data-anima");
            if (isEmpty(animation)) animation = $(this).attr("data-fullpage-anima");
            if (!isEmpty(animation)) {
                $(cnt).find("[data-timeline] .anima").removeAttr("data-anima").removeAttr("data-fullpage-anima");
                $(cnt).find("." + animation).each(function () {
                    $(this).removeClass(animation).removeAttr("aid").css("opacity", 0);
                });
            }
        });
    }(jQuery));
}

//OTHERS
window.onload = function () { function a(a, b) { var c = /^(?:file):/, d = new XMLHttpRequest, e = 0; d.onreadystatechange = function () { 4 == d.readyState && (e = d.status), c.test(location.href) && d.responseText && (e = 200), 4 == d.readyState && 200 == e && (a.outerHTML = d.responseText) }; try { d.open("GET", b, !0), d.send() } catch (f) { } } var b, c = document.getElementsByTagName("*"); for (b in c) c[b].hasAttribute && c[b].hasAttribute("data-include") && a(c[b], c[b].getAttribute("data-include")) };

/*
 * # FUNCTIONS
 * -------------------------------------------------------------
 * getURLParameter - Read the parameters of the url like www.site.com?paramter-name=value
 * openWindow - Open a url in a new center window similar to a popup window
 * onePageScroll - Scroll the page on target position with animations
 * getOptionsString - Get a array of options from HTML, details: www.framework-y.com/components/components-base.html#base-javascript
 * isEmpty - Perform multiple checks to determinate if a variable is null or empty
 * correctValue - Convert strings to number or boolean
 * isScrollView - Check if the target element is visible on the user's screen
 *
*/

function getURLParameter(name) {
    return decodeURIComponent((new RegExp('[?|&]' + name + '=' + '([^&;]+?)(&|#|;|$)').exec(location.search) || [, ""])[1].replace(/\+/g, '%20') || "");
}
function openWindow(link, width, height) {
    if (typeof width === 'undefined') width = 550;
    if (typeof height === 'undefined') height = 350;
    var left = (screen.width / 2) - (width / 2);
    var top = (screen.height / 2) - (height / 2);
    window.open(link, 'targetWindow', 'toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=' + width + ',height=' + height + ', top=' + top + ', left=' + left);
    return false;
}
function onePageScroll(t) {
    if (!isEmpty(t)) {
        jQuery(t).find('a[href ^= "#"]').on('click', function (e) {
            e.preventDefault();
            var target = this.hash,
                jtarget = jQuery(target);
            if (jtarget.length > 0) {
                try {
                    jQuery('html, body').stop().animate({
                        'scrollTop': (jtarget.offset().top - 150)
                    }, 900, 'swing', function () {
                        if (history.pushState) {
                            history.pushState(null, null, target);
                        } else {
                            location.hash = target;
                        }
                    });
                } catch (e) { }
            } else {
                if (target != "#" && target.length > 2 && jQuery(this).closest("nav").length) document.location = window.location.protocol + "//" + window.location.host;
            }
        });
    }
}
function getOptionsString(txt, mainArray) {
    var optionsArr = txt.split(",");
    for (var i = 0; i < optionsArr.length; i++) {
        mainArray[optionsArr[i].split(":")[0]] = correctValue(optionsArr[i].split(":")[1]);
    }
    return mainArray;
}
function isEmpty(obj) { if (typeof (obj) !== "undefined" && obj !== null && obj != false && (obj.length > 0 || typeof (obj) == 'number' || typeof (obj.length) == "undefined") && obj !== "undefined") return false; else return true; }
function correctValue(n) { return typeof n == "number" ? parseFloat(n) : n == "true" ? !0 : n == "false" ? !1 : n }
function isScrollView(t) {
    if (!isEmpty(t)) {
        var elementTop = jQuery(t).offset().top - 20;
        var elementBottom = elementTop + jQuery(t).outerHeight() + 20;
        var viewportTop = jQuery(window).scrollTop();
        var viewportBottom = viewportTop + jQuery(window).height();
        return elementBottom > viewportTop && elementTop < viewportBottom;
    }
}

//MAIN BLOCK
(function ($) {
    var arrFA = [];
    var firstLoad = true;
    var animaTimeout = [];
    var animaTimeout_2 = [];
    var default_anima;
    var window_width = $(window).width();
    var window_height = $(window).height();
    var body;
    var nav;
    var header;
    var parallax_items;
    var parallax_title_video = false;
    var footer_parallax;
    var sections_slider_parallax;

    //# DOCUMENT READY
    $(document).ready(function () {
        body = $("body");
        nav = $("body > nav");
        header = $(body).find(" > header");
        parallax_items = $(body).find("[data-parallax]");
        sections_slider_parallax = $(body).find(" > main > .section-slider[data-slider-parallax]");

        //SOCIAL
        $(body).on("click", "[data-social]", function () {
            var a = $(this).attr("data-social");
            var link = $(this).attr("data-social-url");
            var purl = link;
            if (isEmpty(link)) purl = window.location.href;

            var url = 'https://www.facebook.com/sharer/sharer.php?u=' + purl;
            if (a == 'share-twitter') {
                url = 'https://twitter.com/intent/tweet?text=' + $('meta[name=description]').attr("content");
                if (!isEmpty(link)) url = 'https://twitter.com/intent/tweet?url=' + link;
            }
            if (a == 'share-pinterest') url = 'http://pinterest.com/pin/create/button/?url=' + purl;
            if (a == 'share-linkedin') url = 'https://www.linkedin.com/shareArticle?url=' + purl;
            openWindow(url);
        });

        //ANIMATIONS
        $(body).find("[data-anima]").each(function () {
            var tr = $(this).attr("data-trigger");
            if (isEmpty(tr) || tr == "scroll" || tr == "load") {
                var an = $(this).find(".anima,*[data-anima]");
                if (isEmpty(an)) an = this;
                var cont = null;
                var x = 0;
                $(an).each(function () {
                    if (!$(this).hasClass("anima") && an != this) {
                        cont = this;
                    } else {
                        if (cont != null && !$.contains(cont, this) || cont == null) {
                            $(this).css("opacity", 0);
                            x++;
                        }
                    }
                });
                if (x == 0) $(this).css("opacity", 0);
            }
            if (!isEmpty(tr) && tr == "load") initAnima(this);
        });
        $(body).on("click", "[data-anima][data-trigger=click]", function () {
            outAnima(this);
            initAnima(this);
        });
        $(body).on("mouseenter", "[data-anima][data-trigger=hover]", function () {
            initAnima(this);
        });
        $(body).on("mouseleave", "[data-anima][data-trigger=hover]", function () {
            $(this).stop(true, false);
            outAnima(this);
        });

        //# NAVIGATION MENU
        if (window_width > 992) {
            $(nav).on("click", ".search-box-menu > i", function () {
                $(this).parent().toggleClass("active");
            });
            $(body).on("mouseenter", "nav[data-menu-anima] .menu-cnt > ul > li > a,nav[data-menu-anima] .lan-menu > li > a", function () {
                $(this).parent().find(" > ul, > .mega-menu").css("opacity", 0).showAnima($("nav").data("menu-anima"), null, 250);
            });
            $(body).on("mouseenter", ".menu-subline .menu-cnt > ul li", function () {
                $(nav).find(".subline-bar ul").removeClass("active").eq($(this).index()).addClass("active");
            });
            $(nav).on("mouseleave", ".subline-bar", function () {
                $(this).find("ul").removeClass("active");
            });
            if ($(nav).hasClass("menu-side-collapse")) {
                $(nav).on("click", ".menu-cnt > ul > li > a", function () {
                    var item = $(this).parent();
                    var active = $(item).hasClass("active");
                    $(nav).find(".menu-cnt > ul > li").removeClass("active");
                    if (!active) {
                        var sub_menu = $(item).find(" > ul,.mega-menu");
                        $(item).addClass("active");
                        if (sub_menu.length) {
                            var item_height = $(sub_menu).height() + 15;
                            $(sub_menu).css("height", "0");
                            $(sub_menu).animate({
                                height: item_height
                            }, 500, function () { $(sub_menu).css("height", "") });
                        }
                    }
                });
            }
        } else {
            $(body).on("click", ".menu-cnt > ul > li > a,.lan-menu a", function () {
                var li = $(this).parent();
                var active = $(li).hasClass("active");
                $(body).find(" > nav .menu-cnt > ul > li,.shop-menu-cnt,.lan-menu li").removeClass("active");
                if (!active) $(li).addClass("active");
                if ($(nav).hasClass("menu-subline")) {
                    var bar = $(".subline-bar");
                    $(nav).find(".subline-bar ul").removeClass("active");
                    $(bar).css("margin-top", $(li).offset().top + $(li).height() + "px");
                    if (!active) {
                        $(nav).find(".subline-bar ul").removeClass("active").eq($(li).index()).addClass("active");
                    }
                }
            });
            $(nav).on("click", ".dropdown > a", function (e) {
                e.preventDefault();
            });
            $(body).on("click", ".shop-menu-cnt", function () {
                $(this).toggleClass("active");
            });
            $(body).on("click", ".menu-inner:not(.menu-inner-vertical) > ul > li > a", function () {
                $(this).parent().toggleClass("active");
            });
            if ($(nav).hasClass("menu-fixed")) {
                $(nav).find(".menu-cnt").css("max-height", (window_height - 5 - $(nav).find(" > .container").outerHeight()) + "px");
            }
        }
        $(nav).on("click", ".menu-btn", function () {
            let active = $(nav).hasClass("active");
            let menu_cnt = $(nav).find(".menu-cnt");
            let h;
            if (active) {
                h = 0
            } else {
                $(menu_cnt).css({ "opacity": "0", "height": "auto" });
                h = $(menu_cnt).outerHeight();
                $(menu_cnt).css({ "opacity": "", "height": "" });
            }
            if (!active) $(nav).addClass("active");
            $(menu_cnt).animate({
                height: h
            }, 300, function () {
                setTimeout(function () {
                    if (active) {
                        $(nav).removeClass("active");
                        $(menu_cnt).css("height", "0");
                    } else {
                        $(menu_cnt).css("height", "auto");
                    }
                }, 300);

            });
        });
        $(body).on("click", ".menu-inner-vertical .dropdown", function () {
            var active = $(this).hasClass("active");
            $(this).closest(".menu-inner").find(".dropdown").removeClass("active");
            if (!active) $(this).addClass("active");
        });
        $(body).on("click", ".menu-inner:not(.menu-inner-vertical) > div", function () {
            $(this).parent().toggleClass("active");
        });
        onePageScroll($(".menu-inner,.menu-one-page"));


        //# OTHERS
        if (navigator.userAgent.indexOf('MSIE') !== -1 || navigator.appVersion.indexOf('Trident/') > 0) {
            $(body).addClass("iex");
        }
        $(body).on("click", 'a[href="#"]', function (e) {
            e.preventDefault();
        });
        $(body).find("[data-href].lightbox").each(function () {
            $(this).attr("href", $(this).attr("data-href"));
            $(this).initMagnificPopup();
        });
        $(body).on("click", "[data-href]", function (e) {
            var css = $(this).attr("class");
            if (css.indexOf("scroll-to") == -1 && css.indexOf("lightbox") == -1) {
                var link = $(this).attr("data-href");
                if (link.indexOf("#") == 0) {
                    if (link != "#") {
                        $(link).scrollTo();
                    }
                    e.preventDefault();
                } else {
                    var target = $(this).attr("data-target");
                    if (!isEmpty(target) && target == "_blank") window.open(link);
                    else document.location = link;
                }
            }
        });
        $(body).find(".full-screen").each(function () {
            $(this).sizeFullScreen($(this).attr("data-offset"), $(this).attr("data-min-height"));
        });
        $(body).on("click", ".icon-links-popup > i", function () {
            $(this).parent().toggleClass("active");
        });
        if (window_width > 992) {
            $(".section-two-blocks .content").each(function () {
                var t = this;
                setTimeout(function () {
                    var h = $(t).outerHeight();
                    var cnt = $(t).closest(".section-two-blocks");
                    if (isEmpty($(cnt).attr("data-parallax"))) $(cnt).css("height", h);
                    $(cnt.find(".row > div:first-child")).renderLoadedImgs();
                }, 300);
            });
            footer_parallax = $(body).find(" > footer.footer-parallax");
            if ($(footer_parallax).length) {
                $(body).find("main").addClass("footer-parallax-cnt").css("margin-bottom", $(footer_parallax).outerHeight() + "px");
            }
        }

        //# WP
        $(nav).find("a").each(function () {
            if ($(this).attr("href") == window.location.href) {
                if ($(this).closest(".dropdown-menu").length) {
                    $(this).closest(".dropdown.multi-level:not(.dropdown-submenu),.dropdown.mega-dropdown").addClass("active");
                } else {
                    $(this).closest("li").addClass("active");
                }
            }
        });

        //ALBUM 
        $(body).on("click", ".album-box", function () {
            var album = $(this).closest(".album");
            var animation = $(album).attr("data-album-anima");
            if (isEmpty(animation)) {
                animation = "fade-bottom";
            }
            $(".album-title > span").html($(this).find(".caption h3").html());
            $(album).addClass("active");
            $(album).find(".album-item").removeClass("active " + animation).eq($(this).index()).addClass("active").showAnima(animation);
        });
        $(body).on("click", ".album-title > a", function () {
            var album = $(this).closest(".album");
            $(album).removeClass("active");
            $(album).find(".album-list").css("opacity", 0).showAnima("fade-in")
        });


        //BACKGROUND VIDEO
        if ($(header).hasClass("header-video")) {
            $(header).find(" > video").set_video_size($(header).width(), $(header).height());
            if ($(header).hasClass("video-parallax")) parallax_title_video = true;
        }
        $(body).find("[data-video-youtube]").each(function () {
            $(this).initYoutubeVideo();
        });
        $(".section-video,.section-block .block-media").each(function () {
            $(this).find(" > video").set_video_size($(this).width(), $(this).height());
        });

        //BACKGROUND SLIDER
        $(".section-slider").each(function () {
            $(this).init_background_slider();
        });
        if ($(header).hasClass("header-slider")) {
            $(header).init_background_slider();
        }

        //SCROLL METHODS
        setTimeout(function () { $(window).scroll(); }, 50);
        $(body).on("click", ".scroll-top", function () {
            $("html, body").stop().animate({ scrollTop: 0 }, '500', 'swing');
        });
        $(body).on("click", ".scroll-to", function (e) {
            var t = $(this).attr("data-scroll-to");
            if (isEmpty(t)) t = $(this).attr("href");
            if (isEmpty(t)) t = $(this).attr("data-href");
            try {
                $(t).scrollTo();
            } catch (e) { }
            if (t.indexOf("#") == 0 && ($(this).hasClass("btn") || $(this).hasClass("btn-text"))) e.preventDefault();
        });

        //LOADER
        $(body).addClass("no-transactions");
        $(body).find(' > #preloader').fadeOut(300);
        setTimeout(function () {
            $(body).removeClass("no-transactions");
        }, 400);

        //# PAGE SCROLL
        var header_cnt = $(header).find(".container");
        var data_anima = $("[data-anima]");
        var scroll_1 = $(".scroll-hide");
        var scroll_2 = $(".scroll-change");
        var scroll_3 = $(".scroll-show");
        var scroll_4 = $(".scroll-top-btn");
        var scroll_5 = $(".scroll-show-mobile");
        var scroll_6 = $("nav.scroll-change .navbar-brand");
        var scroll_len = $(".fp-enabled").length;
        var old_scroll = 0;

        //Scroll detect
        var scroll_detect = $("[data-scroll-detect] li");
        var scroll_detect_items = [];
        if (scroll_detect.length) {
            $(scroll_detect).each(function () {
                var target = $(this).find("a").attr("href");
                if (target.length > 1 && target.indexOf("#") == 0) {
                    scroll_detect_items.push($(body).find($(this).find("a").attr("href")));
                }
            });
        }
        $(window).scroll(function () {
            var scroll = $(window).scrollTop();
            if (old_scroll != scroll || old_scroll == 0) {
                var po = window.pageYOffset;
                var go = true;
                var dh = $(document).height();
                old_scroll = scroll;

                //TOP TITLE
                if (scroll < 2200) {
                    $(header_cnt).css("margin-top", po).css("opacity", (100 / po < 1) ? (100 / po) : 1);
                    if (parallax_title_video) {
                        $(header).find(" > video").css("margin-top", po);
                    }
                }

                //SCROLL FUNCTIONS
                if (scroll > 100 && go) {
                    go = false;
                    $(scroll_1).addClass('hidden');
                    $(scroll_2).addClass("scroll-css");
                    $(scroll_3).addClass('showed');
                    $(scroll_4).css("opacity", 1);
                    $(nav).addClass("scroll-menu");
                    if (window_width < 994) $(scroll_5).removeClass('hidden');
                    $(scroll_6).hide().show(0);
                    if ($(footer_parallax).length) {
                        if (scroll + window_height > (dh - window_height)) {
                            $(footer_parallax).css("opacity", 1);
                        } else $(footer_parallax).css("opacity", 0);
                    }
                }
                if (scroll < 100) {
                    go = true;
                    $(scroll_1).removeClass("hidden");
                    if (!scroll_len) $(scroll_2).removeClass("scroll-css");
                    $(scroll_3).removeClass('showed');
                    $(scroll_5).css("opacity", 0);
                    $(scroll_6).hide().show(0);
                    $(nav).removeClass("scroll-menu");
                }

                //Scroll animation
                $(data_anima).each(function () {
                    var tr = $(this).attr("data-trigger");
                    if (isEmpty(tr) || tr == "scroll") {
                        if (isScrollView(this)) {
                            if (!isEmpty($(this).attr("data-anima"))) initAnima(this);
                            $(this).attr("data-anima", "");
                        }
                    }
                });

                //Scroll detect
                $(scroll_detect_items).each(function (index) {
                    if (isScrollView(this)) { $(scroll_detect).removeClass("active").eq(index).addClass("active") }
                });

                //Section slider parallax
                $(sections_slider_parallax).each(function (index) {
                    if (isScrollView(this)) {
                        $(sections_slider_parallax).eq(index).addClass("active")
                    } else {
                        $(this).removeClass("active");
                    }
                });
            }
        });
        $(window).resize(function () {
            window_width = $(window).width();
            window_height = $(window).height();
        });

        //WOOCOMMERCE AND WORDPRESS
        var shop_menu = $(body).find(".shop-menu-cnt");
        populateShoppingCart();
        function populateShoppingCart() {
            if ($("meta[content='wordpress']").length && $(shop_menu).length) {
                jQuery.ajax({
                    method: "POST",
                    url: ajax_url,
                    data: {
                        action: 'hc_get_wc_cart_items'
                    }
                }).done(function (response) {
                    if (!isEmpty(response) && response.length > 10) {
                        var arr = JSON.parse(response);
                        if (arr.length > 0) {
                            var currency = $(shop_menu).find(".cart-total").attr("data-currency");
                            var total = 0;
                            var html = "";
                            for (var i = 0; i < arr.length; i++) {
                                total += arr[i]["price"] * arr[i]["quantity"];
                                html += '<li onclick="document.location = \'' + arr[i]["link"] + '\'" class="cart-item"><img src="' + arr[i]["image"] + '" alt=""><div class="cart-content"><h5>' + arr[i]["title"] + '</h5><span class="cart-quantity">' + arr[i]["quantity"] + ' x ' + currency + "" + arr[i]["price"] + '</span></div></li>';
                            }
                            $(shop_menu).find(".shop-cart").html(html);
                            $(shop_menu).find(".cart-total span").html(currency + "" + total);
                            $(shop_menu).removeClass("shop-menu-empty");
                            $(shop_menu).find("i").html('<span class="cart-count">' + arr.length + '</span>');
                        }
                    }
                });
            }
        }
        $(body).on("click", ".ajax_add_to_cart,.product-remove a", function () {
            setTimeout(function () {
                populateShoppingCart();
            }, 2000);
        });
        if ($(body).find(".comment-list .item").length == 0) {
            $(body).find(".comments-cnt").addClass("no-comments");
        }

        /*
        * # THIRD PARTS PLUGINS
        * -------------------------------------------------------------
        * Functions and methods that menage the execution of external plugins
        */

        //jquery.miniTip.min.js
        $(body).find("[data-minitip]").executeFunction("miniTip", function () {
            $("[data-minitip]").each(function () {
                $(this).initMinitip();
            });
        });

        //imagesloaded.min.js
        $(body).find(".img-box,.media-box").executeFunction("imagesLoaded", function () {
            $(".img-box,.media-box").each(function () {
                $(this).renderLoadedImgs();
            });
        });

        //isotope.min.js
        $(body).find(".maso-list").executeFunction("isotope", function () {
            setTimeout(function () { $.fn.setGlobalVar(false, "firstLoad"); }, 1000);
            $('.maso-list').each(function () {
                if ($(this).attr("data-trigger") != "manual") $(this).initIsotope();
            });
        });
        $(body).on("click", ".maso-list .menu-inner a", function () {
            var f = $(this).attr('data-filter');
            var t = $(this).closest(".maso-list");
            if (!isEmpty(f)) $(t).find('.maso-box').isotope({ filter: "." + $(this).attr('data-filter') });
            var lm = $(t).find('.load-more');
            if (lm.length) {
                setTimeout(function () {
                    var i = 0;
                    var num = parseInt($(lm).attr("data-page-items"), 10);
                    $(t).find('.maso-box .maso-item').each(function () {
                        if ($(this).attr("style").indexOf("display: none") == -1) i++;
                    });
                    if (i < num) {
                        $(t).find('.load-more').click();
                        var intervalMaso = setInterval(function () {
                            if ($(t).find('.maso-box').height() < 10) {
                                $(t).find('.load-more').click();
                            } else {
                                clearInterval(intervalMaso);
                            }
                        }, 100);
                    }
                }, 450);
            }
            if ($(t).find('.maso-box .maso-item').length < 3) $(t).find('.load-more-maso').click();
            $(t).find("li").removeClass("active");
            $(this).closest("li").addClass("active");
        });
        $(body).on("click", ".maso-order", function () {
            var t = $(this).closest(".maso-list").find('.maso-box');
            var sort = $(this).attr("data-sort");
            if (sort == "asc") {
                t.isotope({ sortAscending: false });
                $(this).attr("data-sort", "desc");
                $(this).html("<i class='fa fa-arrow-up'></i>");
            } else {
                t.isotope({ sortAscending: true });
                $(this).attr("data-sort", "asc");
                $(this).html("data-sort");
                $(this).html("<i class='fa fa-arrow-down'></i>");
            }
        });

        //magnific-popup.min.js
        $(body).find(".grid-list.list-gallery .grid-box,.maso-list.list-gallery .maso-box, .lightbox,.box-lightbox,.lightbox-trigger,.woocommerce-product-gallery__image a").executeFunction("magnificPopup", function () {
            $(body).find('.grid-list.list-gallery .grid-box,.maso-list.list-gallery .maso-box,.lightbox,.woocommerce-product-gallery__image a').each(function () {
                $(this).initMagnificPopup();
            });
            $(window).scroll(function (event) {
                $(body).find("[data-trigger=scroll].lightbox-trigger").each(function () {
                    if (isScrollView(this)) {
                        $($(this).attr("href")).initMagnificPopup();
                        $(this).attr("data-trigger", "null");
                    }
                });
            });

            //Deep linking
            var url = getURLParameter("lightbox");
            var id = getURLParameter("id");
            if (!isEmpty(id)) id = "#" + id + " ";
            if (!isEmpty(url)) {
                if (url.indexOf("list") > -1) {
                    $(id + ".grid-box .grid-item:nth-child(" + url.replace("list-", "") + ") .img-box").click();
                    $(id + ".maso-box .maso-item:nth-child(" + url.replace("list-", "") + ") .img-box").click();
                } else {
                    if (url.indexOf("slide") > -1) {
                        $(id + ".slides > li:nth-child(" + url.replace("slide-", "") + ") .img-box").click();
                    } else {
                        var t = $("#" + url);
                        if ($(t).length) {
                            if ($(t).hasClass("img-box") || $(t).hasClass("lightbox")) {
                                $(t).click();
                            } else {
                                var c = $(t).find(".img-box,.lightbox");
                                if (c.length) {
                                    $(c).click();
                                } else {
                                    if ($(t).hasClass("box-lightbox")) {
                                        $.magnificPopup.open({
                                            type: 'inline',
                                            items: { 'src': '#' + url },
                                            mainClass: 'lightbox-on-load'
                                        });
                                    }
                                }
                            }
                        }
                    }

                }
            }
        });
        $(body).find("[data-trigger=load].box-lightbox").each(function () {
            var e = $(this).attr("data-expire");
            if (!isEmpty(e) && e > 0) {
                var id = $(this).attr("id");
                if (isEmpty(Cookies.get(id))) {
                    $(this).initMagnificPopup();
                    Cookies.set(id, 'expiration-cookie', { expire: e });
                }
            } else $(this).initMagnificPopup();
        });

        //slimscroll.min.js
        $(body).find(".scroll-box,.menu-side").executeFunction("slimScroll", function () {
            $(body).find(".scroll-box").each(function () {
                $(this).initSlimScroll();
            });
            if (window_width > 992) {
                var menu = $(body).find(".menu-side-collapse .menu-cnt");
                $(menu).slimScroll({ height: $(menu).height() - 20, color: $(menu).attr("data-bar-color"), size: '4px', distance: '0px', start: 'bottom' });
            }
        });

        //parallax.min.js
        $(parallax_items).executeFunction("parallax", function () {
            $(parallax_items).each(function (index) {
                var ken = "";
                $(this).initParallax();
                if ($(this).hasClass("ken-burn-in")) ken = "ken-burn-in";
                if ($(this).hasClass("ken-burn-out")) ken = "ken-burn-out";
                if ($(this).hasClass("ken-burn-center")) ken = "ken-burn-center";
                if ($(this).hasClass("parallax-side")) ken += " parallax-side-cnt";
                if (ken != "") { setTimeout(function () { $(".parallax-mirror:eq(" + (index - 1) + ")").addClass(ken); }, 100) }
            });
            var timerVar;
            var times = 0;
            var isFP = $("html").hasClass("fp-enabled");
            timerVar = self.setInterval(function () {
                if (times > 30) {
                    clearInterval(timerVar);
                } else {
                    if (!isFP) $(window).trigger('resize').trigger('scroll');
                }
                times = times + 1;
            }, 100);
        });

        //Progress, counter, countdown
        $(body).find(".countdown [data-time],[data-to],[data-progress]").executeFunction("downCount", function () {
            $(body).init_progress_all();
        });
    });

    /*
     * FUNCTIONS
     * -------------------------------------------------------------
     * toggleClick - Manage on click and on second click events
     * showAnima - Start an animation
     * sizeFullScreen - Set fullscreen sizes to the target element
     * scrollTo - Scroll the page on target position with animations
     * expandItem - Open a container with animation
     * collapseItem - Close a container with animation
     * set_video_size - Set the background video sizes on mobile and desktop
     * getHeight - Get the correct height of an item
     * executeFunction - Check if a script is loaded and execute the gived function the script load has been completed
     * getGlobalVar - Read a global variable
     * setGlobalVar - Set a global variable
    */
    (function (n) { if (typeof define == "function" && define.amd) define(n); else if (typeof exports == "object") module.exports = n(); else { var i = window.Cookies, t = window.Cookies = n(); t.noConflict = function () { return window.Cookies = i, t } } })(function () { function n() { for (var n = 0, r = {}, t, i; n < arguments.length; n++) { t = arguments[n]; for (i in t) r[i] = t[i] } return r } function t(i) { function r(t, u, f) { var o, s; if (arguments.length > 1) { f = n({ path: "/" }, r.defaults, f); typeof f.expires == "number" && (s = new Date, s.setMilliseconds(s.getMilliseconds() + f.expires * 864e5), f.expires = s); try { o = JSON.stringify(u); /^[\{\[]/.test(o) && (u = o) } catch (y) { } return u = encodeURIComponent(String(u)), u = u.replace(/%(23|24|26|2B|3A|3C|3E|3D|2F|3F|40|5B|5D|5E|60|7B|7D|7C)/g, decodeURIComponent), t = encodeURIComponent(String(t)), t = t.replace(/%(23|24|26|2B|5E|60|7C)/g, decodeURIComponent), t = t.replace(/[\(\)]/g, escape), document.cookie = [t, "=", u, f.expires && "; expires=" + f.expires.toUTCString(), f.path && "; path=" + f.path, f.domain && "; domain=" + f.domain, f.secure ? "; secure" : ""].join("") } t || (o = {}); for (var l = document.cookie ? document.cookie.split("; ") : [], a = /(%[0-9A-Z]{2})+/g, h = 0; h < l.length; h++) { var v = l[h].split("="), c = v[0].replace(a, decodeURIComponent), e = v.slice(1).join("="); e.charAt(0) === '"' && (e = e.slice(1, -1)); try { if (e = i && i(e, c) || e.replace(a, decodeURIComponent), this.json) try { e = JSON.parse(e) } catch (y) { } if (t === c) { o = e; break } t || (o[c] = e) } catch (y) { } } return o } return r.get = r.set = r, r.getJSON = function () { return r.apply({ json: !0 }, [].slice.call(arguments)) }, r.defaults = {}, r.remove = function (t, i) { r(t, "", n(i, { expires: -1 })) }, r.withConverter = t, r } return t() });
    $.fn.toggleClick = function (n) { var t = arguments, r = n.guid || $.guid++, i = 0, u = function (r) { var u = ($._data(this, "lastToggle" + n.guid) || 0) % i; return $._data(this, "lastToggle" + n.guid, u + 1), r.preventDefault(), t[u].apply(this, arguments) || !1 }; for (u.guid = r; i < t.length;) t[i++].guid = r; return this.click(u) };
    $.fn.showAnima = function (a, b, time) {
        var t = this;
        if (a === "default") a = $.fn.getGlobalVar("default_anima");
        $(t).removeClass(a);
        if (!isEmpty(b) && b === "complete") {
            $(t).attr("data-anima", a).attr("data-trigger", "manual"); initAnima(t);
        } else {
            setTimeout(function () {
                if (isEmpty(time)) {
                    time = $(t).data("time");
                }
                $(t).css(cssInit(0, (isEmpty(time) ? 500 : time))).addClass(a); $(t).css('opacity', '')
            }, 100);
        }
    };
    $.fn.sizeFullScreen = function (offset, min) {
        if (isEmpty(offset)) offset = 0;
        if (isEmpty(min)) min = 0;
        if ((window_height - offset) > min && !isEmpty(this)) {
            $(this).css("height", window_height - offset);
        }
    }
    $.fn.scrollTo = function () {
        if (!isEmpty(this)) {
            $('html, body').animate({
                scrollTop: $(this).offset().top - 50
            }, 1000);
        }
    }
    $.fn.expandItem = function () {
        var t = this;
        $(t).css("display", "block").css("height", "");
        var h = $(t).height();
        $(t).css("height", 0).css("opacity", 1);
        $(t).animate({
            height: h
        }, 300, function () { $(t).css("height", "") });
    }
    $.fn.collapseItem = function () {
        var t = this;
        $(t).animate({
            height: 0
        }, 300, function () { $(t).css("display", "none") });
    }
    $.fn.set_video_size = function (w, h) {
        var video_height = $(this).height();
        var video_width = $(this).width();
        var new_width;
        var new_height;

        //Check if video height/width smaller
        if (video_height < h || video_width < w) {
            var proportion = h / video_height;
            new_width = w * proportion;
            $(this).css("width", new_width + "px");
        }

        new_height = $(this).height();

        //Check if video X centered
        if (new_width > w) {
            $(this).css("margin-left", "-" + ((new_width - w) / 2) + "px");
        }

        //Check if video Y centered
        if (new_height > h) {
            $(this).css("margin-top", "-" + ((new_height - h) / 2) + "px");
        }
    }
    $.fn.initYoutubeVideo = function () {
        var id = $(this).attr("data-video-youtube");
        if (!isEmpty(id)) {
            if (id.indexOf("http:") != -1 || id.indexOf("www.you") != -1 || id.indexOf("youtu.be") != -1) {
                if (id.indexOf("?v=") != -1) id = id.substring(id.indexOf("v=") + 2);
                if (id.indexOf("youtu.be") != -1) id = id.substring(id.lastIndexOf("/") + 1);
            }
            var vq = $(this).attr("data-video-quality");
            var pars = "";
            if (!isEmpty(vq)) {
                if (vq == "hc-hd") pars += "&amp;vq=hd1080";
            }
            $(this).html('<iframe frameborder="0" allowfullscreen="0" src="https://www.youtube.com/embed/' + id + '?playlist=' + id + '&amp;vq=hd1080&amp;loop=1&amp;start=0&amp;autoplay=1&amp;mute=1&amp;controls=0&amp;showinfo=0&amp;wmode=transparent&amp;iv_load_policy=3&amp;modestbranding=1&amp;rel=0&amp;enablejsapi=1&amp;volume=0' + pars + '"></iframe>');
        }
    }
    $.fn.getHeight = function () {
        if (!isEmpty(this)) return $(this)[0].clientHeight;
        else return 0;
    }
    $.fn.executeFunction = function (functionName, myfunction) {
        var timer;
        if ($(this).length > 0) {
            if (typeof window["jQuery"]["fn"][functionName] === "function" || typeof window[functionName] === "function") {
                myfunction();
            } else {
                timer = setInterval(function () {
                    if (typeof window["jQuery"]["fn"][functionName] === "function" || typeof window[functionName] === "function") {
                        myfunction();
                        clearInterval(timer);
                    }
                }, 300);
            }
        }
    }
    $.fn.getGlobalVar = function (name) {
        return eval(name);
    };
    $.fn.setGlobalVar = function (value, name) {
        window[name] = value;
    };
    $.fn.init_background_slider = function () {
        var interval = $(this).attr("data-interval");
        var slides = $(this).find(" > .background-slider > div");
        var count = $(slides).length;
        var index = 1;
        if (interval != "0") {
            if (isEmpty(interval)) interval = 3000;
            setInterval(function () {
                index += 1;
                $(slides).removeClass("active");
                $(slides).eq((index % count)).addClass("active");
            }, interval);
        }
    };

    /*
     * # THIRD PARTS PLUGINS FUNCTIONS
     * -------------------------------------------------------------
     * Functions and methods that menage the execution of external plugins
    */

    //jquery.miniTip.min.js
    $.fn.initMinitip = function () {
        $(this).miniTip({
            title: $(this).attr("data-title"),
            content: $(this).attr("data-minitip"),
            anchor: $(this).attr("data-pos"),
            delay: 500,
        });
    }

    //imagesloaded.min.js
    $.fn.renderLoadedImgs = function () {
        if ($.isFunction($.fn.imagesLoaded)) {
            var isIsotope = false;
            var $isotope;
            var imgLoad = imagesLoaded($(this));
            if ($(this).hasClass("maso-box")) { isIsotope = true; $isotope = this; }
            imgLoad.on('progress', function (instance, image) {
                if (!$(image.img).parent().hasClass("hc-image")) {
                    var result = image.isLoaded ? 'loaded' : 'broken';
                    var target = "a"
                    if ($(image.img).closest(".img-box").length) target = ".img-box";
                    else if ($(image.img).closest(".media-box").length) target = ".media-box";
                    else if ($(image.img).closest("ul.glide__slides").length) target = ".glide__slides";
                    else if ($(image.img).closest("figure").length) target = "figure";


                    var cont = $(image.img).closest(target);
                    var imgHeight = image.img.clientHeight;
                    var imgWidth = image.img.clientWidth;
                    var colWidth = 0;
                    var colHeight = 0;
                    if (!isEmpty(cont.get(0))) {
                        colWidth = cont.get(0).clientWidth;
                        colHeight = cont.get(0).clientHeight;
                    }

                    if (result == "loaded") {
                        if (isIsotope) {
                            $isotope.isotope('layout');
                            var mi = $(image.img).closest('.maso-item');
                            $(mi).css("visibility", "visible");
                            $(mi).find("> *").animate({ "opacity": 1 }, 300);
                        }
                        $(image.img).css("transition", "none");
                        if (imgHeight > colHeight) {
                            $(image.img).css("margin-top", "-" + Math.floor(((imgHeight - colHeight) / 2)) + "px");
                        } else {
                            var proportion = colHeight / imgHeight;
                            var newWidth = imgWidth * proportion;
                            if (newWidth / imgWidth > 1) {
                                $(image.img).css("max-width", Math.ceil(newWidth) + "px").css("width", Math.ceil(newWidth) + "px");
                                $(image.img).css("margin-left", "-" + Math.floor(((newWidth - colWidth) / 2)) + "px");
                            }
                        }
                        setTimeout(function () {
                            $(image.img).css("transition", "");
                        }, 100);
                    }
                }
            });
        }
    }

    //isotope.min.js
    $.fn.initPagination = function () {
        var opt = $(this).attr("data-options");
        var a = $(this).attr("data-pagination-anima");
        var p = parseInt($(this).attr("data-page-items"), 10);
        var c = $(this).closest(".maso-list");
        var t = $(c).find(".maso-box");
        var items = t.isotope('getItemElements');
        var n = $(items).length;
        var type = "";
        if ($(this).hasClass('load-more')) type = 'load-more';
        if ($(this).hasClass('pagination')) type = 'pagination';

        for (var i = p; i < n; i++) {
            $(items[i]).addClass("isotope-hidden");
        }
        setTimeout(function () {
            for (var i = p; i < n; i++) {
                t.isotope('remove', items[i]);
            }
            t.isotope('layout');
        }, 1000); 

        if (type == 'pagination') {
            var optionsArr;
            var options = {
                totalPages: Math.ceil(n / p),
                visiblePages: 7,
                first: "First",
                last: "Last",
                next: "Next",
                prev: "Previous",
                onPageClick: function (event, page) {
                    t.isotope('remove', t.isotope('getItemElements'));
                    for (var i = (p * (page - 1)); i < (p * (page)); i++) {
                        $(items[i]).removeClass("isotope-hidden");
                        t.isotope('insert', items[i]);
                    }
                    t.isotope('layout');
                    if (!isEmpty(opt) && opt.indexOf("scrollTop:true") != -1) $(c).scrollTo();
                }
            }
            if (!isEmpty(opt)) {
                optionsArr = opt.split(",");
                options = getOptionsString(opt, options);
            }
            $(this).twbsPagination(options);
        }
        if (type == 'load-more') {
            var tl = this;
            $(tl).on("click", function (index) {
                loadMoreMaso(this);
            });
            if (!isEmpty(opt) && opt.indexOf("lazyLoad:true") != -1) {
                $(window).scroll(function () {
                    if ($(window).scrollTop() + window_height == $(document).height()) {
                        if ($.fn.getGlobalVar("firstLoad")) setTimeout(function () { loadMoreMaso(tl) }, 800);
                        else loadMoreMaso(tl);
                    }
                });
            }
        }

        function loadMoreMaso(obj) {
            var page = $(obj).attr("data-current-page");
            if (isEmpty(page)) page = 1;
            page++;
            $(obj).attr("data-current-page", page);
            var s = p * (page - 1);
            var e = p * (page);
            for (var i = s; i < (p * (page)); i++) {
                $(items[i]).removeClass("isotope-hidden");
                t.isotope('insert', items[i]);
            }
            t.isotope('layout');
            if ($.isFunction($.fn.renderLoadedImgs)) {
                $(t).renderLoadedImgs();
            }
            if (e >= n) $(obj).hide(300);
        }
    }
    $.fn.initIsotope = function () {
        if ($.isFunction($.fn.isotope)) {
            var m = $(this).find('.maso-box');
            var menu = $(this).find(".menu-inner");
            var optionsString = $(this).attr("data-options");
            var optionsArr;
            var options = {
                itemSelector: '.maso-item',
                percentPosition: true,
                masonry: {
                    columnWidth: '.maso-item',
                    horizontalOrder: true
                },
                getSortData: {
                    number: function (e) {
                        return parseInt(jQuery(e).attr('data-sort'), 10);
                    }
                },
                sortBy: 'number'
            }
            if (!isEmpty(optionsString)) {
                optionsArr = optionsString.split(",");
                options = getOptionsString(optionsString, options);
            }
            if ($(menu).length) {
                var len = $(m).find(".maso-item").length;
                $(menu).find("li a:not(.maso-filter-auto)").each(function () {
                    var cat = $(this).attr("data-filter");
                    var current_len = $(m).find("." + cat).length;
                    if ((current_len == len || current_len == 0) && (cat != "maso-item" && !isEmpty(cat))) {
                        $(this).closest("li").remove();
                    }
                });
            }
            $(m).isotope(options);
            if ($.isFunction($.fn.renderLoadedImgs)) {
                var items = m.isotope('getItemElements');
                $(m).renderLoadedImgs();
            }
            if ($(this).find("img").length == 0) {
                $(this).find(".maso-item").css("visibility", "visible");
                $(this).find(".maso-item > *").animate({ "opacity": 1 }, 300);
            }
            $(this).find(".pagination,.load-more").initPagination();
        }
    };

    //parallax.min.js
    $.fn.initParallax = function (img) {
        var bleed = $(this).attr("data-bleed");
        var pos = $(this).attr("data-position");
        var id = $(this).attr("id");
        if (isEmpty(bleed) && !(bleed == "0")) bleed = 70;
        if (isEmpty(pos)) pos = "center";
        var arr = { bleed: bleed, positionY: pos };
        if (!isEmpty(img)) {
            arr["imageSrc"] = img;
        }
        $(this).parallax(arr);
    }

    //magnific-popup.min.js
    $.fn.initMagnificPopup = function () {
        var obj = this;
        var optionsString = $(obj).attr("data-options");
        var trigger = $(obj).attr("data-trigger");
        if (isEmpty(trigger)) trigger = "";
        var a = $(obj).attr("data-lightbox-anima");
        var href = $(obj).attr("href");
        if (isEmpty(href)) href = "";
        if (isEmpty(a)) a = $(obj).parent().attr("data-lightbox-anima");
        var optionsArr;
        var options = {
            type: 'iframe',
            image: {
                cursor: 'mfp-zoom-out-cur mfp-active' + (!isEmpty($(this).attr("title")) ? " mfp-title-active" : ""),
            }
        }
        if (!isEmpty(optionsString)) {
            optionsArr = optionsString.split(",");
            options = getOptionsString(optionsString, options);
        }
        if (isEmpty(options['mainClass'])) options['mainClass'] = "";
        if (trigger == "load" || trigger == "scroll") {
            var l = $(obj).attr("data-link");
            var c = $(obj).attr("data-click");
            if (isEmpty(l)) { href = "#" + $(this).attr("id"); options['mainClass'] += ' custom-lightbox'; }
            else href = l;

            if (!isEmpty(c)) {
                $("body").on("click", ".lightbox-on-load", function () {
                    if ($(obj).attr("data-click-target") == "_blank") window.open(c);
                    else document.location = c;
                });
            }
        }

        if ($(obj).hasClass("grid-box") || $(obj).hasClass("maso-box")) {
            options["type"] = "image";
            options["delegate"] = "a.img-box,.cnt-box a:not(.img-box),.media-box";
            options["gallery"] = { enabled: 1 };
        }
        if ((href.indexOf(".jpg") != -1) || (href.indexOf(".png") != -1) || (href.indexOf("placehold.it") != -1)) {
            options['type'] = 'image';
        }
        if (href.indexOf("#") == 0) {
            options['type'] = 'inline';
            options['mainClass'] += ' box-inline';
            options['closeBtnInside'] = 0;
        }
        options["callbacks"] = {
            open: function () {
                var mfp_cnt = $('.mfp-content');
                if (!isEmpty(a)) {
                    $(mfp_cnt).showAnima(a);
                    $(mfp_cnt).css("opacity", 0);
                } else {
                    if ((!isEmpty(optionsString)) && optionsString.indexOf("anima:") != -1) {
                        $(mfp_cnt).showAnima(options['anima']);
                        $(mfp_cnt).css("opacity", 0);
                    }
                }
                if (href.indexOf("#") == 0) {
                    $(href).css("display", "block");
                }
                var gm = $(mfp_cnt).find(".google-map");
                if ($.isFunction($.fn.googleMap) && $(gm).length) $(gm).googleMap();
            },
            change: function (item) {
                var h = this.content;
                $('.mfp-container').removeClass("active");
                setTimeout(function () { $('.mfp-container').addClass("active"); }, 500);
                var gm = $(h).find(".google-map");
                if ($.isFunction($.fn.googleMap) && $(gm).length) $(gm).googleMap();
            },
            close: function () {
                if ($.isFunction($.fn.fullpage) && $.isFunction($.fn.fullpage.setMouseWheelScrolling)) $.fn.fullpage.setMouseWheelScrolling(true);
            }
        };
        if (trigger != "load" && trigger != "scroll") $(obj).magnificPopup(options);
        else {
            if (href.indexOf("#") == 0) $(href).css("display", "block");
            options['items'] = { 'src': href }
            options['mainClass'] += ' lightbox-on-load';
            $.magnificPopup.open(options);
        }
    };

    //slimscroll.min.js
    $.fn.restartSlimScroll = function () {
        var parent = $(this).parent();
        if ($(parent).hasClass("slimScrollDiv")) {
            $(this).removeData('slimScroll');
            $(parent).find(".slimScrollBar").remove();
            $(parent).find(".slimScrollRail").remove();
            $(parent).replaceWith(this);
        }
        $(this).initSlimScroll();
    }
    $.fn.initSlimScroll = function () {
        function getHeightFullscreen(t, wh) {
            if (vh == "fullscreen") {
                var h = wh;
                if (!isEmpty(lh) && ((wh - lh) > 150)) h = wh - lh;
                else h = wh - 100;
                vh = h;
            }
            return vh;
        }
        let disable_md = $(this).hasClass("disable-md");
        let disable_sm = $(this).hasClass("disable-sm");
        if ((disable_md && window_width > 992) || (disable_sm && window_width > 768) || (!disable_md && !disable_sm) || window_width > 992) {
            var vh = $(this).attr("data-height");
            var lh = $(this).attr("data-offset");
            var railColor = $(this).attr("data-rail-color");
            var barColor = $(this).attr("data-bar-color");
            var optionsString = $(this).attr("data-options");
            var optionsArr;
            var options = {
                height: 0,
                size: '4px',
                railVisible: true,
                alwaysVisible: true,
                color: (isEmpty(barColor) ? '#888888' : barColor),
                railColor: (isEmpty(railColor) ? '#e6e6e6' : railColor),
                railOpacity: 1
            }
            if (!isEmpty(optionsString)) {
                optionsArr = optionsString.split(",");
                options = getOptionsString(optionsString, options);
            }
            if (window_width < 993) options['alwaysVisible'] = true;
            if (vh == "fullscreen") {
                let h;
                if (!isEmpty(lh) && ((window_height - lh) > 150)) h = window_height - lh;
                else h = window_height - 100;
                vh = h;
            }
            var lh = $(this).attr("data-height-remove");
            if (isEmpty(lh)) lh = 0;
            vh += "";

            if ((vh.indexOf("#") != -1) || (vh.indexOf(".") != -1)) {
                $(this).css("height", "0px").css("overflow", "hidden");
                vh = "" + ($(this).closest(vh).height() - lh);
                $(this).css("height", "").css("overflow", "");
            }

            options['height'] = vh + "px";
            $(this).slimScroll(options);

            var gm = $(this).find(".google-map");
            if ($.isFunction($.fn.googleMap) && $(gm).length) $(gm).googleMap();

            if (!options['alwaysVisible']) $(".slimScrollBar").hide();
            if (window_width < 993) $(this).find(".slimScrollBar").css("height", "50px");
            $(this).on("mousewheel DOMMouseScroll", function (n) { n.preventDefault() });
            $(this).slimScroll().bind('slimscroll', function (e, pos) {
                if (pos != "scrolling") {
                    $(this).removeClass("scroll-pos-top").removeClass("scroll-pos-bottom");
                }
                $(this).addClass("scroll-pos-" + pos);
            });
        }
    }
}(jQuery));

jQuery.fn.extend({
    cssInt: function (prop) {
        return parseInt(this.css(prop), 10) || 0;
    },
    hasAnyClass: function () {
        for (var i = 0; i < arguments.length; i++) {
            if (this.hasClass(arguments[i])) {
                return true;
            }
        }
        return false;
    }
});