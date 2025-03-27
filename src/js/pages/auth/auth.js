/**
 * AUTH - Login
 */

'use strict'

$(document).ready(function () {
  $("#formAuthentication").parsley();
});

window.verifyLogin = (input, evt) => {
  $("#btn-login").disableBtnIcon();
  evt.preventDefault();
  if ($(input).parsley().isValid()) {
    var formData = new FormData(input);
    $("#btn-login").enableBtn();

    // Mengirim data menggunakan Ajax
    $.ajax({
      url: '/Auth/Login',  // Ganti dengan URL endpoint yang sesuai
      type: 'POST',
      data: formData,
      contentType: false,  // Karena kita menggunakan FormData, jangan set content-type manual
      processData: false,  // Jangan proses data, biarkan FormData mengelolanya
      beforeSend: function() {
        $("#btn-login").disableBtnIcon();
      },
      success: function (response) {
        $("#btn-login").enableBtn();
        if (response.code == 200) {
          $("#login-label").fadeOut(100);
          window.location.href = '/Admin';
        } else {
          $("#login-label").fadeIn(100);
        }
      },
      error: function (xhr, status, error) {
        $("#btn-login").enableBtn();
        $("#login-label").fadeIn(100);
        console.error('Terjadi kesalahan: ' + error);
      },

      complete: function() {
        $("#btn-login").enableBtn();
      }
    });

  } else {
    $("#btn-login").enableBtn();
  }

  // alert();
}
