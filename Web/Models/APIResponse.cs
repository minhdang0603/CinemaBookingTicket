﻿using System.Net;

namespace Web.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string ErrorMessages { get; set; }
        public object Result { get; set; }
    }
}
