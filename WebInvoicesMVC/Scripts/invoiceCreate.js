// Use IIFE
//(function() {
//})();

//JQuery document ready function
$(function () {
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
