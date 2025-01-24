'use strict';

function getValueById(id) {
  return $(`#${id}`).val();
}

function setTextById(id, value) {
  console.log('value: ' + value);
  if (value != null || value != '' || value != undefined) {
    return $(`#${id}`).text(value);
  }
  return $(`#${id}`).text('-');
}

function beforeLoadingButton(el, val = null) {
  if (val == null) {
    el.html(
      '<div class="d-flex justify-content-center align-items-center"><span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span></div>'
    );
  } else {
    el.html(
      `<div class="d-flex justify-content-center align-items-center"><span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span> ${val}</div>`
    );
  }
  el.addClass('disabled');
  el.prop('disabled', true);
}

function afterLoadingButton(el, val = null) {
  if (val == null) {
    el.html('Save');
  } else {
    el.html(val);
  }
  el.removeClass('disabled');
  el.prop('disabled', false);
}

function showMessage(status, title, message) {
  Swal.fire(title, message, status);
}

// HTTP REQUEST
async function postData(url = '', data = {}) {
  return await axios.post(url, data);
}

async function deleteData(url = '', data = {}) {
  return await axios.delete(url, data);
}

async function updateData(url = '', id, data = {}) {
  return await axios.post(url, id, data);
}

async function putData(url = '', data = {}) {
  return await axios.put(url, data);
}

async function getData(url, id) {
  return await axios.get(url, id);
}

async function getResult(url, params) {
  return await axios.get(url, { params: params });
}

async function getRegion(url = '', data = {}) {
  return await axios.get(url, { params: data });
}

/* Date Time Fuction */

function convertDateTimeId(input) {
  let date = new Date(input);
  if (input != new Date()) date = new Date(input);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const result = year + '-' + month + '-' + day;
  return result;
}

function formatErrorMessage(jqXHR, exception) {
  if (jqXHR.status === 0) {
    return 'Not connected.\nPlease verify your network connection.';
  } else if (jqXHR.status == 404) {
    return 'The requested page not found.';
  } else if (jqXHR.status == 401) {
    return 'Sorry!! You session has expired. Please login to continue access.';
  } else if (jqXHR.status == 500) {
    return 'Terjadi kesalahan, silahkan coba lagi.';
  } else if (exception === 'parsererror') {
    return 'Requested JSON parse failed.';
  } else if (exception === 'timeout') {
    return 'Time out error.';
  } else if (exception === 'abort') {
    return 'Ajax request aborted.';
  } else {
    return 'Unknown error occured. Please try again.';
  }
}

jQuery.fn.extend({
  disableBtn: function () {
    return this.each(function () {
      // Hapus ripple yang sudah ada
      $(this).find('.waves-ripple').remove();

      var $this = $(this).html();
      $(this).html(
        '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span> Processing...'
      );
      $(this).prop('origin-text', $this);
      $(this).attr('disabled', true);
      $(this).addClass('disabled');
    });
  },
  disableBtnIcon: function () {
    return this.each(function () {
      // Hapus ripple yang sudah ada
      $(this).find('.waves-ripple').remove();

      var $this = $(this).html();
      $(this).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>');
      $(this).prop('origin-text', $this);
      $(this).attr('disabled', true);
      $(this).addClass('disabled');
    });
  },

  enableBtn: function () {
    return this.each(function () {
      // Kembalikan teks asli tombol
      if ($(this).prop('origin-text') != null) $(this).html($(this).prop('origin-text'));
      $(this).attr('disabled', false);
      $(this).removeClass('disabled');
    });
  }
});
