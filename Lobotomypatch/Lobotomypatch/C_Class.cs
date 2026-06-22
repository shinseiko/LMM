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
using UnityEngine.SceneManagement;

namespace Lobotomypatch
{
   
    [ModifiesType("CreatureInfo.CreatureInfoKitLayoutController")]
    public class CreatureInfoKitLayoutController_patch
    {
        [ModifiesMember("SetDataList")]
        public void SetDataList_patch(global::CreatureTypeInfo metaInfo, global::CreatureObserveInfoModel observeInfo)
        {
            this.Clear();
            if (metaInfo.specialSkillTable == null)
            {
                metaInfo.specialSkillTable = global::CreatureTypeList.instance.ForceTypeChange<CreatureTypeList_patch>().GetSkillTipData_Mod(CreatureTypeInfo_patch.GetLcId(metaInfo)).GetCopy();
            }
            int num = Mathf.Max(metaInfo.specialSkillTable.descList.Count, metaInfo.desc.Count);
            num = Mathf.Min(num, global::CreatureModel.careTakingRegion.Length);
            string collectionName = metaInfo.collectionName;
            int i = 0;
            while (i < num)
            {
                string text = string.Empty;
                if (i < metaInfo.desc.Count)
                {
                    text = metaInfo.desc[i];
                }
                string text2 = string.Empty;
                if (i < metaInfo.specialSkillTable.descList.Count)
                {
                    text2 = metaInfo.specialSkillTable.descList[i].original;
                }
                string text3 = text;
                int num2 = 0;
                for (; ; )
                {
                    int num3 = text3.IndexOf("#" + num2);
                    if (text3 == string.Empty || text3 == null)
                    {
                        break;
                    }
                    if (num3 == -1)
                    {
                        break;
                    }
                    num2++;
                }
                string text4 = text2;
                int num4 = 0;
                for (; ; )
                {
                    int num5 = text4.IndexOf("#" + num4);
                    if (text4 == string.Empty || text4 == null)
                    {
                        break;
                    }
                    if (num5 == -1)
                    {
                        break;
                    }
                    num4++;
                }
                text4 = text4.Replace("$0", collectionName);
                CreatureInfoKitDataSlot creatureInfoKitDataSlot = this.MakeEmptySlot();
                creatureInfoKitDataSlot.Left_RecordText.text = text3;
                creatureInfoKitDataSlot.Right_UseText.text = text4;
                int observeCost = observeInfo.GetObserveCost(global::CreatureModel.careTakingRegion[i]);
                if (metaInfo.creatureKitType == global::CreatureKitType.ONESHOT)
                {
                    if (observeInfo.totalKitUseCount < observeCost)
                    {
                        creatureInfoKitDataSlot.disabled.SetActive(true);
                        creatureInfoKitDataSlot.needCostText.text = global::LocalizeTextDataModel.instance.GetText("CreatureInfoKit_NeedUseCount") + "\n" + observeCost.ToString();
                    }
                    else
                    {
                        creatureInfoKitDataSlot.disabled.SetActive(false);
                    }
                }
                else
                {
                    int num6 = (int)observeInfo.totalKitUseTime;
                    if (num6 < observeCost)
                    {
                        creatureInfoKitDataSlot.disabled.SetActive(true);
                        creatureInfoKitDataSlot.needCostText.text = global::LocalizeTextDataModel.instance.GetText("CreatureInfoKit_NeedUseTime") + "\n" + string.Format("{0}:{1:D2}", observeCost / 60, observeCost % 60);
                    }
                    else
                    {
                        creatureInfoKitDataSlot.disabled.SetActive(false);
                    }
                }
                i++;
                continue;
            }
        }

        [MemberAlias("MakeEmptySlot", typeof(CreatureInfoKitLayoutController))]
        public CreatureInfoKitDataSlot MakeEmptySlot()
        {
            return null;
        }
        [MemberAlias("Clear",typeof(CreatureInfoKitLayoutController))]
        public void Clear()
        {
            
        }
    }

    [ModifiesType("CreatureInfo.CreatureInfoCaretakingRoot")]
    public class CreatureInfoCaretakingRoot_patch
    {
        [ModifiesMember("ListInit")]
        private void ListInit_patch()
        {
            if (get_MetaInfo().specialSkillTable == null)
            {
                get_MetaInfo().specialSkillTable = CreatureTypeList.instance.ForceTypeChange<CreatureTypeList_patch>().GetSkillTipData_Mod(CreatureTypeInfo_patch.GetLcId(get_MetaInfo())).GetCopy();
            }
            int count = get_MetaInfo().specialSkillTable.descList.Count;
            this.revealedCount = count;
            string unitName = global::CreatureModel.GetUnitName(get_MetaInfo(), get_ObserveInfo());
            int i = 0;
            while (i < count)
            {
                string original = get_MetaInfo().specialSkillTable.descList[i].original;
                string text = original;
                int num = 0;
                for (; ; )
                {
                    int num2 = text.IndexOf("#" + num);
                    if (text == string.Empty || text == null)
                    {
                        break;
                    }
                    if (num2 == -1)
                    {
                        break;
                    }
                    global::AgentName agentName = null;
                    if (!get_MetaInfo().GetAgentName(i * 100 + num, out agentName))
                    {
                        agentName = global::AgentNameList.instance.GetFakeNameByInfo();
                        get_MetaInfo().AddAgentName(i * 100 + num, agentName);
                    }
                    text = text.Replace("#" + num, agentName.GetName());
                    num++;
                }
                text = text.Replace("$0", unitName);
                CreatureInfoCaretakingSlot creatureInfoCaretakingSlot = this.slots[i];
                this.slots[i].gameObject.SetActive(true);
                creatureInfoCaretakingSlot.SetData(text);
                if (i > 0)
                {
                    creatureInfoCaretakingSlot.PrevSlot = this.slots[i - 1];
                }
                i++;
                continue;
            }
            for (int j = count; j < 7; j++)
            {
                this.slots[j].gameObject.SetActive(false);
            }
            
        }

        [MemberAlias("get_MetaInfo",typeof(CreatureInfoController))]
        public global::CreatureTypeInfo get_MetaInfo()
        {
            return global::CreatureInfoWindow.CurrentWindow.MetaInfo;
        }
        [MemberAlias("get_ObserveInfo", typeof(CreatureInfoController))]
        public global::CreatureObserveInfoModel get_ObserveInfo()
        {
            return global::CreatureInfoWindow.CurrentWindow.ObserveInfo;
        }


        [MemberAlias("revealedCount", typeof(CreatureInfoCaretakingRoot))]
        private int revealedCount;
        [MemberAlias("slots", typeof(CreatureInfoCaretakingRoot))]
        public List<CreatureInfoCaretakingSlot> slots;
    }
    [ModifiesType("CreatureLayer")]
    public class CreatureLayer_patch
    {
        [ModifiesMember("AddCreature")]
        private void AddCreature_patch(CreatureModel model)
        {
            
            if (model == null)
            {
                return;
            }
            try
            {
                global::CreatureUnit component = global::ResourceCache.instance.LoadPrefab("Unit/CreatureBase").GetComponent<global::CreatureUnit>();
                component.transform.SetParent(get_transform(), false);
                component.model = model;
                model.SetUnit(component);
                if (model.metaInfo.animSrc != string.Empty)
                {
                    string[] array = model.metaInfo.animSrc.Split(new char[]
                    {
                '/'
                    });
                    if (array[0] == "Custom")
                    {
                        DirectoryInfo directoryInfo = null;
                        foreach (ModInfo mod in ((Add_On_patch)(object)Add_On.instance).ModList)
                        {
                            ModInfo_patch pmodinfo = (ModInfo_patch)(object)mod;
                            if (pmodinfo.modid == CreatureTypeInfo_patch.GetLcId(model.metaInfo).packageId)
                            {
                                if (Directory.Exists(pmodinfo.modpath.FullName + "/CreatureAnimation/" + array[1]))
                                {
                                    directoryInfo = new DirectoryInfo(pmodinfo.modpath.FullName + "/CreatureAnimation/" + array[1]);
                                    break;
                                }
                            }
                        }
                        if (directoryInfo != null)
                        {
                            List<Texture2D> list = new List<Texture2D>();
                            foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                            {
                                if (fileInfo.FullName.Contains(".png"))
                                {
                                    byte[] data = File.ReadAllBytes(fileInfo.FullName);
                                    Texture2D texture2D = new Texture2D(2, 2);
                                    texture2D.LoadImage(data);
                                    texture2D.name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                                    list.Add(texture2D);
                                }
                            }
                            //
                            string atlasText = File.ReadAllText(directoryInfo.FullName + "/atlas.txt");
                            Shader shader = null;
                            Spine.Unity.AtlasAsset atlasAsset = Spine.Unity.AtlasAsset.CreateRuntimeInstance(atlasText, list.ToArray(), shader, true);
                            GameObject gameObject;
                            if (File.Exists(directoryInfo.FullName + "/json.txt"))
                            {
                                gameObject = Spine.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(Spine.Unity.SkeletonDataAsset.CreateRuntimeInstance(File.ReadAllText(directoryInfo.FullName + "/json.txt"), atlasAsset, true, 0.01f)).gameObject;
                            }
                            else
                            {
                                gameObject = Spine.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(Spine.Unity.SkeletonDataAsset.CreateRuntimeInstance(File.ReadAllBytes(directoryInfo.FullName + "/skeleton.skel"), atlasAsset, true, 0.01f)).gameObject;
                            }
                            Type type = null;
                            type = ExtenionUtil.GetType(array[1]);
                            gameObject.AddComponent(type);
                            component.animTarget = gameObject.GetComponent<global::CreatureAnimScript>();
                            gameObject.transform.SetParent(component.transform, false);
                        }
                    }
                    else
                    {
                        GameObject gameObject2 = global::Prefab.LoadPrefab(model.metaInfo.animSrc);
                        component.animTarget = gameObject2.GetComponent<global::CreatureAnimScript>();
                        gameObject2.transform.SetParent(component.transform, false);
                    }
                }
                if (model.metaInfo.roomReturnSrc != string.Empty)
                {
                    component.returnObject = global::Prefab.LoadPrefab(model.metaInfo.roomReturnSrc);
                    component.returnObject.transform.SetParent(component.transform);
                    component.returnObject.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
                    component.returnObject.transform.localPosition = new Vector3(0f, -0.2f, 0f);
                    component.returnObject.SetActive(false);
                    component.returnSpriteRenderer.gameObject.SetActive(false);
                }
                else
                {
                    component.returnObject = component.returnSpriteRenderer.gameObject;
                    component.returnObject.SetActive(false);
                }
                GameObject gameObject3 = global::Prefab.LoadPrefab("IsolateRoom");
                gameObject3.transform.SetParent(get_transform(), false);
                global::IsolateRoom component2 = gameObject3.GetComponent<global::IsolateRoom>();
                int item = UnityEngine.Random.Range(1, 4);
                this.tempIntforSprite.Add(item);
                string name = this.directory + 1;
                component2.RoomSpriteRenderer.sprite = ResourceCache.instance.GetSprite(name);
                component2.SetCreature(component);
                component2.Init();
                gameObject3.transform.position = model.basePosition;
                component.room = component2;
                this.creatureList.Add(component);
                this.creatureDic.Add(model.instanceId, component);
            }
            catch (Exception ex)
            {
                ModDebug.Log("AddCreature error - " + ex.Message + Environment.NewLine + ex.StackTrace);
            }

            
        }




        [MemberAlias("get_transform", typeof(Component))]
        public Transform get_transform()
        {
            return null;
        }



        [MemberAlias("creatureList", typeof(CreatureLayer))]
        private List<global::CreatureUnit> creatureList;
        [MemberAlias("creatureDic", typeof(CreatureLayer))]
        private Dictionary<long, global::CreatureUnit> creatureDic;
        [MemberAlias("ordealCreatureList", typeof(CreatureLayer))]
        private List<global::CreatureUnit> ordealCreatureList;
        [MemberAlias("sefiraBossList", typeof(CreatureLayer))]
        private List<global::CreatureUnit> sefiraBossList;
        [MemberAlias("eventCreatureList", typeof(CreatureLayer))]
        private List<global::CreatureUnit> eventCreatureList;
        [MemberAlias("tempIntforSprite", typeof(CreatureLayer))]
        private List<int> tempIntforSprite = new List<int>();
        [MemberAlias("etcList", typeof(CreatureLayer))]
        private List<global::EtcUnit> etcList;
        [MemberAlias("etcDic", typeof(CreatureLayer))]
        private Dictionary<long, global::EtcUnit> etcDic;
        [MemberAlias("directory", typeof(CreatureLayer))]
        private string directory = "Sprites/IsolateRoom/isolate_";
        [MemberAlias("dark", typeof(CreatureLayer))]
        private string dark = "_dark";
        [MemberAlias("isolateRoomUI",typeof(CreatureLayer))]
        public global::CreatureLayer.IsolateRoomUI isolateRoomUI;
    }
    [ModifiesType("CreatureInfo.CreatureInfoEquipmentRoot")]
    public class CreatureInfoEquipmentRoot_patch
    {
        [ModifiesMember("OnClickWeapon")]
        public void OnClickWeapon()
        {
            global::CreatureEquipmentMakeInfo creatureEquipmentMakeInfo = get_MetaInfo().equipMakeInfos.Find((global::CreatureEquipmentMakeInfo x) => x.equipTypeInfo.type == global::EquipmentTypeInfo.EquipmentType.WEAPON);
            int costAfterUpgrade = creatureEquipmentMakeInfo.GetCostAfterUpgrade();
            if (creatureEquipmentMakeInfo.level > this.GetObserveLevel())
            {
                Debug.Log("cannot make 1");
                global::CreatureInfoWindow.CurrentWindow.audioClipPlayer.OnPlayInList(0);
                return;
            }
            if (costAfterUpgrade > get_ObserveInfo().cubeNum)
            {
                this.weaponSlot.MakeCount.text = global::LocalizeTextDataModel.instance.GetText("CreatureInfo_NoCost");
                global::CreatureInfoWindow.CurrentWindow.audioClipPlayer.OnPlayInList(0);
                return;
            }
            if (!((InventoryModel_patch)(object)InventoryModel.Instance).CheckEquipmentCount_Mod(EquipmentTypeInfo_patch.GetLcId(creatureEquipmentMakeInfo.equipTypeInfo)))
            {
                this.weaponSlot.MakeCount.text = global::LocalizeTextDataModel.instance.GetText("CreatureInfo_NoRemain");
                global::CreatureInfoWindow.CurrentWindow.audioClipPlayer.OnPlayInList(0);
                return;
            }
            global::CreatureInfoWindow.CurrentWindow.PurchaseAnim(costAfterUpgrade);
            global::EquipmentModel equipmentModel = ((InventoryModel_patch)(object)InventoryModel.Instance).CreateEquipment_Mod(EquipmentTypeInfo_patch.GetLcId(creatureEquipmentMakeInfo.equipTypeInfo));
            if (equipmentModel != null)
            {
                get_ObserveInfo().Transaction(-costAfterUpgrade);
            }
            global::CreatureInfoWindow.CurrentWindow.audioClipPlayer.OnPlayInList(2);
            this.Initialize();
        }
        [ModifiesMember("OnClickArmor")]
        public void OnClickArmor_patch()
        {
            CreatureEquipmentMakeInfo creatureEquipmentMakeInfo = get_MetaInfo().equipMakeInfos.Find((global::CreatureEquipmentMakeInfo x) => x.equipTypeInfo.type == global::EquipmentTypeInfo.EquipmentType.ARMOR);
            int costAfterUpgrade = creatureEquipmentMakeInfo.GetCostAfterUpgrade();
            if (creatureEquipmentMakeInfo.level > this.GetObserveLevel())
            {
                Debug.Log("cannot make 1");
                global::CreatureInfoWindow.CurrentWindow.audioClipPlayer.OnPlayInList(0);
                return;
            }
            if (costAfterUpgrade > get_ObserveInfo().cubeNum)
            {
                this.armorSlot.MakeCount.text = global::LocalizeTextDataModel.instance.GetText("CreatureInfo_NoCost");
                global::CreatureInfoWindow.CurrentWindow.audioClipPlayer.OnPlayInList(0);
                return;
            }
            if (!((InventoryModel_patch)(object)InventoryModel.Instance).CheckEquipmentCount_Mod(EquipmentTypeInfo_patch.GetLcId(creatureEquipmentMakeInfo.equipTypeInfo)))
            {
                this.armorSlot.MakeCount.text = global::LocalizeTextDataModel.instance.GetText("CreatureInfo_NoRemain");
                global::CreatureInfoWindow.CurrentWindow.audioClipPlayer.OnPlayInList(0);
                return;
            }
            global::CreatureInfoWindow.CurrentWindow.PurchaseAnim(costAfterUpgrade);
            global::EquipmentModel equipmentModel = ((InventoryModel_patch)(object)InventoryModel.Instance).CreateEquipment_Mod(EquipmentTypeInfo_patch.GetLcId(creatureEquipmentMakeInfo.equipTypeInfo));
            if (equipmentModel != null && get_CurrentModel() != null)
            {
                get_ObserveInfo().Transaction(-costAfterUpgrade);
            }
            global::CreatureInfoWindow.CurrentWindow.audioClipPlayer.OnPlayInList(2);
            this.Initialize();
        }

        [MemberAlias("Initialize", typeof(CreatureInfoEquipmentRoot))]
        public void Initialize()
        {
        }
        [MemberAlias("GetObserveLevel", typeof(CreatureInfoEquipmentRoot))]
        public int GetObserveLevel()
        {
            return 0;
        }

        [MemberAlias("get_CurrentModel", typeof(CreatureInfoController))]
        public global::CreatureModel get_CurrentModel()
        {
            return global::CreatureInfoWindow.CurrentWindow.CurrentModel;
        }
        [MemberAlias("get_MetaInfo",typeof(CreatureInfoController))]
        public global::CreatureTypeInfo get_MetaInfo()
        {
            return global::CreatureInfoWindow.CurrentWindow.MetaInfo;
        }
        [MemberAlias("get_ObserveInfo", typeof(CreatureInfoController))]
        public global::CreatureObserveInfoModel get_ObserveInfo()
        {
            return global::CreatureInfoWindow.CurrentWindow.ObserveInfo;
        }


        [MemberAlias("giftSlot", typeof(CreatureInfoEquipmentRoot))]
        public GiftSlot giftSlot;
        [MemberAlias("weaponSlot", typeof(CreatureInfoEquipmentRoot))]
        public WeaponSlot weaponSlot;
        [MemberAlias("armorSlot", typeof(CreatureInfoEquipmentRoot))]
        public ArmorSlot armorSlot;
    }
    [ModifiesType("ConsoleCommand")]
    public class ConsoleCommand_patch
    {
        [ModifiesMember("ChangeLanguageCommad")]
        public void ChangeLanguageCommad_patch(string ln)
        {
            string currentLanguage = GlobalGameManager.instance.GetCurrentLanguage();
            Debug.Log(ln);
            if (currentLanguage == ln)
            {
                return;
            }
            GlobalGameManager.instance.ForceTypeChange<GlobalGameManager_patch>().ChangeLanguage_new(ln);
            if (GlobalEtcDataModel.instance.trueEndingDone)
            {
                SceneManager.LoadSceneAsync("AlterTitleScene");
            }
            else
            {
                SceneManager.LoadSceneAsync("NewTitleScene");
            }
        }
        [NewMember]
        public static void RemoveGift_Mod(long id, LcId equipid)
        {
            global::AgentModel agent = global::AgentManager.instance.GetAgent(id);
            global::EGOgiftModel egogiftModel = null;
            foreach (global::EGOgiftModel egogiftModel2 in agent.GetAllGifts())
            {
                if (EquipmentTypeInfo_patch.GetLcId(egogiftModel2.metaInfo) == equipid)
                {
                    egogiftModel = egogiftModel2;
                    break;
                }
            }
            if (egogiftModel != null)
            {
                agent.ReleaseEGOgift(egogiftModel);
            }
        }
        [ModifiesMember("RemoveGift")]
        public void RemoveGift_patch(long id, int equipid)
        {
            RemoveGift_Mod(id, new LcId(equipid));
        }
    }
    [ModifiesType("CreatureBase")]
    public class CreatureBase_patch
    {
        [ModifiesMember("get_GetSaveSrc")]
        public string get_GetSaveSrc_patch()
        {
            return string.Concat(new object[]
                {
                Application.persistentDataPath,
                "/creatureData/",
                ((CreatureTypeList_patch)(object)CreatureTypeList.instance).GetModId(this.model.metaInfo)+this.model.metadataId,
                ".dat"
                });
        }
        [MemberAlias("model", typeof(CreatureBase))]
        public CreatureModel model;
    }
    [ModifiesType("CreatureModel")]
    public class CreatureModel_patch
    {
        [ModifiesMember("Suppressed")]
        public void Suppressed()
        {
            global::Notice.instance.Send(global::NoticeName.OnCreatureSuppressed, new object[] { this });
            foreach (UnitBuf buf in _bufList)
            {
                buf.ForceTypeChange<UnitBuf_patch>().OnSuppressed(this.ForceTypeChange<CreatureModel>());
            }
            this.set_state(global::CreatureState.SUPPRESSED); 
            this.script.OnSuppressed();
            this.commandQueue.Clear();
            ClearWorkerEncounting();
            if (this.roomNode != null)
            {
                try
                {
                    global::Sefira sefira = global::SefiraManager.instance.GetSefira(this.roomNode.GetAttachedPassage().GetSefiraName());
                    this.sefira = sefira;
                    this.sefiraNum = this.sefira.indexString;
                }
                catch (Exception)
                {
                }
            }
            if (this.sefira != null)
            {
                this.sefira.OnSuppressedCreature(this.ForceTypeChange<CreatureModel>());
            }
            this.SetFaction(global::FactionTypeList.StandardFaction.IdleCreature);
        }
        [ModifiesMember("GetSaveData")]
        public Dictionary<string, object> GetSaveData_patch()
        {

            return new Dictionary<string, object>
        {
            {
                "instanceId",
                this.instanceId
            },
            {
                "metadataId",
                this.metadataId
            },
            {
                "entryNodeId",
                this.entryNodeId
            },
            {
                "sefiraNum",
                this.sefiraNum
            },
            {
                "basePosition",
                new global::Vector2Serializer(this.basePosition)
                },
            {
                "modid",
                ((CreatureTypeList_patch)(object)CreatureTypeList.instance).GetModId(metaInfo)
                }
        };
        }



        [MemberAlias("_bufList", typeof(UnitModel))]
        public List<global::UnitBuf> _bufList;
        [MemberAlias("SetFaction", typeof(UnitModel))]
        public virtual void SetFaction(string factionCode)
        {
        }
        [MemberAlias("sefira", typeof(CreatureModel))]
        public global::Sefira sefira;
        [MemberAlias("roomNode", typeof(CreatureModel))]
        public global::MapNode roomNode;
        [MemberAlias("ClearWorkerEncounting", typeof(UnitModel))]
        public void ClearWorkerEncounting()
        {
        }
        [MemberAlias("commandQueue", typeof(CreatureModel))]
        public global::CreatureCommandQueue commandQueue;
        [MemberAlias("script", typeof(CreatureModel))]
        public global::CreatureBase script;
        [MemberAlias("set_state", typeof(CreatureModel))]
        public void set_state(global::CreatureState value)
        {
        }
        [MemberAlias("get_state", typeof(CreatureModel))]
        public global::CreatureState get_state()
        {
            return default(CreatureState);
        }
        [MemberAlias("metaInfo", typeof(CreatureModel))]
        public CreatureTypeInfo metaInfo;
        [MemberAlias("basePosition", typeof(CreatureModel))]
        public Vector2 basePosition;
        [MemberAlias("sefiraNum", typeof(CreatureModel))]
        public string sefiraNum;
        [MemberAlias("entryNodeId", typeof(CreatureModel))]
        public string entryNodeId;
        [MemberAlias("metadataId", typeof(CreatureModel))]
        public long metadataId;
        [MemberAlias("instanceId", typeof(UnitModel))]
        public long instanceId;

    }
    [ModifiesType("CreatureObserveInfoModel")]
    public class CreatureObserveInfoModel_patch
    {
        [NewMember]
        public void Init_Mod(LcIdLong lcid)
        {

            this.creatureTypeId = lcid.id;
            this.lcid = lcid;
            _metaInfo = CreatureTypeList.instance.ForceTypeChange<CreatureTypeList_patch>().GetData_Mod(lcid);
            InitData_Mod(lcid);
        }
        [NewMember]
        public void InitData_Mod(LcIdLong lcid)
        {
            try
            {
                this.InitObserveRegion(((CreatureTypeList_patch)(object)CreatureTypeList.instance).GetData_Mod(lcid).observeData);
            }
            catch (Exception arg)
            {
                ModDebug.Log(string.Format("{0}\n{1}", this.creatureTypeId, arg));
            }
        }

