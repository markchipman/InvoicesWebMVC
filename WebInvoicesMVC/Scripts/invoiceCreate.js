// Use IIFE
//(function() {
//})();

//JQuery document ready function
$(function () { 
  $("#addProductBtn")
    .click(function (e) {
      var data = $("form").serialize();

      $.ajax({
        url: "/Invoices/AddProduct",
        type: "post",
        data: data
      })
      .done(function(result) {
          $("#products").html(result);
        });
    });
});
