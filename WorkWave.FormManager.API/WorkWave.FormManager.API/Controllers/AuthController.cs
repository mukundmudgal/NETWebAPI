using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WorkWave.FormManager.API.Controllers
{
    public class AuthController : ApiController
    {
        [HttpGet]
        public String Get()
        {
            return "Welcome To From Manager API";
        }
    }
}
