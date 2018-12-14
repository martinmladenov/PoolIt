function fillLocation(imageId, fieldId) {
    var image = $(imageId);
    image.addClass('fa-spin');

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (pos) {
                $.ajax({
                    url: '/api/location/gettownname',
                    type: "GET",
                    data: {
                        latitude: pos.coords.latitude,
                        longitude: pos.coords.longitude
                    },
                    success: function (result) {
                        $(fieldId).val(result.townName);
                        image.removeClass('fa-spin');
                    }
                });
            },
            function (error) {
                var text;

                switch (error.code) {
                    case error.PERMISSION_DENIED:
                        text = "You've denied the geolocation request. Check your browser's settings.";
                        break;
                    case error.POSITION_UNAVAILABLE:
                        text = "Location information is unavailable. Try again later.";
                        break;
                    case error.TIMEOUT:
                        text = "The location request took too long. Try again later.";
                        break;
                    default:
                        text = "An error occurred. Try again later.";
                        break;
                }

                showPopover(image, text);
            });
    } else {
        showPopover(image, "Your browser doesn't support geolocation.");
    }
}

function showPopover(target, text) {
    target.popover({
        content: text
    });
    target.popover('show');
    target.removeClass('fa-spin');
}