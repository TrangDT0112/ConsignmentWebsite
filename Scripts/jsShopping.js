$(document).ready(function () {
    ShowCount();
    $('body').off('click', '.btnAddToCart');
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();
        var $btn = $(this); 
        $btn.prop('disabled', true); 
        var id = $btn.data('id');
        var quantity = 1;
        var tQuantity = $('#quantity_value').text();
        if (tQuantity !== '') {
            quantity = parseInt(tQuantity);
        }
        $.ajax({
            url: '/shoppingcart/addtocart',
            type: 'POST',
            data: { id: id, quantity: quantity },
            success: function (rs) {
                if (rs && rs.success) {
                    $('#checkout_items').html(rs.Count);
                    alert(rs.msg);
                } else if (rs && rs.msg) {
                    alert(rs.msg);
                } else {
                    alert("You must log in before adding products to cart!!");
                }
            },
            error: function () {
                alert("Error connecting to server");
            },
            complete: function () {
                $btn.prop('disabled', false); 
            }
        });
    });

    $('body').on('click', '.btnDelete', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var conf = confirm('Are you sure you want to remove this product from your cart?');
        if (conf == true) {
            $.ajax({
                url: '/shoppingcart/delete',
                type: 'POST',
                data: { id: id },
                success: function (rs) {
                    if (rs.success) {
                        $('#checkout_items').html(rs.Count);
                        $('#trow_' + id).remove();
                        LoadCart();
                    }
                }


            });
        }
        
    });

    $('body').on('click', '.btnDeleteAll', function (e) {
        e.preventDefault();
        var conf = confirm('Are you sure you want to remove all products from your cart?');
        if (conf == true) {
            DeleteAll();
        }

    });

    $('body').on('click', '.btnUpdate', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var quantity = $('#Quantity_' + id).val();
        Update(id, quantity);
      

    });
});
function ShowCount() {
    $.ajax({
        url: '/shoppingcart/ShowCount',
        type: 'GET',
        success: function (rs) {
                $('#checkout_items').html(rs.Count);
            
        }

    });
}
function Update(id,quantity) {
    $.ajax({
        url: '/shoppingcart/Update',
        type: 'POST',
        data: { id: id, quantity: quantity },
        success: function (rs) {
            if (rs.success) {
                LoadCart();
            }

        }

    });
}
function DeleteAll() {
    $.ajax({
        url: '/shoppingcart/DeleteAll',
        type: 'POST',       
        success: function (rs) {
            if (rs.success) {
                LoadCart();
            }
        }

    });
}
function LoadCart() {
    $.ajax({
        url: '/shoppingcart/Partial_ItemCart',
        type: 'GET',
        success: function (rs) {
                $('#load_data').html(rs);
            
        }

    });
}