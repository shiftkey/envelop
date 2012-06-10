using Windows.ApplicationModel.Resources;

namespace Cocoon.Helpers
{
    internal static class ResourceHelper
    {
        // *** Static Fields ***

        private static ResourceLoader errorResourceLoader;

        // *** Methods ***

        public static string GetErrorResource(string resourceName)
        {
            if (errorResourceLoader == null)
                errorResourceLoader = new ResourceLoader("Cocoon/Errors");

            return errorResourceLoader.GetString(resourceName);
        }
    }
}
