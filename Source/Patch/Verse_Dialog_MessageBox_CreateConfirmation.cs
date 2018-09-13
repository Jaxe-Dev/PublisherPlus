using Harmony;
using PublisherPlus.Interface;
using RimWorld;
using Verse;

namespace PublisherPlus.Patch
{
    [HarmonyPatch(typeof(Dialog_MessageBox), "CreateConfirmation")]
    internal static class Verse_Dialog_MessageBox_CreateConfirmation
    {
        private static bool Prefix(ref Window __result, string text)
        {
            if (text != "ConfirmSteamWorkshopUpload".Translate()) { return true; }

            var page = Find.WindowStack.WindowOfType<Page_ModsConfig>();
            if (page?.selectedMod == null) { return true; }

            __result = new Dialog_Publish(page.selectedMod.GetWorkshopItemHook());

            return false;
        }
    }
}
