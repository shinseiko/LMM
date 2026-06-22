using LobotomyBaseMod;
using Patchwork.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using LobotomyBaseModLib;
using System.Reflection;

namespace Lobotomypatch
{
    [ModifiesType("SupportedLanguage")]
    public static class SupportedLanguage_patch
    {
        [ModifiesMember("GetSupprotedList")]
        public static List<string> GetSupprotedList_patch()
        {
            return new List<string>
        {
            "en",
            "kr",
            "cn",
            "cn_tr",
            "jp",
            "ru",
            "vn",
            "bg",
            "es",
            "fr",
            "pt_br",
            "pt_pt"
        };
        }
        [ModifiesMember("GetCurrentLanguageName")]
        public static string GetCurrentLanguageName_patch(string language)
        {
            switch (language)
            {
                case "kr":
                    return "한국어";
                case "cn":
                    return "中文(简体)";
                case "cn_tr":
                    return "中文(繁體)";
                case "jp":
                    return "日本語";
                case "ru":
                    return "русский";
                case "vn":
                    return "Tiếng Việt";
                case "bg":
                    return "български";
                case "es":
                    return "Español Latinoamérica";
                case "fr":
                    return "français";
                case "pt_br":
                    return "Português do Brasil";
                case "pt_pt":
                    return "Português";
            }
            return "English";
        }

        [NewMember]
        public const string pt_br = "pt_br";

        [NewMember]
        public const string pt_br_Name = "Português do Brasil";

        [NewMember]
        public const string pt_pt = "pt_pt";

        [NewMember]
        public const string pt_pt_Name = "Português";
    }
    [ModifiesType("SpecialEventManager")]
    public class SpecialEventManager_patch
    {
        [NewMember]
        private void BuildCreature_Mod(global::EventCreatureModel model, LcIdLong metadataId)
        {
            model.observeInfo = new global::CreatureObserveInfoModel(metadataId.id);
            model.observeInfo.ForceTypeChange<CreatureObserveInfoModel_patch>().InitData_Mod(metadataId);
            string text = "1";
            model.sefira = global::SefiraManager.instance.GetSefira(text);
            model.sefiraNum = text;
            global::CreatureTypeInfo data = global::CreatureTypeList.instance.ForceTypeChange<CreatureTypeList_patch>().GetData_Mod(metadataId);
            model.metadataId = metadataId.id;
            model.metaInfo = data;
            if (global::CreatureTypeList.instance.ForceTypeChange<CreatureTypeList_patch>().GetSkillTipData_Mod(metadataId) != null)
            {
                model.metaInfo.specialSkillTable = global::CreatureTypeList.instance.ForceTypeChange<CreatureTypeList_patch>().GetSkillTipData_Mod(metadataId).GetCopy();
            }
            object obj = ExtenionUtil.GetTypeInstance<CreatureBase>(data.script);
            if (obj == null)
            {
                obj = Activator.CreateInstance(Type.GetType(data.script));
            }
            if (obj is global::CreatureBase)
            {
                model.script = (global::CreatureBase)obj;
            }
            else
            {
                Debug.Log("Creature Script not found");
            }
            model.script.SetModel(model);
            model.script.OnInitialBuild();
        }
        [ModifiesMember("BuildCreature")]
        private void BuildCreature_patch(global::EventCreatureModel model, long metadataId)
        {
            BuildCreature_Mod(model, new LcIdLong(metadataId));
        }
        [NewMember]
        public global::EventCreatureModel AddCreature_Mod(LcIdLong metadataId, global::MapNode pos, global::EventBase eventBase)
        {
            global::EventCreatureModel eventCreatureModel = new global::EventCreatureModel((long)this.nextInstId++);
            this.BuildCreature_Mod(eventCreatureModel, metadataId);
            eventCreatureModel.GetMovableNode().SetCurrentNode(pos);
            eventCreatureModel.GetMovableNode().SetActive(true);
            eventCreatureModel.baseMaxHp = eventCreatureModel.metaInfo.maxHp;
            eventCreatureModel.hp = (float)eventCreatureModel.metaInfo.maxHp;
            eventCreatureModel.SetEventBase(eventBase);
            this.eventCreatureList.Add(eventCreatureModel);
            global::Notice.instance.Send(global::NoticeName.AddEventCreature, new object[]
            {
            eventCreatureModel
            });
            eventCreatureModel.script.OnInit();
            global::Sefira sefira = global::SefiraManager.instance.GetSefira(pos.GetAttachedPassage().GetSefiraName());
            eventCreatureModel.sefira = sefira;
            eventCreatureModel.sefiraNum = sefira.indexString;
            return eventCreatureModel;
        }
        [ModifiesMember("AddCreature")]
        public global::EventCreatureModel AddCreature_patch(long metadataId, global::MapNode pos, global::EventBase eventBase)
        {
            return AddCreature_Mod(new LcIdLong(metadataId),pos,eventBase);
        }



