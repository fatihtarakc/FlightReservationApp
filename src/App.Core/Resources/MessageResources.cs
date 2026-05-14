using System.Globalization;
using System.Resources;

namespace App.Core.Resources
{
    public class MessageResources
    {
        private static readonly ResourceManager resourceManager =
            new ResourceManager("App.Core.Resources.MessageResources", typeof(MessageResources).Assembly);

        public static string GetString(string key) =>
            resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }
}
