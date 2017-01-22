using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace UserService.Models
{
    /// <summary>
    /// User
    /// </summary>
    public class User
    {
        /// <summary>
        /// Id of the user
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Points of the user
        /// </summary>
        public int? Points { get; set; }
    }

    /// <summary>
    /// User
    /// </summary>
    public class UserPost
    {
        /// <summary>
        /// Name of the user
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Points of the user
        /// </summary>
        [Required]
        public int? Points { get; set; }
        // assume points is required
        // other option is make it optional and set the default value to 0

        // also assume negative points are allowed,
        // otherwise, we can use Range attribute too
    }

    /// <summary>
    /// User
    /// </summary>
    public class UserPut
    {
        /// <summary>
        /// Points of the user
        /// </summary>
        [Required]
        public int? Points { get; set; }
    }
}