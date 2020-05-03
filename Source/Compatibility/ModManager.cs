using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

namespace PublisherPlus.Compatibility
{
    internal static class ModManager
    {
        private static readonly Assembly Assembly = LoadedModManager.RunningModsListForReading.FirstOrDefault(mod => mod.Name == "Mod Manager")?.assemblies.loadedAssemblies.FirstOrDefault(assembly => assembly.GetName().Name == "ModManager");
        public static readonly bool Loaded = Assembly != null;

        public static ModMetaData GetSelectedMod()
        {
            //Thanks to Orion for figuring this out.
            var pageType = Assembly.GetType("ModManager.Page_BetterModConfig");
            if (pageType == null) { return null; }

            var page = Traverse.Create(pageType).Property("Instance")?.GetValue();
            var selectedModButton = Traverse.Create(page).Property("Selected")?.GetValue();
            if (selectedModButton == null) { return null; }

            var buttonManager = Assembly.GetType("ModManager.ModButtonManager");
            var method = AccessTools.Method(buttonManager, "AttributesFor", new[] { Assembly.GetType("ModManager.ModButton") });
            var value = method.Invoke(null, new[] { selectedModButton });
            return Traverse.Create(value).Property<ModMetaData>("Mod").Value;
        }
    }
}
