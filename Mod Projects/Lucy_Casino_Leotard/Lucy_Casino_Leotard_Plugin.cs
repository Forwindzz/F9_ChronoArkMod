using ChronoArkMod;
using ChronoArkMod.ModData;
using ChronoArkMod.Plugin;
using HarmonyLib;
using System.Reflection;
using Debug = UnityEngine.Debug;

namespace Lucy_Casino_Leotard
{

    public static class IDs
    {
        public const string ID_Harmony_Name = "Lucy_Casino_Leotard";
        public const string ID_Mod = "Lucy_Casino_Leotard";
    }

    public class Lucy_Casino_Leotard_Plugin : ChronoArkPlugin
    {
        private static Harmony harmony = null;
        public override void Dispose()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
                harmony = null;
            }
            else
            {
                Debug.LogWarning($"Harmony already unpatched for {IDs.ID_Harmony_Name}, but try to unpatch again, this should not happen!");
            }
        }

        public override void Initialize()
        {
            if (harmony == null)
            {
                harmony = new Harmony(IDs.ID_Harmony_Name);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Debug.Log("Harmony Patch Applied for " + IDs.ID_Harmony_Name);
            }
            else
            {
                Debug.LogWarning($"Harmony already patched for {IDs.ID_Harmony_Name}, but try to patch again, this should not happen!");
            }
        }
    }
}