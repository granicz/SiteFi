"use strict";
!function (t) { "function" == typeof define && define.amd ? define(["jquery"], t) : "object" == typeof exports ? t(require("jquery")) : t(jQuery) }(function (t) { var e = function (i, n) { this.$element = t(i), this.options = t.extend({}, e.DEFAULTS, this.dataOptions(), n), this.init() }; e.DEFAULTS = { from: 0, to: 0, speed: 1e3, refreshInterval: 100, decimals: 0, formatter: function (t, e) { return t.toFixed(e.decimals) }, onUpdate: null, onComplete: null }, e.prototype.init = function () { this.value = this.options.from, this.loops = Math.ceil(this.options.speed / this.options.refreshInterval), this.loopCount = 0, this.increment = (this.options.to - this.options.from) / this.loops }, e.prototype.dataOptions = function () { var t = { from: this.$element.data("from"), to: this.$element.data("to"), speed: this.$element.data("speed"), refreshInterval: this.$element.data("refresh-interval"), decimals: this.$element.data("decimals") }, e = Object.keys(t); for (var i in e) { var n = e[i]; void 0 === t[n] && delete t[n] } return t }, e.prototype.update = function () { this.value += this.increment, this.loopCount++ , this.render(), "function" == typeof this.options.onUpdate && this.options.onUpdate.call(this.$element, this.value), this.loopCount >= this.loops && (clearInterval(this.interval), this.value = this.options.to, "function" == typeof this.options.onComplete && this.options.onComplete.call(this.$element, this.value)) }, e.prototype.render = function () { var t = this.options.formatter.call(this.$element, this.value, this.options); this.$element.text(t) }, e.prototype.restart = function () { this.stop(), this.init(), this.start() }, e.prototype.start = function () { this.stop(), this.render(), this.interval = setInterval(this.update.bind(this), this.options.refreshInterval) }, e.prototype.stop = function () { this.interval && clearInterval(this.interval) }, e.prototype.toggle = function () { this.interval ? this.stop() : this.start() }, t.fn.countTo = function (i) { return this.each(function () { var n = t(this), s = n.data("countTo"), a = "object" == typeof i ? i : {}, r = "string" == typeof i ? i : "start"; (!s || "object" == typeof i) && (s && s.stop(), n.data("countTo", s = new e(this, a))), s[r].call(s) }) } }), function (t) { if ("function" == typeof define && define.amd) define(["jquery"], t); else if ("object" == typeof module && module.exports) { var e = require("jquery"); t(e), module.exports = e } else t(jQuery) }(function (t) { function e(t) { this.init(t) } e.prototype = { value: 0, size: 100, startAngle: -Math.PI, thickness: "auto", fill: { gradient: ["#3aeabb", "#fdd250"] }, emptyFill: "rgba(0, 0, 0, .1)", animation: { duration: 1200, easing: "circleProgressEasing" }, animationStartValue: 0, reverse: !1, lineCap: "butt", insertMode: "prepend", constructor: e, el: null, canvas: null, ctx: null, radius: 0, arcFill: null, lastFrameValue: 0, init: function (e) { t.extend(this, e), this.radius = this.size / 2, this.initWidget(), this.initFill(), this.draw(), this.el.trigger("circle-inited") }, initWidget: function () { this.canvas || (this.canvas = t("<canvas>")["prepend" == this.insertMode ? "prependTo" : "appendTo"](this.el)[0]); var e = this.canvas; if (e.width = this.size, e.height = this.size, this.ctx = e.getContext("2d"), window.devicePixelRatio > 1) { var i = window.devicePixelRatio; e.style.width = e.style.height = this.size + "px", e.width = e.height = this.size * i, this.ctx.scale(i, i) } }, initFill: function () { var e, i = this, n = this.fill, s = this.ctx, a = this.size; if (!n) throw Error("The fill is not specified!"); if ("string" == typeof n && (n = { color: n }), n.color && (this.arcFill = n.color), n.gradient) { var r = n.gradient; if (1 == r.length) this.arcFill = r[0]; else if (r.length > 1) { for (var o = n.gradientAngle || 0, l = n.gradientDirection || [a / 2 * (1 - Math.cos(o)), a / 2 * (1 + Math.sin(o)), a / 2 * (1 + Math.cos(o)), a / 2 * (1 - Math.sin(o))], h = s.createLinearGradient.apply(s, l), c = 0; c < r.length; c++) { var d = r[c], f = c / (r.length - 1); t.isArray(d) && (f = d[1], d = d[0]), h.addColorStop(f, d) } this.arcFill = h } } function u() { var n = t("<canvas>")[0]; n.width = i.size, n.height = i.size, n.getContext("2d").drawImage(e, 0, 0, a, a), i.arcFill = i.ctx.createPattern(n, "no-repeat"), i.drawFrame(i.lastFrameValue) } n.image && (n.image instanceof Image ? e = n.image : (e = new Image).src = n.image, e.complete ? u() : e.onload = u) }, draw: function () { this.animation ? this.drawAnimated(this.value) : this.drawFrame(this.value) }, drawFrame: function (t) { this.lastFrameValue = t, this.ctx.clearRect(0, 0, this.size, this.size), this.drawEmptyArc(t), this.drawArc(t) }, drawArc: function (t) { if (0 !== t) { var e = this.ctx, i = this.radius, n = this.getThickness(), s = this.startAngle; e.save(), e.beginPath(), this.reverse ? e.arc(i, i, i - n / 2, s - 2 * Math.PI * t, s) : e.arc(i, i, i - n / 2, s, s + 2 * Math.PI * t), e.lineWidth = n, e.lineCap = this.lineCap, e.strokeStyle = this.arcFill, e.stroke(), e.restore() } }, drawEmptyArc: function (t) { var e = this.ctx, i = this.radius, n = this.getThickness(), s = this.startAngle; t < 1 && (e.save(), e.beginPath(), t <= 0 ? e.arc(i, i, i - n / 2, 0, 2 * Math.PI) : this.reverse ? e.arc(i, i, i - n / 2, s, s - 2 * Math.PI * t) : e.arc(i, i, i - n / 2, s + 2 * Math.PI * t, s), e.lineWidth = n, e.strokeStyle = this.emptyFill, e.stroke(), e.restore()) }, drawAnimated: function (e) { var i = this, n = this.el, s = t(this.canvas); s.stop(!0, !1), n.trigger("circle-animation-start"), s.css({ animationProgress: 0 }).animate({ animationProgress: 1 }, t.extend({}, this.animation, { step: function (t) { var s = i.animationStartValue * (1 - t) + e * t; i.drawFrame(s), n.trigger("circle-animation-progress", [t, s]) } })).promise().always(function () { n.trigger("circle-animation-end") }) }, getThickness: function () { return t.isNumeric(this.thickness) ? this.thickness : this.size / 14 }, getValue: function () { return this.value }, setValue: function (t) { this.animation && (this.animationStartValue = this.lastFrameValue), this.value = t, this.draw() } }, t.circleProgress = { defaults: e.prototype }, t.easing.circleProgressEasing = function (t) { return t < .5 ? .5 * (t *= 2) * t * t : 1 - .5 * (t = 2 - 2 * t) * t * t }, t.fn.circleProgress = function (i, n) { var s = "circle-progress", a = this.data(s); if ("widget" == i) { if (!a) throw Error('Calling "widget" method on not initialized instance is forbidden'); return a.canvas } if ("value" == i) { if (!a) throw Error('Calling "value" method on not initialized instance is forbidden'); if (void 0 === n) return a.getValue(); var r = arguments[1]; return this.each(function () { t(this).data(s).setValue(r) }) } return this.each(function () { var n = t(this), a = n.data(s), r = t.isPlainObject(i) ? i : {}; if (a) a.init(r); else { var o = t.extend({}, n.data()); "string" == typeof o.fill && (o.fill = JSON.parse(o.fill)), "string" == typeof o.animation && (o.animation = JSON.parse(o.animation)), (r = t.extend(o, r)).el = n, a = new e(r), n.data(s, a) } }) } }), function (t) { t.fn.downCount = function (e, i) { var n = t.extend({ date: null, offset: null }, e); n.date || t.error("Date is not defined."), Date.parse(n.date) || t.error("Incorrect date format, it should look like this, 12/24/2012 12:00:00."); var s = this, a = setInterval(function () { var t = new Date(n.date) - function () { var t = new Date, e = t.getTime() + 6e4 * t.getTimezoneOffset(); return new Date(e + 36e5 * n.offset) }(); if (t < 0) return clearInterval(a), void (i && "function" == typeof i && i()); var e = Math.floor(t / 864e5), r = Math.floor(t % 864e5 / 36e5), o = Math.floor(t % 36e5 / 6e4), l = Math.floor(t % 6e4 / 1e3), h = 1 === (e = String(e).length >= 2 ? e : "0" + e) ? "day" : "days", c = 1 === (r = String(r).length >= 2 ? r : "0" + r) ? "hour" : "hours", d = 1 === (o = String(o).length >= 2 ? o : "0" + o) ? "minute" : "minutes", f = 1 === (l = String(l).length >= 2 ? l : "0" + l) ? "second" : "seconds"; s.find(".days").text(e), s.find(".hours").text(r), s.find(".minutes").text(o), s.find(".seconds").text(l), s.find(".days_ref").text(h), s.find(".hours_ref").text(c), s.find(".minutes_ref").text(d), s.find(".seconds_ref").text(f) }, 1e3) } }(jQuery);

