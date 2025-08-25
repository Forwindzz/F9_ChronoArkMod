using ChronoArkMod.ModData;
using ChronoArkMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RHA_Merankori
{
    public static class ResUtils
    {
        private static string ASSET_BUNDLE_NAME = "rha_merankoriunityassetbundle";

        public static GameObject LoadModPrefab(string assetPath)
        {
            ModInfo info = ModManager.getModInfo(IDs.ID_Mod);
            string path = info.assetInfo.ObjectFromAsset<GameObject>(ASSET_BUNDLE_NAME, assetPath);
            //Debug.Log($"Try to load asset from {path}");
            return AddressableLoadManager.Instantiate(path, AddressableLoadManager.ManageType.None);
        }

        public static T LoadModAsset<T>(string assetPath) where T : UnityEngine.Object
        {
            ModInfo info = ModManager.getModInfo(IDs.ID_Mod);
            //Debug.Log($"Try to load asset from {assetPath}");
            return info.assetInfo.LoadFromAsset<T>(ASSET_BUNDLE_NAME, assetPath);
        }
    }
}
