using ChronoArkMod.ModData;
using ChronoArkMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RHA_Merankori
{
    public static class ResUtils
    {
        public static GameObject LoadModPrefab(string assetPath)
        {
            ModInfo info = ModManager.getModInfo(IDs.ID_Mod);
            string path = info.assetInfo.ObjectFromAsset<GameObject>("rha_merankoriunityassetbundle", assetPath);
            Debug.Log($"Try to load asset from {path}");
            return AddressableLoadManager.Instantiate(path, AddressableLoadManager.ManageType.None);
        }
    }
}