/*
* ===========================================================
* PROGRESS BAR - CIRCLE PROGRESS BAR - COUNTER - COUNTDOWN - THEMEKIT
* ===========================================================
* This script manage the following component: progress bar, circle progress bar, counter and countdown.
* 
* Copyright (c) Federico Schiocchet - schiocco.com - Themekit
*/

(function ($) {
    var progress_circle;
    $.fn.init_progress_all = function () {
        var t = this;
        $(t).find("[data-time]").each(function () {
            $(this).progressCountdown();
        });
        $(window).scroll(function () {
            $(t).find("[data-to]").each(function () {
                if (isScrollView(this)) {
                    var trigger = $(this).attr("data-trigger");
                    if (isEmpty(trigger) || trigger == "scroll") {
                        $(this).progressCounter();
                    }
                }
            });
            $(t).find(".progress-bar").each(function () {
                if (isScrollView(this)) {
                    var trigger = $(this).attr("data-trigger");
                    if (isEmpty(trigger) || trigger == "scroll") {
                        $(this).find("[data-progress]").progressBar();
                    }
                }
            });
            progress_circle = $(t).find(".progress-circle");
            $(progress_circle).each(function () {
                if (isScrollView(this)) {
                    var trigger = $(this).attr("data-trigger");
                    if (isEmpty(trigger) || trigger == "scroll") {
                        $(this).progressCircle();
                    }
                }
            });
        })
    }
    $.fn.progressCountdown = function () {
        var date = $(this).attr("data-time");
        if (!isEmpty(date)) {
            $(this).downCount({
                date: date,
                offset: $(this).attr("data-utc-offset")
            });
        }
    }
    $.fn.progressCounter = function () {
        var separator = $(this).attr("data-separator");
        var unit = $(this).attr("data-unit");
        if (separator == null) separator = "";
        if (unit == null) unit = "";
        else unit = " " + unit;
        $(this).countTo({
            formatter: function (value, options) {
                return value.toFixed(options.decimals).replace(/\B(?=(?:\d{3})+(?!\d))/g, separator) + unit;
            }
        });
        $(this).attr("data-trigger", "null");
    }
    $.fn.progressBar = function () {
        $(this).css("width", 0);
        $(this).css("width", $(this).attr("data-progress") + "%");
        $(this).attr("data-trigger", "null");
    }

    $.fn.progressCircle = function () {
        var optionsArr;
        var optionsString = $(this).attr("data-options");
        var size = progress_circle_get_size(this);
        var color = $(this).attr("data-color");
        var unit = $(this).attr("data-unit");
        var thickness = $(this).attr("data-thickness");
        var lineCap = $(this).attr("data-linecap");
        if (isEmpty(color)) color = "#565656";
        if (isEmpty(thickness)) thickness = 2;
        if (unit == null) unit = "%";

        var options = {
            value: parseInt($(this).attr("data-progress"), 10) / 100,
            size: size,
            lineCap: lineCap,
            fill: {
                gradient: [color, color]
            },
            thickness: thickness,
            startAngle: -Math.PI / 2
        }
        if (!isEmpty(optionsString)) {
            optionsArr = optionsString.split(",");
            options = getOptionsString(optionsString, options);
        }
        $(this).circleProgress(options);
        $(this).css("width", size + "px").attr("data-trigger", "null");
    }
    $(window).resize(function () {
        $(progress_circle).each(function () {
            var size = progress_circle_get_size(this);
            $(this).css("width", size + "px").find("canvas").css("height", size + "px").css("width", size + "px");
        });
    });
    function progress_circle_get_size(t) {
        var window_width = $(window).width();
        var size = $(t).attr("data-size");
        if (window_width < 994) {
            size = $(t).attr("data-size-md");
        }
        if (window_width < 768) {
            size = $(t).attr("data-size-sm");
        }
        if (isEmpty(size) || size == "auto") size = $(t).outerWidth();
        if (isEmpty(size)) size = $(t).parent().width();
        if (isEmpty(size)) size = $(t).attr("data-size");
        return size;
    }
}(jQuery));