        [ModifiesMember("InitData")]
        public void InitData_patch()
        {
           
            try
            {
                if (CreatureTypeList.instance.GetData(this.creatureTypeId) == null)
                {
                    return;
                }
                this.InitObserveRegion(global::CreatureTypeList.instance.GetData(this.creatureTypeId).observeData);
            }
            catch (Exception arg)
            {
                Debug.LogError(string.Format("{0}\n{1}", this.creatureTypeId, arg));
            }
        }

        [ModifiesMember("IsMaxObserved")]
        public bool IsMaxObserved_patch()
        {
            if(this._metaInfo == null)
            {
                ModDebug.Log("IsMaxObserved - observeInfo NULL!");
                return false;
            }
            if (this._metaInfo.creatureWorkType == global::CreatureWorkType.KIT)
            {
                if (this._metaInfo.specialSkillTable == null)
                {
                    this._metaInfo.specialSkillTable = CreatureTypeList.instance.ForceTypeChange<CreatureTypeList_patch>().GetSkillTipData_Mod(CreatureTypeInfo_patch.GetLcId(this._metaInfo)).GetCopy();
                }
                int num = Mathf.Max(this._metaInfo.specialSkillTable.descList.Count, this._metaInfo.desc.Count);
                num = Mathf.Min(num, global::CreatureModel.careTakingRegion.Length);
                return this.GetObservationLevel() >= num;
            }
            return this.GetObservationLevel() >= 4;
        }




        [MemberAlias("GetObservationLevel", typeof(CreatureObserveInfoModel), AliasCallMode.NoChange)]
        public int GetObservationLevel()
        {
           
            return 0;
        }
        [MemberAlias("InitObserveRegion", typeof(CreatureObserveInfoModel), AliasCallMode.NoChange)]
        public void InitObserveRegion(List<ObserveInfoData> data)
        {
            this.observeRegions.Clear();
            foreach (ObserveInfoData info in data)
            {
                ObserveRegion observeRegion = new ObserveRegion
                {
                    info = info,
                    isObserved = false
                };
                this.observeRegions.Add(observeRegion.info.regionName, observeRegion);
            }
        }

        [NewMember]
        public LcIdLong lcid;

        [MemberAlias("observeRegions", typeof(CreatureObserveInfoModel), AliasCallMode.NoChange)]
        private Dictionary<string, ObserveRegion> observeRegions;
        [MemberAlias("_metaInfo", typeof(CreatureObserveInfoModel), AliasCallMode.NoChange)]
        private CreatureTypeInfo _metaInfo;
        [MemberAlias("creatureTypeId", typeof(CreatureObserveInfoModel), AliasCallMode.NoChange)]
        public long creatureTypeId;


    }
    [ModifiesType("CreatureManager")]
    public class CreatureManager_patch
    {
        [ModifiesMember("ResetSpecialSkillTable")]
        public void ResetSpecialSkillTable_patch()
        {
            foreach (global::CreatureModel creatureModel in this.creatureList)
            {
                LcIdLong lcid = new LcIdLong(CreatureTypeList_patch.instance.GetModId(creatureModel.metaInfo), creatureModel.metadataId);
                creatureModel.metaInfo.specialSkillTable = CreatureTypeList_patch.instance.GetSkillTipData_Mod(lcid).GetCopy();
                creatureModel.observeInfo.observeProgress = 0;
                creatureModel.metaInfo.specialSkillTable.Init();
            }
        }
        [NewMember]
        public CreatureModel ReplaceCreature_Mod(LcIdLong metadataId, global::CreatureModel exist)
        {
            long instanceId;
            this.nextInstId = (instanceId = this.nextInstId) + 1L;
            CreatureModel creatureModel = new CreatureModel(instanceId);
            this.ReplaceBuildCreatureModel_Mod(creatureModel, metadataId, exist);
            this.ReplaceCommand(exist, creatureModel);
            this.UnRegisterCreature(exist);
            this.RegisterByReplace(creatureModel);
            creatureModel.script.OnInit();
            return creatureModel;
        }
        [ModifiesMember("ReplaceCreature")]
        public CreatureModel ReplaceCreature_patch(long metadataId, global::CreatureModel exist)
        {
            return ReplaceCreature_Mod(new LcIdLong(metadataId), exist);
        }
        [NewMember]
        public void ReplaceBuildCreatureModel_Mod(CreatureModel model, LcIdLong metadataId, CreatureModel old)
        {
            if (metadataId.packageId == String.Empty)
            {
                if (this.observeInfoList.ContainsKey(metadataId.id))
                {
                    this.observeInfoList.TryGetValue(metadataId.id, out model.observeInfo);
                }
                else
                {
                    model.observeInfo = new global::CreatureObserveInfoModel(metadataId.id);
                    ((CreatureObserveInfoModel_patch)(object)model.observeInfo).Init_Mod(metadataId);
                    this.observeInfoList.Add(metadataId.id, model.observeInfo);
                }
            }
            else
            {
                if (this.observeInfoList_mod.ContainsKey(metadataId))
                {
                    this.observeInfoList_mod.TryGetValue(metadataId, out model.observeInfo);
                }
                else
                {
                    model.observeInfo = new global::CreatureObserveInfoModel(metadataId.id);
                    ((CreatureObserveInfoModel_patch)(object)model.observeInfo).Init_Mod(metadataId);
                    this.observeInfoList_mod.Add(metadataId, model.observeInfo);
                }
            }
            model.sefiraOrigin = old.sefiraOrigin;
            model.sefira = old.sefira;
            model.sefiraNum = old.sefiraNum;
            global::SefiraIsolate isolateRoomData = old.isolateRoomData;
            model.specialSkillPos = isolateRoomData.pos;
            model.isolateRoomData = isolateRoomData;
            global::CreatureTypeInfo data = CreatureTypeList_patch.instance.GetData_Mod(metadataId);
            model.metadataId = metadataId.id;
            model.metaInfo = data;
            if (CreatureTypeList_patch.instance.GetSkillTipData_Mod(metadataId) != null)
            {
                model.metaInfo.specialSkillTable = CreatureTypeList_patch.instance.GetSkillTipData_Mod(metadataId).GetCopy();
            }
            model.basePosition = new Vector2(isolateRoomData.x, isolateRoomData.y);
            model.script = ExtenionUtil.GetTypeInstance<CreatureBase>(data.script);
            model.script.SetModel(model);
            model.entryNodeId = isolateRoomData.nodeId;
            global::MapNode nodeById = global::MapGraph.instance.GetNodeById(isolateRoomData.nodeId);
            model.entryNode = nodeById;
            nodeById.connectedCreature = model;
            old.CopyNodeData(model);
            model.script.OnInitialBuild();
        }
        [ModifiesMember("ReplaceBuildCreatureModel")]
        private void ReplaceBuildCreatureModel_patch(CreatureModel model, long metadataId, CreatureModel old)
        {
            ReplaceBuildCreatureModel_Mod(model, new LcIdLong(metadataId), old);
        }
        [ModifiesMember("ReplaceAllDlcCreature")]
        public bool ReplaceAllDlcCreature_patch()
        {
            List<long> list = new List<long>(global::CreatureGenerateInfo.GetAll(false));
            List<long> list2 = new List<long>();
            List<CreatureModel> list3 = new List<CreatureModel>();
            CreatureModel[] array = this.GetCreatureList();
            foreach (global::CreatureModel creatureModel in array)
            {
                if (CreatureTypeList_patch.instance.GetModId(creatureModel.metaInfo) == String.Empty)
                {
                    list.Remove(creatureModel.metadataId);
                    if (creatureModel.metadataId == 100015L)
                    {
                        list.Remove(100014L);
                    }
                    foreach (long num in global::CreatureGenerateInfo.creditCreatures)
                    {
                        if (num == creatureModel.metadataId)
                        {
                            list3.Add(creatureModel);
                        }
                    }
                }
            }
            List<long>[] array3 = new List<long>[5];
            for (int k = 0; k < 5; k++)
            {
                array3[k] = new List<long>();
            }
            foreach (long num2 in list)
            {
                global::CreatureTypeInfo data = global::CreatureTypeList.instance.GetData(num2);
                if (data != null)
                {
                    int num3 = (int)data.GetRiskLevel();
                    if (num2 == 100064L)
                    {
                        num3 = 4;
                    }
                    array3[num3].Add(num2);
                }
            }
            bool result = false;
            foreach (global::CreatureModel creatureModel2 in list3)
            {
                global::CreatureTypeInfo metaInfo = creatureModel2.metaInfo;
                if (metaInfo != null)
                {
                    int num4 = (int)metaInfo.GetRiskLevel();
                    if (metaInfo.id == 100064L)
                    {
                        num4 = 4;
                    }
                    if (array3[num4].Count > 0)
                    {
                        int index = UnityEngine.Random.Range(0, array3[num4].Count);
                        long num5 = array3[num4][index];
                        this.ReplaceCreature_Mod(new LcIdLong(num5), creatureModel2);
                        array3[num4].Remove(num5);
                        list.Remove(num5);
                        result = true;
                    }
                    else
                    {
                        int index2 = UnityEngine.Random.Range(0, list.Count);
                        long num6 = list[index2];
                        this.ReplaceCreature_Mod(new LcIdLong(num6), creatureModel2);
                        list.Remove(num6);
                        foreach (List<long> list4 in array3)
                        {
                            list4.Remove(num6);
                        }
                        result = true;
                    }
                }
            }
            return result;
        }

