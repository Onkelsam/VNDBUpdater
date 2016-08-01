using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace CommunicationLib.VNDB.Connection
{
    internal class LoginData
    {
        public int protocol { get; }
        public string client { get; }
        public string clientver { get; }
        public string username { get; }
        public string password { get; }
        
        public LoginData(string Username, string EncryptedPassword)
        {
            protocol = 1;
            client = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
            clientver = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            username = Username;
            password = Unprotect(EncryptedPassword);
        }

        private string Unprotect(string encryptedString, string entropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] clearBytes = ProtectedData.Unprotect(Convert.FromBase64String(encryptedString), null, scope);

            return Encoding.UTF8.GetString(clearBytes);
        }
    }
}
