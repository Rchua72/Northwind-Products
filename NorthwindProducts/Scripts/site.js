(function() {
    var rowID;

    $('#confirm-delete').on('show.bs.modal', function (e) {
        var target = $(e.relatedTarget);
        //var rowIDHiddenField = $('#rowID');
        var pos = target.attr("id").indexOf("_");
        rowID = target.attr("id").slice(pos + 1);
        $(this).find('.danger').attr('href', $(e.relatedTarget).data('href'));
    });

    $('.danger').on('click', function (e) {
        //var rowIDHiddenField = $('#rowID');
        var token = $(':input:hidden[name*="RequestVerificationToken"]');
        var data = {};
        data[token.attr('name')] = token.val();
        data["id"] = rowID;
        deleteUrl = '/product/delete';
        $.ajax({
            url: deleteUrl,
            type: 'POST',
            data: data,
            success: function (result) {
                location.reload();
            },
            error: function () {
                location.reload();
            }
        });
        $('#confirm-delete').modal('hide');
    });

}())