        [MemberAlias("eventCreatureList", typeof(SpecialEventManager))]
        private List<global::EventCreatureModel> eventCreatureList = new List<global::EventCreatureModel>();

        [MemberAlias("nextInstId",typeof(SpecialEventManager))]
        private int nextInstId = 1000;
    }
    [ModifiesType("SoundEffectPlayer")]
    public class SoundEffectPlayer_patch
    {
        [NewMember]
        public static SoundEffectPlayer PlayOnce_Mod(string modid, string soundname, Vector2 position, float pitch, float volume, AudioRolloffMode mode)
        {
            AudioClip audioClip = ModAudioClipManager.Instance.GetAudioClip(modid, soundname);
            if (audioClip == null) audioClip = Resources.Load<AudioClip>("Sounds/" + soundname);
            if (audioClip == null)
            {
                return null;
            }
            GameObject gameObject = global::Prefab.LoadPrefab("SoundEffectPlayer");
            global::SoundEffectPlayer component = gameObject.GetComponent<global::SoundEffectPlayer>();
            AudioSource component2 = gameObject.GetComponent<AudioSource>();
            component2.pitch = pitch;
            gameObject.transform.position = new Vector3(position.x, position.y, Camera.main.transform.position.z);
            component2.clip = audioClip;
            component2.volume = volume;
            component2.rolloffMode = mode;
            component2.Play();
            component.SetFieldValue("destroyTime", audioClip.length);
            return component;
        }

        [NewMember]
        public static SoundEffectPlayer PlayOnce_Mod(string modid, string filename, Vector2 position)
        {
            return PlayOnce_Mod(modid,filename, position, 1, 1, AudioRolloffMode.Logarithmic);
        }

        [NewMember]
        public static global::SoundEffectPlayer PlayOnce_Mod(string modid, string filename, float pitch, Vector2 position)
        {
            return PlayOnce_Mod(modid, filename, position, pitch, 1, AudioRolloffMode.Logarithmic);
        }

        [NewMember]
        public static global::SoundEffectPlayer PlayOnce_Mod(string modid, string filename, Vector2 position, float volume)
        {
            return PlayOnce_Mod(modid, filename, position, 1, volume, AudioRolloffMode.Logarithmic);
        }

        [NewMember]
        public static global::SoundEffectPlayer PlayOnce_Mod(string modid, string filename, Vector2 position, AudioRolloffMode mode)
        {
            return PlayOnce_Mod(modid, filename, position, 1, 1, mode);
        }

        [NewMember]
        public static SoundEffectPlayer PlayOnce_legacy(string filename, float pitch, float volume, Vector2 position, AudioRolloffMode mode)
        {
            return PlayOnce_Mod(string.Empty, filename, position, pitch, volume, mode);
            
        }
        [ModifiesMember("PlayOnce")]
        public static SoundEffectPlayer PlayOnce_patch(string filename, Vector2 position)
        {
            return PlayOnce_legacy(filename, 1,1, position,AudioRolloffMode.Logarithmic);
        }

        [ModifiesMember("PlayOnce")]
        public static global::SoundEffectPlayer PlayOnce_patch(string filename, float pitch, Vector2 position)
        {
            return PlayOnce_legacy(filename, pitch, 1, position, AudioRolloffMode.Logarithmic);
        }

        [ModifiesMember("PlayOnce")]
        public static global::SoundEffectPlayer PlayOnce_patch(string filename, Vector2 position, float volume)
        {
            return PlayOnce_legacy(filename, 1, volume, position, AudioRolloffMode.Logarithmic);
        }

        [ModifiesMember("PlayOnce")]
        public static global::SoundEffectPlayer PlayOnce_patch(string filename, Vector2 position, AudioRolloffMode mode)
        {
            return PlayOnce_legacy(filename, 1, 1, position, mode);
        }

