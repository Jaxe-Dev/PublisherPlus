using System.Reflection;
using Harmony;
using Verse.Steam;

namespace PublisherPlus.Patch
{
    internal static class Access
    {
        private static readonly MethodInfo Method_Verse_Steam_Workshop_Upload = AccessTools.Method(typeof(Workshop), "Upload", new[] { typeof(WorkshopUploadable) });

        public static void Method_Verse_Steam_Workshop_Upload_Call(WorkshopUploadable item) => Method_Verse_Steam_Workshop_Upload.Invoke(null, new object[] { item });
    }
}
