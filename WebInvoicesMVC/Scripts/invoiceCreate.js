// Use IIFE
//(function() {
//})();

//JQuery document ready function
$(function () {

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
