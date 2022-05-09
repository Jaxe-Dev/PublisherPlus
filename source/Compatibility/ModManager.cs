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
      var pageType = Assembly.GetType("ModManager.Page_BetterModConfig");
      var modButtonInstalledType = Assembly.GetType("ModManager.ModButton_Installed");
      if (pageType == null || modButtonInstalledType == null) { return null; }

      var selectedModButton = Traverse.Create(pageType).Field("_instance")?.Field("_selected")?.GetValue();
      if (selectedModButton == null) { return null; }

      var modButtonInstalled = Convert.ChangeType(selectedModButton, modButtonInstalledType);
      if (modButtonInstalled == null) { return null; }

      var selected = (ModMetaData) Traverse.Create(modButtonInstalled).Field("_selected").GetValue();
      return selected;
    }
  }
}
