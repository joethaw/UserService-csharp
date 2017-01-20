using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace UserService.Models
{
    public class User
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int? Points { get; set; }
        // assume points is required
        // other option is make it optional and set the default value to 0

        // also assume negative points are allowed,
        // otherwise, we can use Range attribute too
    }

    public class UserPut
    {
        [Required]
        public int? Points { get; set; }
    }
}