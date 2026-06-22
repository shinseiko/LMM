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
using LobotomyBaseModLib;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lobotomypatch
{
    [ModifiesType("WhiteNightSpace.DeathAngel")]
    public class DeathAngel_patch
    {
        [ModifiesMember("ActivateQliphothCounter")]
        public void ActivateQliphothCounter_patch()
        {
            global::PlaySpeedSettingUI.instance.SetNormalSpeedForcely();
            this.get_AnimScript().AdventClockUI.SetAdventEffectEndEvent(new AdventClockUI.EndEvent(this.Escape));
            List<ApostleGenData> adeventTargets;
            if(apostleData.Count == 0)
            {
             string str =   string.Concat(new object[]
            {
                Application.persistentDataPath,
                "/creatureData/",
                "100014",
                ".dat"
            });
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(str, FileMode.Open);
                Dictionary<string, object> dic = (Dictionary<string, object>)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
                LoadData(dic);
            }
            if (this.apostles.Count == 0)
            {
                adeventTargets = DeathAngel.GetAdeventTargets(this.apostleData);
            }
            else
            {
                adeventTargets = this.genDataSave;
            }
            this.GenApostle(adeventTargets);
            this.get_AnimScript().AdventClockUI.StartSimpleAdventEvent();
            this.get_AnimScript().AdventClockUI.SimpleAdventStart(adeventTargets);
        }
        [NewMember]
        public void LoadData(Dictionary<string, object> dic)
        {
			
            int num = -1;
            this.apostleData.Clear();
            if (!global::GameUtil.TryGetValue<int>(dic, "apostleListCount", ref num))
            {
                PlagueDoctor.Log("Failed To ApostleData", true);
            }
            else
            {
                if (num == -1)
                {
                    return;
                }
                Dictionary<int, Dictionary<string, object>> data = null;
                if (global::GameUtil.TryGetValue<Dictionary<int, Dictionary<string, object>>>(dic, "apostleList", ref data))
                {
                    this.LoadApostleSaveData(num, data);
                }
            }
        }
        [NewMember]
        private void LoadApostleSaveData(int max, Dictionary<int, Dictionary<string, object>> data)
        {
            for (int i = 0; i < max; i++)
            {
                Dictionary<string, object> data2 = null;
                if (data.TryGetValue(i, out data2))
                {
                    ApostleData apostleData = new ApostleData(data2);
                    this.apostleData.Add(apostleData);
                    PlagueDoctor.Log("Load Apostle : " + apostleData.NameId, false);
                }
            }
        }



        [MemberAlias("GenApostle", typeof(DeathAngel))]
        public void GenApostle(List<ApostleGenData> genDataList)
        {
        }
        [MemberAlias("Escape", typeof(DeathAngel))]
        public void Escape()
        {
        }
        [MemberAlias("get_AnimScript", typeof(DeathAngel))]
        public DeathAngelAnim get_AnimScript()
        {
            return null;
        }

        [MemberAlias("genDataSave", typeof(DeathAngel))]
        private List<ApostleGenData> genDataSave;
        [MemberAlias("apostles", typeof(DeathAngel))]
        private List<DeathAngelApostle> apostles;
        [MemberAlias("apostleData", typeof(DeathAngel))]
        private List<ApostleData> apostleData;
    }
}
