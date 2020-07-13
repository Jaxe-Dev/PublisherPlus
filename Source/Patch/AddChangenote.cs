using HarmonyLib;
using Steamworks;
using System.IO;
using System.Text.RegularExpressions;
using Verse;
using Verse.Steam;

namespace PublisherPlus.Patch
{
    [HarmonyPatch(typeof(Workshop), "Upload")]
    class UploadFetchPatch
    {
        static void Prefix(ref WorkshopUploadable item)
        {
            Mod.currentMod = item;
        }
    }

    [HarmonyPatch(typeof(SteamUGC), "SubmitItemUpdate")]
    class SubmitItemUpdatePatch
    {
        static void Prefix(ref UGCUpdateHandle_t handle, ref string pchChangeNote)
        {
            if (!pchChangeNote.StartsWith("[Auto-generated text]") || Mod.currentMod == null)
                return;
            var modName = Mod.currentMod.GetWorkshopName();
            var modDirectory = Mod.currentMod.GetWorkshopUploadDirectory();
            var changelogFile = new FileInfo(modDirectory.FullName + "\\About\\Changelog.txt");
            if(!changelogFile.Exists)
            {
                Mod.Log("Could not find changelog-file for mod " + modName + " at path " + changelogFile.FullName + ", skipping modification of changenote.");
                return;
            }
            var manifestFile = new FileInfo(modDirectory.FullName + "\\About\\Manifest.xml");
            if (!manifestFile.Exists)
            {
                Mod.Log("Could not find manifest-file for mod " + modName + " at path " + manifestFile.FullName + ", skipping modification of changenote.");
                return;
            }
            string currentVersion = null;
            foreach (var line in File.ReadAllLines(manifestFile.FullName))
            {
                if(!line.Contains("<version>"))
                {
                    continue;
                }
                currentVersion = line.Replace("<version>", "|").Split('|')[1].Split('<')[0];
            }
            if (string.IsNullOrEmpty(currentVersion))
            {
                Mod.Log("Could not find version in manifest-file for mod " + modName + ", skipping modification of changenote.");
                return;
            }

            Log.Message(currentVersion);
            bool isExtracting = false;
            string changelogMessage = null;
            Regex versionRegex = new Regex(@"\d+(?:\.\d+){1,3}");
            foreach (var line in File.ReadAllLines(changelogFile.FullName))
            {
                if (line.StartsWith(currentVersion))
                {
                    isExtracting = true;
                    changelogMessage += line;
                    continue;
                }
                Match match = versionRegex.Match(line);
                if (isExtracting)
                {                    
                    if (match.Success)
                        break;
                    changelogMessage += line;
                    continue;
                }
            }
            if(string.IsNullOrEmpty(changelogMessage))
            {
                Mod.Log("Could not find latest changenote in changelog-file for mod " + modName + ", skipping modification of changenote.");
                return;
            }
            pchChangeNote = changelogMessage;
        }
    }
}