        [NewMember]
        public static global::SoundEffectPlayer Play_Mod(string modid, string soundname, Transform transf, float pitch, float volume, AudioRolloffMode mode)
        {
            AudioClip audioClip = ModAudioClipManager.Instance.GetAudioClip(modid, soundname);
            if (audioClip == null) audioClip = Resources.Load<AudioClip>("Sounds/" + soundname);
            if (audioClip == null)
            {
                return null;
            }
            GameObject gameObject = global::Prefab.LoadPrefab("SoundEffectPlayer");
            Vector2 vector = transf.position;
            gameObject.transform.SetParent(transf);
            gameObject.transform.localScale = Vector3.one;
            global::SoundEffectPlayer component = gameObject.GetComponent<global::SoundEffectPlayer>();
            component.SetFieldValue("onshot", false);
            AudioSource component2 = gameObject.GetComponent<AudioSource>();
            component2.pitch = pitch;
            gameObject.transform.position = new Vector3(vector.x, vector.y, Camera.main.transform.position.z);
            component2.clip = audioClip;
            component2.loop = true;
            component2.volume = volume;
            component2.rolloffMode = mode;
            component2.Play();
            return component;
        }

        [ModifiesMember("Play")]
        public static global::SoundEffectPlayer Play_patch(string filename, Transform transf)
        {
            return Play_patch(filename, transf, 1);
        }

        [ModifiesMember("Play")]
        public static global::SoundEffectPlayer Play_patch(string filename, Transform transf, float volume)
        {
            return Play_Mod(string.Empty, filename, transf, 1, volume, AudioRolloffMode.Logarithmic);
        }


    }
    [ModifiesType("SoundInfo")]
    public class SoundInfo_patch
    {
        [NewMember]
        public SoundEffectPlayer PlaySound_Mod(string modid, Vector2 pos)
        {
            return SoundEffectPlayer_patch.PlayOnce_Mod(modid,this.soundSrc, pos);
        }
        [ModifiesMember("PlaySound")]
        public SoundEffectPlayer PlaySound_patch(Vector2 pos)
        {
            return PlaySound_Mod(string.Empty, pos);
        }

        [MemberAlias("soundSrc",typeof(SoundInfo))]
        public string soundSrc;
    }
    [ModifiesType("SefiraPanel")]
    public class SefiraPanel_patch
    {
        [ModifiesMember("OnClickCreaturePortriat")]
        public void OnClickCreaturePortriat_patch(int i)
        {
            if (this.creatureSlots[i].isInit)
            {
                if (!this.creatureSlots[i].creature.script.OnOpenCollectionWindow())
                {
                    return;
                }
                CreatureInfoWindow_patch.CreateWindow_Mod(CreatureTypeList_patch.instance.GetLcId(this.creatureSlots[i].creature.metaInfo));
                this.SefiraSound.OnPlayInList(1);
            }
        }

        [MemberAlias("SefiraSound", typeof(SefiraPanel))]
        public AudioClipPlayer SefiraSound;
        [MemberAlias("creatureSlots", typeof(SefiraPanel))]
        public SefiraPanel.CreaturePortrait[] creatureSlots;
    }

    [ModifiesType("SefiraIsolateManagement")]
    public class SefiraIsolateManagement_patch
    {
        [NewMember]
        public global::SefiraIsolate[] GenIsolateByCreatureAryByOrder_Mod(LcIdLong[] creatureIdAry)
        {
            List<global::SefiraIsolate> list = new List<global::SefiraIsolate>();
            foreach (LcIdLong creatureId in creatureIdAry)
            {
                global::SefiraIsolate notUsed = this.GetNotUsed();
                notUsed.creatureId = creatureId.id;
                ((SefiraIsolate_patch)(object)notUsed).modid = creatureId.packageId;
                list.Add(notUsed);
            }
            return list.ToArray();
        }

        [MemberAlias("GetNotUsed", typeof(SefiraIsolateManagement))]
        public global::SefiraIsolate GetNotUsed()
        {
            return null;
        }
    }
    [ModifiesType("SefiraIsolate")]
    public class SefiraIsolate_patch
    {
        [NewMember]
        public LcIdLong GetLcId()
        {
            if (modid == null)
            {
                return new LcIdLong(creatureId);
            }
            return new LcIdLong(modid, creatureId);
        }
        [NewMember]
        public string modid;

