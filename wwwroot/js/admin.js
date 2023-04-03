$('.deleteColor').off('click').on('click', function () {
    const id = $(this).data('id');
    const productId = $('#productId').val();
    $.ajax({
        type: "GET",
        url: '/Admin/Product/DeleteProductColor',
        data: {
            id: id,
        },
        success: function (res) {
            if (res.status) {
                window.location.href = '/Admin/Product/CreateProductColor?id=' + productId + '';
            }
        },
        error: function (err) {
            console.log(err);
        }
    });
});