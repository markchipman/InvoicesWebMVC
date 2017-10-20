// Use IIFE
//(function() {
//})();

//JQuery document ready function
$(function () {

  $("#productId").change(function (e) {
    var productId = $("#productId").val();
    alert(productId);

    //if (productId) {
    //  $.ajax({
    //    type: "post",
    //    url: "/Invoices/GetPrice",
    //    dataType: "Json",
    //    data: productId,
    //    success: function(price) {
    //      $("#price").val(price);
    //    }
    //  });
    //}

    var data = $("form").serialize();
    if (productId) {
      $.ajax({
        url: "/Invoices/GetPrice",
        type: "post",
        //data: JSON.stringify(productId),
        data: { productId: productId },
        dataType: "json",
        cache: false,
        traditional: true,
        success: function (result) {
          alert(result);
          $("#price").val(result);
        }
      });
      //.done(function (result) {
      //  alert(result);
      //}
      //);
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
