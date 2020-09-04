using System;
namespace AuthWithB2C
{
    public static class Constants
    {
        public static readonly string TenantId = "cmatdevb2c.onmicrosoft.com";
        public static readonly string TenantName = "cmatdevb2c";
        public static readonly string ClientId = "4567430d-6a71-4818-baf5-43335ba95e36";
        public static readonly string SignInPolicy = "b2c_1_sisu";
        public static readonly string IosKeychainSecurityGroup = "com.microsoft.cmb2cauthorization";
        public static readonly string AuthorityBase = $"https://{TenantName}.b2clogin.com/tfp/{TenantId}/";
        public static readonly string AuthoritySignIn = $"{AuthorityBase}{SignInPolicy}";
        public static readonly string[] Scopes = new string[]{ "openid", "offline_access" };
    }
}
