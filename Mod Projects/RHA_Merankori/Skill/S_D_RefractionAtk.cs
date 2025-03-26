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
namespace RHA_Merankori
{
	/// <summary>
	/// 折射
	/// 折射来自“湮裂的燐焰晶”的伤害
	/// </summary>
    public class S_D_RefractionAtk:S_Attack_All
    {
		//注意，这里复用了原卡的效果，原卡会有判定该技能是否为反射释放
    }
}