        [ModifiesMember("LoadObserveData")]
        public void LoadObserveData_patch(Dictionary<string, object> dic)
        {
            this.observeInfoList = new Dictionary<long, global::CreatureObserveInfoModel>();
            this.observeInfoList_mod = new Dictionary<LcIdLong, CreatureObserveInfoModel>();
            Dictionary<long, Dictionary<string, object>> dictionary = new Dictionary<long, Dictionary<string, object>>();
            Dictionary<string, Dictionary<long, Dictionary<string, object>>> moddic = new Dictionary<string, Dictionary<long, Dictionary<string, object>>>();

            GameUtil.TryGetValue<Dictionary<string, Dictionary<long, Dictionary<string, object>>>>(dic, "observeListMod", ref moddic);
            foreach (KeyValuePair<string, Dictionary<long, Dictionary<string, object>>> pair1 in moddic)
            {
                foreach (KeyValuePair<long, Dictionary<string, object>> pair2 in pair1.Value)
                    try
                    {
                        LcIdLong lcid = new LcIdLong(pair1.Key, pair2.Key);
                        if (CreatureTypeList_patch.instance.GetData_Mod(lcid) != null)
                        {
                            CreatureObserveInfoModel creatureObserveInfoModel = new CreatureObserveInfoModel(pair2.Key);
                            ((CreatureObserveInfoModel_patch)(object)creatureObserveInfoModel).Init_Mod(lcid);
                            creatureObserveInfoModel.LoadGlobalData(pair2.Value);
                            this.observeInfoList_mod.Add(lcid, creatureObserveInfoModel);
                            ModDebug.Log("Add ObserveData - " + lcid);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModDebug.Log("CM.LODerror_Mod - " + ex.Message + Environment.NewLine + ex.StackTrace);
                    }
            }

            GameUtil.TryGetValue<Dictionary<long, Dictionary<string, object>>>(dic, "observeList", ref dictionary);
            foreach (KeyValuePair<long, Dictionary<string, object>> keyValuePair in dictionary)
            {
                try
                {
                    if (CreatureTypeList.instance.GetData(keyValuePair.Key) != null)
                    {
                        CreatureObserveInfoModel creatureObserveInfoModel = new CreatureObserveInfoModel(keyValuePair.Key);
                        ((CreatureObserveInfoModel_patch)(object)creatureObserveInfoModel).Init_Mod(new LcIdLong(keyValuePair.Key));
                        creatureObserveInfoModel.LoadGlobalData(keyValuePair.Value);
                        this.observeInfoList.Add(keyValuePair.Key, creatureObserveInfoModel);
                        ModDebug.Log("Add ObserveData - " + keyValuePair.Key);
                    } else
                    {
                        foreach(ModInfo info in ((Add_On_patch)(object)Add_On.instance).ModList)
                        {
                            ModInfo_patch pinfo = info.ForceTypeChange<ModInfo_patch>();
                            if(pinfo.modid != String.Empty)
                            {
                                LcIdLong lcid = new LcIdLong(pinfo.modid, keyValuePair.Key);
                                if (observeInfoList_mod.ContainsKey(lcid)) continue;
                                if (CreatureTypeList_patch.instance.GetData_Mod(lcid) != null)
                                {
                                    CreatureObserveInfoModel creatureObserveInfoModel = new CreatureObserveInfoModel(keyValuePair.Key);
                                    ((CreatureObserveInfoModel_patch)(object)creatureObserveInfoModel).Init_Mod(lcid);
                                    creatureObserveInfoModel.LoadGlobalData(keyValuePair.Value);
                                    this.observeInfoList_mod.Add(lcid, creatureObserveInfoModel);
                                    ModDebug.Log("Add ObserveData - " + lcid);
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModDebug.Log("CM.LODerror - " + ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
           
        }
        [ModifiesMember("LoadData")]
        public void LoadData_patch(Dictionary<string, object> dic)
        {
            if (!GameUtil.TryGetValue<long>(dic, "nextInstId", ref this.nextInstId))
            {
                int num = 0;
                if (GameUtil.TryGetValue<int>(dic, "nextInstId", ref num))
                {
                    this.nextInstId = (long)num;
                }
            }
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            GameUtil.TryGetValue<List<Dictionary<string, object>>>(dic, "creatureList", ref list);
            List<long> list2 = new List<long>();
            foreach (Dictionary<string, object> dic2 in list)
            {
                long item = 0L;
                ExtenionUtil.TryGetValue<long>(dic2, "metadataId", ref item);
                list2.Add(item);
            }
            foreach (Dictionary<string, object> dic3 in list)
            {
                long id = 0L;
                string modid = string.Empty;
                GameUtil.TryGetValue<long>(dic3, "metadataId", ref id);
                GameUtil.TryGetValue<string>(dic3, "modid", ref modid);
                LcIdLong lcid = new LcIdLong(modid, id);
                if (CreatureTypeList_patch.instance.GetData_Mod(lcid) != null)
                {
                    long num2 = 0L;
                    GameUtil.TryGetValue<long>(dic3, "instanceId", ref num2);
                    CreatureModel creatureModel = new CreatureModel(num2);
                    creatureModel.LoadData(dic3);
                    if (modid == string.Empty)
                    {
                        if (creatureModel.metadataId == 300109L && !list2.Contains(100104L))
                        {
                            new List<long>(global::CreatureGenerateInfo.GetAll(false));
                            creatureModel.metadataId = 100104L;
                            list2.Add(100104L);
                        }
                    }
                    global::Sefira sefira = global::SefiraManager.instance.GetSefira(creatureModel.sefiraNum);
                    try
                    {
                        creatureModel.isolateRoomData = sefira.isolateManagement.GenIsolateByCreatureByNodeId(creatureModel.metadataId, creatureModel.entryNodeId);
                    }
                    catch (global::SefiraIsolateException ex)
                    {
                        Debug.LogError("Emergency Load " + ex.nodeId);
                        string nodeId = ex.nodeId;
                        Sefira sefira2 = null;
                        if (!SefiraManager.instance.GetSefiraByGenNodeId(nodeId, out sefira2))
                        {
                            Debug.LogError(string.Concat(new object[]
                            {
                        "Failed to gen sefira creature + ",
                        nodeId,
                        "   Creature ID : ",
                        num2
                            }));
                            continue;
                        }
                        creatureModel.isolateRoomData = sefira2.isolateManagement.GenIsolateByCreatureByNodeId(creatureModel.metadataId, creatureModel.entryNodeId);
                    }
                    this.BuildCreatureModel_Mod(creatureModel, lcid, creatureModel.isolateRoomData, creatureModel.sefiraNum);
                    this.RegisterCreature(creatureModel);
                    this.AddCreatureInSefira(creatureModel, creatureModel.sefiraNum);
                    creatureModel.script.OnInit();
                } else if (modid == String.Empty)
                {
                    foreach (ModInfo info in ((Add_On_patch)(object)Add_On.instance).ModList)
                    {
                        ModInfo_patch pinfo = info.ForceTypeChange<ModInfo_patch>();
                        if (pinfo.modid != String.Empty)
                        {
                            lcid = new LcIdLong(pinfo.modid, id);
                            if (CreatureTypeList_patch.instance.GetData_Mod(lcid) != null)
                            {
                                long num2 = 0L;
                                GameUtil.TryGetValue<long>(dic3, "instanceId", ref num2);
                                CreatureModel creatureModel = new CreatureModel(num2);
                                creatureModel.LoadData(dic3);
                                if (modid == string.Empty)
                                {
                                    if (creatureModel.metadataId == 300109L && !list2.Contains(100104L))
                                    {
                                        new List<long>(global::CreatureGenerateInfo.GetAll(false));
                                        creatureModel.metadataId = 100104L;
                                        list2.Add(100104L);
                                    }
                                }
                                global::Sefira sefira = global::SefiraManager.instance.GetSefira(creatureModel.sefiraNum);
                                try
                                {
                                    creatureModel.isolateRoomData = sefira.isolateManagement.GenIsolateByCreatureByNodeId(creatureModel.metadataId, creatureModel.entryNodeId);
                                }
                                catch (global::SefiraIsolateException ex)
                                {
                                    Debug.LogError("Emergency Load " + ex.nodeId);
                                    string nodeId = ex.nodeId;
                                    Sefira sefira2 = null;
                                    if (!SefiraManager.instance.GetSefiraByGenNodeId(nodeId, out sefira2))
                                    {
                                        Debug.LogError(string.Concat(new object[]
                                        {
                        "Failed to gen sefira creature + ",
                        nodeId,
                        "   Creature ID : ",
                        num2
                                        }));
                                        break;
                                    }
                                    creatureModel.isolateRoomData = sefira2.isolateManagement.GenIsolateByCreatureByNodeId(creatureModel.metadataId, creatureModel.entryNodeId);
                                }
                                this.BuildCreatureModel_Mod(creatureModel, lcid, creatureModel.isolateRoomData, creatureModel.sefiraNum);
                                this.RegisterCreature(creatureModel);
                                this.AddCreatureInSefira(creatureModel, creatureModel.sefiraNum);
                                creatureModel.script.OnInit();
                                break;
                            }
                        }
                    }
                }
            }
        }
        [NewMember]
        public bool IsCreatureActivated_Mod(LcIdLong metaId)
        {
            bool result = false;
            foreach (global::CreatureModel creatureModel in this.creatureList)
            {
                if (creatureModel.metadataId == metaId.id && (CreatureTypeList_patch.instance.GetModId(creatureModel.metaInfo) == metaId.packageId))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        [ModifiesMember("IsCreatureActivated")]
        public bool IsCreatureActivated_patch(long metaId)
        {
            return IsCreatureActivated_Mod(new LcIdLong(metaId));
        }

        [ModifiesMember("GetSaveObserveData")]
        public Dictionary<string, object> GetSaveObserveData_patch()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            Dictionary<long, Dictionary<string, object>> dictionary2 = new Dictionary<long, Dictionary<string, object>>();
            Dictionary<string, Dictionary<long, Dictionary<string, object>>> moddic = new Dictionary<string, Dictionary<long, Dictionary<string, object>>>();
            foreach (KeyValuePair<long, CreatureObserveInfoModel> keyValuePair in this.observeInfoList)
            {
                dictionary2.Add(keyValuePair.Key, keyValuePair.Value.GetSaveGlobalData());
            }
            foreach (KeyValuePair<LcIdLong, CreatureObserveInfoModel> pair in this.observeInfoList_mod)
            {
                if (!moddic.ContainsKey(pair.Key.packageId))
                {
                    moddic[pair.Key.packageId] = new Dictionary<long, Dictionary<string, object>>();
                }
                moddic[pair.Key.packageId][pair.Key.id] = pair.Value.GetSaveGlobalData();
            }
            dictionary.Add("observeList", dictionary2);
            dictionary.Add("observeListMod", moddic);
            return dictionary;
        }
        [NewMember]
        public int GetObserveLevel_Mod(LcIdLong metadataId)
        {
            CreatureObserveInfoModel creatureObserveInfoModel = null;
            if (metadataId.IsBasic())
            {
                if (this.observeInfoList.TryGetValue(metadataId.id, out creatureObserveInfoModel))
                {
                    return creatureObserveInfoModel.GetObservationLevel();
                }
            }
            if (this.observeInfoList_mod.TryGetValue(metadataId, out creatureObserveInfoModel))
            {
                return creatureObserveInfoModel.GetObservationLevel();
            }
            return 0;
        }
        [ModifiesMember("GetObserveLevel")]
        public int GetObserveLevel(long metadataId)
        {
            return GetObserveLevel_Mod(new LcIdLong(metadataId));
        }
        [ModifiesMember("GetObserveInfoList")]
        public List<CreatureObserveInfoModel> GetObserveInfoList()
        {
            List<CreatureObserveInfoModel> list = new List<CreatureObserveInfoModel>();
            foreach (CreatureObserveInfoModel item in this.observeInfoList.Values)
            {
                list.Add(item);
            }
            foreach (KeyValuePair<LcIdLong, CreatureObserveInfoModel> item in this.observeInfoList_mod)
            {
                list.Add(item.Value);
            }
            return list;
        }
        [NewMember]
        public CreatureObserveInfoModel GetObserveInfo_Mod(LcIdLong metadataId)
        {
            global::CreatureObserveInfoModel result = null;
            if (metadataId.IsBasic())
            {
                if (this.observeInfoList.TryGetValue(metadataId.id, out result))
                {
                    return result;
                }
            }
            if (this.observeInfoList_mod.TryGetValue(metadataId, out result))
            {
                return result;
            }

            return null;
        }
        [ModifiesMember("GetObserveInfo")]
        public CreatureObserveInfoModel GetObserveInfo(long metadataId)
        {
            return GetObserveInfo_Mod(new LcIdLong(metadataId));
        }
        [NewMember]
        public CreatureModel GetCreature_Mod(LcIdLong id)
        {
            CreatureModel result = null;
            foreach (CreatureModel creatureModel in this.creatureList)
            {
                LcIdLong cid = new LcIdLong(((CreatureTypeList_patch)(object)CreatureTypeList.instance).GetModId(creatureModel.metaInfo), creatureModel.metadataId);
                if (cid == id)
                {
                    result = creatureModel;
                    break;
                }
            }
            return result;
        }
        [ModifiesMember("GetCreature")]
        public CreatureModel GetCreature(long id)
        {
            return GetCreature_Mod(new LcIdLong(id));
        }
        [NewMember]
        public CreatureModel FindCreature_Mod(LcIdLong metaId)
        {
            CreatureModel result = null;
            foreach (CreatureModel creatureModel in this.creatureList)
            {
                LcIdLong cid = new LcIdLong(((CreatureTypeList_patch)(object)CreatureTypeList.instance).GetModId(creatureModel.metaInfo), creatureModel.metadataId);
                if (cid == metaId)
                {
                    result = creatureModel;
                    break;
                }
            }
            return result;
        }
        [ModifiesMember("FindCreature")]
        public CreatureModel FindCreature(long metaId)
        {
            return FindCreature_Mod(new LcIdLong(metaId));
        }
        [ModifiesMember("AddChildObserveInfo")]
        public void AddChildObserveInfo(CreatureObserveInfoModel infoModel)
        {
            if (infoModel.creatureTypeId == 100038L && !this.observeInfoList.ContainsKey(100038L) && ((CreatureObserveInfoModel_patch)(object)infoModel).lcid.packageId == String.Empty)
            {
                this.observeInfoList.Add(infoModel.creatureTypeId, infoModel);
            }
        }
        [ModifiesMember("ResetObserveData")]
        public void ResetObserveData_patch()
        {
            observeInfoList_mod = new Dictionary<LcIdLong, CreatureObserveInfoModel>();
            this.observeInfoList = new Dictionary<long, CreatureObserveInfoModel>();
            foreach (CreatureModel creatureModel in this.creatureList)
            {
                string pid = ((CreatureTypeList_patch)(object)CreatureTypeList.instance).GetModId(creatureModel.metaInfo);
                ((CreatureObserveInfoModel_patch)(object)creatureModel.observeInfo).InitData_Mod(new LcIdLong(pid, creatureModel.metaInfo.id));
            }
        }
        [ModifiesMember("Init")]
        public void Init_patch()
        {
            observeInfoList_mod = new Dictionary<LcIdLong, CreatureObserveInfoModel>();
            this.InitValues();
        }

        [NewMember]
        public CreatureModel AddCreature_Mod(LcIdLong metadataId, SefiraIsolate roomData, string sefiraNum)
        {
            long instanceId;
            this.nextInstId = (instanceId = this.nextInstId) + 1L;
            CreatureModel creatureModel = new CreatureModel(instanceId);
            this.BuildCreatureModel_Mod(creatureModel, metadataId, roomData, sefiraNum);
            this.AddCreatureInSefira(creatureModel, sefiraNum);
            this.RegisterCreature(creatureModel);
            creatureModel.script.OnInit();
            creatureModel.script.OnInitialBuild();
            return creatureModel;
        }
        [ModifiesMember("AddCreature")]
        public CreatureModel AddCreature(long metadataId, SefiraIsolate roomData, string sefiraNum)
        {
            return AddCreature_Mod(new LcIdLong(metadataId), roomData, sefiraNum);
        }


        [NewMember]
        public void BuildCreatureModel_Mod(CreatureModel model, LcIdLong metadataId, SefiraIsolate roomData, string sefiraNum)
        {
            CreatureTypeInfo data = ((CreatureTypeList_patch)(object)CreatureTypeList.instance).GetData_Mod(metadataId);
            if (data == null)
            {
                return;
            }
            object obj = null;
            foreach (Assembly assembly in Add_On.instance.AssemList)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == data.script)
                    {
                        obj = Activator.CreateInstance(type);
                    }
                }
            }
            if (obj == null)
            {
                obj = Activator.CreateInstance(Type.GetType(data.script));
            }
            model.script = (CreatureBase)obj;
            if (metadataId.packageId == String.Empty)
            {
                if (this.observeInfoList.ContainsKey(metadataId.id))
                {
                    this.observeInfoList.TryGetValue(metadataId.id, out model.observeInfo);
                }
                else
                {
                    model.observeInfo = new global::CreatureObserveInfoModel(metadataId.id);
                    ((CreatureObserveInfoModel_patch)(object)model.observeInfo).Init_Mod(metadataId);
                    this.observeInfoList.Add(metadataId.id, model.observeInfo);
                }
            }
            else
            {
                if (this.observeInfoList_mod.ContainsKey(metadataId))
                {
                    this.observeInfoList_mod.TryGetValue(metadataId, out model.observeInfo);
                }
                else
                {
                    model.observeInfo = new global::CreatureObserveInfoModel(metadataId.id);
                    ((CreatureObserveInfoModel_patch)(object)model.observeInfo).Init_Mod(metadataId);
                    this.observeInfoList_mod.Add(metadataId, model.observeInfo);
                }
            }

            model.sefira = (model.sefiraOrigin = SefiraManager.instance.GetSefira(sefiraNum));
            model.sefiraNum = sefiraNum;
            model.specialSkillPos = roomData.pos;
            model.isolateRoomData = roomData;
            model.metadataId = metadataId.id;
            model.metaInfo = data;
            if (((CreatureTypeList_patch)(object)CreatureTypeList.instance).GetSkillTipData_Mod(metadataId) != null)
            {
                model.metaInfo.specialSkillTable = ((CreatureTypeList_patch)(object)CreatureTypeList.instance).GetSkillTipData_Mod(metadataId).GetCopy();
            }
            model.basePosition = new Vector2(roomData.x, roomData.y);
            model.script.SetModel(model);
            model.entryNodeId = roomData.nodeId;
            MapNode nodeById = MapGraph.instance.GetNodeById(roomData.nodeId);
            model.entryNode = nodeById;
            nodeById.connectedCreature = model;
            Dictionary<string, MapNode> dictionary = new Dictionary<string, MapNode>();
            List<MapEdge> list = new List<MapEdge>();
            MapNode mapNode = null;
            PassageObjectModel passageObjectModel = new PassageObjectModel(roomData.nodeId + "@creature", nodeById.GetAreaName(), "Map/Passage/PassageEmpty");
            passageObjectModel.isDynamic = true;
            passageObjectModel.Activate();
            passageObjectModel.scaleFactor = 0.75f;
            passageObjectModel.SetToIsolate();
            passageObjectModel.position = new Vector3(roomData.x, roomData.y, 0f);
            passageObjectModel.type = PassageType.ISOLATEROOM;
            IEnumerator enumerator2 = data.nodeInfo.GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    object obj2 = enumerator2.Current;
                    XmlNode xmlNode = (XmlNode)obj2;
                    string text = roomData.nodeId + "@" + xmlNode.Attributes.GetNamedItem("id").InnerText;
                    float x = model.basePosition.x + float.Parse(xmlNode.Attributes.GetNamedItem("x").InnerText);
                    float y = model.basePosition.y + float.Parse(xmlNode.Attributes.GetNamedItem("y").InnerText);
                    XmlNode namedItem = xmlNode.Attributes.GetNamedItem("type");
                    MapNode mapNode2;
                    if (namedItem != null && namedItem.InnerText == "workspace")
                    {
                        mapNode2 = new MapNode(text, new Vector2(x, y), nodeById.GetAreaName(), passageObjectModel);
                        passageObjectModel.AddNode(mapNode2);
                        model.SetWorkspaceNode(mapNode2);
                    }
                    else if (namedItem != null && namedItem.InnerText == "custom")
                    {
                        mapNode2 = new MapNode(text, new Vector2(x, y), nodeById.GetAreaName(), passageObjectModel);
                        passageObjectModel.AddNode(mapNode2);
                        model.SetCustomNode(mapNode2);
                    }
                    else if (namedItem != null && namedItem.InnerText == "creature")
                    {
                        mapNode2 = new MapNode(text, new Vector2(x, y), nodeById.GetAreaName(), passageObjectModel);
                        passageObjectModel.AddNode(mapNode2);
                        model.SetRoomNode(mapNode2);
                        model.SetCurrentNode(mapNode2);
                    }
                    else
                    {
                        if (namedItem == null || !(namedItem.InnerText == "innerDoor"))
                        {
                            continue;
                        }
                        mapNode = (mapNode2 = new MapNode(text, new Vector2(x, y), nodeById.GetAreaName(), passageObjectModel));
                        passageObjectModel.AddNode(mapNode2);
                        DoorObjectModel doorObjectModel = new DoorObjectModel(string.Concat(new object[]
                        {
                        nodeById,
                        "@",
                        text,
                        "@inner"
                        }), "DoorIsolate", passageObjectModel, mapNode);
                        doorObjectModel.position = new Vector3(mapNode.GetPosition().x, mapNode.GetPosition().y, -0.01f);
                        passageObjectModel.AddDoor(doorObjectModel);
                        mapNode.SetDoor(doorObjectModel);
                        doorObjectModel.Close();
                    }
                    dictionary.Add(text, mapNode2);
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = (enumerator2 as IDisposable)) != null)
                {
                    disposable.Dispose();
                }
            }
            PassageObjectModel attachedPassage = nodeById.GetAttachedPassage();
            MapNode mapNode3 = new MapNode(roomData.nodeId + "@outter", new Vector2(nodeById.GetPosition().x, nodeById.GetPosition().y), nodeById.GetAreaName(), attachedPassage);
            string id = roomData.nodeId + "@outterDoor";
            string type2 = "MalkuthDoorMiddle";
            switch (model.sefira.sefiraEnum)
            {
                case SefiraEnum.MALKUT:
                    type2 = "MalkuthDoorMiddle";
                    break;
                case SefiraEnum.YESOD:
                    type2 = "YesodDoorMiddle";
                    break;
                case SefiraEnum.HOD:
                    type2 = "HodDoorMiddle";
                    break;
                case SefiraEnum.NETZACH:
                    type2 = "NetzachDoorMiddle";
                    break;
                case SefiraEnum.TIPERERTH1:
                case SefiraEnum.TIPERERTH2:
                    type2 = "TipherethDoorMiddle";
                    break;
                case SefiraEnum.GEBURAH:
                    type2 = "GeburahDoorMiddle";
                    break;
                case SefiraEnum.CHESED:
                    type2 = "ChesedDoorMiddle";
                    break;
                case SefiraEnum.BINAH:
                    type2 = "BinahDoorMiddle";
                    break;
                case SefiraEnum.CHOKHMAH:
                    type2 = "ChokhmahDoorMiddle";
                    break;
                case SefiraEnum.KETHER:
                    type2 = "KetherDoorMiddle";
                    break;
            }
            DoorObjectModel doorObjectModel2 = new DoorObjectModel(id, type2, attachedPassage, mapNode3);
            doorObjectModel2.position = new Vector3(mapNode3.GetPosition().x, mapNode3.GetPosition().y, -0.01f);
            attachedPassage.AddDoor(doorObjectModel2);
            mapNode3.SetDoor(doorObjectModel2);
            doorObjectModel2.Close();
            attachedPassage.AddNode(mapNode3);
            MapEdge mapEdge = new MapEdge(mapNode3, nodeById, "road");
            list.Add(mapEdge);
            mapNode3.AddEdge(mapEdge);
            nodeById.AddEdge(mapEdge);
            if (mapNode != null)
            {
                MapEdge mapEdge2 = new MapEdge(mapNode3, mapNode, "door", 0.01f);
                doorObjectModel2.Connect(mapNode.GetDoor());
                list.Add(mapEdge2);
                mapNode3.AddEdge(mapEdge2);
                mapNode.AddEdge(mapEdge2);
            }
            dictionary.Add(mapNode3.GetId(), mapNode3);
            if (model.GetCustomNode() == null)
            {
                model.SetCustomNode(model.GetCurrentNode());
            }
            IEnumerator enumerator3 = data.edgeInfo.GetEnumerator();
            try
            {
                while (enumerator3.MoveNext())
                {
                    object obj3 = enumerator3.Current;
                    XmlNode xmlNode2 = (XmlNode)obj3;
                    string text2 = roomData.nodeId + "@" + xmlNode2.Attributes.GetNamedItem("node1").InnerText;
                    string text3 = roomData.nodeId + "@" + xmlNode2.Attributes.GetNamedItem("node2").InnerText;
                    string innerText = xmlNode2.Attributes.GetNamedItem("type").InnerText;
                    MapNode mapNode4 = null;
                    MapNode mapNode5 = null;
                    if (!dictionary.TryGetValue(text2, out mapNode4) || !dictionary.TryGetValue(text3, out mapNode5))
                    {
                        Debug.Log(string.Concat(new string[]
                        {
                        "cannot create edge - (",
                        text2,
                        ", ",
                        text3,
                        ")"
                        }));
                    }
                    XmlNode namedItem2 = xmlNode2.Attributes.GetNamedItem("cost");
                    MapEdge mapEdge3;
                    if (namedItem2 != null)
                    {
                        mapEdge3 = new MapEdge(mapNode4, mapNode5, innerText, float.Parse(namedItem2.InnerText));
                    }
                    else
                    {
                        mapEdge3 = new MapEdge(mapNode4, mapNode5, innerText);
                    }
                    list.Add(mapEdge3);
                    mapNode4.AddEdge(mapEdge3);
                    mapNode5.AddEdge(mapEdge3);
                }
            }
            finally
            {
                IDisposable disposable2;
                if ((disposable2 = (enumerator3 as IDisposable)) != null)
                {
                    disposable2.Dispose();
                }
            }
            MapGraph.instance.RegisterPassage(passageObjectModel);
        }
        [ModifiesMember("BuildCreatureModel")]
        private void BuildCreatureModel(global::CreatureModel model, long metadataId, global::SefiraIsolate roomData, string sefiraNum)
        {
            BuildCreatureModel_Mod(model, new LcIdLong(metadataId), roomData, sefiraNum);
        }
        [MemberAlias("GetCreatureList", typeof(CreatureManager), AliasCallMode.NoChange)]
        public CreatureModel[] GetCreatureList()
        {
            return this.creatureList.ToArray();
        }
        [MemberAlias("RegisterCreature", typeof(CreatureManager), AliasCallMode.NoChange)]
        public void RegisterCreature(CreatureModel model)
        {
            model.GetMovableNode().SetActive(true);
            this.creatureList.Add(model);
            Notice.instance.Send(NoticeName.AddCreature, new object[]
            {
            model
            });
        }
        [MemberAlias("AddCreatureInSefira", typeof(CreatureManager), AliasCallMode.NoChange)]
        public void AddCreatureInSefira(CreatureModel creature, string sefira)
        {
            SefiraManager.instance.GetSefira(sefira).creatureList.Add(creature);
        }
        [MemberAlias("InitValues", typeof(CreatureManager), AliasCallMode.NoChange)]
        private void InitValues()
        {
        }
        [MemberAlias("ReplaceCommand", typeof(CreatureManager), AliasCallMode.NoChange)]
        public void ReplaceCommand(global::CreatureModel old, global::CreatureModel replaced)
        {
            int index = this.creatureList.IndexOf(old);
            int index2 = old.sefira.creatureList.IndexOf(old);
            this.creatureList[index] = replaced;
            old.sefira.creatureList[index2] = replaced;
        }
        [MemberAlias("UnRegisterCreature", typeof(CreatureManager), AliasCallMode.NoChange)]
        public void UnRegisterCreature(global::CreatureModel model)
        {
            model.GetMovableNode().SetActive(false);
            this.creatureList.Remove(model);
            global::Notice.instance.Send(global::NoticeName.RemoveCreature, new object[]
            {
        model
            });
        }
        [MemberAlias("RegisterByReplace", typeof(CreatureManager), AliasCallMode.NoChange)]
        public void RegisterByReplace(global::CreatureModel model)
        {
            model.GetMovableNode().SetActive(true);
            global::Notice.instance.Send(global::NoticeName.AddCreature, new object[]
            {
        model
            });
        }

        [NewMember]
        public Dictionary<LcIdLong, CreatureObserveInfoModel> observeInfoList_mod;
        [MemberAlias("creatureListNode", typeof(CreatureManager), AliasCallMode.NoChange)]
        public GameObject creatureListNode;
        [MemberAlias("creatureList", typeof(CreatureManager), AliasCallMode.NoChange)]
        private List<CreatureModel> creatureList;
        [MemberAlias("observeInfoList", typeof(CreatureManager), AliasCallMode.NoChange)]
        private Dictionary<long, CreatureObserveInfoModel> observeInfoList;
        [MemberAlias("specialSkillTable", typeof(CreatureManager), AliasCallMode.NoChange)]
        private Dictionary<long, CreatureSpecialSkillTipTable> specialSkillTable;
        [MemberAlias("nextInstId", typeof(CreatureManager), AliasCallMode.NoChange)]
        private long nextInstId;

        [MemberAlias("get_instance", typeof(CreatureManager))]
        public static CreatureManager_patch instance()
        {
            return null;
        }
    }

    [ModifiesType("CreatureTypeList")]
    public class CreatureTypeList_patch
    {
        [NewMember]
        public void Init_Mod(Dictionary<string, List<CreatureTypeInfo>> CTIdic, Dictionary<string, List<CreatureSpecialSkillTipTable>> CSSTTdic)
        {
            _modlist = CTIdic;
            _modtableList = CSSTTdic;
        }
        [NewMember]
        public CreatureTypeInfo GetData_Mod(LcIdLong lcid)
        {
            if (lcid.packageId == string.Empty)
            {
                return CreatureTypeList.instance.GetData(lcid.id);
            }
            if (_modlist.ContainsKey(lcid.packageId))
            {
                CreatureTypeInfo result = _modlist[lcid.packageId].Find(x => x.id == lcid.id);
                return result;
            }
            return null;
        }
        [NewMember]
        public CreatureSpecialSkillTipTable GetSkillTipData_Mod(LcIdLong lcid)
        {
            if (lcid.packageId == string.Empty)
            {
                return CreatureTypeList.instance.GetSkillTipData(lcid.id);
            }
            if (_modtableList.ContainsKey(lcid.packageId))
            {
                CreatureSpecialSkillTipTable result = _modtableList[lcid.packageId].Find(x => x.creatureTypeId == lcid.id);
                return result;
            }
            return null;
        }
        [NewMember]
        public LcIdLong GetLcId(CreatureTypeInfo info)
        {
            if (info is ChildCreatureTypeInfo)
            {
                foreach (KeyValuePair<string, List<CreatureTypeInfo>> pair in _modlist)
                {
                    CreatureTypeInfo result = pair.Value.Find(x => x.childTypeInfo == info);
                    if (result != null) return new LcIdLong(pair.Key, info.id);
                }
            }
            foreach (KeyValuePair<string, List<CreatureTypeInfo>> pair in _modlist)
            {
                if (pair.Value.Contains(info))
                {
                    return new LcIdLong(pair.Key, info.id);
                }
            }
            return new LcIdLong(info.id);
        }
        [NewMember]
        public LcIdLong GetLcId(CreatureSpecialSkillTipTable info)
        {
            return new LcIdLong(((CreatureSpecialSkillTipTable_patch)(object)info).modid, info.creatureTypeId);
        }
        [NewMember]
        public string GetModId(CreatureTypeInfo info)
        {
            if (info is ChildCreatureTypeInfo)
            {
                foreach (KeyValuePair<string, List<CreatureTypeInfo>> pair in _modlist)
                {
                    CreatureTypeInfo result = pair.Value.Find(x => x.childTypeInfo == info);
                    if (result != null) return pair.Key;
                }
            }
            foreach (KeyValuePair<string, List<CreatureTypeInfo>> pair in _modlist)
            {
                if (pair.Value.Contains(info))
                {
                    return pair.Key;
                }
            }
            return string.Empty;
        }
        [NewMember]
        public string GetModId(CreatureSpecialSkillTipTable info)
        {
            return ((CreatureSpecialSkillTipTable_patch)(object)info).modid;
            foreach (KeyValuePair<string, List<CreatureSpecialSkillTipTable>> pair in _modtableList)
            {
                if (pair.Value.Contains(info))
                {
                    return pair.Key;
                }
            }
            return string.Empty;
        }

        [NewMember]
        public Dictionary<string, List<CreatureTypeInfo>> _modlist;

        [NewMember]
        public Dictionary<string, List<CreatureSpecialSkillTipTable>> _modtableList;

        [MemberAlias("_instance", typeof(CreatureTypeList))]
        public static CreatureTypeList_patch instance;
    }

    [ModifiesType("CreatureTypeInfo")]
    public class CreatureTypeInfo_patch
    {
        [NewMember]
        public LcIdLong LcId
        {
            get
            {
                return GetLcId(this.ForceTypeChange<CreatureTypeInfo>());
            }
        }
        [NewMember]
        public static LcIdLong GetLcId(CreatureTypeInfo equip)
        {
            CreatureTypeInfo_patch pequip = (CreatureTypeInfo_patch)(object)equip;
            if (pequip.modid == null) return new LcIdLong(string.Empty, equip.id);
            return new LcIdLong(((CreatureTypeInfo_patch)(object)equip).modid, equip.id);
        }

        [ModifiesMember("get_CurrentObserveLevel")]
        public int get_CurrentObserveLevel()
        {
            return CreatureManager.instance.ForceTypeChange<CreatureManager_patch>().GetObserveLevel_Mod(GetLcId(this.ForceTypeChange<CreatureTypeInfo>()));
        }

        [NewMember]
        [NonSerialized]
        public string modid;
    }
    [ModifiesType("CreatureSpecialSkillTipTable")]
    public class CreatureSpecialSkillTipTable_patch
    {
        [ModifiesMember(".ctor")]
        public void Ctor(long creatureTypeId)
        {
            this.creatureTypeId = creatureTypeId;
            this.descList = new List<global::CreatureSpecialSkillDesc>();
            modid = string.Empty;
        }
        [NewMember]
        [NonSerialized]
        public string modid;
        [MemberAlias("descList", typeof(CreatureSpecialSkillTipTable))]
        public List<global::CreatureSpecialSkillDesc> descList;
        [MemberAlias("creatureTypeId", typeof(CreatureSpecialSkillTipTable))]
        public long creatureTypeId;
    }
    [ModifiesType]
    public class CreatureDataLoader_patch : CreatureDataLoader
    {
        [NewMember]
        private void LoadCreatureStat_Mod(XmlNode stat, XmlNode statCreature, global::CreatureTypeInfo model, string modid)
        {
            XmlNode xmlNode;
            if ((xmlNode = statCreature.SelectSingleNode("script")) != null)
            {
                model.script = xmlNode.InnerText;
            }
            XmlNode xmlNode2;
            if ((xmlNode2 = statCreature.SelectSingleNode("workAnim")) != null)
            {
                model.workAnim = xmlNode2.InnerText;
                XmlNode namedItem = xmlNode2.Attributes.GetNamedItem("face");
                if (namedItem != null)
                {
                    model.workAnimFace = namedItem.InnerText;
                }
            }
            XmlNode xmlNode3;
            if ((xmlNode3 = statCreature.SelectSingleNode("kitIcon")) != null)
            {
                model.kitIconSrc = xmlNode3.InnerText;
            }
            XmlNode xmlNode4;
            if ((xmlNode4 = stat.SelectSingleNode("workType")) != null)
            {
                string innerText = xmlNode4.InnerText;
                if (innerText != null)
                {
                    if (!(innerText == "normal"))
                    {
                        if (innerText == "kit")
                        {
                            model.creatureWorkType = global::CreatureWorkType.KIT;
                        }
                    }
                    else
                    {
                        model.creatureWorkType = global::CreatureWorkType.NORMAL;
                    }
                }
            }
            XmlNode xmlNode5;
            if ((xmlNode5 = stat.SelectSingleNode("kitType")) != null)
            {
                string innerText2 = xmlNode5.InnerText;
                if (innerText2 != null)
                {
                    if (!(innerText2 == "equip"))
                    {
                        if (!(innerText2 == "channel"))
                        {
                            if (innerText2 == "oneshot")
                            {
                                model.creatureKitType = global::CreatureKitType.ONESHOT;
                            }
                        }
                        else
                        {
                            model.creatureKitType = global::CreatureKitType.CHANNEL;
                        }
                    }
                    else
                    {
                        model.creatureKitType = global::CreatureKitType.EQUIP;
                    }
                }
            }
            XmlNode xmlNode6;
            if ((xmlNode6 = stat.SelectSingleNode("qliphoth")) != null)
            {
                model.qliphothMax = int.Parse(xmlNode6.InnerText);
            }
            XmlNode xmlNode7;
            if ((xmlNode7 = stat.SelectSingleNode("speed")) != null)
            {
                model.speed = float.Parse(xmlNode7.InnerText);
            }
            XmlNode xmlNode8 = stat.SelectSingleNode("escapeable");
            if (xmlNode8 != null)
            {
                bool booleanData = this.GetBooleanData(xmlNode8.InnerText.Trim());
                model.isEscapeAble = booleanData;
            }
            else
            {
                model.isEscapeAble = true;
            }
            XmlNode xmlNode9 = stat.SelectSingleNode("hp");
            if (xmlNode9 != null)
            {
                model.maxHp = (int)float.Parse(xmlNode9.InnerText);
            }
            else
            {
                model.maxHp = 5;
            }
            IEnumerator enumerator = stat.SelectNodes("workProb").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object obj = enumerator.Current;
                    XmlNode xmlNode10 = (XmlNode)obj;
                    global::RwbpType type = global::CreatureDataLoader.ConvertToRWBP(xmlNode10.Attributes.GetNamedItem("type").InnerText);
                    IEnumerator enumerator2 = xmlNode10.SelectNodes("prob").GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            object obj2 = enumerator2.Current;
                            XmlNode xmlNode11 = (XmlNode)obj2;
                            int level = int.Parse(xmlNode11.Attributes.GetNamedItem("level").InnerText);
                            float prob = float.Parse(xmlNode11.InnerText);
                            model.workProbTable.SetWorkProb(type, level, prob);
                        }
                    }
                    finally
                    {
                        IDisposable disposable;
                        if ((disposable = (enumerator2 as IDisposable)) != null)
                        {
                            disposable.Dispose();
                        }
                    }
                }
            }
            finally
            {
                IDisposable disposable2;
                if ((disposable2 = (enumerator as IDisposable)) != null)
                {
                    disposable2.Dispose();
                }
            }
            XmlNode xmlNode12 = stat.SelectSingleNode("workCooltime");
            if (xmlNode12 != null)
            {
                model.workCooltime = int.Parse(xmlNode12.InnerText);
            }
            XmlNode xmlNode13 = stat.SelectSingleNode("workSpeed");
            if (xmlNode13 != null)
            {
                model.cubeSpeed = float.Parse(xmlNode13.InnerText);
            }
            XmlNode xmlNode14 = statCreature.SelectSingleNode("skillTrigger");
            if (xmlNode14 != null)
            {
                this.LoadSkillTrigger(xmlNode14, model);
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            IEnumerator enumerator3 = statCreature.SelectNodes("sound").GetEnumerator();
            try
            {
                while (enumerator3.MoveNext())
                {
                    object obj3 = enumerator3.Current;
                    XmlNode xmlNode15 = (XmlNode)obj3;
                    string innerText3 = xmlNode15.Attributes.GetNamedItem("action").InnerText;
                    string innerText4 = xmlNode15.Attributes.GetNamedItem("src").InnerText;
                    dictionary.Add(innerText3, innerText4);
                }
            }
            finally
            {
                IDisposable disposable3;
                if ((disposable3 = (enumerator3 as IDisposable)) != null)
                {
                    disposable3.Dispose();
                }
            }
            model.soundTable = dictionary;
            model.nodeInfo = statCreature.SelectNodes("graph/node");
            model.edgeInfo = statCreature.SelectNodes("graph/edge");
            XmlNode xmlNode16 = statCreature.SelectSingleNode("anim");
            if (xmlNode16 != null)
            {
                model.animSrc = xmlNode16.Attributes.GetNamedItem("prefab").InnerText;
            }
            XmlNode xmlNode17 = statCreature.SelectSingleNode("returnImg");
            if (xmlNode17 != null)
            {
                model.roomReturnSrc = xmlNode17.Attributes.GetNamedItem("src").InnerText;
            }
            else
            {
                model.roomReturnSrc = string.Empty;
            }
            XmlNode xmlNode18 = stat.SelectSingleNode("feelingStateCubeBounds");
            if (xmlNode18 != null)
            {
                List<int> list = new List<int>();
                IEnumerator enumerator4 = xmlNode18.SelectNodes("cube").GetEnumerator();
                try
                {
                    while (enumerator4.MoveNext())
                    {
                        object obj4 = enumerator4.Current;
                        XmlNode xmlNode19 = (XmlNode)obj4;
                        list.Add(int.Parse(xmlNode19.InnerText));
                    }
                }
                finally
                {
                    IDisposable disposable4;
                    if ((disposable4 = (enumerator4 as IDisposable)) != null)
                    {
                        disposable4.Dispose();
                    }
                }
                model.feelingStateCubeBounds.upperBounds = list.ToArray();
            }
            XmlNode xmlNode20 = stat.SelectSingleNode("workDamage");
            if (xmlNode20 != null)
            {
                model.workDamage = global::CreatureDataLoader.ConvertToDamageInfo(xmlNode20);
            }
            XmlNode xmlNode21 = stat.SelectSingleNode("specialDamage");
            if (xmlNode21 != null)
            {
                Dictionary<string, global::EquipmentTypeInfo> dictionary2 = new Dictionary<string, global::EquipmentTypeInfo>();
                IEnumerator enumerator5 = xmlNode21.ChildNodes.GetEnumerator();
                try
                {
                    while (enumerator5.MoveNext())
                    {
                        object obj5 = enumerator5.Current;
                        XmlNode xmlNode22 = (XmlNode)obj5;
                        if (xmlNode22.Name == "damage")
                        {
                            string innerText5 = xmlNode22.Attributes.GetNamedItem("id").InnerText;
                            global::EquipmentTypeInfo value = global::EquipmentTypeInfo.MakeWeaponInfoByDamageInfo(global::CreatureDataLoader.ConvertToDamageInfo(xmlNode22));
                            dictionary2.Add(innerText5, value);
                        }
                        else if (xmlNode22.Name == "weapon")
                        {
                            string innerText6 = xmlNode22.Attributes.GetNamedItem("id").InnerText;
                            string innerText7 = xmlNode22.Attributes.GetNamedItem("weaponId").InnerText;
                            global::EquipmentTypeInfo data = global::EquipmentTypeList.instance.GetData(int.Parse(innerText7));
                            dictionary2.Add(innerText6, data);
                        }
                    }
                }
                finally
                {
                    IDisposable disposable5;
                    if ((disposable5 = (enumerator5 as IDisposable)) != null)
                    {
                        disposable5.Dispose();
                    }
                }
                model.creatureSpecialDamageTable.Init(dictionary2);
            }
            Dictionary<string, global::DefenseInfo> dictionary3 = new Dictionary<string, global::DefenseInfo>();
            IEnumerator enumerator6 = stat.SelectNodes("defense").GetEnumerator();
            try
            {
                while (enumerator6.MoveNext())
                {
                    object obj6 = enumerator6.Current;
                    XmlNode xmlNode23 = (XmlNode)obj6;
                    string innerText8 = xmlNode23.Attributes.GetNamedItem("id").InnerText;
                    global::DefenseInfo defenseInfo = new global::DefenseInfo();
                    IEnumerator enumerator7 = xmlNode23.SelectNodes("defenseElement").GetEnumerator();
                    try
                    {
                        while (enumerator7.MoveNext())
                        {
                            object obj7 = enumerator7.Current;
                            XmlNode xmlNode24 = (XmlNode)obj7;
                            string innerText9 = xmlNode24.Attributes.GetNamedItem("type").InnerText;
                            if (innerText9 != null)
                            {
                                if (!(innerText9 == "R"))
                                {
                                    if (!(innerText9 == "W"))
                                    {
                                        if (!(innerText9 == "B"))
                                        {
                                            if (innerText9 == "P")
                                            {
                                                defenseInfo.P = float.Parse(xmlNode24.InnerText);
                                            }
                                        }
                                        else
                                        {
                                            defenseInfo.B = float.Parse(xmlNode24.InnerText);
                                        }
                                    }
                                    else
                                    {
                                        defenseInfo.W = float.Parse(xmlNode24.InnerText);
                                    }
                                }
                                else
                                {
                                    defenseInfo.R = float.Parse(xmlNode24.InnerText);
                                }
                            }
                        }
                    }
                    finally
                    {
                        IDisposable disposable6;
                        if ((disposable6 = (enumerator7 as IDisposable)) != null)
                        {
                            disposable6.Dispose();
                        }
                    }
                    dictionary3.Add(innerText8, defenseInfo);
                }
            }
            finally
            {
                IDisposable disposable7;
                if ((disposable7 = (enumerator6 as IDisposable)) != null)
                {
                    disposable7.Dispose();
                }
            }
            model.defenseTable.Init(dictionary3);
            XmlNode xmlNode25 = stat.SelectSingleNode("observeInfo");
            if (xmlNode25 != null)
            {
                List<global::ObserveInfoData> list2 = new List<global::ObserveInfoData>();
                IEnumerator enumerator8 = xmlNode25.SelectNodes("observeElement").GetEnumerator();
                try
                {
                    while (enumerator8.MoveNext())
                    {
                        object obj8 = enumerator8.Current;
                        XmlNode xmlNode26 = (XmlNode)obj8;
                        string regionName = xmlNode26.Attributes.GetNamedItem("name").InnerText.Trim();
                        int observeCost = (int)float.Parse(xmlNode26.Attributes.GetNamedItem("cost").InnerText);
                        global::ObserveInfoData item = new global::ObserveInfoData
                        {
                            observeCost = observeCost,
                            regionName = regionName
                        };
                        list2.Add(item);
                    }
                }
                finally
                {
                    IDisposable disposable8;
                    if ((disposable8 = (enumerator8 as IDisposable)) != null)
                    {
                        disposable8.Dispose();
                    }
                }
                model.observeData = list2;
            }
            else
            {
                List<global::ObserveInfoData> list3 = new List<global::ObserveInfoData>();
                for (int i = 0; i < global::CreatureModel.regionName.Length; i++)
                {
                    global::ObserveInfoData item2 = new global::ObserveInfoData
                    {
                        observeCost = 0,
                        regionName = global::CreatureModel.regionName[i]
                    };
                    list3.Add(item2);
                }
                for (int j = 0; j < global::CreatureModel.careTakingRegion.Length; j++)
                {
                    global::ObserveInfoData item3 = new global::ObserveInfoData
                    {
                        observeCost = 0,
                        regionName = global::CreatureModel.careTakingRegion[j]
                    };
                    list3.Add(item3);
                }
                model.observeData = list3;
            }
            List<global::CreatureEquipmentMakeInfo> list4 = new List<global::CreatureEquipmentMakeInfo>();
            IEnumerator enumerator9 = stat.SelectNodes("equipment").GetEnumerator();
            try
            {
                while (enumerator9.MoveNext())
                {
                    object obj9 = enumerator9.Current;
                    XmlNode xmlNode27 = (XmlNode)obj9;
                    XmlNode namedItem2 = xmlNode27.Attributes.GetNamedItem("equipId");
                    XmlNode namedItem3 = xmlNode27.Attributes.GetNamedItem("level");
                    XmlNode namedItem4 = xmlNode27.Attributes.GetNamedItem("cost");
                    XmlNode namedItem5 = xmlNode27.Attributes.GetNamedItem("prob");
                    global::CreatureEquipmentMakeInfo creatureEquipmentMakeInfo = new global::CreatureEquipmentMakeInfo();
                    if (namedItem2 != null)
                    {
                        int id = int.Parse(namedItem2.InnerText);
                        LcId lcid = new LcId(modid, id);
                        creatureEquipmentMakeInfo.equipTypeInfo = EquipmentTypeList_patch.instance.GetData_Mod(lcid);
                        if (creatureEquipmentMakeInfo.equipTypeInfo == null)
                        {
                            continue;
                        }
                    }
                    if (namedItem3 != null)
                    {
                        creatureEquipmentMakeInfo.level = int.Parse(namedItem3.InnerText);
                    }
                    if (namedItem4 != null)
                    {
                        creatureEquipmentMakeInfo.cost = int.Parse(namedItem4.InnerText);
                    }
                    if (namedItem5 != null)
                    {
                        creatureEquipmentMakeInfo.prob = float.Parse(namedItem5.InnerText);
                    }
                    list4.Add(creatureEquipmentMakeInfo);
                }
            }
            finally
            {
                IDisposable disposable9;
                if ((disposable9 = (enumerator9 as IDisposable)) != null)
                {
                    disposable9.Dispose();
                }
            }
            model.equipMakeInfos = list4;
            List<global::CreatureObserveBonusData> list5 = new List<global::CreatureObserveBonusData>();
            IEnumerator enumerator10 = stat.SelectNodes("observeBonus").GetEnumerator();
            try
            {
                while (enumerator10.MoveNext())
                {
                    object obj10 = enumerator10.Current;
                    XmlNode xmlNode28 = (XmlNode)obj10;
                    int level2 = int.Parse(xmlNode28.Attributes.GetNamedItem("level").InnerText);
                    string innerText10 = xmlNode28.Attributes.GetNamedItem("type").InnerText;
                    global::CreatureObserveBonusData creatureObserveBonusData = new global::CreatureObserveBonusData();
                    if (innerText10 != null)
                    {
                        if (!(innerText10 == "prob"))
                        {
                            if (innerText10 == "speed")
                            {
                                creatureObserveBonusData.bonus = global::CreatureObserveBonusData.BonusType.SPEED;
                            }
                        }
                        else
                        {
                            creatureObserveBonusData.bonus = global::CreatureObserveBonusData.BonusType.PROB;
                        }
                    }
                    creatureObserveBonusData.level = level2;
                    creatureObserveBonusData.value = int.Parse(xmlNode28.InnerText);
                    list5.Add(creatureObserveBonusData);
                }
            }
            finally
            {
                IDisposable disposable10;
                if ((disposable10 = (enumerator10 as IDisposable)) != null)
                {
                    disposable10.Dispose();
                }
            }
            model.observeBonus.Init(list5);
            XmlNode xmlNode29 = stat.SelectSingleNode("maxWorkCount");
            if (xmlNode29 != null)
            {
                model.maxWorkCount = int.Parse(xmlNode29.InnerText);
            }
            XmlNode xmlNode30 = stat.SelectSingleNode("maxProbReductionCounter");
            if (xmlNode30 != null)
            {
                model.maxProbReductionCounter = int.Parse(xmlNode30.InnerText);
            }
            XmlNode xmlNode31 = stat.SelectSingleNode("probReduction");
            if (xmlNode31 != null)
            {
                model.probReduction = float.Parse(xmlNode31.InnerText);
            }
        }

