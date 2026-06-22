using LobotomyBaseMod;
using Patchwork.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using WorkerSprite;
using LobotomyBaseModLib;
using System.Reflection;

namespace Lobotomypatch
{
    [ModifiesType("EGOgiftModel")]
    public class EGOgiftModel_patch
    {
        
        [ModifiesMember("MakeGift")]
        public static EGOgiftModel MakeGift(EquipmentTypeInfo info)
        {
            EGOgiftModel egogiftModel = new EGOgiftModel();
            egogiftModel.metaInfo = info;
            Type type = Type.GetType(info.script);
            
            object obj = null;
                foreach (Assembly assembly in Add_On.instance.AssemList)
                {
                    foreach (Type type2 in assembly.GetTypes())
                    {
                        if (type2.Name == info.script)
                        {
                            obj = Activator.CreateInstance(type2);
                        }
                    }
                }
                
                if (obj == null)
                {
                try
                {
                    obj = Activator.CreateInstance(type);
                }
                catch (Exception e)
                {

                }
                }
            
            if (obj != null && obj is EquipmentScriptBase)
            {
                egogiftModel.script = (global::EquipmentScriptBase)obj;
            }
            egogiftModel.script.SetModel(egogiftModel);
            
            return egogiftModel;
        }
    }
    [ModifiesType("EventCreatureModel")]
    public class EventCreatureModel_patch
    {
        [ModifiesMember("OnFixedUpdate")]
        public void OnFixedUpdate()
        {
            UpdateBufState();
            this.tempAnim.OnFixedUpdate();
            if (this._equipment.weapon != null)
            {
                this._equipment.weapon.OnFixedUpdate();
            }
            this.commandQueue.Execute(this.ForceTypeChange<CreatureModel>());
            if (!this.script.UniqueMoveControl())
            {
                if (this.GetMovableNode().IsMoving())
                {
                    this.SetMoveAnimState(true);
                }
                else if (this._unit != null && this._unit.animTarget != null)
                {
                    this._unit.animTarget.StopMoving();
                }
            }
            if (global::GameManager.currentGameManager.ManageStarted)
            {
                this.script.OnFixedUpdate(this.ForceTypeChange<CreatureModel>());
            }
            this.movableNode.ProcessMoveNode(this.metaInfo.speed * this.movementScale);
        }

        [MemberAlias("movementScale", typeof(CreatureModel))]
        public float movementScale;
        [MemberAlias("metaInfo", typeof(CreatureModel))]
        public global::CreatureTypeInfo metaInfo;
        [MemberAlias("_unit", typeof(CreatureModel))]
        public CreatureUnit _unit;
        [MemberAlias("SetMoveAnimState", typeof(CreatureModel))]
        public void SetMoveAnimState(bool b)
        {
        }
        [MemberAlias("tempAnim", typeof(UnitModel))]
        public DummyAttackAnimator tempAnim;
        [MemberAlias("UpdateBufState", typeof(UnitModel))]
        public void UpdateBufState()
        {
        }
        [MemberAlias("movableNode", typeof(UnitModel))]
        public MovableObjectNode movableNode;
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
        [MemberAlias("GetMovableNode", typeof(UnitModel))]
        public MovableObjectNode GetMovableNode()
        {
            return null;
        }

