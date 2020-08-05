/*
* ===========================================================
* GOOGLE MAPS - THEMEKIT
* ===========================================================
* Google Maps script for show Google maps in different ways
* The script require maps.googleapis.com/maps/api/js script
* 
* Copyright (c) Federico Schiocchet - schiocco.com - Themekit
*/
 
"use strict";
(function ($) {
    $.fn.googleMap = function (n) {
        if ($(this).hasClass("google-map") && $(this).attr("data-trigger") != "initialized") {
            var arrGmap = getAttrs(this);
            if (isEmpty(arrGmap["coords"])) {
                initializeGMapByAdress(this);
            } else initializeGMap(this);
            $(this).attr("data-trigger", "initialized");
        }
    };
    function getAttrs(target) {
        var arrGmap = {};
        arrGmap["coords"] = $(target).attr("data-coords");
        arrGmap["address"] = $(target).attr("data-address");
        arrGmap["marker"] = $(target).attr("data-marker");
        arrGmap["marker-pos"] = $(target).attr("data-marker-pos");
        arrGmap["marker-pos-top"] = $(target).attr("data-marker-offset-top");
        arrGmap["marker-pos-left"] = $(target).attr("data-marker-offset-left");
        arrGmap["skin"] = $(target).attr("data-skin");
        arrGmap["zoom"] = $(target).attr("data-zoom");
        if (isEmpty(arrGmap["zoom"])) arrGmap["zoom"] = 12;
        if (isEmpty(arrGmap["marker-pos-top"])) arrGmap["marker-pos-top"] = 0;
        if (isEmpty(arrGmap["marker-pos-left"])) arrGmap["marker-pos-left"] = 0;
        return arrGmap;
    }
    function initializeGMap(target) {
        var arrGmap = getAttrs(target);
        var cx = parseFloat(arrGmap["coords"].split(",")[0]);
        var cy = parseFloat(arrGmap["coords"].split(",")[1]);
        var geocoder, GMap;
        geocoder = new google.maps.Geocoder();
        var latlng = new google.maps.LatLng(cx, cy);
        var mapOptions = {
            zoom: parseInt(arrGmap["zoom"], 10),
            scrollwheel: false,
            center: latlng
        }
        GMap = new google.maps.Map(target.get(0), mapOptions);
        if (!isEmpty(arrGmap["skin"])) GMap.setOptions({ styles: arrSkins[arrGmap["skin"]] });
        var arrMarker = {
            position: latlng,
            map: GMap,
        };
        if (!isEmpty(arrGmap["marker"])) arrMarker = { position: latlng, map: GMap, icon: arrGmap["marker"] };
        var marker = new google.maps.Marker(arrMarker);

        var x = 0 - arrGmap["marker-pos-left"];
        var y = 0 - arrGmap["marker-pos-top"];
        var w = $(window).width();
        if (!isEmpty(arrGmap["marker-pos"]) && w > 768) {
            var width_move = $(target).parent().find(".container").outerWidth();
            if (arrGmap["marker-pos"] == 'right') {
                x = x - 1 * width_move / 2;
            }
            if (arrGmap["marker-pos"] == 'left') {
                x = x + width_move / 2;
            }
        }
        if (w < 769) x = 0;
        else GMap.panBy(x, y);
    }
    function initializeGMapByAdress(target) {
        var arrGmap = getAttrs(target);
        var geocoder;
        geocoder = new google.maps.Geocoder();
        geocoder.geocode({ 'address': arrGmap["address"] }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                $(target).attr("data-coords", results[0].geometry.location.lat() + "," + results[0].geometry.location.lng());
                initializeGMap(target);
            }
        });
    }
    $(document).ready(function () {
        $(".google-map").each(function (index) {
            if ($(this).attr("data-trigger") != "manual") $(this).googleMap();
        });
    });
    var arrSkins = {
        "gray": [{ "featureType": "water", "elementType": "geometry", "stylers": [{ "color": "#e9e9e9" }, { "lightness": 17 }] }, { "featureType": "landscape", "elementType": "geometry", "stylers": [{ "color": "#f5f5f5" }, { "lightness": 20 }] }, { "featureType": "road.highway", "elementType": "geometry.fill", "stylers": [{ "color": "#ffffff" }, { "lightness": 17 }] }, { "featureType": "road.highway", "elementType": "geometry.stroke", "stylers": [{ "color": "#ffffff" }, { "lightness": 29 }, { "weight": 0.2 }] }, { "featureType": "road.arterial", "elementType": "geometry", "stylers": [{ "color": "#ffffff" }, { "lightness": 18 }] }, { "featureType": "road.local", "elementType": "geometry", "stylers": [{ "color": "#ffffff" }, { "lightness": 16 }] }, { "featureType": "poi", "elementType": "geometry", "stylers": [{ "color": "#f5f5f5" }, { "lightness": 21 }] }, { "featureType": "poi.park", "elementType": "geometry", "stylers": [{ "color": "#dedede" }, { "lightness": 21 }] }, { "elementType": "labels.text.stroke", "stylers": [{ "visibility": "on" }, { "color": "#ffffff" }, { "lightness": 16 }] }, { "elementType": "labels.text.fill", "stylers": [{ "saturation": 36 }, { "color": "#333333" }, { "lightness": 40 }] }, { "elementType": "labels.icon", "stylers": [{ "visibility": "off" }] }, { "featureType": "transit", "elementType": "geometry", "stylers": [{ "color": "#f2f2f2" }, { "lightness": 19 }] }, { "featureType": "administrative", "elementType": "geometry.fill", "stylers": [{ "color": "#fefefe" }, { "lightness": 20 }] }, { "featureType": "administrative", "elementType": "geometry.stroke", "stylers": [{ "color": "#fefefe" }, { "lightness": 17 }, { "weight": 1.2 }] }],
        "black": [{ "featureType": "all", "elementType": "labels.text.fill", "stylers": [{ "saturation": 36 }, { "color": "#000000" }, { "lightness": 40 }] }, { "featureType": "all", "elementType": "labels.text.stroke", "stylers": [{ "visibility": "on" }, { "color": "#000000" }, { "lightness": 16 }] }, { "featureType": "all", "elementType": "labels.icon", "stylers": [{ "visibility": "off" }] }, { "featureType": "administrative", "elementType": "geometry.fill", "stylers": [{ "color": "#000000" }, { "lightness": 20 }] }, { "featureType": "administrative", "elementType": "geometry.stroke", "stylers": [{ "color": "#000000" }, { "lightness": 17 }, { "weight": 1.2 }] }, { "featureType": "landscape", "elementType": "geometry", "stylers": [{ "color": "#000000" }, { "lightness": 20 }] }, { "featureType": "poi", "elementType": "geometry", "stylers": [{ "color": "#000000" }, { "lightness": 21 }] }, { "featureType": "road.highway", "elementType": "geometry.fill", "stylers": [{ "color": "#000000" }, { "lightness": 17 }] }, { "featureType": "road.highway", "elementType": "geometry.stroke", "stylers": [{ "color": "#000000" }, { "lightness": 29 }, { "weight": 0.2 }] }, { "featureType": "road.arterial", "elementType": "geometry", "stylers": [{ "color": "#000000" }, { "lightness": 18 }] }, { "featureType": "road.local", "elementType": "geometry", "stylers": [{ "color": "#000000" }, { "lightness": 16 }] }, { "featureType": "transit", "elementType": "geometry", "stylers": [{ "color": "#000000" }, { "lightness": 19 }] }, { "featureType": "water", "elementType": "geometry", "stylers": [{ "color": "#000000" }, { "lightness": 17 }] }],
        "green": [{ "featureType": "water", "elementType": "geometry", "stylers": [{ "visibility": "on" }, { "color": "#aee2e0" }] }, { "featureType": "landscape", "elementType": "geometry.fill", "stylers": [{ "color": "#abce83" }] }, { "featureType": "poi", "elementType": "geometry.fill", "stylers": [{ "color": "#769E72" }] }, { "featureType": "poi", "elementType": "labels.text.fill", "stylers": [{ "color": "#7B8758" }] }, { "featureType": "poi", "elementType": "labels.text.stroke", "stylers": [{ "color": "#EBF4A4" }] }, { "featureType": "poi.park", "elementType": "geometry", "stylers": [{ "visibility": "simplified" }, { "color": "#8dab68" }] }, { "featureType": "road", "elementType": "geometry.fill", "stylers": [{ "visibility": "simplified" }] }, { "featureType": "road", "elementType": "labels.text.fill", "stylers": [{ "color": "#5B5B3F" }] }, { "featureType": "road", "elementType": "labels.text.stroke", "stylers": [{ "color": "#ABCE83" }] }, { "featureType": "road", "elementType": "labels.icon", "stylers": [{ "visibility": "off" }] }, { "featureType": "road.local", "elementType": "geometry", "stylers": [{ "color": "#A4C67D" }] }, { "featureType": "road.arterial", "elementType": "geometry", "stylers": [{ "color": "#9BBF72" }] }, { "featureType": "road.highway", "elementType": "geometry", "stylers": [{ "color": "#EBF4A4" }] }, { "featureType": "transit", "stylers": [{ "visibility": "off" }] }, { "featureType": "administrative", "elementType": "geometry.stroke", "stylers": [{ "visibility": "on" }, { "color": "#87ae79" }] }, { "featureType": "administrative", "elementType": "geometry.fill", "stylers": [{ "color": "#7f2200" }, { "visibility": "off" }] }, { "featureType": "administrative", "elementType": "labels.text.stroke", "stylers": [{ "color": "#ffffff" }, { "visibility": "on" }, { "weight": 4.1 }] }, { "featureType": "administrative", "elementType": "labels.text.fill", "stylers": [{ "color": "#495421" }] }, { "featureType": "administrative.neighborhood", "elementType": "labels", "stylers": [{ "visibility": "off" }] }]
    };

}(jQuery));

