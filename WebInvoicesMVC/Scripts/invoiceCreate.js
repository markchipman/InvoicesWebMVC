// Use IIFE
//(function() {
//})();

//JQuery document ready function
$(function () {

  //$("#updateProductBtn")
  //  .click(function (e) {
  //    var id = $(this).data("id");
  //    console.log(id);
  //  });

  $(".updateProductBtnAjax")
    .click(function (e) {
      var invoiceProductId = $(this).data("id");
      var invoiceProduct = $.grep(jsModel, function (e) { return e.InvoiceProductId === invoiceProductId });
      if (invoiceProduct) {
        var quantity = invoiceProduct[0].Quantity;
        if (quantity) {
          $.ajax({
            url: "/Invoices/UpdateInvoiceProduct",
            type: "post",
            data: { invoiceProductId: invoiceProductId, quantity: quantity },
            dataType: "json",
            cache: false,
            traditional: true,
            success: function(result) {
              console.log(result);
            }
        });
        }
      }      
    });


  //$(document).click("click", ".updateProductBtnAjax", function (e) {
  //  var id = $(this).data("id");
  //  console.log(id);
  //});

  $("#productId").change(function (e) {
    var productId = $("#productId").val();

    if (productId) {
      $.ajax({
        url: "/Invoices/GetPrice",
        type: "post",
        data: { productId: productId },
        dataType: "json",
        cache: false,
        traditional: true,
        success: function (result) {
          $("#price").val(result);
        }
      });     
    }
  });

  $("#addProductBtn")
    .click(function (e) {
      var quantity = $("form")["0"]["Quantity"].value;

      if (quantity === "0") {
        alert("Quantity can't be equal to 0.");
      } else {
        var data = $("form").serialize();
        $.ajax({
          url: "/Invoices/AddProduct",
          type: "post",
          data: data
        })
        .done(function (result) {
          $("#products").html(result);
          alert("New product added.");
        });
      }
    });
});