        [MemberAlias("commandQueue", typeof(CreatureModel))]
        public CreatureCommandQueue commandQueue;
    }
    [ModifiesType("EffectInfo")]
    public class EffectInfo_patch
    {
        [NewMember]
        public static EffectInvoker MakeEffect_Mod(EffectInfo_patch info, global::MovableObjectNode mov,string modid)
        {
            global::EffectInvoker result = null;
            
            try
            {
                string[] array = info.effectSrc.Split(new char[]
                {
                '/'
                });
                if (array[0].ToLower() == "custom" && global::Add_On.instance.EffectList.ContainsKey(array[1]))
                {
                    GameObject gameObject = Spine.Unity.SkeletonAnimation.NewSkeletonAnimationGameObject(global::Add_On.instance.EffectList[array[1]]).gameObject;
                    gameObject.AddComponent<global::CustomEffect>();
                    gameObject.transform.SetParent(global::EffectLayer.currentLayer.transform);
                    gameObject.name = string.Format("{0}[owner:{1}]", gameObject.name, mov.GetUnit().GetUnitName());
                    Vector3 vector = gameObject.transform.position;
                    Vector3 a = info.relativePosition;
                    Vector3 localScale = gameObject.transform.localScale;
                    if (mov.GetDirection() == global::UnitDirection.LEFT)
                    {
                        localScale.x *= -1f;
                        a.x *= -1f;
                    }
                    a *= mov.currentScale;
                    vector += a + mov.GetCurrentViewPosition();
                    gameObject.transform.position = vector;
                    gameObject.transform.rotation = Quaternion.Euler(0f, 0f, info.rotation);
                    gameObject.transform.localScale = localScale;
                    gameObject.SetActive(true);
                    result = null;
                }
                else if(array[0].ToLower() == "assetbundle" && ModAssetBundleManager.Instance.GetAsset(new KeyValuePairSS(modid, array[1])) != null)
                {
                    GameObject assetbundle = ModAssetBundleManager.Instance.GetAsset(new KeyValuePairSS(modid, array[1]));
                    Vector3 vector = assetbundle.transform.position;
                    Vector3 a = info.relativePosition;
                    Vector3 localScale = assetbundle.transform.localScale;
                    if (mov.GetDirection() == global::UnitDirection.LEFT)
                    {
                        localScale.x *= -1f;
                        a.x *= -1f;
                    }
                    a *= mov.currentScale;
                    vector += a + mov.GetCurrentViewPosition();
                    assetbundle.transform.position = vector;
                    assetbundle.transform.rotation = Quaternion.Euler(0f, 0f, info.rotation);
                    assetbundle.transform.localScale = localScale;
                    assetbundle.SetActive(true);
                    AutoDestroyer destoryer = assetbundle.AddComponent<AutoDestroyer>();
                    destoryer.destroyTime = info.lifetime;
                }
                else
                {
                    global::EffectInvoker effectInvoker = global::EffectInvoker.Invoker("DamageInfo/" + info.effectSrc, mov, info.lifetime, info.unscaled);
                    Vector3 vector2 = effectInvoker.transform.position;
                    Vector3 vector3 = info.relativePosition;
                    Vector3 localScale2 = effectInvoker.transform.localScale;
                    if (mov.GetDirection() == global::UnitDirection.LEFT)
                    {
                        localScale2.x *= -1f;
                        vector3.x *= -1f;
                    }
                    effectInvoker.Dettach();
                    vector3 *= mov.currentScale;
                    vector2 += vector3;
                    effectInvoker.transform.position = vector2;
                    effectInvoker.transform.rotation = Quaternion.Euler(0f, 0f, info.rotation);
                    effectInvoker.transform.localScale = localScale2;
                    result = effectInvoker;
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/BaseMods/EFerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
                result = null;
            }
            return result;
        }
        [NewMember]
        public EffectInvoker MakeEffect_Mod(MovableObjectNode mov,string modid)
        {
            return MakeEffect_Mod(this, mov,modid);
        }
        [ModifiesMember("MakeEffect")]
        public EffectInvoker MakeEffect_patch(MovableObjectNode mov)
        {
            return MakeEffect_Mod(mov,String.Empty);
        }
        


        [MemberAlias("EffectPrefix", typeof(EffectInfo))]
        public const string EffectPrefix = "DamageInfo/";
        [MemberAlias("effectType", typeof(EffectInfo))]
        public global::DamageInfo_EffectType effectType;
        [MemberAlias("invokedUnit", typeof(EffectInfo))]
        public global::EffectInvokedUnit invokedUnit;
        [MemberAlias("effectSrc", typeof(EffectInfo))]
        public string effectSrc = string.Empty;
        [MemberAlias("lifetime", typeof(EffectInfo))]
        public float lifetime = 1f;
        [MemberAlias("unscaled", typeof(EffectInfo))]
        public bool unscaled;
        [MemberAlias("invokeOnce", typeof(EffectInfo))]
        public bool invokeOnce = true;
        [MemberAlias("relativePosition", typeof(EffectInfo))]
        public Vector3 relativePosition = Vector3.zero;
        [MemberAlias("rotation",typeof(EffectInfo))]
        public float rotation;

    }

    [ModifiesType("WorkerSprite.EGOGiftRenderData")]
    public class EGOGiftRenderData_patch
    {
        [NewMember]
        public static LcId GetLcId(EGOGiftRenderData data)
        {
            EGOGiftRenderData_patch pdata = ((EGOGiftRenderData_patch)(object)data);
            if (pdata.modid == null) return new LcId((int)data.metaId);
            return new LcId(pdata.modid,(int)data.metaId);
        }

