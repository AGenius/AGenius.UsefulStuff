using System;

namespace AGenius.UsefulStuff.Helpers.ActiveDirectory.Model
{

    public class ADUser
    {
        public ADUser()
        {
        }

        public int? ID { get; set; }

        public string AccountName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

    }
}
