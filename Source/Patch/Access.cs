using System.Reflection;
using Harmony;
using PublisherPlus.Compatibility;
using RimWorld;
using Verse;
using Verse.Steam;

namespace PublisherPlus.Patch
{
    internal static class Access
    {
        private static readonly MethodInfo Method_Verse_Steam_Workshop_Upload = AccessTools.Method(typeof(Workshop), "Upload", new[] { typeof(WorkshopUploadable) });

        public static void Method_Verse_Steam_Workshop_Upload_Call(WorkshopUploadable item) => Method_Verse_Steam_Workshop_Upload.Invoke(null, new object[] { item });

        public static ModMetaData GetSelectedMod()
        {
            if (ModManager.Loaded) { return ModManager.GetSelectedMod() ?? throw new Mod.Exception("Error getting selected mod from ModManager"); }
            return Find.WindowStack.WindowOfType<Page_ModsConfig>()?.selectedMod ?? throw new Mod.Exception("Error getting selected mod");
        }
    }
}
