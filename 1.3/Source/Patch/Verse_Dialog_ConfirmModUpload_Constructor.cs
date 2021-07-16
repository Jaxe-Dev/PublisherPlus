using System;
using HarmonyLib;
using PublisherPlus.Interface;
using RimWorld;
using Verse;

namespace PublisherPlus.Patch
{
    [HarmonyPatch(typeof(Dialog_ConfirmModUpload), MethodType.Constructor, typeof(ModMetaData), typeof(Action))]
    internal static class Verse_Dialog_ConfirmModUpload_Constructor
    {
        private static void Prefix(ref Action acceptAction)
        {
            acceptAction = delegate
            {
                var selectedMod = Access.GetSelectedMod();
                if (selectedMod == null) throw new Mod.Exception("selectedMod is null!");
                Find.WindowStack.Add(new Dialog_Publish(selectedMod.GetWorkshopItemHook()));
            };
        }
    }
}