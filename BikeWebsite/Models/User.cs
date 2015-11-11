using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BikeWebsite.Models
{
    public class User
    {
        private DBEntities db = new DBEntities();

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me on this computer")]
        public bool RememberMe { get; set; }

        public string Type { get; set; }

        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <param name="_username">User name</param>
        /// <param name="_password">User password</param>
        /// <returns>True if user exist and password is correct</returns>
        public bool isValid(string _username, string _password)
        {
            var valid = (from v in db.System_Users
                         where v.Email == _username && v.Password == _password
                         select v.Email).ToList();
            if (valid.Count() == 0)
                return false;
            if (valid[0].ToString() == _username)
                return true;
            return false;
        }
    }
}