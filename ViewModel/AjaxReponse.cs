using System;
using System.Collections.Generic;

namespace Higertech.ViewModels;

 public class AjaxResponse
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public dynamic? Data { get; set; }
    }