        [NewMember]
        public void Loading_Mod(XmlNodeList xmlNodeList, List<CreatureTypeInfo> list, List<CreatureSpecialSkillTipTable> list2, Dictionary<long, int> specialTipSize, string modid)
        {
            IEnumerator enumerator = xmlNodeList.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    XmlNode xmlNode = (XmlNode)enumerator.Current;
                    string innerText = xmlNode.Attributes.GetNamedItem("src").InnerText;
                    string xml = "";
                    foreach (DirectoryInfo dir in Add_On.instance.DirList)
                    {
                        bool flag = false;
                        DirectoryInfo directoryInfo = EquipmentDataLoader.CheckNamedDir(dir, "Creature");
                        if (directoryInfo != null && Directory.Exists(directoryInfo.FullName + "/Creatures"))
                        {
                            DirectoryInfo directoryInfo2 = new DirectoryInfo(directoryInfo.FullName + "/Creatures");
                            if (directoryInfo2.GetFiles().Length != 0)
                            {
                                FileInfo[] files = directoryInfo2.GetFiles();
                                foreach (FileInfo fileInfo in files)
                                {
                                    if (fileInfo.Name == xmlNode.SelectSingleNode("stat").InnerText + ".txt" || fileInfo.Name == xmlNode.SelectSingleNode("stat").InnerText + ".xml")
                                    {
                                        xml = File.ReadAllText(fileInfo.FullName);
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (flag)
                        {
                            break;
                        }
                    }


                    XmlDocument xmlDocument = new XmlDocument();
                    XmlDocument doc = LoadDoc(innerText, currentLn, true);
                    xmlDocument.LoadXml(xml);
                    ChildCreatureData childData = null;
                    CreatureTypeInfo creatureTypeInfo = LoadCreatureTypeInfo(doc, ref list2, ref specialTipSize, out childData);
                    XmlNode xmlNode2 = xmlDocument.SelectSingleNode("creature");
                    XmlNode stat = xmlDocument.SelectSingleNode("creature/stat");
                    XmlNode xmlNode3 = xmlNode2.SelectSingleNode("child");
                    LoadCreatureStat_Mod(stat, xmlNode2, creatureTypeInfo, modid);
                    if (xmlNode3 != null)
                    {
                        string innerText2 = xmlNode3.InnerText;
                        string text = "";
                        foreach (DirectoryInfo dir2 in Add_On.instance.DirList)
                        {
                            if (!Directory.Exists(dir2.FullName + "/Creature/Creatures"))
                            {
                                continue;
                            }

                            FileInfo[] files = new DirectoryInfo(dir2.FullName + "/Creature/Creatures").GetFiles();
                            foreach (FileInfo fileInfo2 in files)
                            {
                                if (fileInfo2.Name == innerText2 + ".txt" || fileInfo2.Name == innerText2 + ".xml")
                                {
                                    text = File.ReadAllText(fileInfo2.FullName);
                                    break;
                                }
                            }

                            if (text != "")
                            {
                                break;
                            }
                        }

                        if (text == "")
                        {
                            if (!File.Exists(Application.dataPath + "/Managed/BaseMod/BaseCreatures/ChildCreatures/" + innerText2 + ".txt"))
                            {
                                text = Resources.Load<TextAsset>("xml/creatureStats/" + innerText2).text;
                                File.WriteAllText(Application.dataPath + "/Managed/BaseMod/BaseCreatures/ChildCreatures/" + innerText2 + ".txt", text);
                            }

                            text = File.ReadAllText(Application.dataPath + "/Managed/BaseMod/BaseCreatures/ChildCreatures/" + innerText2 + ".txt");
                        }

                        XmlDocument xmlDocument2 = new XmlDocument();
                        xmlDocument2.LoadXml(text);
                        ChildCreatureTypeInfo childCreatureTypeInfo = new ChildCreatureTypeInfo();
                        XmlNode xmlNode4 = xmlDocument2.SelectSingleNode("creature");
                        childCreatureTypeInfo.maxHp = (int)float.Parse(xmlNode4.SelectSingleNode("stat/hp").InnerText);
                        childCreatureTypeInfo.speed = float.Parse(xmlNode4.SelectSingleNode("stat/speed").InnerText);
                        XmlNode xmlNode5 = xmlNode4.SelectSingleNode("anim");
                        if (xmlNode5 != null)
                        {
                            childCreatureTypeInfo.animSrc = xmlNode5.Attributes.GetNamedItem("prefab").InnerText;
                        }

                        XmlNode xmlNode6 = xmlNode4.SelectSingleNode("riskLevel");
                        if (xmlNode6 != null)
                        {
                            int riskLevelOpen = (int)float.Parse(xmlNode6.Attributes.GetNamedItem("openLevel").InnerText);
                            string innerText3 = xmlNode6.InnerText;
                            childCreatureTypeInfo.riskLevelOpen = riskLevelOpen;
                            childCreatureTypeInfo._riskLevel = innerText3;
                        }

                        XmlNode xmlNode7 = xmlNode4.SelectSingleNode("attackType");
                        if (xmlNode7 != null)
                        {
                            int attackTypeOpen = (int)float.Parse(xmlNode7.Attributes.GetNamedItem("openLevel").InnerText);
                            string innerText4 = xmlNode7.InnerText;
                            childCreatureTypeInfo.attackTypeOpen = attackTypeOpen;
                            childCreatureTypeInfo._attackType = innerText4;
                        }

                        Dictionary<string, DefenseInfo> dictionary = new Dictionary<string, DefenseInfo>();
                        IEnumerator enumerator3 = xmlNode4.SelectNodes("stat/defense").GetEnumerator();
                        try
                        {
                            while (enumerator3.MoveNext())
                            {
                                XmlNode obj = (XmlNode)enumerator3.Current;
                                string innerText5 = obj.Attributes.GetNamedItem("id").InnerText;
                                DefenseInfo defenseInfo = new DefenseInfo();
                                IEnumerator enumerator4 = obj.SelectNodes("defenseElement").GetEnumerator();
                                try
                                {
                                    while (enumerator4.MoveNext())
                                    {
                                        XmlNode xmlNode8 = (XmlNode)enumerator4.Current;
                                        switch (xmlNode8.Attributes.GetNamedItem("type").InnerText)
                                        {
                                            case "P":
                                                defenseInfo.P = float.Parse(xmlNode8.InnerText);
                                                break;
                                            case "B":
                                                defenseInfo.B = float.Parse(xmlNode8.InnerText);
                                                break;
                                            case "W":
                                                defenseInfo.W = float.Parse(xmlNode8.InnerText);
                                                break;
                                            case "R":
                                                defenseInfo.R = float.Parse(xmlNode8.InnerText);
                                                break;
                                        }
                                    }
                                }
                                finally
                                {
                                    IDisposable disposable;
                                    if ((disposable = enumerator4 as IDisposable) != null)
                                    {
                                        disposable.Dispose();
                                    }
                                }

                                dictionary.Add(innerText5, defenseInfo);
                            }
                        }
                        finally
                        {
                            IDisposable disposable2;
                            if ((disposable2 = enumerator3 as IDisposable) != null)
                            {
                                disposable2.Dispose();
                            }
                        }

                        childCreatureTypeInfo.defenseTable.Init(dictionary);
                        XmlNode xmlNode9 = xmlNode4.SelectSingleNode("script");
                        if (xmlNode9 != null)
                        {
                            childCreatureTypeInfo.script = xmlNode9.InnerText;
                        }

                        XmlNode xmlNode10 = xmlNode4.SelectSingleNode("portrait");
                        if (xmlNode10 != null)
                        {
                            childCreatureTypeInfo._tempPortrait = xmlNode10.InnerText.Trim();
                            childCreatureTypeInfo._isChildAndHasData = true;
                        }

                        XmlNode xmlNode11 = xmlNode4.SelectSingleNode("metaInfo");
                        if (xmlNode11 != null)
                        {
                            string innerText6 = xmlNode11.InnerText;
                            CreatureTypeInfo creatureTypeInfo2 = LoadChildMeta(innerText6, ref list2, ref specialTipSize, true);
                            XmlNode statCreature = xmlNode4;
                            XmlNode stat2 = xmlNode4.SelectSingleNode("stat");
                            LoadCreatureStat_Mod(stat2, statCreature, creatureTypeInfo2, modid);
                            list.Add(creatureTypeInfo2);
                            childCreatureTypeInfo.id = creatureTypeInfo2.id;
                            childCreatureTypeInfo.isHasBaseMeta = true;
                        }

                        XmlNodeList xmlNodeList2 = xmlNode4.SelectNodes("sound");
                        Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                        IEnumerator enumerator5 = xmlNodeList2.GetEnumerator();
                        try
                        {
                            while (enumerator5.MoveNext())
                            {
                                XmlNode obj2 = (XmlNode)enumerator5.Current;
                                string innerText7 = obj2.Attributes.GetNamedItem("action").InnerText;
                                string innerText8 = obj2.Attributes.GetNamedItem("src").InnerText;
                                dictionary2.Add(innerText7, innerText8);
                            }
                        }
                        finally
                        {
                            IDisposable disposable3;
                            if ((disposable3 = enumerator5 as IDisposable) != null)
                            {
                                disposable3.Dispose();
                            }
                        }

                        childCreatureTypeInfo.soundTable = dictionary2;
                        creatureTypeInfo.childTypeInfo = childCreatureTypeInfo;
                        creatureTypeInfo.childTypeInfo.data = childData;
                    }

                    list.Add(creatureTypeInfo);
                }
            }
            finally
            {
                IDisposable disposable4;
                if ((disposable4 = enumerator as IDisposable) != null)
                {
                    disposable4.Dispose();
                }
            }
        }


        [ModifiesMember("Load")]
        public void Load_patch()
        {
            ModDebug.Log("CDL Load 1");
            try
            {
                ModDebug.Log("CDL Load 2");
                FieldInfo field = typeof(CreatureGenerateInfo).GetField("all", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                List<long> list = new List<long>();
                List<LcIdLong> lclist = new List<LcIdLong>();
                string xml = File.ReadAllText(Application.dataPath + "/Managed/BaseMod/BaseCreatureGen.xml");
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                foreach (object obj in xmlDocument.SelectNodes("/All/id"))
                {
                    long item = (long)int.Parse(((XmlNode)obj).InnerText);
                    list.Add(item);
                }
                foreach (ModInfo mod in (Add_On.instance as Add_On_patch).ModList)
                {
                    ModInfo_patch modinfo = (ModInfo_patch)mod;
                    DirectoryInfo directoryInfo = EquipmentDataLoader.CheckNamedDir(modinfo.modpath, "Creature");
                    if (directoryInfo != null && Directory.Exists(directoryInfo.FullName + "/CreatureGen"))
                    {
                        DirectoryInfo directoryInfo2 = new DirectoryInfo(directoryInfo.FullName + "/CreatureGen");
                        if (directoryInfo2.GetFiles().Length != 0)
                        {
                            foreach (FileInfo fileInfo in directoryInfo2.GetFiles())
                            {
                                if (fileInfo.Name.Contains(".xml") || fileInfo.Name.Contains(".txt"))
                                {
                                    xml = File.ReadAllText(fileInfo.FullName);
                                    XmlDocument xmlDocument2 = new XmlDocument();
                                    xmlDocument2.LoadXml(xml);
                                    if (modinfo.modid == String.Empty)
                                    {
                                        foreach (object obj2 in xmlDocument2.SelectNodes("/All/add"))
                                        {
                                            long item2 = (long)int.Parse(((XmlNode)obj2).InnerText);
                                            list.Add(item2);
                                        }
                                        foreach (object obj3 in xmlDocument2.SelectNodes("/All/remove"))
                                        {
                                            long item3 = (long)int.Parse(((XmlNode)obj3).InnerText);
                                            list.Remove(item3);
                                        }
                                    }
                                    else
                                    {
                                        foreach (object obj2 in xmlDocument2.SelectNodes("/All/add"))
                                        {
                                            long item2 = (long)int.Parse(((XmlNode)obj2).InnerText);
                                            LcIdLong id = new LcIdLong(modinfo.modid, item2);
                                            lclist.Add(id);
                                        }
                                        foreach (object obj3 in xmlDocument2.SelectNodes("/All/remove"))
                                        {
                                            long item3 = (long)int.Parse(((XmlNode)obj3).InnerText);
                                            LcIdLong id = new LcIdLong(modinfo.modid, item3);
                                            lclist.Remove(id);
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

                ModDebug.Log("CDL Load 3");

                field.SetValue(null, list.ToArray());
                CreatureGenerateInfo_patch.all_mod = lclist;

                if (!EquipmentTypeList.instance.loaded)
                {
                    ModDebug.Log("LoadCreatureList >> EquipmentTypeList must be loaded. ");
                }
                this.currentLn = GlobalGameManager.instance.GetCurrentLanguage();
                string xml2 = File.ReadAllText(Application.dataPath + "/Managed/BaseMod/BaseList.txt");
                XmlDocument xmlDocument3 = new XmlDocument();
                xmlDocument3.LoadXml(xml2);
                XmlNodeList xmlNodeList = xmlDocument3.SelectNodes("/creature_list/creature");

                List<CreatureTypeInfo> list2 = new List<CreatureTypeInfo>();
                Dictionary<string, List<CreatureTypeInfo>> CTIdic = new Dictionary<string, List<CreatureTypeInfo>>();

                List<CreatureSpecialSkillTipTable> list3 = new List<CreatureSpecialSkillTipTable>();
                Dictionary<string, List<CreatureSpecialSkillTipTable>> CSSTTdic = new Dictionary<string, List<CreatureSpecialSkillTipTable>>();

                Dictionary<long, int> dictionary = new Dictionary<long, int>();

                this.Loading(xmlNodeList, list2, list3, dictionary, false);

                ModDebug.Log("CDL Load 4");

                foreach (ModInfo mod in (Add_On.instance as Add_On_patch).ModList)
                {
                    ModInfo_patch modinfo = (ModInfo_patch)mod;
                    DirectoryInfo directoryInfo3 = EquipmentDataLoader.CheckNamedDir(modinfo.modpath, "Creature");
                    if (directoryInfo3 != null && Directory.Exists(directoryInfo3.FullName + "/CreatureList"))
                    {
                        DirectoryInfo directoryInfo4 = new DirectoryInfo(directoryInfo3.FullName + "/CreatureList");
                        if (directoryInfo4.GetFiles().Length != 0)
                        {
                            if (modinfo.modid != String.Empty)
                            {
                                CTIdic[modinfo.modid] = new List<CreatureTypeInfo>();
                                CSSTTdic[modinfo.modid] = new List<CreatureSpecialSkillTipTable>();
                            }
                            foreach (FileInfo fileInfo2 in directoryInfo4.GetFiles())
                            {
                                if (fileInfo2.Name.Contains(".txt") || fileInfo2.Name.Contains(".xml"))
                                {
                                    if (modinfo.modid == String.Empty)
                                    {
                                        XmlDocument xmlDocument4 = new XmlDocument();
                                        xmlDocument4.LoadXml(File.ReadAllText(fileInfo2.FullName));
                                        XmlNodeList xmlNodeList2 = xmlDocument4.SelectNodes("/creature_list/creature");
                                        List<CreatureTypeInfo> list4 = new List<CreatureTypeInfo>();
                                        List<CreatureSpecialSkillTipTable> list5 = new List<CreatureSpecialSkillTipTable>();
                                        Dictionary<long, int> dictionary2 = new Dictionary<long, int>();
                                        this.Loading(xmlNodeList2, list4, list5, dictionary2, true);
                                        foreach (KeyValuePair<long, int> keyValuePair in dictionary2)
                                        {
                                            if (dictionary.ContainsKey(keyValuePair.Key))
                                            {
                                                for (int j = 0; j < list2.Count; j++)
                                                {
                                                    dictionary.Remove(keyValuePair.Key);
                                                    dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                                                }
                                            }
                                        }
                                        foreach (CreatureTypeInfo creatureTypeInfo in list4)
                                        {
                                            foreach (CreatureTypeInfo creatureTypeInfo2 in list2)
                                            {
                                                if (creatureTypeInfo.id == creatureTypeInfo2.id)
                                                {
                                                    list2.Remove(creatureTypeInfo2);
                                                    break;
                                                }
                                            }
                                            list2.Add(creatureTypeInfo);
                                        }
                                        foreach (CreatureSpecialSkillTipTable creatureSpecialSkillTipTable in list5)
                                        {
                                            foreach (CreatureSpecialSkillTipTable creatureSpecialSkillTipTable2 in list3)
                                            {
                                                if (creatureSpecialSkillTipTable2.creatureTypeId == creatureSpecialSkillTipTable.creatureTypeId)
                                                {
                                                    list3.Remove(creatureSpecialSkillTipTable2);
                                                    break;
                                                }
                                            }
                                            list3.Add(creatureSpecialSkillTipTable);
                                        }
                                    }
                                    else
                                    {

                                        XmlDocument xmlDocument4 = new XmlDocument();
                                        xmlDocument4.LoadXml(File.ReadAllText(fileInfo2.FullName));
                                        XmlNodeList xmlNodeList2 = xmlDocument4.SelectNodes("/creature_list/creature");
                                        List<CreatureTypeInfo> list4 = new List<CreatureTypeInfo>();
                                        List<CreatureSpecialSkillTipTable> list5 = new List<CreatureSpecialSkillTipTable>();
                                        Dictionary<long, int> dictionary2 = new Dictionary<long, int>();
                                        this.Loading_Mod(xmlNodeList2, list4, list5, dictionary2, modinfo.modid);
                                        foreach(CreatureTypeInfo c in list4)
                                        {
                                            ((CreatureTypeInfo_patch)(object)c).modid = modinfo.modid;
                                        }
                                        foreach (CreatureSpecialSkillTipTable t in list5)
                                        {
                                            ((CreatureSpecialSkillTipTable_patch)(object)t).modid = modinfo.modid;
                                        }
                                        CTIdic[modinfo.modid].AddRange(list4);
                                        CSSTTdic[modinfo.modid].AddRange(list5);
                                    }
                                }
                            }
                        }
                    }
                }

                ModDebug.Log("CDL Load 5");
                ((CreatureTypeList_patch)(object)CreatureTypeList.instance).Init_Mod(CTIdic, CSSTTdic);
                CreatureTypeList.instance.Init(list2.ToArray(), list3.ToArray(), dictionary);
               
                ModDebug.Log("CDL Load 6");
            }
            catch (Exception ex)
            {
                ModDebug.Log("CDLerror - " + ex.Message + Environment.NewLine + ex.StackTrace);
                //File.WriteAllText(Application.dataPath + "/BaseMods/CDLerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
    [ModifiesType("CreatureGenerateInfo")]
    public class CreatureGenerateInfo_patch
    {
        [NewMember]
        public static List<LcIdLong> GetAll_Mod(bool removeTool = false)
        {
            List<LcIdLong> result = new List<LcIdLong>(all_mod);
            if (removeTool)
            {
                result.RemoveAll(x => CreatureTypeList_patch.instance.GetData_Mod(x).creatureWorkType == CreatureWorkType.KIT);
            }
            return result;
        }
        [NewMember]
        public static List<LcIdLong> all_mod = new List<LcIdLong>();
    }
    [ModifiesType("CreatureSelect.CreatureSelectUnit")]
    public class CreatureSelectUnit_patch
    {
        [ModifiesMember("LateInit")]
        private void LateInit_patch()
        {
            if (this.savedIdMod != -1L)
            {
                this.Init_Mod(this.savedIdMod);
                this.savedIdMod = new LcIdLong(-1);
            }
        }
        [ModifiesMember("GetName")]
        private string GetName_patch()
        {
            if (this.metaInfo == null)
            {
                return string.Empty;
            }
            string result = string.Empty;
            CreatureObserveInfoModel observeInfo = CreatureManager_patch.instance().GetObserveInfo_Mod(_creatureIdMod);
            if (observeInfo != null)
            {
                result = CreatureModel.GetUnitName(this.metaInfo, observeInfo);
            }
            else
            {
                result = this.metaInfo.codeId;
            }
            return result;
        }
        [NewMember]
        public void Init_Mod(LcIdLong creatureId)
        {
            if (isChanging)
            {
                savedId = creatureId.id;
                savedIdMod = creatureId;
                ModDebug.Log("IsChanging - " + creatureId.ToString());
                return;
            }

            if (creatureId == 100014 && PlagueDoctor.CheckAdvent())
            {
                creatureId = new LcIdLong(100015L);
            }

            _creatureIdMod = creatureId;
            if (_creatureIdMod == -1)
            {
                SetDisabled();
                return;
            }

            this.get_gameObject().SetActive(value: true);
            TransSelected = false;
            metaInfo = CreatureTypeList_patch.instance.GetData_Mod(creatureId);
            IdText.text = GetName_patch();
            DullTimer.StartTimer(get_DullFreq());
            DullAnimCTRL.SetFloat("Speed", 0.2f);
            Image[] frame = Frame;
            foreach (Image image in frame)
            {
                image.enabled = false;
            }

            if (_creatureIdMod.packageId == String.Empty && CreatureGenerateInfo.IsCreditCreature(_creatureIdMod.id))
            {
                NormalCreatureFrame.SetActive(value: false);
                CreditCreatureFrame.SetActive(value: true);
            }
            else
            {
                NormalCreatureFrame.SetActive(value: true);
                CreditCreatureFrame.SetActive(value: false);
            }
            ModDebug.Log("Active Generate Model");
        }
        [ModifiesMember("Init")]
        public void Init_patch(long creatureId)
        {
            Init_Mod(new LcIdLong(creatureId));
        }

        [MemberAlias("get_DullFreq", typeof(CreatureSelectUnit))]
        private float get_DullFreq()
        {
            float num = 1f;
            if (this.pointer)
            {
                num = 0.5f;
            }
            return UnityEngine.Random.Range(4f, 8f) * num;
        }
        [MemberAlias("SetDisabled", typeof(CreatureSelectUnit))]
        public void SetDisabled()
        {
        }

        [NewMember]
        [NonSerialized]
        public LcIdLong savedIdMod;
        [NewMember]
        [NonSerialized]
        public LcIdLong _creatureIdMod;

        [MemberAlias("EmptyId", typeof(CreatureSelectUnit))]
        public const long EmptyId = -1;
        [MemberAlias("RootObject", typeof(CreatureSelectUnit))]
        public GameObject RootObject;
        [MemberAlias("TransitionTime", typeof(CreatureSelectUnit))]
        public float TransitionTime;
        //[MemberAlias("_creatureId", typeof(CreatureSelectUnit))]
        //private long _creatureId;
        [MemberAlias("metaInfo", typeof(CreatureSelectUnit))]
        private global::CreatureTypeInfo metaInfo;
        [MemberAlias("TransitionTimer", typeof(CreatureSelectUnit))]
        private Timer TransitionTimer;
        [MemberAlias("DullTimer", typeof(CreatureSelectUnit))]
        private Timer DullTimer;
        [MemberAlias("IdText", typeof(CreatureSelectUnit))]
        public Text IdText;
        [MemberAlias("TransAnim", typeof(CreatureSelectUnit))]
        public Animator TransAnim;
        [MemberAlias("DoorAnim", typeof(CreatureSelectUnit))]
        public Animator DoorAnim;
        [MemberAlias("PositionPivot", typeof(CreatureSelectUnit))]
        public RectTransform PositionPivot;
        [MemberAlias("DullAnimCTRL", typeof(CreatureSelectUnit))]
        public Animator DullAnimCTRL;
        [MemberAlias("Frame", typeof(CreatureSelectUnit))]
        public Image[] Frame;
        [MemberAlias("NormalCreatureFrame", typeof(CreatureSelectUnit))]
        public GameObject NormalCreatureFrame;
        [MemberAlias("CreditCreatureFrame", typeof(CreatureSelectUnit))]
        public GameObject CreditCreatureFrame;
        [MemberAlias("TransSelected", typeof(CreatureSelectUnit))]
        public bool TransSelected;
        [MemberAlias("pointer", typeof(CreatureSelectUnit))]
        private bool pointer;
        [MemberAlias("savedId", typeof(CreatureSelectUnit))]
        private long savedId = -1L;
        [MemberAlias("isChanging", typeof(CreatureSelectUnit))]
        private bool isChanging;

        [MemberAlias("get_gameObject", typeof(Component))]
        public GameObject get_gameObject()
        {
            return null;
        }
    }

    [ModifiesType("CreatureGenerate.CreatureGenerateDoor")]
    public class CreatureGenerateDoor_patch
    {
        [ModifiesMember("SetCreature")]
        public void SetCreature_patch()
        {
            SetCreature_Mod();
        }
        [NewMember]
        public void SetCreature_Mod()
        {
            this.CreatureMod = new LcIdLong(-1);
            this.CheckProb();
            if (this.get_TotalProb() == 0f)
            {
                return;
            }
            float num = UnityEngine.Random.Range(0f, this.get_TotalProb());
            float num2 = 0f;
            int i = 0;
            for (int j = 0; j < 5; j++)
            {
                if (this.probState[j])
                {
                    num2 += this.prob[j];
                    if (num <= num2)
                    {
                        i = j;
                        break;
                    }
                }
            }
            ActivateStateList list = this.GetList(i);
            ActivateStateModel randomCreature = list.GetRandomCreature();
            CreatureMod = new LcIdLong(((ActivateStateModel_patch)(object)randomCreature).modid, randomCreature.id);

            ((ActivateStateList_patch)(object)list).RemoveAction_Mod(CreatureMod);

        }


        [MemberAlias("GetList", typeof(CreatureGenerateDoor))]
        public ActivateStateList GetList(int i)
        {
            return null;
        }
        [MemberAlias("get_TotalProb", typeof(CreatureGenerateDoor))]
        public float get_TotalProb()
        {
            float num = 0f;
            for (int i = 0; i < 5; i++)
            {
                if (this.probState[i])
                {
                    num += this.prob[i];
                }
            }
            return num;
        }

        [NewMember]
        public LcIdLong CreatureMod;


        [MemberAlias("CheckProb", typeof(CreatureGenerateDoor))]
        public void CheckProb()
        {
        }

        [MemberAlias("zeroAry", typeof(CreatureGenerateDoor))]
        public static readonly float[] zeroAry;
        [MemberAlias("initialState", typeof(CreatureGenerateDoor))]
        public static bool[] initialState;
        [MemberAlias("MAX", typeof(CreatureGenerateDoor))]
        public const int MAX = 1;
        [MemberAlias("prob", typeof(CreatureGenerateDoor))]
        public float[] prob;
        [MemberAlias("probState", typeof(CreatureGenerateDoor))]
        public bool[] probState;
    }

    [ModifiesType("CreatureGenerate.CreatureGenerateModel")]
    public class CreatureGenerateModel_patch
    {

        [ModifiesMember("OnlyAction")]
        public void OnlyAction_patch(params object[] ids)
        {
            foreach (object obj in ids)
            {
                if (obj is long)
                {
                    Debug.Log((long)obj);
                    this.creatureMod.Add(new LcIdLong((long)obj));
                    this.stop = true;
                }
            }
        }

        [ModifiesMember("SetCreature")]
        public void SetCreature_patch()
        {
            if (this.commonAction != null)
            {
                this.commonAction.Exectue();
            }
            ModDebug.Log(stop ? "Stop" : "Not Stop");
            if (!this.stop)
            {
                if (this.door1.commonAction != null)
                {
                    this.door1.commonAction.Exectue();
                }
                if (this.door2.commonAction != null)
                {
                    this.door2.commonAction.Exectue();
                }
                if (this.door3.commonAction != null)
                {
                    this.door3.commonAction.Exectue();
                }
                this.door1.SetCreature();
                this.door2.SetCreature();
                this.door3.SetCreature();
                if (((CreatureGenerateDoor_patch)(object)this.door1).CreatureMod != -1L)
                {
                    this.creatureMod.Add(((CreatureGenerateDoor_patch)(object)this.door1).CreatureMod);
                }
                if (((CreatureGenerateDoor_patch)(object)this.door2).CreatureMod != -1L)
                {
                    this.creatureMod.Add(((CreatureGenerateDoor_patch)(object)this.door2).CreatureMod);
                }
                if (((CreatureGenerateDoor_patch)(object)this.door3).CreatureMod != -1L)
                {
                    this.creatureMod.Add(((CreatureGenerateDoor_patch)(object)this.door3).CreatureMod);
                }
                return;
            }
        }

        [ModifiesMember(".ctor")]
        public void Ctor()
        {
            day = -1;
            door1 = new CreatureGenerateDoor();
            door2 = new CreatureGenerateDoor();
            door3 = new CreatureGenerateDoor();
            creatureMod = new List<LcIdLong>();
        }

        [NewMember]
        //[NonSerialized]
        public List<LcIdLong> creatureMod;

        [MemberAlias("day", typeof(CreatureGenerateModel))]
        public int day;
        [MemberAlias("door1", typeof(CreatureGenerateModel))]
        public CreatureGenerateDoor door1;
        [MemberAlias("door2", typeof(CreatureGenerateModel))]
        public CreatureGenerateDoor door2;
        [MemberAlias("door3", typeof(CreatureGenerateModel))]
        public CreatureGenerateDoor door3;
        //[MemberAlias("creature", typeof(CreatureGenerateModel))]
        //public List<long> creature;
        [MemberAlias("stop", typeof(CreatureGenerateModel))]
        public bool stop;

        [MemberAlias("commonAction", typeof(CreatureGenerateData))]
        public CreatureGenerateData.ActionData commonAction;
    }
    [ModifiesType("CreatureSelectUI")]
    public class CreatureSelectUI_patch
    {
        [ModifiesMember("SetSlotInit")]
        private void SetSlotInit_patch(bool setEmpty = true)
        {
            CurrentCreatures_Mod.Clear();
            if (setEmpty)
            {
                foreach (CreatureSelect.CreatureSelectUnit creatureSelectUnit in this.Units)
                {
                    ((CreatureSelectUnit_patch)(object)creatureSelectUnit).Init_Mod(new LcIdLong(-1));
                }
            }
        }
        [ModifiesMember("OnClickUnit")]
        public void OnClickUnit_patch(CreatureSelect.CreatureSelectUnit unit)
        {
            if (!this.effectRunned)
            {
                this.effectRunned = true;
                CreatureSelectUnit_patch punit = (CreatureSelectUnit_patch)(object)unit;
                if (punit._creatureIdMod == 100015L)
                {
                    global::PlayerModel.instance.AddWaitingCreature(100014L);
                }
                else
                {
                    ((PlayerModel_patch)(object)PlayerModel.instance).AddWaitingCreature_Mod(punit._creatureIdMod);
                }
                ((CreatureGenerateInfoManager_patch)(object)CreatureGenerate.CreatureGenerateInfoManager.Instance).OnUsed_Mod(punit._creatureIdMod);
                this.GlobalControlAnim.SetTrigger("Close");
                this.FadeoutEffect(3f);
            }
            this.TextBoxController.Hide();
        }
        [ModifiesMember("OnClickReExtract")]
        public void OnClickReExtract_patch()
        {
            try
            {
                if (!this.get_ReExtractResearchCompleted())
                {
                    return;
                }
                this._reExtracted = true;
                this.GetCreatureList_patch(true);
                CreatureSelect.CreatureSelectUnit[] units = this.Units;
                for (int i = 0; i < units.Length; i++)
                {
                    units[i].gameObject.SetActive(true);
                    units[i].OnChange();
                }
                ModDebug.Log("CurrentCreatures_Mod Count : " + this.CurrentCreatures_Mod.Count);
                if (this.CurrentCreatures_Mod.Count == 1)
                {
                    this.Units[0].SetDisabled();
                    this.Units[2].SetDisabled();

                    ((CreatureSelectUnit_patch)(object)this.Units[1]).Init_Mod(this.CurrentCreatures_Mod[0]);
                    this.Units[1].transform.SetParent(this.Index_Normal);
                }
                else if (this.CurrentCreatures_Mod.Count == 0)
                {
                    List<LcIdLong> modlist = new List<LcIdLong>();
                    foreach (long id in new List<long>(global::CreatureGenerateInfo.GetAll(true)))
                    {
                        modlist.Add(new LcIdLong(id));
                    }
                    foreach (global::CreatureModel creatureModel in global::CreatureManager.instance.GetCreatureList())
                    {
                        LcIdLong lcid = new LcIdLong(CreatureTypeList_patch.instance.GetModId(creatureModel.metaInfo), creatureModel.metadataId);
                        modlist.Remove(lcid);
                    }
                    for (int j = 0; j < 3; j++)
                    {
                        LcIdLong num = modlist[UnityEngine.Random.Range(0, modlist.Count)];
                        ((CreatureSelectUnit_patch)(object)this.Units[j]).Init_Mod(num);
                        modlist.Remove(num);
                    }
                }
                else
                {
                    List<int> list2 = new List<int>(new int[]
                    {
                0,
                1,
                2
                    });
                    for (int k = 0; k < this.Units.Length; k++)
                    {
                        int num2 = list2[UnityEngine.Random.Range(0, list2.Count)];
                        list2.Remove(num2);
                        CreatureSelect.CreatureSelectUnit creatureSelectUnit2 = this.Units[num2];
                        LcIdLong creatureId = new LcIdLong(-1L);
                        if (k < this.CurrentCreatures_Mod.Count)
                        {
                            creatureId = this.CurrentCreatures_Mod[k];
                        }
                        creatureSelectUnit2.transform.SetParent(this.Index_Normal);
                        if (creatureId == null)
                        {
                            ModDebug.Log(" creatureid NULL!");
                        }
                        else
                        {
                            if (creatureId.packageId == null)
                            {
                                ModDebug.Log(" creatureid - pid NULL!");
                            }
                            else
                            {
                                ModDebug.Log("Init creatureid : " + creatureId.ToString());
                            }

                        }


                        ((CreatureSelectUnit_patch)(object)creatureSelectUnit2).Init_Mod(creatureId);
                    }
                }

                for (int l = this.Units.Length - 1; l >= 0; l--)
                {
                    CreatureSelectUnit_patch creatureSelectUnit3 = (CreatureSelectUnit_patch)(object)this.Units[l];
                    for (int m = 0; m < l; m++)
                    {
                        CreatureSelectUnit_patch creatureSelectUnit4 = (CreatureSelectUnit_patch)(object)this.Units[m];

                        if (l != m && creatureSelectUnit4._creatureIdMod == creatureSelectUnit3._creatureIdMod)
                        {
                            //creatureSelectUnit3.get_gameObject().SetActive(false);
                            break;
                        }
                    }
                }
                if (!this._reExtracted)
                {
                    this.reExtractController.Show();
                    return;
                }
                this.reExtractController.Hide();
            }
            catch (Exception e)
            {
                ModDebug.Log("CSUI.OnClickReExtracterror - " + e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        [ModifiesMember("Init")]
        public void Init_patch()
        {
            try
            {
                if (!this.get_ReExtractResearchCompleted())
                {
                    this.reExtractController.gameObject.SetActive(false);
                }
                else if (!this.reExtractController.gameObject.activeInHierarchy && !this._reExtracted)
                {
                    this.reExtractController.gameObject.SetActive(true);
                }
                if (!this.CheckUIActivateCondition())
                {
                    this.OnUIActionEnd();
                    return;
                }
                this.effectRunned = false;
                this.filter.enabled = true;
                foreach (CreatureSelect.CreatureSelectUnit creatureSelectUnit in this.Units)
                {
                    creatureSelectUnit.gameObject.SetActive(true);
                    creatureSelectUnit.TransAnim.SetTrigger("Exit");
                }
                this.GlobalControlAnim.SetTrigger("Open");
                this.GetCreatureList_patch(true);
                ModDebug.Log("CurrentCreatures_Mod Count : " + this.CurrentCreatures_Mod.Count);
                if (this.CurrentCreatures_Mod.Count == 1)
                {
                    this.Units[0].SetDisabled();
                    this.Units[2].SetDisabled();
                    ((CreatureSelectUnit_patch)(object)this.Units[1]).Init_Mod(this.CurrentCreatures_Mod[0]);
                    this.Units[1].transform.SetParent(this.Index_Normal);
                }
                else if (this.CurrentCreatures_Mod.Count == 0)
                {
                    List<LcIdLong> modlist = new List<LcIdLong>();
                    foreach (long id in new List<long>(global::CreatureGenerateInfo.GetAll(true)))
                    {
                        modlist.Add(new LcIdLong(id));
                    }
                    foreach (global::CreatureModel creatureModel in global::CreatureManager.instance.GetCreatureList())
                    {
                        LcIdLong lcid = new LcIdLong(CreatureTypeList_patch.instance.GetModId(creatureModel.metaInfo), creatureModel.metadataId);
                        modlist.Remove(lcid);
                    }
                    for (int j = 0; j < 3; j++)
                    {
                        LcIdLong num = modlist[UnityEngine.Random.Range(0, modlist.Count)];
                        ((CreatureSelectUnit_patch)(object)this.Units[j]).Init_Mod(num);
                        modlist.Remove(num);
                    }
                }
                else
                {
                    List<int> list2 = new List<int>(new int[]
                    {
                0,
                1,
                2
                    });
                    for (int k = 0; k < this.Units.Length; k++)
                    {
                        int num2 = list2[UnityEngine.Random.Range(0, list2.Count)];
                        list2.Remove(num2);
                        CreatureSelect.CreatureSelectUnit creatureSelectUnit2 = this.Units[num2];
                        LcIdLong creatureId = new LcIdLong(-1L);
                        if (k < this.CurrentCreatures_Mod.Count)
                        {
                            creatureId = this.CurrentCreatures_Mod[k];
                        }
                        creatureSelectUnit2.transform.SetParent(this.Index_Normal);
                        if (creatureId == null)
                        {
                            ModDebug.Log(" creatureid NULL!");
                        }
                        else
                        {
                            if (creatureId.packageId == null)
                            {
                                ModDebug.Log(" creatureid - pid NULL!");
                            }
                            else
                            {
                                ModDebug.Log("Init creatureid : " + creatureId.ToString());
                            }

                        }


                        ((CreatureSelectUnit_patch)(object)creatureSelectUnit2).Init_Mod(creatureId);
                    }
                }
                global::StoryBgm.instance.PlayClip(this.clip, 55f);
                for (int l = this.Units.Length - 1; l >= 0; l--)
                {
                    CreatureSelectUnit_patch creatureSelectUnit3 = (CreatureSelectUnit_patch)(object)this.Units[l];
                    for (int m = 0; m < l; m++)
                    {
                        CreatureSelectUnit_patch creatureSelectUnit4 = (CreatureSelectUnit_patch)(object)this.Units[m];
                        if (l != m && creatureSelectUnit4._creatureIdMod == creatureSelectUnit3._creatureIdMod)
                        {
                            //creatureSelectUnit3.get_gameObject().SetActive(false);
                            break;
                        }
                    }
                }
                if (this.get_ReExtractResearchCompleted() && !this._reExtracted)
                {
                    this.reExtractController.Show();
                    return;
                }
                this.reExtractController.Hide();
            }
            catch (Exception e)
            {
                ModDebug.Log("CSUI.Initerror - " + e.Message + Environment.NewLine + e.StackTrace);
            }
        }
        [NewMember]
        public static bool CheckCreatureExisting_Mod(LcIdLong targetId)
        {
            if (targetId.packageId == String.Empty)
            {
                if (targetId == 100014)
                {
                    return CreatureManager.instance.FindCreature(100015L) != null || CreatureManager.instance.FindCreature(100014L) != null;
                }
            }
            return CreatureManager_patch.instance().FindCreature_Mod(targetId) != null;
        }
        [ModifiesMember("CheckCreatureExisting")]
        public static bool CheckCreatureExisting_patch(long targetId)
        {
            return CheckCreatureExisting_Mod(new LcIdLong(targetId));
        }
        [ModifiesMember("CheckYinAndYang")]
        private void CheckYinAndYang_patch()
        {
            this.threshold++;
            if (this.threshold >= 3)
            {
                return;
            }
            List<CreatureModel> list = new List<CreatureModel>(CreatureManager.instance.GetCreatureList());
            bool flag = CreatureSelectUI.CheckCreatureExisting(100104L);
            bool flag2 = CreatureSelectUI.CheckCreatureExisting(300109L);
            int count = list.Count;
            if (flag2 && flag)
            {
                return;
            }
            if (flag)
            {
                if (this.get_Day() >= 48)
                {
                    if (this.CurrentCreatures_Mod.Count <= 1)
                    {
                        this.GetCreatureList_patch(false);
                        return;
                    }
                    //this.CurrentCreatures.Remove(100104L);
                    this.CurrentCreatures_Mod.Remove(new LcIdLong(100104));
                    return;
                }
                else if (CreatureGenerate.CreatureGenerateInfoManager.Instance.GenKit && !PlayerModel.instance.IsWaitingCreature(300109L))
                {
                    this.CurrentCreatures_Mod.Clear();
                    //this.CurrentCreatures.Add(300109L);
                    this.CurrentCreatures_Mod.Add(new LcIdLong(300109L));
                }
            }
        }
        [ModifiesMember("GetCreatureList")]
        private void GetCreatureList_patch(bool setEmpty = true)
        {
            CurrentCreatures_Mod.Clear();
            //this.CurrentCreatures.Clear();
            CreatureGenerate.CreatureGenerateInfoManager.Instance.CalculateDay();
            this.CheckKitGeneration();
            CreatureGenerate.CreatureGenerateInfoManager.Instance.OnDayChanged();
            this.SetSlotInit(setEmpty);
            List<LcIdLong> list = ((CreatureGenerateInfoManager_patch)(object)CreatureGenerate.CreatureGenerateInfoManager.Instance).GetCreature_Mod();
          
            if (list == null)
            {
                list = new List<LcIdLong>();
                Debug.LogError("null removed + " + (this.get_Day() % 5 == 3).ToString());
                List<long> l = new List<long>(CreatureGenerateInfo.GetAll(true));
                foreach (long id in l)
                {
                    list.Add(new LcIdLong(id));
                }
                List<LcIdLong> lcid = new List<LcIdLong>(CreatureGenerateInfo_patch.GetAll_Mod(true));
                list.AddRange(lcid);
            }
            ModDebug.Log("GetCreatureList list count : " + list.Count);
            List<LcIdLong> list2 = new List<LcIdLong>();
            foreach (long id in new List<long>(CreatureGenerateInfo.GetAll(true)))
            {
                list2.Add(new LcIdLong(id));
            }
            if (list.Count == 0)
            {
                Debug.LogError("Could not make Creature");
                return;
            }
            foreach (CreatureModel creatureModel in CreatureManager.instance.GetCreatureList())
            {
                LcIdLong lcid = new LcIdLong(CreatureTypeList_patch.instance.GetModId(creatureModel.metaInfo), creatureModel.metadataId);
                list2.Remove(lcid);
                if (creatureModel.metadataId == 100014L)
                {
                    list2.Remove(new LcIdLong(100015L));
                }
                if (!list.Remove(lcid))
                {
                    if (lcid == 100015L)
                    {
                        list.Remove(new LcIdLong(100014L));
                    }
                }
            }
            List<LcIdLong> LcIdList = new List<LcIdLong>();
            //List<long> list3 = new List<long>();
            for (int j = 0; j < 3; j++)
            {
                if (list.Count == 0)
                {
                    if (LcIdList.Count != 0)
                    {
                        break;
                    }
                    foreach (CreatureModel creatureModel2 in CreatureManager.instance.GetCreatureList())
                    {
                        LcIdLong lcid = new LcIdLong(CreatureTypeList_patch.instance.GetModId(creatureModel2.metaInfo), creatureModel2.metadataId);
                        if (!list2.Remove(lcid))
                        {
                            if (lcid == 100015L)
                            {
                                list.Remove(new LcIdLong(100014L));
                            }
                        }
                    }
                    list = list2;
                }
                LcIdLong item = list[UnityEngine.Random.Range(0, list.Count)];
                LcIdList.Add(item);
                list.Remove(item);
            }
            this.CurrentCreatures_Mod.AddRange(LcIdList);
            this.CheckYinAndYang_patch();
        }
        [ModifiesMember(".ctor")]
        public void Ctor()
        {
            EffectTimer = new Timer();
            FadeoutEffectTimer = new Timer();
            startVolume = 1f;

            CurrentCreatures_Mod = new List<LcIdLong>();
        }

        [NewMember]
        [NonSerialized]
        public List<LcIdLong> CurrentCreatures_Mod;

        [MemberAlias("FadeoutEffect", typeof(CreatureSelectUI))]
        public void FadeoutEffect(float time = 3f)
        {
            this.FadeoutEffectTimer.StartTimer(time);
            this.startVolume = global::StoryBgm.instance.GetVolume();
        }
        [MemberAlias("OnUIActionEnd", typeof(CreatureSelectUI))]
        public void OnUIActionEnd()
        {
        }
        [MemberAlias("CheckUIActivateCondition", typeof(CreatureSelectUI))]
        private bool CheckUIActivateCondition()
        {
            return false;
        }

        [MemberAlias("get_ReExtractResearchCompleted", typeof(CreatureSelectUI))]
        private bool get_ReExtractResearchCompleted()
        {
            return ResearchDataModel.instance.IsUpgradedAbility("reextract_creature");
        }
        [MemberAlias("get_Day", typeof(CreatureSelectUI))]
        private int get_Day()
        {
            return PlayerModel.instance.GetDay();
        }
        [MemberAlias("SetSlotInit", typeof(CreatureSelectUI))]
        private void SetSlotInit(bool setEmpty = true)
        {
            this.CurrentCreatures_Mod.Clear();
            if (setEmpty)
            {
                foreach (CreatureSelect.CreatureSelectUnit creatureSelectUnit in this.Units)
                {
                    creatureSelectUnit.Init(-1L);
                }
            }
        }
        [MemberAlias("CheckKitGeneration", typeof(CreatureSelectUI))]
        private void CheckKitGeneration()
        {
            int genDay = CreatureGenerate.CreatureGenerateInfoManager.Instance.GenDay;
            if (genDay < 20)
            {
                if (genDay % 5 == 3)
                {
                    CreatureGenerate.CreatureGenerateInfoManager.Instance.GenKit = true;
                }
                else
                {
                    CreatureGenerate.CreatureGenerateInfoManager.Instance.GenKit = false;
                }
            }
            else if (genDay >= 20 && genDay < 25)
            {
                CreatureGenerate.CreatureGenerateInfoManager.Instance.GenKit = false;
                if (genDay == 21)
                {
                    if (this._tiperethRunned)
                    {
                        CreatureGenerate.CreatureGenerateInfoManager.Instance.GenKit = true;
                    }
                }
                else if (genDay == 23 && this._tiperethRunned)
                {
                    CreatureGenerate.CreatureGenerateInfoManager.Instance.GenKit = true;
                }
            }
            else if (genDay >= 25 && genDay < 50)
            {
                if (genDay % 5 == 3)
                {
                    CreatureGenerate.CreatureGenerateInfoManager.Instance.GenKit = true;
                }
                else
                {
                    CreatureGenerate.CreatureGenerateInfoManager.Instance.GenKit = false;
                }
            }
            if (CreatureGenerate.CreatureGenerateInfoManager.Instance.GenKit && !CreatureGenerate.CreatureGenerateInfoManager.Instance.CheckKitCreatureRemains())
            {
                CreatureGenerate.CreatureGenerateInfoManager.Instance.GenKit = false;
            }
        }
        [MemberAlias("SelectStartDay", typeof(CreatureSelectUI))]
        public const int SelectStartDay = 0;
        [MemberAlias("SelectEndDay", typeof(CreatureSelectUI))]
        public const int SelectEndDay = 51;
        [MemberAlias("yin", typeof(CreatureSelectUI))]
        public const long yin = 100104L;
        [MemberAlias("yang", typeof(CreatureSelectUI))]
        public const long yang = 300109L;
        [MemberAlias("plagueDoctor", typeof(CreatureSelectUI))]
        public const long plagueDoctor = 100014L;
        [MemberAlias("deathangel", typeof(CreatureSelectUI))]
        public const long deathangel = 100015L;
        [MemberAlias("_skip", typeof(CreatureSelectUI))]
        private bool _skip;
        [MemberAlias("RootObject", typeof(CreatureSelectUI))]
        public GameObject RootObject;
        [MemberAlias("GlobalControlAnim", typeof(CreatureSelectUI))]
        public Animator GlobalControlAnim;
        [MemberAlias("clip", typeof(CreatureSelectUI))]
        public AudioClip clip;
        [MemberAlias("clipSaved", typeof(CreatureSelectUI))]
        private AudioClip clipSaved;
        [MemberAlias("Units", typeof(CreatureSelectUI))]
        public CreatureSelect.CreatureSelectUnit[] Units;
        [MemberAlias("Index_Normal", typeof(CreatureSelectUI))]
        public RectTransform Index_Normal;
        [MemberAlias("Index_Select", typeof(CreatureSelectUI))]
        public RectTransform Index_Select;
        [MemberAlias("Block", typeof(CreatureSelectUI))]
        public global::UIController Block;
        [MemberAlias("TextBoxController", typeof(CreatureSelectUI))]
        public global::UIController TextBoxController;
        [MemberAlias("TextBoxText", typeof(CreatureSelectUI))]
        public Text TextBoxText;
        [MemberAlias("filter", typeof(CreatureSelectUI))]
        public global::CameraFilterPack_TV_80 filter;
        [MemberAlias("reExtractController", typeof(CreatureSelectUI))]
        public global::UIController reExtractController;
        [MemberAlias("_reExtracted", typeof(CreatureSelectUI))]
        private bool _reExtracted;
        [MemberAlias("effectRunned", typeof(CreatureSelectUI))]
        private bool effectRunned;
        [MemberAlias("EffectTimer", typeof(CreatureSelectUI))]
        private global::Timer EffectTimer = new global::Timer();
        [MemberAlias("threshold", typeof(CreatureSelectUI))]
        private int threshold;
        [MemberAlias("_tiperethRunned", typeof(CreatureSelectUI))]
        private bool _tiperethRunned;
        [MemberAlias("FadeoutEffectTimer", typeof(CreatureSelectUI))]
        private Timer FadeoutEffectTimer;
        [MemberAlias("startVolume", typeof(CreatureSelectUI))]
        private float startVolume;
    }

    [ModifiesType("CreatureGenerate.CreatureGenerateInfoManager")]
    public class CreatureGenerateInfoManager_patch
    {
        [NewMember]
        public void OnUsed_Mod(LcIdLong id)
        {
            global::CreatureTypeInfo data = CreatureTypeList_patch.instance.GetData_Mod(id);
            ActivateStateList activateStateList = null;
            if (this.activateStateDic.TryGetValue(data.GetRiskLevel(), out activateStateList))
            {
                ((ActivateStateList_patch)(object)activateStateList).OnUsed_Mod(id);
            }
        }
        [ModifiesMember("RemoveAction")]
        public void RemoveAction_patch(long id)
        {
            CreatureTypeInfo data = CreatureTypeList.instance.GetData(id);
            ActivateStateList activateStateList = null;
            if (this.GetCreatureState(data.GetRiskLevel(), out activateStateList))
            {
                activateStateList.RemoveAction(id);
            }
        }
        [NewMember]
        public List<LcIdLong> GetCreature_Mod()
        {
            CreatureGenerateModel value = null;
            int genDay = get_GenDay();
            if (dayGenInfoDic.TryGetValue(genDay, out value))
            {
                try
                {
                    value.SetCreature();
                }
                catch (CreatureGenerateInfoManager.ProbCheckExeption)
                {
                    genDay = -2;
                    if (dayGenInfoDic.TryGetValue(genDay, out value))
                    {
                        try
                        {
                            value.SetCreature();
                        }
                        catch (CreatureGenerateInfoManager.ProbCheckExeption)
                        {
                            Debug.LogError("Failed To Gen Creature");
                        }
                    }
                }

                return ((CreatureGenerateModel_patch)(object)value).creatureMod;
            }

            return null;
        }
        [ModifiesMember("InitCreatureList")]
        public void InitCreatureList_patch()
        {
            this.CreatureList.Clear();
        }
        [ModifiesMember("Init")]
        public void Init_patch()
        {
            this.InitCreatureList_patch();
            this.activateStateDic.Clear();
            this._isInitiated = true;
            foreach (long id in global::CreatureGenerateInfo.GetAll(false))
            {
                global::CreatureTypeInfo data = global::CreatureTypeList.instance.GetData(id);
                global::RiskLevel riskLevel = data.GetRiskLevel();
                long id2 = data.id;
                bool isUsed = this.IsUsedCreature(id2);
                bool isKit = data.creatureWorkType == global::CreatureWorkType.KIT;
                ActivateStateModel model = new ActivateStateModel
                {
                    riskLevel = riskLevel,
                    id = id2,
                    isUsed = isUsed,
                    isKit = isKit
                };
                ((ActivateStateModel_patch)(object)model).modid = string.Empty;
                ActivateStateList activateStateList = null;
                if (this.activateStateDic.TryGetValue(riskLevel, out activateStateList))
                {
                    activateStateList.Add(model);
                }
                else
                {
                    activateStateList = new ActivateStateList
                    {
                        riskLevel = riskLevel
                    };
                    activateStateList.Add(model);
                    this.activateStateDic.Add(riskLevel, activateStateList);
                }
            }
            foreach (LcIdLong id in CreatureGenerateInfo_patch.GetAll_Mod(false))
            {
                CreatureTypeInfo data = CreatureTypeList_patch.instance.GetData_Mod(id);
                RiskLevel riskLevel = data.GetRiskLevel();
                long id2 = data.id;
                bool isUsed = CreatureManager_patch.instance().IsCreatureActivated_Mod(id);
                bool isKit = data.creatureWorkType == global::CreatureWorkType.KIT;
                ActivateStateModel model = new ActivateStateModel
                {
                    riskLevel = riskLevel,
                    id = id2,
                    isUsed = isUsed,
                    isKit = isKit
                };
                ((ActivateStateModel_patch)(object)model).modid = id.packageId;
                ActivateStateList activateStateList = null;
                if (this.activateStateDic.TryGetValue(riskLevel, out activateStateList))
                {
                    activateStateList.Add(model);
                }
                else
                {
                    activateStateList = new ActivateStateList
                    {
                        riskLevel = riskLevel
                    };
                    activateStateList.Add(model);
                    this.activateStateDic.Add(riskLevel, activateStateList);
                }
            }
            this.CheckCreatureUseState();
            if (this.get_IsloadedDayData())
            {
                CreatureGenerateInfoManager.Log("Loaded", false);
                return;
            }
            CreatureGenerateInfoManager.Log("Load Fail", true);
        }


        [MemberAlias("GetCreatureState", typeof(CreatureGenerateInfoManager))]
        public bool GetCreatureState(RiskLevel risk, out ActivateStateList list)
        {
            return this.activateStateDic.TryGetValue(risk, out list);
        }
        [MemberAlias("get_GenDay", typeof(CreatureGenerateInfoManager))]
        public int get_GenDay()
        {
            return this._genDay;
        }
        [MemberAlias("get_IsloadedDayData", typeof(CreatureGenerateInfoManager))]
        public bool get_IsloadedDayData()
        {
            return false;
        }
        [MemberAlias("CheckCreatureUseState", typeof(CreatureGenerateInfoManager))]
        private void CheckCreatureUseState()
        {
            foreach (ActivateStateList activateStateList in this.activateStateDic.Values)
            {
                activateStateList.DayUpdate();
                activateStateList.CheckUsableState();
            }
        }
        [MemberAlias("IsUsedCreature", typeof(CreatureGenerateInfoManager))]
        private bool IsUsedCreature(long id)
        {
            return global::CreatureManager.instance.IsCreatureActivated(id);
        }
        [MemberAlias("GenerateCommonActionString", typeof(CreatureGenerateInfoManager))]
        public static readonly string[] GenerateCommonActionString;
        [MemberAlias("GenerateCommonActionList", typeof(CreatureGenerateInfoManager))]
        public List<string> GenerateCommonActionList;
        [MemberAlias("activateStateDic", typeof(CreatureGenerateInfoManager))]
        public Dictionary<global::RiskLevel, ActivateStateList> activateStateDic;
        [MemberAlias("dayGenInfoDic", typeof(CreatureGenerateInfoManager))]
        public Dictionary<int, CreatureGenerateModel> dayGenInfoDic;
        [MemberAlias("CreatureList", typeof(CreatureGenerateInfoManager))]
        public Dictionary<global::RiskLevel, List<long>> CreatureList;
        [MemberAlias("SelectData", typeof(CreatureGenerateInfoManager))]
        public Dictionary<int, CreatureSelectData> SelectData;
        [MemberAlias("_isLoadedDayData", typeof(CreatureGenerateInfoManager))]
        private bool _isLoadedDayData;
        [MemberAlias("_isInitiated", typeof(CreatureGenerateInfoManager))]
        private bool _isInitiated;
        [MemberAlias("_genDay", typeof(CreatureGenerateInfoManager))]
        private int _genDay;
        [MemberAlias("_genKit", typeof(CreatureGenerateInfoManager))]
        private bool _genKit;
    }
    [ModifiesType("ChildCreatureModel")]
    public class ChildCreatureModel_patch : IObserver
    {
        [ModifiesMember("OnFixedUpdate")]
        public void OnFixedUpdate()
        {
            if (this.remainMoveDelay > 0f)
            {
                this.remainMoveDelay -= Time.deltaTime;
            }
            if (this.remainAttackDelay > 0f)
            {
                this.remainAttackDelay -= Time.deltaTime;
            }
            this.UpdateBufState();
            this.commandQueue.Execute(this.ForceTypeChange<CreatureModel>());
            if (this.animAutoSet)
            {
                if (this.GetMovableNode().IsMoving())
                {
                    this.SetMoveAnimState(true);
                }
                else
                {
                    this.SetMoveAnimState(false);
                }
            }
            if (this._equipment.weapon != null)
            {
                this._equipment.weapon.OnFixedUpdate();
            }
            if (global::GameManager.currentGameManager.ManageStarted)
            {
                this.script.OnFixedUpdate(this.ForceTypeChange<CreatureModel>());
            }
            if (get_state() == global::CreatureState.ESCAPE)
            {
                this.OnEscapeUpdate();
            }
            else if (get_state() == global::CreatureState.SUPPRESSED)
            {
            }
            if (this.remainMoveDelay > 0f)
            {
                this.movableNode.ProcessMoveNode(0f);
            }
            else
            {
                this.movableNode.ProcessMoveNode(this.Speed);
            }
        }

        [ModifiesMember("SetParent")]
        public void SetParent_patch(global::CreatureModel creature)
        {
            this._parent = creature;
            this.metaInfo = creature.metaInfo.childTypeInfo;
            this.metadataId = creature.metaInfo.childTypeInfo.id;
            if (this.get_childMetaInfo().isHasBaseMeta)
            {
                LcIdLong lcid = new LcIdLong(CreatureTypeList_patch.instance.GetModId(creature.metaInfo), this.get_childMetaInfo().id);
                this.metaInfo = CreatureTypeList_patch.instance.GetData_Mod(lcid);
                if ((this.observeInfo = CreatureManager_patch.instance().GetObserveInfo_Mod(lcid)) == null)
                {
                    this.observeInfo = new CreatureObserveInfoModel(this.get_childMetaInfo().id);
                    ((CreatureObserveInfoModel_patch)(object)this.observeInfo).Init_Mod(lcid);
                }
                if (CreatureTypeList_patch.instance.GetSkillTipData_Mod(lcid) != null)
                {
                    this.metaInfo.specialSkillTable = CreatureTypeList_patch.instance.GetSkillTipData_Mod(lcid).GetCopy();
                }
                global::CreatureManager.instance.AddChildObserveInfo(this.observeInfo);
            }
            this._unit = this.GenCreatureUnit(null);
            this.LoadScript(this.get_childMetaInfo().script);
            Notice.instance.Observe(global::NoticeName.FixedUpdate, this);
            this.sefiraNum = this.get_parent().sefiraNum;
            this.sefira = this.get_parent().sefira;
        }

        [ModifiesMember("SetParent")]
        public void SetParent_patch(global::CreatureModel creature, string childScriptSrc, string childPrefab)
        {
            this._parent = creature;
            this.metaInfo = creature.metaInfo.childTypeInfo;
            if (this.get_childMetaInfo().isHasBaseMeta)
            {
                LcIdLong lcid = new LcIdLong(CreatureTypeList_patch.instance.GetModId(creature.metaInfo), this.get_childMetaInfo().id);
                this.metaInfo = CreatureTypeList_patch.instance.GetData_Mod(lcid);
                this.observeInfo = new global::CreatureObserveInfoModel(this.get_childMetaInfo().id);
                ((CreatureObserveInfoModel_patch)(object)this.observeInfo).Init_Mod(lcid);
                if (CreatureTypeList_patch.instance.GetSkillTipData_Mod(lcid) != null)
                {
                    this.metaInfo.specialSkillTable = CreatureTypeList_patch.instance.GetSkillTipData_Mod(lcid).GetCopy();
                }
            }
            this._unit = this.GenCreatureUnit(childPrefab);
            this.LoadScript(childScriptSrc);
            Notice.instance.Observe(global::NoticeName.FixedUpdate, this);
            this.sefiraNum = this.get_parent().sefiraNum;
            this.sefira = this.get_parent().sefira;
        }

        public void OnNotice(string notice, params object[] param)
        {
        }


        [MemberAlias("UpdateBufState", typeof(UnitModel))]
        public void UpdateBufState()
        {
        }
        [MemberAlias("movableNode", typeof(UnitModel))]
        public MovableObjectNode movableNode;
        [MemberAlias("OnEscapeUpdate", typeof(ChildCreatureModel))]
        public void OnEscapeUpdate()
        {
        }
        [MemberAlias("get_state", typeof(CreatureModel))]
        public global::CreatureState get_state()
        {
            return default(CreatureState);
        }
        [MemberAlias("set_state", typeof(CreatureModel))]
        public void set_state(global::CreatureState value)
        {
        }
        [MemberAlias("script", typeof(CreatureModel))]
        public CreatureBase script;
        [MemberAlias("_equipment", typeof(UnitModel))]
        public UnitEquipSpace _equipment;
        [MemberAlias("SetMoveAnimState", typeof(ChildCreatureModel))]
        public void SetMoveAnimState(bool b)
        {
        }
        [MemberAlias("GetMovableNode", typeof(UnitModel))]
        public MovableObjectNode GetMovableNode()
        {
            return null;
        }

        [MemberAlias("commandQueue", typeof(CreatureModel))]
        public CreatureCommandQueue commandQueue;
        [MemberAlias("remainAttackDelay", typeof(UnitModel))]
        public float remainAttackDelay;
        [MemberAlias("remainMoveDelay", typeof(UnitModel))]
        public float remainMoveDelay;
        [MemberAlias("LoadScript", typeof(ChildCreatureModel))]
        private void LoadScript(string src)
        {
        }
        [MemberAlias("GenCreatureUnit", typeof(ChildCreatureModel))]
        public ChildCreatureUnit GenCreatureUnit(string prefabSrc = null)
        {
            return null;
        }
        [MemberAlias("get_parent", typeof(ChildCreatureModel))]
        public global::CreatureModel get_parent()
        {
            return this._parent;
        }
        [MemberAlias("get_childMetaInfo", typeof(ChildCreatureModel))]
        public global::ChildCreatureTypeInfo get_childMetaInfo()
        {
            return null;
        }

        [MemberAlias("_parent", typeof(ChildCreatureModel))]
        private global::CreatureModel _parent;
        [MemberAlias("_unit", typeof(ChildCreatureModel))]
        private new global::ChildCreatureUnit _unit;
        [MemberAlias("destroied", typeof(ChildCreatureModel))]
        public bool destroied;
        [MemberAlias("activateState", typeof(ChildCreatureModel))]
        public bool activateState = true;
        [MemberAlias("animAutoSet", typeof(ChildCreatureModel))]
        public bool animAutoSet = true;
        [MemberAlias("RiskLevel", typeof(ChildCreatureModel))]
        public global::RiskLevel RiskLevel = global::RiskLevel.HE;
        [MemberAlias("PortraitSrc", typeof(ChildCreatureModel))]
        public string PortraitSrc = string.Empty;
        [MemberAlias("Speed", typeof(ChildCreatureModel))]
        private float Speed;


        [MemberAlias("sefira", typeof(CreatureModel))]
        public Sefira sefira;
        [MemberAlias("sefiraNum", typeof(CreatureModel))]
        public string sefiraNum;
        [MemberAlias("observeInfo", typeof(CreatureModel))]
        public CreatureObserveInfoModel observeInfo;
        [MemberAlias("metadataId", typeof(CreatureModel))]
        public long metadataId;
        [MemberAlias("metaInfo", typeof(CreatureModel))]
        public global::CreatureTypeInfo metaInfo;
    }
    [ModifiesType("CreatureUnit")]
    public class CreatureUnit_patch
    {
        [ModifiesMember("OnClickCollectionFunc")]
        public void OnClickCollectionFunc_patch()
        {
            if (!this.model.script.OnOpenCollectionWindow())
            {
                return;
            }
            CreatureInfoWindow_patch.CreateWindow_Mod(CreatureTypeList_patch.instance.GetLcId(this.model.metaInfo));
        }

        [MemberAlias("model", typeof(CreatureUnit))]
        public global::CreatureModel model;
    }
    [ModifiesType("CreatureInfoWindow")]
    public class CreatureInfoWindow_patch
    {
        [ModifiesMember("OpenCodexCreatureInfo")]
        public void OpenCodexCreatureInfo(global::CreatureTypeInfo metaInfo)
        {
            OpenCodexCreatureInfo_Mod(CreatureTypeList_patch.instance.GetLcId(metaInfo));
        }
        [ModifiesMember("OpenCodexCreatureInfo")]
        public void OpenCodexCreatureInfo_patch(long metaId)
        {
            OpenCodexCreatureInfo_Mod(new LcIdLong(metaId));
        }
        [NewMember]
        public static global::CreatureInfoWindow CreateWindow_Mod(LcIdLong metaId)
        {
            CreatureInfoWindow_patch.get_CurrentWindow().set_IsCodex(false);
            global::CreatureInfoWindow.CurrentWindow.SetWindowType(false);
            CreatureInfoWindow_patch.get_CurrentWindow().CurrentMetaIdMod = metaId;
            CreatureInfoWindow_patch.get_CurrentWindow().set_IsEnabled(true);
            CreatureInfoWindow_patch.get_CurrentWindow().OnChangeCreature();
            try
            {
                global::CreatureInfoWindow.CurrentWindow.InfoCodexArrowRoot.SetActive(false);
            }
            catch (Exception ex)
            {
            }
            return global::CreatureInfoWindow.CurrentWindow;
        }
        [ModifiesMember("CreateWindow")]
        public static CreatureInfoWindow CreateWindow_patch(long metaId)
        {
            return CreateWindow_Mod(new LcIdLong(metaId));
        }
        [ModifiesMember("CreateCodexWindow")]
        public static CreatureInfoWindow CreateCodexWindow_patch()
        {
            CreatureInfoWindow_patch.get_CurrentWindow().set_IsCodex(true);
            CreatureInfoWindow.CurrentWindow.SetWindowType(true);
            CreatureInfoWindow_patch.get_CurrentWindow().CurrentMetaIdMod = new LcIdLong(-1L);
            CreatureInfoWindow_patch.get_CurrentWindow().set_IsEnabled(true);
            CreatureInfoWindow.CurrentWindow.codex.OnOpen();
            try
            {
                global::CreatureInfoWindow.CurrentWindow.InfoCodexArrowRoot.SetActive(false);
            }
            catch (Exception ex)
            {
            }
            return global::CreatureInfoWindow.CurrentWindow;
        }
        [ModifiesMember("CloseWindow")]
        public void CloseWindow_patch()
        {
            if (this.get_IsCodex() && !this.codex._activeControl.activeInHierarchy)
            {
                this.SetWindowType(true);
                this.InfoCodexArrowRoot.SetActive(false);
                this.codex.OnMetaClose();
                this.codex.OnOpen();
                this.CurrentMetaIdMod = new LcIdLong(-1L);
                return;
            }
            this.CurrentMetaIdMod = new LcIdLong(-1L);
            if (global::AlterTitleController.Controller)
            {
                global::AlterTitleController.Controller.InitEffect();
            }
            this.set_IsEnabled(false);
        }
        [ModifiesMember("Awake")]
        private void Awake_patch()
        {
            _currentWindow = this;
            this.CurrentMetaIdMod = new LcIdLong(-1L);
        }
        [ModifiesMember("IsCurrentMetaNull")]
        public static bool IsCurrentMetaNull_patch()
        {
            return CreatureInfoWindow_patch.get_CurrentWindow().CurrentMetaIdMod == -1L;
        }
        [NewMember]
        public void OpenCodexCreatureInfo_Mod(LcIdLong metaId)
        {
            this.InfoCodexArrowRoot.SetActive(true);
            this.SetWindowType(false);
            this.CurrentMetaIdMod = metaId;
            this.OnChangeCreature();
        }

        [NewMember]
        public LcIdLong CurrentMetaIdMod
        {
            get
            {
                return this._currentCreatureMetaIdMod;
            }
            private set
            {
                this._currentCreatureMetaIdMod = value;
                if (value == -1L)
                {
                    this._metaInfo = null;
                    this._observeInfo = null;
                    this._currentModel = null;
                }
                else
                {
                    this._metaInfo = CreatureTypeList_patch.instance.GetData_Mod(value);
                    this._observeInfo = CreatureManager_patch.instance().GetObserveInfo_Mod(value);
                    this._currentModel = CreatureManager_patch.instance().FindCreature_Mod(value);
                }
            }
        }






        [MemberAlias("get_IsEnabled", typeof(CreatureInfoWindow))]
        public bool get_IsEnabled()
        {
            return this._isEnabled;
        }
        [MemberAlias("set_IsEnabled", typeof(CreatureInfoWindow))]
        private void set_IsEnabled(bool value)
        {
        }
        [MemberAlias("get_IsCodex", typeof(CreatureInfoWindow))]
        public bool get_IsCodex()
        {
            return false;
        }
        [MemberAlias("set_IsCodex", typeof(CreatureInfoWindow))]
        private void set_IsCodex(bool value)
        {
        }
        [MemberAlias("OnChangeCreature", typeof(CreatureInfoWindow))]
        private void OnChangeCreature()
        {
        }
        [MemberAlias("SetWindowType", typeof(CreatureInfoWindow))]
        public void SetWindowType(bool isCodex)
        {
            try
            {
                this.TitleText.gameObject.SetActive(!isCodex);
            }
            catch (Exception ex)
            {
            }
            try
            {
                this.normalCreatureArea.SetActive(!isCodex);
            }
            catch (Exception ex2)
            {
            }
            try
            {
                this.kitCreatureArea.SetActive(!isCodex);
            }
            catch (Exception ex3)
            {
            }
            try
            {
                this.codex._activeControl.SetActive(isCodex);
                this.CodexFrame.color = ((!isCodex) ? this.BrightColor : this.RedColor);
            }
            catch (Exception ex4)
            {
            }
        }









        [NewMember]
        [NonSerialized]
        public LcIdLong _currentCreatureMetaIdMod;


        [MemberAlias("get_CurrentWindow", typeof(CreatureInfoWindow))]
        public static CreatureInfoWindow_patch get_CurrentWindow()
        {
            return null;
        }

        [MemberAlias("_currentWindow", typeof(CreatureInfoWindow))]
        private static CreatureInfoWindow_patch _currentWindow;
        [MemberAlias("_metaInfo", typeof(CreatureInfoWindow))]
        private CreatureTypeInfo _metaInfo;
        [MemberAlias("_observeInfo", typeof(CreatureInfoWindow))]
        private CreatureObserveInfoModel _observeInfo;
        [MemberAlias("_currentModel", typeof(CreatureInfoWindow))]
        [NonSerialized]
        private CreatureModel _currentModel;
        [MemberAlias("_isEnabled", typeof(CreatureInfoWindow))]
        private bool _isEnabled;
        [MemberAlias("RootCanvas", typeof(CreatureInfoWindow))]
        public Canvas RootCanvas;
        [MemberAlias("_UI", typeof(CreatureInfoWindow))]
        [SerializeField]
        private global::CreatureInfoWindow.UI _UI;
        [MemberAlias("normalCreatureArea", typeof(CreatureInfoWindow))]
        public GameObject normalCreatureArea;
        [MemberAlias("kitCreatureArea", typeof(CreatureInfoWindow))]
        public GameObject kitCreatureArea;
        [MemberAlias("audioClipPlayer", typeof(CreatureInfoWindow))]
        public global::AudioClipPlayer audioClipPlayer;
        [MemberAlias("InfoButton", typeof(CreatureInfoWindow))]
        public Button InfoButton;
        [MemberAlias("DescButton", typeof(CreatureInfoWindow))]
        public Button DescButton;
        [MemberAlias("CurrentCumlativeCube_Cost", typeof(CreatureInfoWindow))]
        public Text CurrentCumlativeCube_Cost;
        [MemberAlias("WindowAnimCTRL", typeof(CreatureInfoWindow))]
        public global::UIController WindowAnimCTRL;
        [MemberAlias("CurrentPayedCost", typeof(CreatureInfoWindow))]
        public Text CurrentPayedCost;
        [MemberAlias("PaymentAnimCTRL", typeof(CreatureInfoWindow))]
        public Animator PaymentAnimCTRL;
        [MemberAlias("observeLevelSlot", typeof(CreatureInfoWindow))]
        public List<CreatureInfo.CreatureInfoObserveLevelEffectSlot> observeLevelSlot;
        [MemberAlias("CurrentUsableCost", typeof(CreatureInfoWindow))]
        public Text CurrentUsableCost;
        [MemberAlias("ObserveLevelImage", typeof(CreatureInfoWindow))]
        public Image ObserveLevelImage;
        [MemberAlias("ChallangeModeAsterisk", typeof(CreatureInfoWindow))]
        public Text ChallangeModeAsterisk;
        [MemberAlias("ChallangeModeText", typeof(CreatureInfoWindow))]
        public Text ChallangeModeText;
        [MemberAlias("statRoot", typeof(CreatureInfoWindow))]
        public CreatureInfo.CreatureInfoStatRoot statRoot;
        [MemberAlias("workRoot", typeof(CreatureInfoWindow))]
        public CreatureInfo.CreatureInfoWorkRoot workRoot;
        [MemberAlias("escapeRoot", typeof(CreatureInfoWindow))]
        public CreatureInfo.CreatureInfoEscapeRoot escapeRoot;
        [MemberAlias("caretakingRoot", typeof(CreatureInfoWindow))]
        public CreatureInfo.CreatureInfoCaretakingRoot caretakingRoot;
        [MemberAlias("equipmentRoot", typeof(CreatureInfoWindow))]
        public CreatureInfo.CreatureInfoEquipmentRoot equipmentRoot;
        [MemberAlias("workSlots", typeof(CreatureInfoWindow))]
        public List<CreatureInfo.CreatureInfoWorkSlot> workSlots;
        [MemberAlias("kitStatRoot", typeof(CreatureInfoWindow))]
        public CreatureInfo.CreatureInfoKitStatRoot kitStatRoot;
        [MemberAlias("kitLayerController", typeof(CreatureInfoWindow))]
        public CreatureInfo.CreatureInfoKitLayoutController kitLayerController;
        [MemberAlias("kitObserveLevelSlot", typeof(CreatureInfoWindow))]
        public List<CreatureInfo.CreatureInfoKitObserveLevelEffectSlot> kitObserveLevelSlot;
        [MemberAlias("kitObserveLevelText", typeof(CreatureInfoWindow))]
        public Text kitObserveLevelText;
        [MemberAlias("DisabledCubeImage", typeof(CreatureInfoWindow))]
        public Sprite DisabledCubeImage;
        [MemberAlias("EnabledCubeImage", typeof(CreatureInfoWindow))]
        public Sprite EnabledCubeImage;
        [MemberAlias("DisabledTextColor", typeof(CreatureInfoWindow))]
        public Color DisabledTextColor;
        [MemberAlias("EnabledTextColor", typeof(CreatureInfoWindow))]
        public Color EnabledTextColor;
        [MemberAlias("RedColor", typeof(CreatureInfoWindow))]
        public Color RedColor;
        [MemberAlias("BrightColor", typeof(CreatureInfoWindow))]
        public Color BrightColor;
        [MemberAlias("OrangeColor", typeof(CreatureInfoWindow))]
        public Color OrangeColor;
        [MemberAlias("ObserveLevelSprite", typeof(CreatureInfoWindow))]
        public Sprite[] ObserveLevelSprite;
        [MemberAlias("DescriptionPanel", typeof(CreatureInfoWindow))]
        public RectTransform DescriptionPanel;
        [MemberAlias("InfoPanel", typeof(CreatureInfoWindow))]
        public GameObject InfoPanel;
        [MemberAlias("listParent", typeof(CreatureInfoWindow))]
        [Header("Description")]
        public RectTransform listParent;
        [MemberAlias("descUnit", typeof(CreatureInfoWindow))]
        public GameObject descUnit;
        [MemberAlias("Spacing", typeof(CreatureInfoWindow))]
        public float Spacing = 50f;
        [MemberAlias("LowerSpacing", typeof(CreatureInfoWindow))]
        public float LowerSpacing = 200f;
        [MemberAlias("codex", typeof(CreatureInfoWindow))]
        public CreatureInfo.CreatureInfoCodex codex;
        [MemberAlias("CodexFrame", typeof(CreatureInfoWindow))]
        public Image CodexFrame;
        [MemberAlias("InfoCodexArrowRoot", typeof(CreatureInfoWindow))]
        public GameObject InfoCodexArrowRoot;
        [MemberAlias("PrevCodex", typeof(CreatureInfoWindow))]
        public Button PrevCodex;
        [MemberAlias("NextCodex", typeof(CreatureInfoWindow))]
        public Button NextCodex;
        [MemberAlias("TitleText", typeof(CreatureInfoWindow))]
        public Text TitleText;
        [MemberAlias("descList", typeof(CreatureInfoWindow))]
        [NonSerialized]
        public List<Text> descList = new List<Text>();
        [MemberAlias("_controllers", typeof(CreatureInfoWindow))]
        private List<global::CreatureInfoController> _controllers = new List<global::CreatureInfoController>();
        [MemberAlias("_costTable", typeof(CreatureInfoWindow))]
        private Dictionary<global::CreatureInfoController, int> _costTable = new Dictionary<global::CreatureInfoController, int>();
        [MemberAlias("_oldLevel", typeof(CreatureInfoWindow))]
        private int _oldLevel = -1;
    }
    [ModifiesType("CreatureInfo.CreatureInfoCodex")]
    public class CreatureInfoCodex_patch
    {

        [ModifiesMember("MovePrev")]
        public void MovePrev_patch()
        {
            int num = this.GetCurrentDisplayedIndex_patch();
            if (num == 0)
            {
                return;
            }
            LcIdLong metaId = this.displayList_Mod[--num];
            while (!this.CheckIdValidation_Mod(metaId))
            {
                try
                {
                    metaId = this.displayList_Mod[--num];
                }
                catch (Exception ex)
                {
                    return;
                }
            }
            CreatureInfoWindow_patch.get_CurrentWindow().OpenCodexCreatureInfo_Mod(metaId);
            CreatureInfoWindow.CurrentWindow.audioClipPlayer.OnPlayInList(3);
            this.UpdateArrow_patch(num);
        }
        [NewMember]
        public bool CheckIdValidation_Mod(LcIdLong metaId)
        {
            return metaId != uniqueId[2];
        }
        [ModifiesMember("MoveNext")]
        public void MoveNext_patch()
        {
            int num = this.GetCurrentDisplayedIndex_patch();
            if (num == this.displayList_Mod.Count - 1)
            {
                return;
            }
            LcIdLong metaId = this.displayList_Mod[++num];
            while (!this.CheckIdValidation_Mod(metaId))
            {
                try
                {
                    metaId = this.displayList_Mod[++num];
                }
                catch (Exception ex)
                {
                    return;
                }
            }
            CreatureInfoWindow_patch.get_CurrentWindow().OpenCodexCreatureInfo_Mod(metaId);
            CreatureInfoWindow.CurrentWindow.audioClipPlayer.OnPlayInList(3);
            this.UpdateArrow_patch(num);
        }
        [ModifiesMember("GetCurrentDisplayedIndex")]
        private int GetCurrentDisplayedIndex_patch()
        {
            LcIdLong currentMetaId = ((CreatureInfoWindow_patch)(object)CreatureInfoWindow.CurrentWindow).CurrentMetaIdMod;
            if (currentMetaId == -1L)
            {
                return -1;
            }
            if (!this.displayList_Mod.Contains(currentMetaId))
            {
                return -1;
            }
            return this.displayList_Mod.IndexOf(currentMetaId);
        }
        [ModifiesMember("UpdateArrow")]
        private void UpdateArrow_patch(int current)
        {
            CreatureInfoWindow.CurrentWindow.PrevCodex.interactable = true;
            CreatureInfoWindow.CurrentWindow.NextCodex.interactable = true;
            if (current == 0)
            {
                CreatureInfoWindow.CurrentWindow.PrevCodex.interactable = false;
            }
            else if (current == this.displayList_Mod.Count - 1)
            {
                CreatureInfoWindow.CurrentWindow.NextCodex.interactable = false;
            }
        }
        [ModifiesMember("SetList")]
        public void SetList_patch(int index)
        {
            if (index < 0)
            {
                return;
            }
            if (index > this.maxDisplayIndex)
            {
                return;
            }
            this.MoveEnabledToDisable();
            this.currentDisplayIndex = index;
            int num = this.currentDisplayIndex * 15;
            int num2 = num + 15;
            for (int i = num; i < num2; i++)
            {
                if (i < this.displayList_Mod.Count)
                {
                    GameObject gameObject = this.slotDic_Mod[this.displayList_Mod[i]];
                    gameObject.transform.SetParent(this._layout);
                }
            }
            this.UpperArrow.interactable = true;
            this.LowerArrow.interactable = true;
            if (index == 0)
            {
                this.UpperArrow.interactable = false;
            }
            if (index == this.maxDisplayIndex)
            {
                this.LowerArrow.interactable = false;
            }
        }
        [ModifiesMember("Clear")]
        private void Clear_patch()
        {
            foreach (GameObject gameObject in this.slotDic_Mod.Values)
            {
                UnityEngine.Object.Destroy(gameObject.gameObject);
            }
            this.slotDic_Mod.Clear();
        }
        [NewMember]
        private bool CheckUniqueGeneration_Mod(LcIdLong id, List<CreatureInfoCodex_SortData_Mod> list, int currentIndex, out int changedIndex)
        {
            changedIndex = currentIndex;
            if (id == uniqueId[0])
            {
                return false;
            }
            if (id == uniqueId[3])
            {
                return false;
            }
            if (id == uniqueId[4])
            {
                CreatureTypeInfo data = global::CreatureTypeList.instance.GetData(uniqueId[3]);
                CreatureObserveInfoModel observeInfo = global::CreatureManager.instance.GetObserveInfo(uniqueId[3]);
                if (!observeInfo.IsMaxObserved())
                {
                    observeInfo.ObserveAll(new string[0]);
                }
                this.GenerateSlot_Mod(list, observeInfo, data, currentIndex);
                changedIndex = currentIndex + 1;
            }
            return true;
        }
        [NewMember]
        private void GenerateSlot_Mod(List<CreatureInfoCodex_SortData_Mod> sort, global::CreatureObserveInfoModel info, global::CreatureTypeInfo typeInfo, int index)
        {
            LcIdLong lcid = new LcIdLong(CreatureTypeList_patch.instance.GetModId(typeInfo), typeInfo.id);
            CreatureInfoCodex_SortData_Mod item = new CreatureInfoCodex_SortData_Mod
            {
                index = index,
                id = lcid
            };
            sort.Add(item);
            GameObject gameObject;
            if (lcid.packageId == String.Empty && global::CreatureGenerateInfo.IsCreditCreature(typeInfo.id))
            {
                gameObject = UnityEngine.Object.Instantiate<GameObject>(this._creditSlot);
            }
            else
            {
                gameObject = UnityEngine.Object.Instantiate<GameObject>(this._slot);
            }
            gameObject.transform.SetParent(this._disabledLayout);
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = Vector3.zero;
            this.slotDic_Mod.Add(((CreatureObserveInfoModel_patch)(object)info).lcid, gameObject);
            CreatureInfoCodexSlot component = gameObject.GetComponent<CreatureInfoCodexSlot>();
            component.Init(typeInfo, info);
        }
        [ModifiesMember("Init")]
        public void Init_patch()
        {
            this.Clear_patch();
            List<CreatureObserveInfoModel> observeInfoList = CreatureManager.instance.GetObserveInfoList();
            List<CreatureObserveInfoModel> list = new List<CreatureObserveInfoModel>();
            foreach (CreatureObserveInfoModel creatureObserveInfoModel in observeInfoList)
            {
                if (creatureObserveInfoModel.creatureTypeId <= 200000L || creatureObserveInfoModel.creatureTypeId >= 300000L)
                {
                    if (creatureObserveInfoModel.creatureTypeId <= 400000L)
                    {
                        if (creatureObserveInfoModel.IsMaxObserved())
                        {
                            list.Add(creatureObserveInfoModel);
                        }
                    }
                }
            }
            List<CreatureInfoCodex_SortData_Mod> list2 = new List<CreatureInfoCodex_SortData_Mod>();
            foreach (CreatureObserveInfoModel creatureObserveInfoModel2 in list)
            {
                CreatureObserveInfoModel_patch Minfomodel = ((CreatureObserveInfoModel_patch)(object)creatureObserveInfoModel2);
                CreatureTypeInfo data = CreatureTypeList_patch.instance.GetData_Mod(Minfomodel.lcid);
                string codeId = data.codeId;
                int num = -1;
                if (!this.TryParse(codeId, out num))
                {
                    if (Minfomodel.lcid.packageId != String.Empty || creatureObserveInfoModel2.creatureTypeId != uniqueId[1])
                    {
                        continue;
                    }
                    num = 1000;
                }
                int num2 = num;
                if (Minfomodel.lcid.packageId != String.Empty)
                {
                    this.GenerateSlot_Mod(list2, creatureObserveInfoModel2, data, num);
                    continue;
                }
                if (this.CheckUniqueGeneration_Mod(Minfomodel.lcid, list2, num, out num2))
                {
                    num = num2;
                    this.GenerateSlot_Mod(list2, creatureObserveInfoModel2, data, num);
                }
            }
            List<CreatureInfoCodex_SortData_Mod> list3 = list2;

            Comparison<CreatureInfoCodex_SortData_Mod> comp = new Comparison<CreatureInfoCodex_SortData_Mod>(CreatureInfoCodex_SortData_Mod.Compare);

            list3.Sort(comp);
            this.displayList_Mod.Clear();
            foreach (CreatureInfoCodex_SortData_Mod sortData in list2)
            {
                this.displayList_Mod.Add(sortData.id);
            }
            int num3 = this.displayList_Mod.Count % 15;
            this.maxDisplayIndex = this.displayList_Mod.Count / 15;
            if (num3 == 0)
            {
                this.maxDisplayIndex--;
            }
            this.SetList_patch(0);
            this.allocateFilters.Clear();
            this._dontouchmeCount = 0;
            this.SetPercentage();
        }
        [ModifiesMember(".ctor")]
        public void Ctor()
        {
            ScrollDelay = 0.5f;
            slotDic_Mod = new Dictionary<LcIdLong, GameObject>();
            displayList_Mod = new List<LcIdLong>();
            allocateFilters = new List<MonoBehaviour>();
        }






        [MemberAlias("MoveEnabledToDisable", typeof(CreatureInfoCodex))]
        private void MoveEnabledToDisable()
        {
            List<Transform> list = new List<Transform>();
            IEnumerator enumerator = this._layout.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object obj = enumerator.Current;
                    Transform transform = (Transform)obj;
                    if (!(transform == this._layout))
                    {
                        list.Add(transform);
                    }
                }
            }
            finally
            {
                IDisposable disposable;
                if ((disposable = (enumerator as IDisposable)) != null)
                {
                    disposable.Dispose();
                }
            }
            foreach (Transform transform2 in list)
            {
                transform2.SetParent(this._disabledLayout);
            }
        }
        [MemberAlias("SetPercentage", typeof(CreatureInfoCodex))]
        public void SetPercentage()
        {
            int maxHiddenProgressByObserveLevel = CreatureManager.instance.GetMaxHiddenProgressByObserveLevel();
            int hiddenProgressByObserveLevel = CreatureManager.instance.GetHiddenProgressByObserveLevel();
            int num = 100 * hiddenProgressByObserveLevel / maxHiddenProgressByObserveLevel;
            this.Observation_Percent.text = string.Format("{0}%", num);
        }
        [MemberAlias("TryParse", typeof(CreatureInfoCodex))]
        private bool TryParse(string code, out int index)
        {
            string[] array = code.Split(new char[]
            {
        '-'
            });
            string s = array[array.Length - 1];
            return int.TryParse(s, out index);
        }





        [NewMember]
        [NonSerialized]
        private Dictionary<LcIdLong, GameObject> slotDic_Mod = new Dictionary<LcIdLong, GameObject>();
        [NewMember]
        [NonSerialized]
        private List<LcIdLong> displayList_Mod;




        [MemberAlias("_activeControl", typeof(CreatureInfoCodex))]
        public GameObject _activeControl;
        [MemberAlias("displayCount", typeof(CreatureInfoCodex))]
        private const int displayCount = 15;
        [MemberAlias("riskColor", typeof(CreatureInfoCodex))]
        public Color[] riskColor;
        [MemberAlias("_slot", typeof(CreatureInfoCodex))]
        public GameObject _slot;
        [MemberAlias("_creditSlot", typeof(CreatureInfoCodex))]
        public GameObject _creditSlot;
        [MemberAlias("_layout", typeof(CreatureInfoCodex))]
        public RectTransform _layout;
        [MemberAlias("_disabledLayout", typeof(CreatureInfoCodex))]
        public RectTransform _disabledLayout;
        [MemberAlias("UpperArrow", typeof(CreatureInfoCodex))]
        public Button UpperArrow;
        [MemberAlias("LowerArrow", typeof(CreatureInfoCodex))]
        public Button LowerArrow;
        [MemberAlias("ScrollDelay", typeof(CreatureInfoCodex))]
        public float ScrollDelay;
        [MemberAlias("SimpleStat", typeof(CreatureInfoCodex))]
        public GameObject SimpleStat;
        [MemberAlias("Name", typeof(CreatureInfoCodex))]
        public Text Name;
        [MemberAlias("Code", typeof(CreatureInfoCodex))]
        public Text Code;
        [MemberAlias("Risk", typeof(CreatureInfoCodex))]
        public Text Risk;
        [MemberAlias("Portrait", typeof(CreatureInfoCodex))]
        public Image Portrait;
        [MemberAlias("Observation_Title", typeof(CreatureInfoCodex))]
        public Text Observation_Title;
        [MemberAlias("Observation_Percent", typeof(CreatureInfoCodex))]
        public Text Observation_Percent;
        [MemberAlias("uniqueId", typeof(CreatureInfoCodex))]
        private static long[] uniqueId;
        [MemberAlias("div", typeof(CreatureInfoCodex))]
        private const char div = '-';
        [MemberAlias("currentDisplayIndex", typeof(CreatureInfoCodex))]
        private int currentDisplayIndex;
        [MemberAlias("maxDisplayIndex", typeof(CreatureInfoCodex))]
        private int maxDisplayIndex;
        //[MemberAlias("slotDic", typeof(CreatureInfoCodex))]
        //private Dictionary<long, GameObject> slotDic = new Dictionary<long, GameObject>();
        [MemberAlias("_scrollElap", typeof(CreatureInfoCodex))]
        private float _scrollElap;
        //[MemberAlias("displayList", typeof(CreatureInfoCodex))]
        //private List<long> displayList = new List<long>();
        [MemberAlias("_dontouchmeCount", typeof(CreatureInfoCodex))]
        private int _dontouchmeCount;
        [MemberAlias("allocateFilters", typeof(CreatureInfoCodex))]
        private List<MonoBehaviour> allocateFilters = new List<MonoBehaviour>();
    }
   [ModifiesType("ConsoleScript")]
    public class ConsoleScript_patch
    {
        [NewMember]
        public bool AgentCommand_Mod(string text, string[] array)
        {
            ModDebug.Log("BaseMod Console Command Enter");
            ModDebug.Log("text - " + text);
            string m = "_mod";
            if (text == ConsoleCommand.GiftAdd + m)
            {
                ModDebug.Log("Add Gift");
                if (array.Length == 4)
                {
                    ConsoleCommand_Mod.AddGift(long.Parse(array[2]), int.Parse(array[3]));
                }
                else if (array.Length == 5)
                {
                    ConsoleCommand_Mod.AddGift_Mod(long.Parse(array[2]), new LcId(array[3], int.Parse(array[4])));
                }
                return true;
            }
            if (text == ConsoleCommand.GiftAdd)
            {
                ModDebug.Log("Add Gift");
                if (array.Length == 5)
                {
                    ConsoleCommand_Mod.AddGift_Mod(long.Parse(array[2]), new LcId(array[3], int.Parse(array[4])));
                    return true;
                }
            }
            if (text == ConsoleCommand.GiftRemove + m)
            {
                if (array.Length == 4)
                {
                    ConsoleCommand_Mod.RemoveGift_Mod(long.Parse(array[2]),new LcId(int.Parse(array[3])));
                }
                else if (array.Length == 5)
                {
                    ConsoleCommand_Mod.RemoveGift_Mod(long.Parse(array[2]), new LcId(array[3], int.Parse(array[4])));
                }
                return true;
            }
            if (text == ConsoleCommand.GiftRemove)
            {
                if (array.Length == 5)
                {
                    ConsoleCommand_Mod.RemoveGift_Mod(long.Parse(array[2]), new LcId(array[3], int.Parse(array[4])));
                }
                return true;
            }
            return false;
        }
        [NewMember]
        public bool StandardCommand_Mod(string text, string[] array)
        {
            ModDebug.Log("BaseMod Console Command Enter");
            ModDebug.Log("text - " + text);
            string m = "_mod";
            if (text == ConsoleCommand.WaitingCreature + m)
            {
                ModDebug.Log("Enter AddWaitingGenCreature");
                if (array.Length == 3)
                {
                    ConsoleCommand_Mod.AddWaitingGenCreature(String.Empty, int.Parse(array[2]));
                }
                else
                {
                    ConsoleCommand_Mod.AddWaitingGenCreature(array[2], int.Parse(array[3]));
                }
                return true;
            }
            if (text == ConsoleCommand.WaitingCreature)
            {
                ModDebug.Log("Enter AddWaitingGenCreature");
                if (array.Length == 4)
                {
                    ConsoleCommand_Mod.AddWaitingGenCreature(array[2], int.Parse(array[3]));
                    return true;
                }
              
            }
            if (text == ConsoleCommand.MakeEquipment + m)
            {
                ModDebug.Log("Enter GenerateEquipment");
                if (array.Length == 3)
                {
                    ConsoleCommand_Mod.GenerateEquipment(String.Empty, int.Parse(array[2]));
                }
                else
                {
                    ConsoleCommand_Mod.GenerateEquipment(array[2], int.Parse(array[3]));
                }
                return true;
            }
            if (text == ConsoleCommand.MakeEquipment)
            {
                ModDebug.Log("Enter GenerateEquipment");
                if (array.Length == 4)
                {
                    ConsoleCommand_Mod.GenerateEquipment(array[2], int.Parse(array[3]));
                    return true;
                }
               
            }
            return false;
        }
        [ModifiesMember("OnExitEdit")]
        public void OnExitEdit_patch(string command)
        {
            if (!this.consoleActivated)
            {
                return;
            }
            try
            {
                command = this.GetHmmCommand(command);
                ModDebug.Log("Console Command : " + command);
                this.consoleActivated = false;
                this.ConsoleWnd.gameObject.SetActive(false);
                if (this.angelaLogEnter)
                {
                    global::ConsoleCommand.instance.StandardCommandOperation(2, new object[]
                    {
                    command
                    });
                }
                else if (this.systemLogEnter)
                {
                    global::ConsoleCommand.instance.StandardCommandOperation(0, new object[]
                    {
                    command
                    });
                }
                else
                {
                    char[] separator = new char[]
                    {
                    ' '
                    };
                    string[] array = command.Split(separator);
                    string a = array[0].ToLower();
                    string text = array[1].ToLower();





                    if (a == global::ConsoleCommand.RootCommand)
                    {
                        int num = global::ConsoleCommand.instance.rootCommand.IndexOf(text);
                        if (num != -1)
                        {
                            global::ConsoleCommand.instance.RootCommandOperation(num, new object[]
                            {
                            array[2]
                            });
                        }
                    }
                    else if (a == global::ConsoleCommand.StandardCommand)
                    {
                        if (StandardCommand_Mod(text, array))
                        {
                            return;
                        }

                        int num2 = global::ConsoleCommand.instance.standardCommand.IndexOf(text);
                        if (num2 != -1)
                        {
                            switch (num2)
                            {
                                case 0:
                                    this.systemLogEnter = !this.systemLogEnter;
                                    if (this.systemLogEnter)
                                    {
                                        Debug.Log("SystemLog Enter");
                                    }
                                    else
                                    {
                                        Debug.Log("SystemLog Exit");
                                    }
                                    break;
                                case 1:
                                    {
                                        float num3 = float.Parse(array[2]);
                                        Debug.Log("AddEnergy + " + num3);
                                        global::ConsoleCommand.instance.StandardCommandOperation(1, new object[]
                                        {
                                num3
                                        });
                                        break;
                                    }
                                case 2:
                                    this.angelaLogEnter = !this.angelaLogEnter;
                                    if (this.angelaLogEnter)
                                    {
                                        Debug.Log("Angela Enter");
                                    }
                                    else
                                    {
                                        Debug.Log("Angela Exit");
                                    }
                                    break;
                                case 3:
                                case 4:
                                case 6:
                                    global::ConsoleCommand.instance.StandardCommandOperation(num2, new object[]
                                    {
                                array[2]
                                    });
                                    break;
                                case 5:
                                    global::ConsoleCommand.instance.StandardCommandOperation(5, new object[0]);
                                    break;
                                case 7:
                                    global::ConsoleCommand.instance.StandardCommandOperation(num2, new object[]
                                    {
                                array[2],
                                array[3],
                                array[4]
                                    });
                                    break;
                                case 8:
                                    global::ConsoleCommand.instance.StandardCommandOperation(num2, new object[]
                                    {
                                array[2]
                                    });
                                    break;
                                case 9:
                                    global::ConsoleCommand.instance.StandardCommandOperation(num2, new object[]
                                    {
                                array[2]
                                    });
                                    break;
                                case 10:
                                    global::ConsoleCommand.instance.StandardCommandOperation(10, new object[0]);
                                    break;
                                case 11:
                                case 12:
                                    global::ConsoleCommand.instance.StandardCommandOperation(num2, new object[]
                                    {
                                array[2]
                                    });
                                    break;
                                case 13:
                                    {
                                        string text2 = string.Empty;
                                        if (array.Length <= 2)
                                        {
                                            global::ConsoleCommand.instance.StandardCommandOperation(num2, new object[0]);
                                        }
                                        else
                                        {
                                            for (int i = 2; i < array.Length; i++)
                                            {
                                                text2 = text2 + array[i] + " ";
                                            }
                                            global::ConsoleCommand.instance.StandardCommandOperation(num2, new object[]
                                            {
                                    text2
                                            });
                                        }
                                        break;
                                    }
                                case 14:
                                    global::ConsoleCommand.instance.StandardCommandOperation(num2, new object[]
                                    {
                                array[2]
                                    });
                                    break;
                                case 15:
                                    global::ConsoleCommand.instance.StandardCommandOperation(num2, new object[]
                                    {
                                array[2]
                                    });
                                    break;
                                case 16:
                                case 17:
                                case 18:
                                case 19:
                                case 20:
                                case 21:
                                case 22:
                                    global::ConsoleCommand.instance.StandardCommandOperation(num2, new object[0]);
                                    break;
                                case 23:
                                    global::ConsoleCommand.instance.StandardCommandOperation(num2, new object[]
                                    {
                                array[2]
                                    });
                                    break;
                            }
                        }
                    }
                    else if (a == global::ConsoleCommand.CreatureCommand)
                    {
                        int num4 = global::ConsoleCommand.instance.creatureCommand.IndexOf(text);
                        Debug.Log(string.Concat(new object[]
                        {
                        "Creature Opeartion : ",
                        num4,
                        " ",
                        array.Length
                        }));
                        if (num4 != -1)
                        {
                            float num5;
                            if (num4 == 6)
                            {
                                global::ConsoleCommand.instance.CreatureCommandOperation(6, true, new object[0]);
                            }
                            else if (float.TryParse(array[2], out num5))
                            {
                                if (array.Length >= 4)
                                {
                                    float num6 = float.Parse(array[3]);
                                    global::ConsoleCommand.instance.CreatureCommandOperation(num4, false, new object[]
                                    {
                                    (long)num5,
                                    num6
                                    });
                                }
                                else
                                {
                                    global::ConsoleCommand.instance.CreatureCommandOperation(num4, false, new object[]
                                    {
                                    (long)num5
                                    });
                                }
                            }
                            else if (array.Length >= 4)
                            {
                                float num7 = float.Parse(array[3]);
                                global::ConsoleCommand.instance.CreatureCommandOperation(num4, true, new object[]
                                {
                                (long)num5,
                                num7
                                });
                            }
                            else
                            {
                                global::ConsoleCommand.instance.CreatureCommandOperation(num4, true, new object[]
                                {
                                (long)num5
                                });
                            }
                        }
                    }
                    else if (a == global::ConsoleCommand.AgentCommand)
                    {
                        if (AgentCommand_Mod(text, array))
                        {
                            return;
                        }
                        int num8 = global::ConsoleCommand.instance.agentCommand.IndexOf(text);
                        if (num8 == 6)
                        {
                            global::RwbpType rwbpType = Assets.Scripts.UI.Utils.EnumTextConverter.GetRwbpType(array[2]);
                            long num9 = long.Parse(array[3]);
                            int num10 = int.Parse(array[4]);
                            global::ConsoleCommand.instance.AgentCommandOperation(num8, new object[]
                            {
                            rwbpType,
                            num9,
                            num10
                            });
                        }
                        else
                        {
                            float num11 = float.Parse(array[3]);
                            long num12 = long.Parse(array[2]);
                            Debug.Log(num11 + " " + num12);
                            Debug.Log(text);
                            if (num8 != -1)
                            {
                                global::ConsoleCommand.instance.AgentCommandOperation(num8, new object[]
                                {
                                num12,
                                num11
                                });
                            }
                        }
                    }
                    else if (a == global::ConsoleCommand.OfficerCommand)
                    {
                        int num13 = global::ConsoleCommand.instance.officerCommand.IndexOf(text);
                        if (num13 != -1)
                        {
                            global::ConsoleCommand.instance.OfficerCommandOperation(num13, new object[]
                            {
                            array[2]
                            });
                        }
                    }
                    else if (a == global::ConsoleCommand.BetaCommand)
                    {
                        global::ConsoleCommand.instance.BetaCommandOperation(text, array);
                    }
                    else
                    {
                        if (this.systemLogEnter)
                        {
                            global::ConsoleCommand.instance.StandardCommandOperation(0, new object[]
                            {
                            command
                            });
                        }
                        if (this.angelaLogEnter)
                        {
                            global::ConsoleCommand.instance.StandardCommandOperation(2, new object[]
                            {
                            command
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(string.Concat(new object[]
                {
                "Invalid opearation : ",
                command,
                " ",
                ex
                }));
            }
        }



        [MemberAlias("GetHmmCommand", typeof(ConsoleScript))]
        private string GetHmmCommand(string cmd)
        {
            return cmd;
        }
        [MemberAlias("ConsoleWnd", typeof(ConsoleScript))]
        public GameObject ConsoleWnd;
        [MemberAlias("inputField", typeof(ConsoleScript))]
        private InputField inputField;
        [MemberAlias("consoleActivated", typeof(ConsoleScript))]
        private bool consoleActivated;
        [MemberAlias("systemLogEnter", typeof(ConsoleScript))]
        private bool systemLogEnter;
        [MemberAlias("angelaLogEnter", typeof(ConsoleScript))]
        private bool angelaLogEnter;
    }
}
