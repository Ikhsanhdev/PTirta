/*
 * ATTENTION: The "eval" devtool has been used (maybe by default in mode: "development").
 * This devtool is neither made for production nor for readable output files.
 * It uses "eval()" calls to create a separate source file in the browser devtools.
 * If you are trying to read the output file, select a different devtool (https://webpack.js.org/configuration/devtool/)
 * or disable the default devtool with "devtool: false".
 * If you are looking for production-ready output files, see mode: "production" (https://webpack.js.org/configuration/mode/).
 */
(function webpackUniversalModuleDefinition(root, factory) {
	if(typeof exports === 'object' && typeof module === 'object')
		module.exports = factory();
	else if(typeof define === 'function' && define.amd)
		define([], factory);
	else {
		var a = factory();
		for(var i in a) (typeof exports === 'object' ? exports : root)[i] = a[i];
	}
})(self, () => {
return /******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/js/pages/auth/auth.js":
/*!***********************************!*\
  !*** ./src/js/pages/auth/auth.js ***!
  \***********************************/
/***/ (() => {

eval("/**\r\n * AUTH - Login\r\n */\n\n\n\n$(document).ready(function () {\n  $(\"#formAuthentication\").parsley();\n});\nwindow.verifyLogin = function (input, evt) {\n  $(\"#btn-login\").disableBtnIcon();\n  evt.preventDefault();\n  if ($(input).parsley().isValid()) {\n    var formData = new FormData(input);\n    $(\"#btn-login\").enableBtn();\n\n    // Mengirim data menggunakan Ajax\n    $.ajax({\n      url: '/Auth/Login',\n      // Ganti dengan URL endpoint yang sesuai\n      type: 'POST',\n      data: formData,\n      contentType: false,\n      // Karena kita menggunakan FormData, jangan set content-type manual\n      processData: false,\n      // Jangan proses data, biarkan FormData mengelolanya\n      beforeSend: function beforeSend() {\n        $(\"#btn-login\").disableBtnIcon();\n      },\n      success: function success(response) {\n        $(\"#btn-login\").enableBtn();\n        if (response.code == 200) {\n          $(\"#login-label\").fadeOut(100);\n          window.location.href = '/';\n        } else {\n          $(\"#login-label\").fadeIn(100);\n        }\n      },\n      error: function error(xhr, status, _error) {\n        $(\"#btn-login\").enableBtn();\n        $(\"#login-label\").fadeIn(100);\n        console.error('Terjadi kesalahan: ' + _error);\n      },\n      complete: function complete() {\n        $(\"#btn-login\").enableBtn();\n      }\n    });\n  } else {\n    $(\"#btn-login\").enableBtn();\n  }\n\n  // alert();\n};\n\n//# sourceURL=webpack://sneat-bootstrap-html-aspnet-core-mvc-admin-template-free/./src/js/pages/auth/auth.js?");

/***/ })

/******/ 	});
/************************************************************************/
/******/ 	
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	// This entry module can't be inlined because the eval devtool is used.
/******/ 	var __webpack_exports__ = {};
/******/ 	__webpack_modules__["./src/js/pages/auth/auth.js"]();
/******/ 	
/******/ 	return __webpack_exports__;
/******/ })()
;
});