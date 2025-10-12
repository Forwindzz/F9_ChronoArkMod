using ChronoArkMod;
namespace RHA_Merankori
{
    public static class ModItemKeys
    {
		/// <summary>
		/// 逆流的燐焰晶
		/// <color=#FF6767>湮裂的燐焰晶</color>会根据队友已损失的体力值增加伤害，每&a点增加&b%伤害。
		/// <color=#5061A4>（预期增加&c）</color>
		/// </summary>
        public static string Buff_B_Backflow = "B_Backflow";
		/// <summary>
		/// 后备的燐焰晶
		/// 回合开始时，生成1张<color=#FF6767>湮裂的燐焰晶</color>并放入手牌中。
		/// </summary>
        public static string Buff_B_Backup = "B_Backup";
		/// <summary>
		/// 冷静
		/// 回合结束时，赋予体力值最低的队友1层燐色存护。
		/// <color=#BAC8FF>照料可能因自己受伤的队友...</color>
		/// 回合开始时梅朗柯莉会<color=#BAC8FF><b>冷静</b></color>下来。
		/// 当有人攻击时，进入<color=#FFC5BA><b>慌张</b></color>状态。
		/// </summary>
        public static string Buff_B_Calm = "B_Calm";
		/// <summary>
		/// 镇定
		/// 阻止进入慌张状态，每次阻止会减少1层。
		/// </summary>
        public static string Buff_B_CalmDown = "B_CalmDown";
		/// <summary>
		/// 蓄能
		/// 释放<color=#FF6767>湮裂的燐焰晶</color>时，每层增加其10%的伤害，然后移除此效果。
		/// </summary>
        public static string Buff_B_Charge = "B_Charge";
		/// <summary>
		/// 解除无法战斗抵抗上限
		/// 目前的无法战斗抵抗：&a%
		/// </summary>
        public static string Buff_B_DeadImmuneNoLimit = "B_DeadImmuneNoLimit";
		/// <summary>
		/// 无法战斗抵抗
		/// </summary>
        public static string Buff_B_DeathResist = "B_DeathResist";
		/// <summary>
		/// 导流回路
		/// 释放<color=#FF6767>湮裂的燐焰晶</color>时，获得1点法力值。
		/// </summary>
        public static string Buff_B_DLoop = "B_DLoop";
		/// <summary>
		/// 火之存护
		/// 若回合开始时没有燐色存护，则施加1层燐色存护。
		/// </summary>
        public static string Buff_B_EnsureShield = "B_EnsureShield";
		/// <summary>
		/// 频移微调
		/// 受到<color=#FF6767>湮裂的燐焰晶</color>的攻击时，将受到的伤害转化为等量的治疗，然后移除1层。
		/// </summary>
        public static string Buff_B_FreqShift = "B_FreqShift";
		/// <summary>
		/// 焰晶生成
		/// 每当队友脱离濒死状态时，将<color=#FF6767>湮裂的燐焰晶</color>放入手中。
		/// </summary>
        public static string Buff_B_GenCrystal = "B_GenCrystal";
		/// <summary>
		/// 频移微调-露西
		/// 引导燐焰晶
		/// </summary>
        public static string Buff_B_H_FreqShift_Lucy = "B_H_FreqShift_Lucy";
		/// <summary>
		/// 烈燐焰晶
		/// 燐焰晶的伤害不会致命
		/// </summary>
        public static string Buff_B_NotDeadlyAtk = "B_NotDeadlyAtk";
		/// <summary>
		/// 越频晶石
		/// 下一次<color=#FF6767>湮裂的燐焰晶</color>会附带“震荡冲击”干扰减益
		/// </summary>
        public static string Buff_B_OverClocking = "B_OverClocking";
		/// <summary>
		/// 慌张
		/// <color=#FFC5BA>燐焰晶逐渐炽烈...</color>
		/// 回合开始时，梅朗柯莉会<color=#BAC8FF><b>冷静</b></color>下来。
		/// 当有人攻击时，进入<color=#FFC5BA><b>慌张</b></color>状态。
		/// </summary>
        public static string Buff_B_Panic = "B_Panic";
		/// <summary>
		/// 燐焰晶回流
		/// 回合开始时，赋予1层燐色存护。
		/// 溢出的燐色存护会转化为5%无法战斗抵抗增益。
		/// </summary>
        public static string Buff_B_Reflow = "B_Reflow";
		/// <summary>
		/// 折射
		/// 将<color=#FF6767>湮裂的燐焰晶</color>的伤害和附加效果，折射给生命值最低的敌人。
		/// </summary>
        public static string Buff_B_Refraction = "B_Refraction";
        public static string Buff_B_SCharge = "B_SCharge";
		/// <summary>
		/// 燐色存护
		/// 每层能抵抗1次无法战斗效果。
		/// 无法战斗抵抗<color=#5061A4>（&a%）</color>会增加濒死时受到的治疗量。
		/// 每20%全队的无法战斗抗性<color=#5061A4>（&b%）</color>增加1层燐色存护层数上限。
		/// </summary>
        public static string Buff_B_Shield = "B_Shield";
		/// <summary>
		/// 震荡冲击
		/// 无法使用攻击技能，若是敌人则停止行动一回合。
		/// 持有此效果的角色可以通过丢弃自己的技能来移除此效果。
		/// </summary>
        public static string Buff_B_Shock = "B_Shock";
		/// <summary>
		/// 暖焰
		/// 每当受到<color=#5061A4>&a</color>点及以上的治疗量时，赋予1层燐色存护。
		/// 叠加2层暖焰会减少一半治疗量的阈值。
		/// </summary>
        public static string Buff_B_WarmFire = "B_WarmFire";
		/// <summary>
		/// 梅朗柯莉
		/// Passive:
		/// 任何攻击会让梅朗柯莉<color=#FFC5BA><b>慌张</b></color>。但在回合开始时，梅朗柯莉会<color=#BAC8FF><b>冷静</b></color>下来。
		/// 固定技能始终为<color=#FF6767>湮裂的燐焰晶</color>，会攻击除自己以外所有人，战斗会摧毁周边障碍。
		/// 升级获得物品“燐晶核”2个，进入营地会获得1个，可以摧毁地形并概率发现少量遗失的物品。
		/// </summary>
        public static string Character_C_Merankori = "C_Merankori";
		/// <summary>
		/// 精密调石台
		/// 解除队伍无法战斗抵抗的上限，
		/// 使用燐晶核时，摧毁地图的半径增加1格。
		/// <color=#5061A4>—— 晶石调起来更顺了，比起依赖症，更加粗放的使用不是更好吗？梅朗柯莉酱你怎么不说话了...</color>
		/// </summary>
        public static string Item_Equip_E_AdjustCrystal = "E_AdjustCrystal";
		/// <summary>
		/// 火之守护
		/// 回合开始时，若没有燐色存护增益，则赋予1层燐色存护（抵抗无法战斗1次）。
		/// <color=#5061A4>——这的确是从草刺猬上掉落的，这里没被黑雾浸染的草刺猬都受到了主的关照，只是刚刚那只被主....？...愈术是什么，这里应该只有主的圣术。</color>
		/// </summary>
        public static string Item_Equip_E_FireProtect = "E_FireProtect";
		/// <summary>
		/// 燐晶核
		/// 右击可以让梅朗柯莉摧毁周边的墙体，可能发现小型物资。
		/// <color=#5061A4>  --- 赤红的晶石上透射着碎裂的回路...</color>
		/// </summary>
        public static string Item_Consume_I_RHA = "I_RHA";
		/// <summary>
		/// 逆流的燐焰晶
		/// 根据所有队友已损失的体力值增加伤害，每&a点增加&b%伤害。
		/// <color=#5061A4>（预期增加&c）</color>
		/// </summary>
        public static string SkillExtended_SE_Backflow = "SE_Backflow";
		/// <summary>
		/// 冷静
		/// 可以触发冷静效果
		/// </summary>
        public static string SkillExtended_SE_Calm = "SE_Calm";
		/// <summary>
		/// 蓄能
		/// 根据蓄能效果层数，每层增加10%的伤害。
		/// <color=#5061A4>（预期增加&a）</color>
		/// </summary>
        public static string SkillExtended_SE_Charge = "SE_Charge";
		/// <summary>
		/// 越频晶石
		/// 下一次<color=#FF6767>湮裂的燐焰晶</color>会附带“震荡冲击”效果。
		/// </summary>
        public static string SkillExtended_SE_OverClocking = "SE_OverClocking";
		/// <summary>
		/// 慌张
		/// 可以触发慌张效果
		/// </summary>
        public static string SkillExtended_SE_Panic = "SE_Panic";
		/// <summary>
		/// 整流
		/// 除了已指向的队友，技能也会应用给其他全体队友
		/// </summary>
        public static string SkillExtended_SE_Rectification = "SE_Rectification";
        public static string SkillEffect_SE_S_S_Charge = "SE_S_S_Charge";
        public static string SkillEffect_SE_S_S_D_BuffInfo = "SE_S_S_D_BuffInfo";
        public static string SkillEffect_SE_S_S_ElementHeal = "SE_S_S_ElementHeal";
        public static string SkillEffect_SE_S_S_Manifold = "SE_S_S_Manifold";
        public static string SkillEffect_SE_S_S_OverClocking = "SE_S_S_OverClocking";
        public static string SkillEffect_SE_S_S_WarmFire = "SE_S_S_WarmFire";
        public static string SkillEffect_SE_Tick_B_Backflow = "SE_Tick_B_Backflow";
        public static string SkillEffect_SE_Tick_B_Calm = "SE_Tick_B_Calm";
        public static string SkillEffect_SE_Tick_B_CalmDown = "SE_Tick_B_CalmDown";
        public static string SkillEffect_SE_Tick_B_Panic = "SE_Tick_B_Panic";
        public static string SkillEffect_SE_T_B_Attack_All = "SE_T_B_Attack_All";
        public static string SkillEffect_SE_T_S_Backup = "SE_T_S_Backup";
        public static string SkillEffect_SE_T_S_Care = "SE_T_S_Care";
        public static string SkillEffect_SE_T_S_D_BuffInfo = "SE_T_S_D_BuffInfo";
        public static string SkillEffect_SE_T_S_D_RefractionAtk = "SE_T_S_D_RefractionAtk";
        public static string SkillEffect_SE_T_S_ElementHeal = "SE_T_S_ElementHeal";
        public static string SkillEffect_SE_T_S_ItemRHA = "SE_T_S_ItemRHA";
        public static string SkillEffect_SE_T_S_Manifold = "SE_T_S_Manifold";
        public static string SkillEffect_SE_T_S_OverClocking = "SE_T_S_OverClocking";
        public static string SkillEffect_SE_T_S_P_Shock = "SE_T_S_P_Shock";
        public static string SkillEffect_SE_T_S_Reconstruct = "SE_T_S_Reconstruct";
        public static string SkillEffect_SE_T_S_Retreat = "SE_T_S_Retreat";
        public static string SkillEffect_SE_T_S_Shield = "SE_T_S_Shield";
        public static string SkillEffect_SE_T_S_WarmFire = "SE_T_S_WarmFire";
		/// <summary>
		/// 赋予折射
		/// 赋予1回合折射
		/// 过载，指向单体队友的技能
		/// </summary>
        public static string SkillExtended_SE_U_Refraction = "SE_U_Refraction";
		/// <summary>
		/// 赋予燐色存护
		/// 赋予1层燐色存护
		/// 1费及以上的技能
		/// </summary>
        public static string SkillExtended_SE_U_Shield = "SE_U_Shield";
		/// <summary>
		/// 冷静
		/// 梅朗柯莉处于“<color=#BAC8FF><b>冷静</b></color>”状态时触发。
		/// </summary>
        public static string SkillKeyword_SK_Calm = "SK_Calm";
		/// <summary>
		/// 慌张
		/// 处于“<color=#FFC5BA><b>慌张</b></color>”状态时触发
		/// </summary>
        public static string SkillKeyword_SK_Panic = "SK_Panic";
		/// <summary>
		/// <color=#FF6767>湮裂的燐焰晶</color>
		/// 对所有人造成伤害，除了梅朗柯莉自身。此伤害不会导致队友无法战斗。
		/// 在手中时，附加放逐。
		/// <color=#FFC5BA><b>慌张</b>：技能附加迅速</color>
		/// </summary>
        public static string Skill_S_Attack_All = "S_Attack_All";
		/// <summary>
		/// 后备的燐焰晶
		/// 抽取1个技能，
		/// 将<color=#FF6767>湮裂的燐焰晶</color>放入手中，直到填满手牌。
		/// </summary>
        public static string Skill_S_Backup = "S_Backup";
		/// <summary>
		/// 悉心照料
		/// <color=#BAC8FF><b>冷静</b>：目标若持有梅朗柯莉专属技能相关的正面效果，此效果则会额外增加1层</color>
		/// </summary>
        public static string Skill_S_Care = "S_Care";
		/// <summary>
		/// 蓄能
		/// 在此技能倒计时期间，梅朗柯莉每次进入<color=#BAC8FF>冷静</color>状态时，获得4层蓄能。
		/// 若以<color=#BAC8FF>冷静</color>状态完成蓄能，将2张<color=#FF6767>湮裂的燐焰晶</color>放入手中。
		/// </summary>
        public static string Skill_S_Charge = "S_Charge";
		/// <summary>
		/// 焰晶转化
		/// 丢弃选择的技能，抽取1个技能。
		/// 丢弃技能的拥有者获得2层频移微调。
		/// <color=#FFC5BA><b>慌张</b>：将1张<color=#FF6767>湮裂的燐焰晶</color>放入手中。</color>
		/// <color=#BAC8FF><b>冷静</b>：赋予此技能迅速</color>
		/// </summary>
        public static string Skill_S_ConvertCard = "S_ConvertCard";
		/// <summary>
		/// 常见效果预览
		/// 仅供常见效果预览，按下shift可以查看技能<color=#FF6767>湮裂的燐焰晶</color>
		/// </summary>
        public static string Skill_S_D_BuffInfo = "S_D_BuffInfo";
		/// <summary>
		/// 折射
		/// 折射来自“湮裂的燐焰晶”的伤害
		/// </summary>
        public static string Skill_S_D_RefractionAtk = "S_D_RefractionAtk";
		/// <summary>
		/// 元素调石
		/// 恢复全部体力极限。
		/// 每恢复&a体力极限，赋予自身1层“蓄能”。
		/// <color=#5061A4>（预计提供&b层）</color>
		/// </summary>
        public static string Skill_S_ElementHeal = "S_ElementHeal";
		/// <summary>
		/// 晶核生成
		/// 抽取1个技能。
		/// 获得燐晶核1个，可以用于摧毁地图上的墙体，发现埋藏的物品。
		/// </summary>
        public static string Skill_S_ItemRHA = "S_ItemRHA";
		/// <summary>
		/// 晶体流形
		/// <color=#BAC8FF><b>冷静</b>：手中每个<color=#FF6767>湮裂的燐焰晶</color>会额外提供3层“蓄能”</color>
		/// <color=#5061A4>（预计提供&a层）</color>
		/// </summary>
        public static string Skill_S_Manifold = "S_Manifold";
		/// <summary>
		/// 越频晶石
		/// <color=#BAC8FF><b>冷静</b>：延长“折射”1回合</color>
		/// </summary>
        public static string Skill_S_OverClocking = "S_OverClocking";
		/// <summary>
		/// 震荡冲击
		/// 预览效果
		/// </summary>
        public static string Skill_S_P_Shock = "S_P_Shock";
		/// <summary>
		/// 碎晶重构
		/// </summary>
        public static string Skill_S_Reconstruct = "S_Reconstruct";
		/// <summary>
		/// 回路整流
		/// 可以将梅朗柯莉指向单体队友的专属技能更改为对“全体友军”释放。
		/// 丢弃此技能时，优先抽取&a个梅朗柯莉的技能。
		/// <color=#BAC8FF><b>冷静</b>：抽取1个技能</color>
		/// </summary>
        public static string Skill_S_Rectification = "S_Rectification";
		/// <summary>
		/// 后撤！
		/// 抽取2个技能。
		/// 若指向的队友拥有至少5个正面效果，额外抽取1个技能。
		/// </summary>
        public static string Skill_S_Retreat = "S_Retreat";
		/// <summary>
		/// 燐晶电场
		/// 队友每损失5点体力值，获得1层蓄能。<color=#5061A4>（预期&a层）</color>
		/// <color=#BAC8FF><b>冷静</b>：处于濒死状态的队友额外获得2层“燐色存护”</color>
		/// </summary>
        public static string Skill_S_Shield = "S_Shield";
		/// <summary>
		/// 暖焰
		/// </summary>
        public static string Skill_S_WarmFire = "S_WarmFire";

    }

