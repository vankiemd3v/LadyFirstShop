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
    function IsEmail(email) {
        var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        if (!regex.test(email)) {
            return false;
        } else {
            return true;
        }
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
                    window.location.href = "/gio-hang"
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
                url: '/don-hang',
                data: {
                    id: id,
                    search: "ok"
                },
                success: function (res) {
                    if (res.status == true) {
                        window.location.href = '/don-hang?id=' + id
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            });
        });
        $('body').on('click', '#filterProduct', function (e) {
            e.preventDefault();
            const price = $('#sl2').data('slider').getValue();
            const id = $('#branId').val();
            $.ajax({
                type: "GET",
                url: '/san-pham',
                data: {
                    id: id,
                    min: price[0],
                    max: price[1]
                },
                success: function (res) {
                    window.location.href = '/san-pham?id=' + id + '&min=' + price[0] + '&max=' + price[1] + '';
                },
                error: function (err) {
                    console.log(err);
                }
            });
        });
        $('body').on('click', '#btnOrder', function (e) {
            e.preventDefault();
            $('#btnOrder').attr('disabled', true);
            var result;
            const shipName = $('#shipName').val();
            const shipPhoneNumber = $('#shipPhoneNumber').val();
            const shipAddress = $('#shipAddress').val();
            const shipEmail = $('#shipEmail').val();
            const shipNote = $('#shipNote').val();
            if (shipName == "") {
                $('#btnOrder').attr('disabled', false);
                $('#shipNameValid').text('Vui lòng nhập họ và tên');
            } else {
                $('#shipNameValid').text('');
            }
            if (shipAddress == "") {
                $('#btnOrder').attr('disabled', false);
                $('#shipAddressValid').text('Vui lòng nhập địa chỉ nhận hàng');
            } else {
                $('#shipAddressValid').text('');
            }
            if (shipEmail == "") {
                $('#btnOrder').attr('disabled', false);
                $('#shipEmailValid').text('Vui lòng nhập Email');
            } else {
                result = IsEmail($('#shipEmail').val());
                if (result) {
                    $('#shipEmailValid').text('');
                } else {
                    $('#btnOrder').attr('disabled', false);
                    $('#shipEmailValid').text('Email không đúng định dạng');
                }
            }
            if (shipPhoneNumber == "") {
                $('#btnOrder').attr('disabled', false);
                $('#shipPhoneNumberValid').text('Vui lòng nhập số điện thoại');
            } else {
                $('#shipPhoneNumberValid').text('');
            }
            if (shipName != "" && shipEmail != "" && shipAddress != "" && shipPhoneNumber != "" && result == true) {
                setTimeout("$('body').toggleClass('loading')", 1);
                $.ajax({
                    type: "POST",
                    url: '/gio-hang',
                    data: {
                        shipName: shipName,
                        shipPhoneNumber: shipPhoneNumber,
                        shipAddress: shipAddress,
                        shipEmail: shipEmail,
                        shipNote: shipNote
                    },
                    success: function (res) {
                        if (res.cart == false) {
                            setTimeout("$('body').toggleClass('loading')", 1);
                            $('#btnOrder').attr('disabled', false);
                            $('#messCart').text('Chưa có sản phẩm trong giỏ hàng');
                        }
                        if (res.outstock == false) {
                            setTimeout("$('body').toggleClass('loading')", 1);
                            $('#btnOrder').attr('disabled', false);
                            $('#messCart').text('Sản phẩm đã hết hàng');
                        }
                        if (res.status) {
                            setTimeout("$('body').toggleClass('loading')", 1);
                            window.location.href = "/don-hang?id=" + res.id
                        }
                        
                    },
                    error: function (err) {
                        console.log(err);
                    }
                });
            }
        });
    }
}
