using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Patchwork.Attributes;
using System.IO;
using UnityEngine;
using Spine.Unity;
using System.Reflection;
using LobotomyBaseMod;
using UnityEngine.UI;
using System.Xml;
using static WorkerSprite.WorkerSpriteSaveData;
using System.Collections;
using System.Security.Cryptography;
using CreatureGenerate;
using Spine;
using CreatureSelect;
using WhiteNightSpace;
using Lobotomypatch;
using CreatureInfo;
using UnityEngine.SocialPlatforms;

namespace Lobotomypatch
{
    [ModifiesType("BossBird")]
    public class BossBird
    {
        [ModifiesMember("CheckTwilight")]
        private bool CheckTwilight_patch()
        {
            List<global::AgentModel> list = new List<global::AgentModel>(global::AgentManager.instance.GetAgentList());
            foreach (global::AgentModel agentModel in list)
            {
                if (!agentModel.IsDead())
                {
                    global::WeaponModel weapon = agentModel.Equipment.weapon;
                    global::ArmorModel armor = agentModel.Equipment.armor;
                    if (weapon != null && EquipmentTypeInfo_patch.GetLcId(weapon.metaInfo) == 200038)
                    {
                        return false;
                    }
                    if (armor != null && EquipmentTypeInfo_patch.GetLcId(armor.metaInfo) == 300038)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
