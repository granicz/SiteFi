$(document).ready(function() {

    // Scroll View Demo
    $(".header").on("click",".demo", function (event) {
        event.preventDefault();
        var id  = $(this).attr('href'),
            top = $(id).offset().top;
        $('body,html').animate({scrollTop: top}, 1500);
    });

    // SVG Man
    if(document.getElementById('errorman')){
        var svg = Snap("#errorman");
        Snap.load("assets/404/img/man.svg", function (f) {
            g = f.select("g");
            g.attr({
                transform: 't0,0 s1'
            });
            svg.append(g);
        });
    }
    
});