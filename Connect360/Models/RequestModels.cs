using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Connect360.Models
{
    public class registered_users_model
    {
        [Required]
        public string host { get; set; }

    }

    public class register_model
    {
        [Required]
        public string user { get; set; }

        [Required]
        public string host { get; set; }

        [Required]
        public string password { get; set; }
    }

    public class sign_in_model
    {
        [Required]
        public string pos { get; set; }

        [Required]
        public string ecr { get; set; }

        [Required]
        public string host { get; set; }

        [Required]
        public string password { get; set; }

        public bool IsAdmin { get; set; }

    }

    public class message_model
    {
        [Required]
        public string type { get; set; }

        [Required]
        public string from { get; set; }

        [Required]
        public string to { get; set; }

        [Required]
        public string subject { get; set; }

        [Required]
        public string body { get; set; }
    }

    public class add_rosteritem_model
    {
        [Required]
        public string localuser { get; set; }

        [Required]
        public string localhost { get; set; }

        [Required]
        public string user { get; set; }

        [Required]
        public string host { get; set; }

        [Required]
        public string nick { get; set; }

        [Required]
        public string group { get; set; }

        [Required]
        public string subs { get; set; }
    }

    public class delete_rosteritem_model
    {
        [Required]
        public string localuser { get; set; }

        [Required]
        public string localhost { get; set; }

        [Required]
        public string user { get; set; }

        [Required]
        public string host { get; set; }
    }

    public class get_roster_model
    {
        [Required]
        public string host { get; set; }

        [Required]
        public string user { get; set; }


    }

}