        [MemberAlias("creatureId", typeof(SefiraIsolate))]
        public long creatureId;
    }
    [ModifiesType("SefiraManager")]
    public class SefiraManager_patch
    {
        [NewMember]
        private void AddCreature_Mod(LcIdLong[] list, global::Sefira sefira)
        {
            if (list.Length == 0)
            {
                return;
            }
            List<LcIdLong> list2 = new List<LcIdLong>();
            foreach (long id in new List<long>(global::CreatureGenerateInfo.GetAll(false)))
            {
                list2.Add(new LcIdLong(id));
            }
            list2.AddRange(CreatureGenerateInfo_patch.GetAll_Mod(false));

            foreach (global::CreatureModel creatureModel in global::CreatureManager.instance.GetCreatureList())
            {
                LcIdLong lcid = new LcIdLong(CreatureTypeList_patch.instance.GetModId(creatureModel.metaInfo), creatureModel.metadataId);
                list2.Remove(lcid);
            }
            List<LcIdLong> list3 = new List<LcIdLong>();
            foreach (LcIdLong item in list)
            {
                if (list2.Contains(item))
                {
                    list3.Add(item);
                }
            }
            if (list3.Count == 0)
            {
                return;
            }
            List<LcIdLong> list4 = new List<LcIdLong>();
            list4 = list3;
            global::SefiraIsolate[] array = ((SefiraIsolateManagement_patch)(object)sefira.isolateManagement).GenIsolateByCreatureAryByOrder_Mod(list4.ToArray());
            foreach (global::SefiraIsolate sefiraIsolate in array)
            {
                CreatureManager_patch.instance().AddCreature_Mod(((SefiraIsolate_patch)(object)sefiraIsolate).GetLcId(), sefiraIsolate, sefira.indexString);
            }
        }
        [ModifiesMember("OpenSefiraWithCreature")]
        public void OpenSefiraWithCreature_patch(global::SefiraEnum sefiraEnum)
        {
            global::Sefira sefira = this.GetSefira(sefiraEnum);
            if (!sefira.activated)
            {
                sefira.Activate();
                global::SefiraCharacterManager.instance.OnOpenSefira(sefira.sefiraEnum);
                global::Notice.instance.Send(global::NoticeName.OpenArea, new object[]
                {
                sefira
                });
            }
            sefira.AddOpenLevel();
            if (sefiraEnum == global::SefiraEnum.TIPERERTH1 && sefira.openLevel >= 3)
            {
                global::Sefira sefira2 = this.GetSefira(global::SefiraEnum.TIPERERTH2);
                if (!sefira2.activated)
                {
                    sefira2.Activate();
                    global::MapGraph.instance.ActivateArea(sefira2.indexString, "1");
                    global::Notice.instance.Send(global::NoticeName.OpenArea, new object[]
                    {
                    sefira2
                    });
                }
                sefira2.SetOpenLevel(sefira.openLevel);
                global::MapGraph.instance.ActivateArea(sefira2.indexString, "1");
            }
            else if (sefiraEnum == global::SefiraEnum.KETHER && sefira.openLevel >= 5)
            {
                global::Sefira sefira3 = this.GetSefira(global::SefiraEnum.DAAT);
                if (!sefira3.activated)
                {
                    sefira3.Activate();
                    global::MapGraph.instance.ActivateArea(sefira3.indexString, "1");
                    global::Notice.instance.Send(global::NoticeName.OpenArea, new object[]
                    {
                    sefira3
                    });
                    sefira3.SetOpenLevel(1);
                }
            }
            global::MapGraph.instance.ActivateArea(sefira.indexString, sefira.openLevel.ToString());
            if (sefira.openLevel <= 4)
            {
                if (sefiraEnum == global::SefiraEnum.TIPERERTH1)
                {
                    if (sefira.openLevel <= 2)
                    {
                        this.AddCreature_Mod(this.GetCreatureGenerationList_Mod(sefira.openLevel), sefira);
                    }
                    else
                    {
                        global::Sefira sefira4 = this.GetSefira(global::SefiraEnum.TIPERERTH2);
                        this.AddCreature_Mod(this.GetCreatureGenerationList_Mod(sefira.openLevel), sefira4);
                    }
                }
                else if (sefiraEnum == global::SefiraEnum.KETHER)
                {
                    this.AddCreature_Mod(this.GetCreatureGenerationList_Mod(sefira.openLevel), sefira);
                }
                else if (sefiraEnum != global::SefiraEnum.DAAT)
                {
                    this.AddCreature_Mod(this.GetCreatureGenerationList_Mod(sefira.openLevel), sefira);
                }
            }
        }
        [NewMember]
        private LcIdLong[] GetCreatureGenerationList_Mod(int openLevel)
        {
            if (openLevel > 4)
            {
                return new LcIdLong[0];
            }
            LcIdLong item;
            List<LcIdLong> list = new List<LcIdLong>();
            while (((PlayerModel_patch)(object)PlayerModel.instance).GetWaitingCreature_Mod(out item))
            {
                list.Add(item);
            }
            return list.ToArray();
        }

        [MemberAlias("GetSefira", typeof(SefiraManager))]
        public global::Sefira GetSefira(global::SefiraEnum sefira)
        {
            return null;
        }
    }
}
