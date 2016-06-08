$(function () {
    var id = $(".row").data('image-id');
    setInterval(function() {
        $.get('/home/getcount', { id: id }, function(result) {
            $("#viewCount").text(result.viewCount);
        });
    }, 500);
});