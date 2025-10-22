using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using GameDataEditor;
using I2.Loc;
using DarkTonic.MasterAudio;
using ChronoArkMod;
using ChronoArkMod.Plugin;
using ChronoArkMod.Template;
using Debug = UnityEngine.Debug;
using ChronoArkMod.ModData;
using HarmonyLib;
using System.Reflection;
namespace RHA_Merankori
{
    public class RHA_Plugin: ChronoArkPlugin
    {
        private static Harmony harmony = null;
        public override void Dispose()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
                SkillNotCountTracer.Disable();
                harmony = null;
            }
            else
            {
                Debug.LogWarning($"Harmony already unpatched for {IDs.ID_Harmony_Name}, but try to unpatch again, this should not happen!");
            }
        }

        public override void Initialize()
        {
            if(harmony==null)
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