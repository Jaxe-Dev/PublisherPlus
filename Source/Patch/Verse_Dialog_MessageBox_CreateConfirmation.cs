using HarmonyLib;
using PublisherPlus.Interface;
using Verse;

namespace PublisherPlus.Patch
{
    [HarmonyPatch(typeof(Dialog_MessageBox), "CreateConfirmation")]
    internal static class Verse_Dialog_MessageBox_CreateConfirmation
    {
        private static bool Prefix(ref Window __result, TaggedString text)
        {
            if (text != "ConfirmSteamWorkshopUpload".Translate()) { return true; }

            var selectedMod = Access.GetSelectedMod();
            if (selectedMod == null) { return false; }
            __result = new Dialog_Publish(selectedMod.GetWorkshopItemHook());

            return false;
        }
    }
}