        [NewMember]
        [NonSerialized]
        public string modid;
    }
    [ModifiesType("EquipmentModel")]
    public class EquipmentModel_patch
    {
        [ModifiesMember("CheckRequire")]
        public bool CheckRequire_patch(UnitModel unit)
        {
            if (!(unit is global::AgentModel))
            {
                return true;
            }
            global::AgentModel agent = (global::AgentModel)unit;
            LcId lcid = EquipmentTypeInfo_patch.GetLcId(this.metaInfo);
            if (lcid == 300034 || lcid == 200034)
            {
                if (!agent.spriteData.FrontHair.Equals(Resources.Load<Sprite>("Sprites/Worker/Basic/Hair/Front/Bald")))
                {
                    return false;
                }
                if (!agent.spriteData.FrontHair.Equals(Resources.Load<Sprite>("Sprites/Worker/Basic/Hair/Front/Bald")))
                {
                    return false;
                }
            }
            if (lcid == 200038 || lcid == 300038)
            {
                return agent.maxHp >= 110 && agent.maxMental >= 110 && agent.workProb >= 110 && agent.workSpeed >= 110 && agent.attackSpeed >= 110f && agent.movement >= 110f;
            }
            global::EgoRequire egoRequire = this.metaInfo.requires.Find((global::EgoRequire x) => (x.type == global::EgoRequireType.R && agent.fortitudeLevel < x.value) || (x.type == global::EgoRequireType.W && agent.prudenceLevel < x.value) || (x.type == global::EgoRequireType.B && agent.temperanceLevel < x.value) || (x.type == global::EgoRequireType.P && agent.justiceLevel < x.value) || (x.type == global::EgoRequireType.level && agent.level < x.value));
            return egoRequire == null;
        }


        [MemberAlias("metaInfo",typeof(EquipmentModel))]
        public global::EquipmentTypeInfo metaInfo;
    }
    [ModifiesType]
    public class EquipmentTypeInfo_patch : EquipmentTypeInfo
    {
        [NewMember]
        public LcId LcId
        {
            get
            {
                return GetLcId(this.ForceTypeChange<EquipmentTypeInfo>());
            }
        }
        [ModifiesMember("get_MaxNum")]
        public int get_MaxNum_patch()
        {
            LcId lcid = new LcId(modid, id);
            if (this.type != global::EquipmentTypeInfo.EquipmentType.SPECIAL && global::MissionManager.instance.ExistsFinishedBossMission(global::SefiraEnum.GEBURAH) && lcid != 200038 && lcid != 300038 && lcid != 200015 && lcid != 200061)
            {
                return Mathf.Min(5, this.maxNum + 1);
            }
            return this.maxNum;
        }
        [ModifiesMember(".ctor")]
        public void Ctor()
        {
            modid = String.Empty;
            localizeData = new Dictionary<string, string>();
            sprite = string.Empty;
            maxNum = 1;
            defenseInfo = DefenseInfo.zero;
            List<EgoRequire> requires = new List<EgoRequire>();
            script = string.Empty;
            grade = "1";
            weaponClassType = WeaponClassType.AXE;
            specialWeaponAnim = string.Empty;
            animationNames = new string[]
        {
    "test",
    "test2"
        };
            damageInfos = new DamageInfo[]
        {
    DamageInfo.zero
        };
            splashInfo = new SplashInfo();
            attackSpeed = 1f;
            bonus = new EGObonusInfo();
            attachPos = string.Empty;
        }

        [NewMember]
        public static LcId GetLcId(EquipmentTypeInfo equip)
        {
            EquipmentTypeInfo_patch pequip = (EquipmentTypeInfo_patch)(object)equip;
            if (pequip.modid == null) return new LcId(string.Empty, equip.id);
            return new LcId(((EquipmentTypeInfo_patch)(object)equip).modid, equip.id);
        }
        [NewMember]
        [NonSerialized]
        public string modid = string.Empty;
    }

    [ModifiesType("EquipmentTypeList")]
    public class EquipmentTypeList_patch
    {
        [NewMember]
        public void Init_Mod(Dictionary<string, Dictionary<int, EquipmentTypeInfo>> dic)
        {
            moddic = dic;
        }

        [NewMember]
        public EquipmentTypeInfo GetData_Mod(LcId id)
        {
            if (id.packageId == string.Empty)
            {
                ModDebug.Log("none packageId");
                return EquipmentTypeList.instance.GetData(id.id);
            }
            if (moddic.ContainsKey(id.packageId))
            {
                EquipmentTypeInfo info = null;
                moddic[id.packageId].TryGetValue(id.id, out info);
                return info;
            }
            return null;
        }
        [NewMember]
        public string GetModId(EquipmentTypeInfo equip)
        {
            return ((EquipmentTypeInfo_patch)(object)equip).modid;
        }
        [MemberAlias("_instance", typeof(EquipmentTypeList))]
        public static EquipmentTypeList_patch instance;

