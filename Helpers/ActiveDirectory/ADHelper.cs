using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Security.Principal;
using System.Text;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using AGenius.UsefulStuff.Helpers.ActiveDirectory.Model;
using System.Threading.Tasks;

namespace AGenius.UsefulStuff.Helpers.ActiveDirectory
{
    public class ADHelper
    {
        public static bool IsInGroup(string principal, string application)
        {
            return IsInGroup(principal, application, null);
        }
        /// <summary>
        /// Returns if a user is in a specific AD Group
        /// </summary>
        /// <param name="principal">The user name to check</param>
        /// <param name="application"></param>
        /// <param name="ADPathOverride"></param>
        /// <returns></returns>
        public static bool IsInGroup(string principal, string application, string ADPathOverride)
        {
            string group = ConfigurationManager.AppSettings[application];
            List<string> userGroups = GetUserGroups(principal, ADPathOverride);

            foreach (string grp in userGroups)
            {
                if (grp == group)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Check if user credentials are valid on the domain
        /// </summary>
        /// <param name="userName">User Name as found in AD</param>
        /// <param name="password">Users password to test</param>
        /// <param name="domainName">the domain </param>
        /// <param name="adPassowrd">The Active Directory admin UserName</param>
        /// <param name="adUser">The Active Directory admin Password</param>
        /// <returns></returns>
        public static bool IsValidAuth(string userName, string password, string domainName, string adUser, string adPassowrd)
        {
            try
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainName, "ou=users,ou=system", $"uid={adUser},ou=system", adPassowrd))
                {
                    // validate the credentials
                    return pc.ValidateCredentials(userName, password);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Return - 
        /// </summary>
        /// <param name="userName">User Name as found in AD</param>
        /// <param name="domainName">the domain </param>
        /// <returns></returns>
        public static UserPrincipal GetADPrincipal(string userName, string domainName)
        {
            try
            {
                PrincipalContext pc = new PrincipalContext(ContextType.Domain, $"{domainName}");
                return UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, $"{domainName}\\" + userName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static Boolean CheckMember(List<string> userGroups, string theGroup)
        {
            foreach (string grp in userGroups)
            {
                if (grp == theGroup)
                    return true;
            }

            return false;
        }
        public static string GetUserName(string principal)
        {
            return GetUserName(principal, null);
        }
        public static string GetUserName(string principal, string ADPathOverride)
        {
            if (!String.IsNullOrEmpty(principal))
            {
                DirectoryEntry theUser = GetDirectoryEntry(principal, ADPathOverride);
                if (theUser != null)
                {
                    return theUser.Properties["displayName"][0].ToString();
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "Unknown";
            }
        }

        public static List<string> GetUserGroups(string principal)
        {
            return GetUserGroups(principal, null);
        }
        public static List<string> GetUserGroupNames(string principal)
        {
            return GetUserGroupNames(principal, null);
        }
        public static List<string> GetUserGroupNames(string principal, string ADPathOverride)
        {
            if (!String.IsNullOrEmpty(principal))
            {
                DirectoryEntry theUser = GetDirectoryEntry(principal, ADPathOverride);
                if (theUser != null)
                {
                    theUser.RefreshCache(new string[] { "tokenGroups" });

                    List<string> userGroups = ExpandGroupNames(theUser);

                    return userGroups;
                }
                else
                {
                    return new List<string> { "Unknown" };

                }

            }
            else
            {
                //List<string> fake = new List<string>();
                //fake.Add("Unknown");

                return new List<string> { "Unknown" };
            }
        }
        public static List<string> GetUserGroups(string principal, string ADPathOverride)
        {
            if (!String.IsNullOrEmpty(principal))
            {
                DirectoryEntry theUser = GetDirectoryEntry(principal, ADPathOverride);
                if (theUser != null)
                {
                    theUser.RefreshCache(new string[] { "tokenGroups" });

                    IdentityReferenceCollection irc = ExpandTokenGroups(theUser).Translate(typeof(NTAccount));

                    List<string> userGroups = new List<string>();

                    foreach (IdentityReference ir in irc)
                    {
                        userGroups.Add(ir.Value);
                    }

                    return userGroups;
                }
                else
                {
                    return new List<string> { "Unknown" };

                }

            }
            else
            {
                //List<string> fake = new List<string>();
                //fake.Add("Unknown");

                return new List<string> { "Unknown" };
            }
        }

        private static DirectoryEntry GetDirectoryEntry(string principal)
        {
            return GetDirectoryEntry(principal, null);
        }
        private static DirectoryEntry GetDirectoryEntry(string principal, string ADPathOverride)
        {
            string domain = "";
            if (string.IsNullOrEmpty(ADPathOverride))
            {
                domain = ConfigurationManager.AppSettings["ADPath"];
            }
            else
            {
                domain = ADPathOverride;
            }

            string filter = string.Format("(&(ObjectClass={0})(sAMAccountName={1}))", "person", principal);
            string[] properties = new string[] { "fullname" };

            DirectoryEntry adRoot = null;

            adRoot = new DirectoryEntry("LDAP://" + domain, null, null, AuthenticationTypes.Secure);

            DirectorySearcher searcher = new DirectorySearcher(adRoot);
            searcher.SearchScope = SearchScope.Subtree;
            searcher.ReferralChasing = ReferralChasingOption.All;
            searcher.PropertiesToLoad.AddRange(properties);
            searcher.Filter = filter;
            SearchResult result = null;
            try
            {
                result = searcher.FindOne();

            }
            catch (Exception ex)
            {

            }

            if (result == null)
                return null;

            DirectoryEntry theUser = result.GetDirectoryEntry();

            return theUser;
        }


        private static IdentityReferenceCollection ExpandTokenGroups(DirectoryEntry user)
        {
            user.RefreshCache(new string[] { "tokenGroups" });

            IdentityReferenceCollection irc = new IdentityReferenceCollection();

            foreach (byte[] sidBytes in user.Properties["tokenGroups"])
            {
                irc.Add(new SecurityIdentifier(sidBytes, 0));
            }

            return irc;
        }
        private static List<string> ExpandGroupNames(DirectoryEntry user)
        {
            user.RefreshCache(new string[] { "memberOf" });

            //IdentityReferenceCollection irc = new IdentityReferenceCollection();
            List<String> Items = new List<string>();
            foreach (string name in user.Properties["memberOf"])
            {
                //irc.Add(name);
                string[] splitup = name.Split(',');

                Items.Add(splitup[0].Replace("CN=", ""));
            }
            return Items;
        }

        public static List<string> GetLocalUserList()
        {
            DirectoryEntry directoryEntry = new DirectoryEntry("WinNT://" + Environment.MachineName);
            List<string> userNames = new List<string>();
            foreach (DirectoryEntry child in directoryEntry.Children)
            {
                if (child.SchemaClassName == "User")
                {
                    userNames.Add(child.Name);
                }
            }
            return userNames;
        }

        public static List<string> GetADUserList(string domainName, string adUserName, string adPassword, string GroupName = null)
        {
            List<string> userNames = new List<string>();
            PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, domainName, adUserName, adPassword);
            GroupPrincipal group = GroupPrincipal.FindByIdentity(principalContext, GroupName);

            foreach (Principal principal in group.Members)
            {
                string name = principal.Name;
                userNames.Add(name);
            }

            return userNames;
        }
        public static List<ADUser> GetADUsers(string domainName, string adUserName, string adPassword, string GroupName = null)
        {
            List<ADUser> userNames = new List<ADUser>();
            PrincipalContext principalContext = new PrincipalContext(ContextType.Domain, domainName, adUserName, adPassword);
            GroupPrincipal group = GroupPrincipal.FindByIdentity(principalContext, GroupName);

            foreach (UserPrincipal principal in group.Members)
            {
                ADUser userItem = new ADUser();
                userItem.AccountName = principal.SamAccountName;
                userItem.DisplayName = principal.Name;
                userItem.Email = principal.EmailAddress;

                userNames.Add(userItem);

            }

            return userNames;
        }
    }
}
