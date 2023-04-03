var SiteController = function () {
    this.initialize = function () {
        registerEvent()
        loadCart();
    }

    function loadCart() {
        $.ajax({
            type: "GET",
            url: '/Cart/GetCartItem',
            success: function (res) {
                $('#numberCartItem').text(res.length);
            }
        });
    }
    function registerEvent() {
        $('body').on('click', '#addToCart', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            const colorId = $(this).data('infor');
            const quantity = $('#inputQuantity').val();
            $.ajax({
                type: "POST",
                url: '/Cart/AddToCart',
                data: {
                    id: id,
                    colorId: colorId,
                    quantity: quantity
                },
                success: function (res) {
                    $('#numberCartItem').text(res.length);
                    window.location.href = "/Cart/Index"
                },
                error: function (err) {
                    console.log(err);
                }
            });
        });
        $('body').on('click', '#searchOrder', function (e) {
            e.preventDefault();
            const id = $('#inputSearchOrder').val();
            $.ajax({
                type: "GET",
                url: '/Cart/CheckOrder',
                data: {
                    id: id,
                    search: "ok"
                },
                success: function (res) {
                    window.location.href = '/Cart/CheckOrder/'+ id
                },
                error: function (err) {
                    console.log(err);
                }
            });
        });
    }
}