        [NewMember]
        public Dictionary<string, Dictionary<int, EquipmentTypeInfo>> moddic;
    }
    [ModifiesType()]
    public class EquipmentDataLoader_patch : EquipmentDataLoader
    {
        [ModifiesMember("Load", ModificationScope.All)]
        public void Load_patch()
        {
            try
            {
                ModDebug.Log("EDL Load 1");
                XmlDocument xmlDocument = new XmlDocument();
                if (!File.Exists(Application.dataPath + "/Managed/BaseMod/BaseEquipment.txt"))
                {
                    TextAsset textAsset = Resources.Load<TextAsset>("xml/Equipment/Equipment");
                    File.WriteAllText(Application.dataPath + "/Managed/BaseMod/BaseEquipment.txt", textAsset.text);
                }
                string xml = File.ReadAllText(Application.dataPath + "/Managed/BaseMod/BaseEquipment.txt");
                xmlDocument.LoadXml(xml);
                Dictionary<int, EquipmentTypeInfo> dictionary = this.LoadEquips(xmlDocument);
                Dictionary<string, Dictionary<int, EquipmentTypeInfo>> moddic = new Dictionary<string, Dictionary<int, EquipmentTypeInfo>>();
                ModDebug.Log("EDL Load 2");
                foreach (ModInfo mod in ((Add_On_patch)Add_On.instance).ModList)
                {
                    ModInfo_patch modinfo = (ModInfo_patch)mod;
                    DirectoryInfo directoryInfo = EquipmentDataLoader.CheckNamedDir(modinfo.modpath, "Equipment");
                    if (directoryInfo != null && Directory.Exists(directoryInfo.FullName + "/txts"))
                    {
                        DirectoryInfo directoryInfo2 = new DirectoryInfo(directoryInfo.FullName + "/txts");
                        if (directoryInfo2.GetFiles().Length != 0)
                        {
                            if (modinfo.modid == string.Empty)
                            {
                                foreach (FileInfo fileInfo in directoryInfo2.GetFiles())
                                {
                                    if (fileInfo.Name.Contains(".txt") || fileInfo.Name.Contains(".xml"))
                                    {
                                        XmlDocument xmlDocument2 = new XmlDocument();
                                        xmlDocument2.LoadXml(File.ReadAllText(fileInfo.FullName));
                                        foreach (KeyValuePair<int, EquipmentTypeInfo> keyValuePair in this.LoadEquips(xmlDocument2))
                                        {
                                            if (dictionary.ContainsKey(keyValuePair.Key))
                                            {
                                                dictionary.Remove(keyValuePair.Key);
                                            }
                                            dictionary.Add(keyValuePair.Key, keyValuePair.Value);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Dictionary<int, EquipmentTypeInfo> privatemoddic = new Dictionary<int, EquipmentTypeInfo>();
                                foreach (FileInfo fileInfo in directoryInfo2.GetFiles())
                                {
                                    if (fileInfo.Name.Contains(".txt") || fileInfo.Name.Contains(".xml"))
                                    {
                                        XmlDocument xmlDocument2 = new XmlDocument();
                                        xmlDocument2.LoadXml(File.ReadAllText(fileInfo.FullName));

                                        foreach (KeyValuePair<int, EquipmentTypeInfo> keyValuePair in this.LoadEquips(xmlDocument2))
                                        {
                                            ((EquipmentTypeInfo_patch)keyValuePair.Value).modid = modinfo.modid;
                                            if (privatemoddic.ContainsKey(keyValuePair.Key))
                                            {
                                                privatemoddic.Remove(keyValuePair.Key);
                                            }
                                            privatemoddic.Add(keyValuePair.Key, keyValuePair.Value);
                                        }
                                    }
                                }
                                moddic[modinfo.modid] = privatemoddic;
                            }
                        }
                    }
                }
                ModDebug.Log("EDL Load 3");
                EquipmentTypeList.instance.Init(dictionary);
                object obj = (EquipmentTypeList.instance as object);
                EquipmentTypeList_patch patch = obj as EquipmentTypeList_patch;
                patch.Init_Mod(moddic);
                ModDebug.Log("EDL Load 4");
            }
            catch (Exception e)
            {
                ModDebug.Log("EDL Load Error - " + e.Message + Environment.NewLine + e.StackTrace);
            }
        }
    }
}
