var CartController = function () {
    this.initialize = function () {
        registerEvent();
        loadCart();
    }
    function registerEvent() {
        $('body').on('click', '.cart_quantity_up', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            const colorId = $(this).data('infor');
            const quantity = parseInt($('#input_' + id + colorId + '').val()) + 1;
            const quantityMax = parseInt($('#inputMax_' + id + colorId + '').val())
            if (quantity <= quantityMax) {
                updateCart(id, colorId, quantity);
            }
            else {
                alert("Chỉ còn " + quantityMax + " sản phẩm");
            }
        });
        $('body').on('click', '.cart_quantity_down', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            const colorId = $(this).data('infor');
            const quantity = parseInt($('#input_' + id + colorId +'').val()) - 1;
            updateCart(id, colorId, quantity);
        });
        $('body').on('click', '.cart_quantity_delete', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            const colorId = $(this).data('infor');
            updateCart(id, colorId, 0);
        });
    }
    function updateCart(id, colorId, quantity) {
        $.ajax({
            type: "POST",
            url: '/Cart/UpdateCart',
            data: {
                id: id,
                colorId: colorId,
                quantity: quantity
            },
            success: function (res) {
                $('#input_' + id + '').val(quantity);
                $('#numberCartItem').text(res.length);
                loadCart();
            },
            error: function (err) {
                console.log(err);
            }
        });
    }
    function loadCart() {
        $.ajax({
            type: "GET",
            url: '/Cart/GetCartItem',
            success: function (res) {
                var html = '';
                var totalPayment = 0;
                $.each(res, function (i, item) {
                    var amount = item.price * item.quantity;
                    if (item.quantity > item.quantityMax) {
                        item.quantity = item.quantityMax;
                    }
                    html+= "<tr>"
                        + "<td style=\"width: 40px;\" class=\"cart_product\">"
                        + "<img style=\"width: 100%;\" src=\"/product-image/" + item.image + "\" />"
                        +"</td>"
                        + "<td class=\"cart_description\" style=\"width: 270px;\">"
                        + "<h4 class=\"shortName\"><a href=\"/chi-tiet-san-pham?id=" + item.productId + "&colorId=" + item.colorId + "\" style=\"font-size: 12px;\">" + item.name + "</a></h4>"
                        + "<p>Màu sắc: "+ item.colorName+"</p>"
                        + "</td>"
                        + "<td class=\"cart_price\">"
                        + "<p style=\"margin-top: 12px;font-size: 17px;\">" + numberWithCommas(item.price) + "<sup>đ</sup></p>"
                        + "</td>"
                        + "<td class=\"cart_quantity\">"
                        + "<div class=\"cart_quantity_button\" style=\"width: 80px;\">"
                        + "<button style=\"width: 25px;\" data-id=\"" + item.productId + "\" data-infor=\"" + item.colorId + "\" class=\"cart_quantity_down\" > - </button>"
                        + "<input id=\"input_" + item.productId + item.colorId + "\" style=\"width: 30px;font-size:14px;\" class=\"cart_quantity_input\" type=\"text\" name=\"quantity\" value=\"" + item.quantity + "\" min=\"1\" max=\"" + item.quantityMax + "\">"
                        + "<input id=\"inputMax_" + item.productId + item.colorId + "\" type=\"hidden\" value=\"" + item.quantityMax + "\"/>"
                        + "<button style=\"width: 25px;\" data-id=\"" + item.productId + "\" data-infor=\"" + item.colorId + "\" class=\"cart_quantity_up\" > + </button>"
                        +    "</div>"
                        +"</td>"
                        +"<td class=\"cart_total\">"
                        + "<p style=\"margin-top: 12px;font-size: 17px;\" class=\"cart_total_price\">" + numberWithCommas(amount) +"<sup>đ</sup></p>"
                        +"</td>"
                        +"<td style=\"width: 30px;\" class=\"cart_delete\">"
                        + "<a data-id=\"" + item.productId + "\" data-infor=\"" + item.colorId + "\" class=\"cart_quantity_delete\" ><i class=\"fa fa-times\"></i></a>"
                        +"</td>"
                        + "</tr>"
                    totalPayment += amount;
                });
                $('#cartBody').html(html);
                $('#numberCartItem').text(res.length);
                $('#totalPayment').text(numberWithCommas(totalPayment));
            }
        });
    }
    function numberWithCommas(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
}