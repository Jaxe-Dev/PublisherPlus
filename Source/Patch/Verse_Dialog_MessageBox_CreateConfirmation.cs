using HarmonyLib;
using PublisherPlus.Interface;
using RimWorld;
using System;
using Verse;

namespace PublisherPlus.Patch
{
    [HarmonyPatch(typeof(Dialog_ConfirmModUpload), MethodType.Constructor, new Type[] { typeof(ModMetaData), typeof(Action)})]
    internal static class Verse_Dialog_MessageBox_CreateConfirmation
    {
        private static void Postfix(ref Dialog_MessageBox __instance)
        {
            Dialog_MessageBox self = __instance;

            if (self.text != "ConfirmSteamWorkshopUpload".Translate()) { return; }

            var selectedMod = Access.GetSelectedMod();
            if (selectedMod == null) { return; }

            self.buttonAAction = delegate ()
            {
                Find.WindowStack.Add(new Dialog_Publish(selectedMod.GetWorkshopItemHook()));
            };

            self.acceptAction = self.buttonAAction;
        }
    }
}
