using System.Linq;
using System.Reflection;
using HarmonyLib;
using PublisherPlus.Interface;
using RimWorld;
using Verse;

namespace PublisherPlus.Compatibility
{
  internal static class FluffyModManager
  {
    private static readonly Assembly Assembly = LoadedModManager.RunningModsListForReading.FirstOrDefault(mod => mod.Name == "Mod Manager")?.assemblies.loadedAssemblies.FirstOrDefault(assembly => assembly.GetName().Name == "ModManager");

    public static void AddCompatibility(Harmony harmony)
    {
      if (Assembly == null) { return; }

      var workshopType = Assembly.GetType("ModManager.Workshop");
      var workshopUploadMethod = workshopType.GetMethod("Upload", new[] { typeof(ModMetaData) });
      var workshopUploadPrefix = typeof(FluffyModManager).GetMethod("WorkshopUploadPrefix");

      harmony.Patch(workshopUploadMethod, new HarmonyMethod(workshopUploadPrefix));
    }

    public static bool WorkshopUploadPrefix(ModMetaData mod)
    {
      Find.WindowStack.Add(new Dialog_ConfirmModUpload(mod, () => Find.WindowStack.Add(new Dialog_Publish(mod.GetWorkshopItemHook()))));
      return false;
    }
  }
}
