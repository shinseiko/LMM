using LobotomyBaseMod;
using Patchwork.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LobotomyBaseModLib;
using UnityEngine;

namespace Lobotomypatch
{
    [ModifiesType("OptionUI")]
    public class OptionUI_patch
    {
        [ModifiesMember("Awake")]
        private void Awake_patch()
        {
            if (OptionUI.Instance != null)
            {
                UnityEngine.Object.Destroy(get_gameObject());
                return;
            }
            credit = new string[]
    {
        "Poten / Ro / KevinGlass / ade007 / Amiba / Sea / Nicholas_Okra",
        "acane / 甘輪 / いすひろし / とらきす / 翻訳協力者" + Environment.NewLine + "もちみかん / Youkan / ログノ / 6人目のサメの餌",
        "surolanter",
        "Tales&Stories Team / GregorLesnov / BlinkRaven / Knightey",
        "Misui",
        "Dimitar Topkov (di_TOP)",
        "Main Translator : @UrathObsidian" + Environment.NewLine + "Programmer/Language Assistant : @casual_watson ",
        "Cool Kids Club Translation : NEETPenguin and Casual Watson\r\nHelpers : Catling and Kuroteru\r\nLastly… Many thanks to all who tested our script and supported us!",
        "Traduction Eden Office : Azuro, Nakys, Skriff, Skun et Pacman\r\nTous nos remerciements à l'ensemble de nos testeurs et aux personnes qui nous ont aidé !",
        "Muito Obrigado Pela Tradução!\r\n\r\nTradução Team P.A.T.O: Shoes & Arucato\r\nAgradecimentos adicionais a nosso grande testador: Efeshis, todos aqueles que nos ajudaram até aqui, e a equipe de nosso companheiro Milk!",
        "OBRIGADO PELA VOSSA TRADUÇÃO!\r\nTradutora : R.ANAKOVA"
    };
            set_Instance(this.ForceTypeChange<OptionUI>());
            this.creditText.Add("cn", credit[0]);
            this.creditText.Add("jp", credit[1]);
            this.creditText.Add("cn_tr", credit[2]);
            this.creditText.Add("ru", credit[3]);
            this.creditText.Add("vn", credit[4]);
            this.creditText.Add("bg", credit[5]);
            this.creditText.Add("es", credit[6]);
            this.creditText.Add("en", credit[7]);
            this.creditText.Add("fr", credit[8]);
            this.creditText.Add("pt_br", credit[9]);
            this.creditText.Add("pt_pt", credit[10]);
        }
      


        [MemberAlias("set_Instance", typeof(OptionUI))]
        private static void set_Instance(global::OptionUI value)
        {
        }
        [MemberAlias("get_gameObject", typeof(Component))]
        public GameObject get_gameObject()
        {
            return null;
        }

        [MemberAlias("creditText", typeof(OptionUI))]
        private Dictionary<string, string> creditText;
        [MemberAlias("credit", typeof(OptionUI))]
        private static string[] credit;
    }
    [ModifiesType("OrdealManager")]
    public class OrdealManager_patch
    {
        [NewMember]
        private void BuildCreature_Mod(global::OrdealCreatureModel model, LcIdLong metadataId)
        {
            CreatureTypeInfo data = CreatureTypeList.instance.ForceTypeChange<CreatureTypeList_patch>().GetData_Mod(metadataId);
            object obj = ExtenionUtil.GetTypeInstance<CreatureBase>(data.script);
            if (obj == null)
            {
                obj = Activator.CreateInstance(Type.GetType(data.script));
            }
            model.script = (global::CreatureBase)obj;
            model.observeInfo = new global::CreatureObserveInfoModel(metadataId.id);
            model.observeInfo.ForceTypeChange<CreatureObserveInfoModel_patch>().InitData_Mod(metadataId);
            string text = "1";
            model.sefira = global::SefiraManager.instance.GetSefira(text);
            model.sefiraNum = text;
            model.metadataId = metadataId.id;
            model.metaInfo = data;
            if (global::CreatureTypeList.instance.ForceTypeChange<CreatureTypeList_patch>().GetSkillTipData_Mod(metadataId) != null)
            {
                model.metaInfo.specialSkillTable = global::CreatureTypeList.instance.ForceTypeChange<CreatureTypeList_patch>().GetSkillTipData_Mod(metadataId).GetCopy();
            }
            model.script.SetModel(model);
            model.script.OnInitialBuild();
        }
        [ModifiesMember("BuildCreature")]
        private void BuildCreature_patch(global::OrdealCreatureModel model, long metadataId)
        {
            BuildCreature_Mod(model, new LcIdLong(metadataId));
        }
        [NewMember]
        public global::OrdealCreatureModel AddCreature_Mod(LcIdLong metadataId, global::MapNode pos, global::OrdealBase ordealBase)
        {
            global::OrdealCreatureModel ordealCreatureModel = new global::OrdealCreatureModel((long)this.nextInstId++);
            this.BuildCreature_Mod(ordealCreatureModel, metadataId);
            ordealCreatureModel.GetMovableNode().SetCurrentNode(pos);
            ordealCreatureModel.GetMovableNode().SetActive(true);
            ordealCreatureModel.baseMaxHp = ordealCreatureModel.metaInfo.maxHp;
            ordealCreatureModel.hp = (float)ordealCreatureModel.metaInfo.maxHp;
            ordealCreatureModel.SetOrdealBase(ordealBase);
            this.ordealCreatureList.Add(ordealCreatureModel);
            global::Notice.instance.Send(global::NoticeName.AddOrdealCreature, new object[]
            {
        ordealCreatureModel
            });
            ordealCreatureModel.script.OnInit();
            global::Sefira sefira = global::SefiraManager.instance.GetSefira(pos.GetAttachedPassage().GetSefiraName());
            ordealCreatureModel.sefira = sefira;
            ordealCreatureModel.sefiraNum = sefira.indexString;
            return ordealCreatureModel;
        }
        [ModifiesMember("AddCreature")]
        public global::OrdealCreatureModel AddCreature_patch(long metadataId, global::MapNode pos, global::OrdealBase ordealBase)
        {
            return AddCreature_Mod(new LcIdLong(metadataId),pos,ordealBase);
        }


        [MemberAlias("ordealCreatureList", typeof(OrdealManager))]
        private List<global::OrdealCreatureModel> ordealCreatureList = new List<global::OrdealCreatureModel>();

        [MemberAlias("nextInstId",typeof(OrdealManager))]
        private int nextInstId = 1;
    }
}
