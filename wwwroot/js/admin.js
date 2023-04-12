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
$('#updateStatus').off('click').on('click', function () {
    const id = $('#orderId').val();
    var status = $('#SelectLm').prop('selected', true).val();
    $.ajax({
        type: "POST",
        url: '/Admin/Order/Update',
        data: {
            id: id,
            status
        },
        success: function (res) {
            if (res.status) {
                window.location.href = '/Admin/Order/Update/' + id;
            }
        },
        error: function (err) {
            console.log(err);
        }
    });
});
$('.updateQuantityProColor').off('click').on('click', function () {
    const productId = $('#productId').val();
    const id = $(this).data('id');
    const quantity = $('#input_' + id + '').val();
    $.ajax({
        type: "POST",
        url: '/Admin/Product/UpdateProductColor',
        data: {
            id: id,
            quantity: quantity
        },
        success: function (res) {
            if (res.status) {
                window.location.href = '/Admin/Product/CreateProductColor/' + productId;
            }
        },
        error: function (err) {
            console.log(err);
        }
    });
});