    public static class ModLocalization
    {
		/// <summary>
		/// Korean:
		/// English:
		/// When disabled, Lucy's Spine model will no longer be replaced with Melancholy's.
		/// Japanese:
		/// Chinese:
		/// 关闭后，不再使用梅朗柯莉替换露西Spine模型。
		/// Chinese-TW:
		/// 關閉後，不再使用梅朗柯莉替換露西Spine模型。
		/// </summary>
        public static string Setting_CustomSpine_Desc => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("Setting_CustomSpine_Desc");
		/// <summary>
		/// Korean:
		/// English:
		/// Never replace Lucy
		/// Japanese:
		/// Chinese:
		/// 不替换Spine人物模型
		/// Chinese-TW:
		/// 不替換Spine人物模型
		/// </summary>
        public static string Setting_CustomSpine_Title => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("Setting_CustomSpine_Title");
		/// <summary>
		/// Korean:
		/// English:
		/// I don’t need these cheap tricks—such fake power means nothing to me!  
		/// ---
		/// What’s the point of that... huh!  
		/// ---
		/// So tell me—are you the tank, or am I?  
		/// ---
		/// That’s not fiery enough for me.
		/// Japanese:
		/// Chinese:
		/// 我可不需要这些小伎俩，这种伪造的力量！
		/// ---
		/// 这种有什么用啊...啊！
		/// ---
		/// 你是t还是我是t？
		/// ---
		/// 你这不够火爆啊
		/// Chinese-TW:
		/// "我可不需要這些小伎倆，這種偽造的力量！
		/// ---
		/// 這種有什麼用啊...啊！
		/// ---
		/// 你是t還是我是t？
		/// ---
		/// 你這不夠火爆啊"
		/// </summary>
        public static string TSB_Phoenix_FirstShield => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("TSB_Phoenix_FirstShield");
		/// <summary>
		/// Korean:
		/// English:
		/// I’m starving—shields can’t fill my stomach!  
		/// ---
		/// ...A phoenix is iron, but bread is steel!
		/// Japanese:
		/// Chinese:
		/// 我饿了，盾填不了我的肚子！
		/// ---
		/// ..凤凰是铁，面包是钢！
		/// Chinese-TW:
		/// "我餓了，盾填不了我的肚子！
		/// ---
		/// ..鳳凰是鐵，麵包是鋼！ "
		/// </summary>
        public static string TSB_Phoenix_LowHP_0_Shield => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("TSB_Phoenix_LowHP_0_Shield");
		/// <summary>
		/// Korean:
		/// English:
		/// Are you trying to squeeze me dry??  
		/// ---
		/// I’m starving to death... well, can’t die, but still starving!
		/// Japanese:
		/// Chinese:
		/// 你是要榨干我吗？？
		/// ---
		/// 我要饿死...死不了但是饿啊！
		/// Chinese-TW:
		/// "你是要榨乾我嗎？？
		/// ---
		/// 我要餓死...死不了但是餓啊！ "
		/// </summary>
        public static string TSB_Phoenix_LowHP_100_Shield => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("TSB_Phoenix_LowHP_100_Shield");
		/// <summary>
		/// Korean:
		/// English:
		/// N...nood... bread!  
		/// ---
		/// I... I’m a ph...phoenix that... can’t starve to death...  
		/// ---
		/// I... I just wanna go back... and paint…
		/// Japanese:
		/// Chinese:
		/// 面...是包！
		/// ---
		/// 我可...不...饿不死的凤...
		/// ---
		/// 我...我想回去画画...
		/// Chinese-TW:
		/// "面...是包！
		/// ---
		/// 我可...不...餓不死的鳳...
		/// ---
		/// 我...我想回去畫畫..."
		/// </summary>
        public static string TSB_Phoenix_LowHP_2000_Shield => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("TSB_Phoenix_LowHP_2000_Shield");
		/// <summary>
		/// Korean:
		/// English:
		/// Y-you... you can’t do this to me!  
		/// ---
		/// Lucy! Stop her! I can’t take it anymore!  
		/// ---
		/// I’m the invincible Phoenix—don’t you dare turn me into fried chicken!
		/// Japanese:
		/// Chinese:
		/// 你..你不能这么对我！
		/// ---
		/// 露西！快阻止她！我受不了了！
		/// ---
		/// 我是无敌的凤凰，别拿我做炸鸡！
		/// Chinese-TW:
		/// "你..你不能這麼對我！
		/// ---
		/// 露西！快阻止她！我受不了了！
		/// ---
		/// 我是無敵的鳳凰，別拿我當炸雞！ "
		/// </summary>
        public static string TSB_Phoenix_LowHP_500_Shield => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("TSB_Phoenix_LowHP_500_Shield");
		/// <summary>
		/// Korean:
		/// English:
		/// Guess it’s not totally useless after all.  
		/// ---
		/// I... I’m the immortal Phoenix, you know!
		/// Japanese:
		/// Chinese:
		/// 这不是有点用嘛。
		/// ---
		/// 我..我可是不死的凤凰！
		/// Chinese-TW:
		/// "這不是有點用嘛。
		/// ---
		/// 我..我可是不死的鳳凰！ "
		/// </summary>
        public static string TSB_Phoenix_NoPassive_Shield => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("TSB_Phoenix_NoPassive_Shield");
		/// <summary>
		/// Korean:
		/// English:
		/// ...(shakes head)  
		/// ---
		/// It’s... it’s already clear around here, and I... I...  
		/// ---
		/// n-no... I... I don’t think so…
		/// Japanese:
		/// Chinese:
		/// ...（摇头）
		/// ---
		/// 周围已经是空地而且我...我...
		/// ---
		/// 不...不可以吧...
		/// Chinese-TW:
		/// "...（搖頭）
		/// ---
		/// 周圍已經是空地而且我...我...
		/// ---
		/// 不...不可以吧..."
		/// </summary>
        public static string TS_Merankori_RHA_CannotUse => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("TS_Merankori_RHA_CannotUse");
		/// <summary>
		/// Korean:
		/// English:
		/// I... I really can’t do it...  
		/// ---
		/// I’m a little confused too... I don’t understand why...  
		/// ---
		/// L...Lucy, I... I think I can’t break that area... and... and I already tried my best, but it didn’t budge at all, I... I don’t know why!
		/// Japanese:
		/// Chinese:
		/// 我...我真的做不到...
		/// ---
		/// 我也有点困惑...我不明白为什么...
		/// ---
		/// 露..露西我想我应该拆不了那边的区域而且...而且我已经尽力去弄了但是它丝毫不动我..我也不知道为什么！
		/// Chinese-TW:
		/// 「我...我真的做不到...
		/// ---
		/// 我也有點困惑...我不明白為什麼...
		/// ---
		/// 露..露西我想我應該拆不了那邊的區域而且...而且我已經盡力去弄了但是它絲毫不動我..我也不知道為什麼！ "
		/// </summary>
        public static string TS_Merankori_RHA_CannotUseHidden => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("TS_Merankori_RHA_CannotUseHidden");
		/// <summary>
		/// Korean:
		/// English:
		/// ...(looks confusedly at the charred tree)  
		/// ---
		/// m...maybe try... one more time...?  
		/// ---
		/// so... so confused…
		/// Japanese:
		/// Chinese:
		/// ...（困惑地看向焦黑的树木）
		/// ---
		/// 再...再试一次？
		/// ---
		/// 好困惑呀...
		/// Chinese-TW:
		/// "...（困惑地看向焦黑的樹木）
		/// ---
		/// 再...再試一次？
		/// ---
		/// 好困惑呀..."
		/// </summary>
        public static string TS_Merankori_RHA_FindHidden => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("TS_Merankori_RHA_FindHidden");
		/// <summary>
		/// Korean:
		/// English:
		/// You blew me up into a hedgehog—now hurry up and fix my feathers!  
		/// ---
		/// I just groomed those feathers!
		/// Japanese:
		/// Chinese:
		/// 你把我给炸成刺猬了，快给我梳毛！
		/// ---
		/// 我刚梳的毛！
		/// Chinese-TW:
		/// "你把我給炸成刺蝟了，快給我梳毛！
		/// ---
		/// 我剛梳的毛！ "
		/// </summary>
        public static string TS_Phoenix_UseRHA => ModManager.getModInfo("RHA_Merankori").localizationInfo.SystemLocalizationUpdate("TS_Phoenix_UseRHA");

    }
}