using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UserService.Models
{
    public class NotFoundException : ArgumentException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }

    public class BadRequestException : ArgumentException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}