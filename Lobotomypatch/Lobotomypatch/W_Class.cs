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
using WorkerSprite;
using static UnityEngineInternal.Input.NativeTrackingEvent;
using Spine.Unity.Modules.AttachmentTools;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine.Rendering;
using System.Xml.Linq;
using WorkerSpine;
using LobotomyBaseModLib;

namespace Lobotomypatch
{
    [ModifiesType("WorkerUnit")]
    public class WorkerUnit_patch
    {
        [ModifiesMember("UpdateAnimatorChange")]
        protected void UpdateAnimatorChange_patch()
        {
            this._animChangeTimer.StopTimer();
            if (this._animChangeReady && !this._animChanged)
            {
                this._animChanged = true;
                if (this.workerModel.Equipment.weapon != null && this.workerModel.Equipment.weapon.metaInfo.weaponClassType == global::WeaponClassType.SPECIAL)
                {
                    KeyValuePairSS SS = new KeyValuePairSS(EquipmentTypeInfo_patch.GetLcId(this.workerModel.Equipment.weapon.metaInfo).packageId, this.workerModel.Equipment.weapon.metaInfo.specialWeaponAnim);
                    ((WorkerAnimatorChanger_patch)(object)this.animChanger).ChangeAnimator_Mod(SS, true);
                }
                if (!this.workerModel.IsPanic())
                {
                    this.SetWorkerFaceType(global::WorkerSprite.WorkerFaceType.BATTLE);
                }
            }
            if (!this._animChangeReady && this._animChanged)
            {
                this._animChanged = false;
                if (this.workerModel.Equipment.weapon != null && this.workerModel.Equipment.weapon.metaInfo.weaponClassType == global::WeaponClassType.SPECIAL)
                {
                    this.animChanger.ChangeAnimator();
                }
                if (!this.workerModel.IsPanic())
                {
                    this.SetWorkerFaceType(global::WorkerSprite.WorkerFaceType.DEFAULT);
                }
            }
        }
        [NewMember]
        public void ChangeAnimatorForcely_Mod(KeyValuePairSS name, bool uniqueFace, bool useSep = false)
        {
            this._animChangeTimer.StopTimer();
            this._animChangeReady = false;
            this._animChanged = false;
            if (uniqueFace)
            {
                ((WorkerAnimatorChanger_patch)(object)this.animChanger).ChangeAnimatorWithUniqueFace_Mod(name, useSep);
            }
            else
            {
                ((WorkerAnimatorChanger_patch)(object)this.animChanger).ChangeAnimator_Mod(name);
            }
        }
        [ModifiesMember("ChangeAnimatorForcely")]
        public void ChangeAnimatorForcely_patch(string name, bool uniqueFace, bool useSep = false)
        {
            ChangeAnimatorForcely_Mod(new KeyValuePairSS(String.Empty, name), uniqueFace, useSep);
        }


        [MemberAlias("SetWorkerFaceType", typeof(WorkerUnit))]
        public void SetWorkerFaceType(global::WorkerSprite.WorkerFaceType type)
        {
        }


        [MemberAlias("workerModel", typeof(WorkerUnit))]
        public global::WorkerModel workerModel;
        [MemberAlias("spineRenderer", typeof(WorkerUnit))]
        public Spine.Unity.SkeletonRenderer spineRenderer;
        [MemberAlias("_inCamera", typeof(WorkerUnit))]
        protected bool _inCamera;
        [MemberAlias("uiRoot", typeof(WorkerUnit))]
        public Canvas uiRoot;
        [MemberAlias("animRoot", typeof(WorkerUnit))]
        public Transform animRoot;
        [MemberAlias("clickArea", typeof(WorkerUnit))]
        public GameObject clickArea;
        [MemberAlias("shadow", typeof(WorkerUnit))]
        public GameObject shadow;
        [MemberAlias("animEventHandler", typeof(WorkerUnit))]
        public global::AnimatorEventHandler animEventHandler;
        [MemberAlias("spriteSetter", typeof(WorkerUnit))]
        public global::WorkerSprite.WorkerSpriteSetter spriteSetter;
        [MemberAlias("weaponSetter", typeof(WorkerUnit))]
        public global::WeaponSetter weaponSetter;
        [MemberAlias("_animController", typeof(WorkerUnit))]
        protected global::UnitAnimatorController _animController;
        [MemberAlias("showSpeech", typeof(WorkerUnit))]
        public global::AgentSpeech showSpeech;
        [MemberAlias("barrierParent", typeof(WorkerUnit))]
        public Transform barrierParent;
        [MemberAlias("blockRotation", typeof(WorkerUnit))]
        public bool blockRotation;
        [MemberAlias("blockMoving", typeof(WorkerUnit))]
        public bool blockMoving;
        [MemberAlias("zValueDefault", typeof(WorkerUnit))]
        private float zValueDefault;
        [MemberAlias("zValue", typeof(WorkerUnit))]
        public float zValue;
        [MemberAlias("recoilPosition", typeof(WorkerUnit))]
        protected Vector3 recoilPosition = new Vector3(0f, 0f, 0f);
        [MemberAlias("uiActivated", typeof(WorkerUnit))]
        protected bool uiActivated = true;
        [MemberAlias("effectAttached", typeof(WorkerUnit))]
        protected List<GameObject> effectAttached = new List<GameObject>();
        [MemberAlias("animChanger", typeof(WorkerUnit))]
        public WorkerSpine.WorkerAnimatorChanger animChanger;
        [MemberAlias("bufUI", typeof(WorkerUnit))]
        public global::BufStateUI bufUI;
        [MemberAlias("_animChangeReady", typeof(WorkerUnit))]
        protected bool _animChangeReady;
        [MemberAlias("_animChanged", typeof(WorkerUnit))]
        protected bool _animChanged;
        [MemberAlias("_animChangeTimer",typeof(WorkerUnit))]
        protected global::Timer _animChangeTimer = new global::Timer();
    }
    [ModifiesType("WorkerSpine.WorkerAnimatorChanger")]
    public class WorkerAnimatorChanger_patch
    {
        [NewMember]
        public void ChangeAnimator_Mod(KeyValuePairSS name)
        {
            WorkerSpineAnimatorData animator = null;
            if (((WorkerSpineAnimatorManager_patch)(object)WorkerSpineAnimatorManager.instance).GetDataWithCheck_Mod(name, out animator))
            {
                this.SetAnimator(animator);
            }
            else
            {
                Debug.Log("Error in founding spine animator data : " + name);
            }
            this._setter.BaiscRendererInit();
        }
        [ModifiesMember("ChangeAnimator")]
        public void ChangeAnimator_patch(string name)
        {
            ChangeAnimator_Mod(new KeyValuePairSS(string.Empty, name));
        }
        [NewMember]
        public void ChangeAnimator_Mod(KeyValuePairSS name, bool separator)
        {
            WorkerSpineAnimatorData data = null;
            if (((WorkerSpineAnimatorManager_patch)(object)WorkerSpineAnimatorManager.instance).GetDataWithCheck_Mod(name, out data))
            {
                this.SetAnimator(data, separator);
            }
            else
            {
                Debug.Log("Error in founding spine animator data : " + name);
            }
            this._setter.BaiscRendererInit();
        }
        [ModifiesMember("ChangeAnimator")]
        public void ChangeAnimator_patch(string name, bool separator)
        {
            ChangeAnimator_Mod(new KeyValuePairSS(string.Empty, name), separator);
        }
        [NewMember]
        public void ChangeAnimatorWithUniqueFace_Mod(KeyValuePairSS name, bool separator)
        {
            WorkerSpineAnimatorData data = null;
            if (((WorkerSpineAnimatorManager_patch)(object)WorkerSpineAnimatorManager.instance).GetDataWithCheck_Mod(name, out data))
            {
                this.SetAnimatorWithUniqueFace(data, separator);
                return;
            }
            Debug.Log("Error in founding spine animator data : " + name);
        }
        [ModifiesMember("ChangeAnimatorWithUniqueFace")]
        public void ChangeAnimatorWithUniqueFace_patch(string name, bool separator)
        {
            ChangeAnimatorWithUniqueFace_Mod(new KeyValuePairSS(string.Empty, name), separator);
        }
        [ModifiesMember("SetAnimatorWithUniqueFace")]
        private void SetAnimatorWithUniqueFace_patch(WorkerSpineAnimatorData data, bool separator)
        {
            if (!data.IsLoaded)
            {
                data.LoadData();
            }

                get_skeletonAnimator().skeletonDataAsset = data.skeletonData;
                get_animator().runtimeAnimatorController = data.animator;
                get_skeletonAnimator().Initialize(true);
            
            if (data.name.Contains("_"))
            {
                if (get_skeletonAnimator().state == null || data.skeletonData.GetAnimationStateData() != get_skeletonAnimator().state.Data)
                {
                    get_skeletonAnimator().state = new global::Spine.AnimationState(data.skeletonData.GetAnimationStateData());
                    this.ForceTypeChange<WorkerAnimatorChanger>().state = get_skeletonAnimator().state;
                    this.ForceTypeChange<WorkerAnimatorChanger>().skeleton = get_skeletonAnimator().skeleton;
                }
                if (data.name.Split(new char[] { '_' })[1].ToLower() == "custom")
                {
                    get_skeletonAnimator().UseState = true;
                }
            }
            else
            {
                get_skeletonAnimator().state = null;
                this.ForceTypeChange<WorkerAnimatorChanger>().state = null;
                this.ForceTypeChange<WorkerAnimatorChanger>().skeleton = null;
                get_skeletonAnimator().UseState = false;
            }
            this._setter.UniqueFaceReskin();
            if (!separator)
            {
                this._setter.DisableSeparatorForUnique();
            }
            this.ForceTypeChange<WorkerAnimatorChanger>().CurrentData = data;
        }
        [ModifiesMember("SetAnimator")]
        private void SetAnimator_patch(WorkerSpineAnimatorData data, bool separator)
        {
            if (!data.IsLoaded)
            {
                data.LoadData();
            }

                get_skeletonAnimator().skeletonDataAsset = data.skeletonData;
                get_animator().runtimeAnimatorController = data.animator;
                get_skeletonAnimator().Initialize(true);
            
            if (data.name.Contains("_"))

            {
                if (get_skeletonAnimator().state == null || data.skeletonData.GetAnimationStateData() != get_skeletonAnimator().state.Data)
                {
                    get_skeletonAnimator().state = new global::Spine.AnimationState(data.skeletonData.GetAnimationStateData());
                    this.ForceTypeChange<WorkerAnimatorChanger>().state = get_skeletonAnimator().state;
                    this.ForceTypeChange<WorkerAnimatorChanger>().skeleton = get_skeletonAnimator().skeleton;
                }
                if (data.name.Split(new char[] { '_' })[1].ToLower() == "custom")
                {
                    get_skeletonAnimator().UseState = true;
                }
            }
            else
            {
                get_skeletonAnimator().state = null;
                this.ForceTypeChange<WorkerAnimatorChanger>().state = null;
                this.ForceTypeChange<WorkerAnimatorChanger>().skeleton = null;
                get_skeletonAnimator().UseState = false;
            }
            this._setter.UniqueFaceReskin();
            if (!separator)
            {
                this._setter.DisableSeparatorForUnique();
            }
            this.ForceTypeChange<WorkerAnimatorChanger>().CurrentData = data;
        }

        [ModifiesMember("SetAnimator")]
        public void SetAnimator_patch(WorkerSpineAnimatorData data)
        {
            if (!data.IsLoaded)
            {
                data.LoadData();
            }
                get_skeletonAnimator().skeletonDataAsset = data.skeletonData;
                get_animator().runtimeAnimatorController = data.animator;
                get_skeletonAnimator().Initialize(true);
            
            if (data.name.Contains("_"))
            {
                if (get_skeletonAnimator().state == null || data.skeletonData.GetAnimationStateData() != get_skeletonAnimator().state.Data)
                {
                    get_skeletonAnimator().state = new global::Spine.AnimationState(data.skeletonData.GetAnimationStateData());
                    this.ForceTypeChange<WorkerAnimatorChanger>().state = get_skeletonAnimator().state;
                    this.ForceTypeChange<WorkerAnimatorChanger>().skeleton = get_skeletonAnimator().skeleton;
                }
                if (data.name.Split(new char[] { '_' })[1].ToLower() == "custom")
                {
                    get_skeletonAnimator().UseState = true;
                }
            }
            else
            {
                get_skeletonAnimator().state = null;
                this.ForceTypeChange<WorkerAnimatorChanger>().state = null;
                this.ForceTypeChange<WorkerAnimatorChanger>().skeleton = null;
                get_skeletonAnimator().UseState = false;
            }
            this._setter.Reskin();
            this.ForceTypeChange<WorkerAnimatorChanger>().CurrentData = data;
        }

        [MemberAlias("get_animator", typeof(WorkerAnimatorChanger))]
        private Animator get_animator()
        {
            return null;
        }
        [MemberAlias("get_skeletonAnimator", typeof(WorkerAnimatorChanger))]
        private Spine.Unity.SkeletonAnimator get_skeletonAnimator()
        {
            return null;
        }
        [MemberAlias("SetAnimator", typeof(WorkerAnimatorChanger))]
        private void SetAnimator(WorkerSpineAnimatorData data)
        {
            
        }

        [MemberAlias("SetAnimator", typeof(WorkerAnimatorChanger))]
        private void SetAnimator(WorkerSpineAnimatorData data, bool separator)
        {
            
        }
        [MemberAlias("SetAnimatorWithUniqueFace",typeof(WorkerAnimatorChanger))]
        private void SetAnimatorWithUniqueFace(WorkerSpineAnimatorData data, bool separator)
        {

        }

        [MemberAlias("_setter", typeof(WorkerAnimatorChanger))]
        private WorkerSprite.WorkerSpriteSetter _setter;
    }
    [ModifiesType("WorkerSpine.WorkerSpineAnimatorManager")]
    public class WorkerSpineAnimatorManager_patch
    {
        [NewMember]
        public bool GetDataWithCheck_Mod(KeyValuePairSS name, out WorkerSpineAnimatorData output)
        {
            return this.nameDicMod.TryGetValue(name, out output);
        }
        [ModifiesMember("GetDataWithCheck")]
        public bool GetDataWithCheck_patch(string name, out WorkerSpineAnimatorData output)
        {
            return GetDataWithCheck_Mod(new KeyValuePairSS(string.Empty, name),out output);
        }
        [NewMember]
        public void FindNewSkinandSkel_Mod(WorkerSpineAnimatorData data, Dictionary<KeyValuePairSS, object> dic, Dictionary<KeyValuePairSS, string> dic2, Dictionary<KeyValuePairSS, object> dic_c, Dictionary<KeyValuePairSS, string> dic2_c)
        {
            data.LoadData();
            foreach(KeyValuePairSS SS in dic.Keys.ToList())
            {
                if(SS.value == data.name)
                {
                    if (dic[SS] is string)
                    {
                        this.FNSS_skel(data, (string)dic[SS]);
                    }
                    else
                    {
                        this.FNSS_skel(data, (byte[])dic[SS]);
                    }
                    dic_c.Remove(SS);
                }
            }
            foreach(KeyValuePairSS SS in dic2.Keys.ToList())
            {
                if(SS.value == data.name)
                {
                    this.FNSS_skin(data, dic2[SS]);
                    dic2_c.Remove(SS);
                }
            }
           
        }
        [ModifiesMember("Init")]
        public void Init_patch(List<WorkerSpineAnimatorData> data)
        {
            try
            {
                nameDicMod = new Dictionary<KeyValuePairSS, WorkerSpineAnimatorData>();
                Dictionary<KeyValuePairSS, object> dictionary = new Dictionary<KeyValuePairSS, object>();
                Dictionary<KeyValuePairSS, string> dictionary2 = new Dictionary<KeyValuePairSS, string>();
                new Dictionary<string, string>();
                if (global::Add_On.instance.DirList.Count > 0)
                {
                    foreach (ModInfo mod in ((Add_On_patch)(object)Add_On.instance).ModList)
                    {
                        ModInfo_patch pmodinfo = (ModInfo_patch)(object)mod;
                        DirectoryInfo directoryInfo = global::Add_On.CheckNamedDir(pmodinfo.modpath, "AgentAnimation");
                        string modid = pmodinfo.modid;
                        if (directoryInfo != null && directoryInfo.GetDirectories().Length != 0)
                        {
                            foreach (DirectoryInfo directoryInfo2 in directoryInfo.GetDirectories())
                            {
                                KeyValuePairSS SS = new KeyValuePairSS(modid, directoryInfo2.Name);
                                if (File.Exists(directoryInfo2.FullName + "/json.txt"))
                                {
                                    dictionary.Add(SS, File.ReadAllText(directoryInfo2.FullName + "/json.txt"));
                                }
                                else if (File.Exists(directoryInfo2.FullName + "/skeleton.skel"))
                                {
                                    dictionary.Add(SS, File.ReadAllBytes(directoryInfo2.FullName + "/skeleton.skel"));
                                }
                                dictionary2.Add(SS, directoryInfo2.FullName);
                            }
                        }
                    }
                }
                Dictionary<KeyValuePairSS, object> dictionary3 = new Dictionary<KeyValuePairSS, object>(dictionary);
                Dictionary<KeyValuePairSS, string> dictionary4 = new Dictionary<KeyValuePairSS, string>(dictionary2);
                foreach (WorkerSpineAnimatorData workerSpineAnimatorData in data)
                {
                    try
                    {
                        this.FindNewSkinandSkel_Mod(workerSpineAnimatorData, dictionary, dictionary2, dictionary3, dictionary4);

                        this.nameDic.Add(workerSpineAnimatorData.name, workerSpineAnimatorData);
                        this.nameDicMod.Add(new KeyValuePairSS(string.Empty, workerSpineAnimatorData.name), workerSpineAnimatorData);
                        this.idDic.Add(workerSpineAnimatorData.id, workerSpineAnimatorData);
                    }
                    catch (Exception arg)
                    {
                        Debug.LogError(workerSpineAnimatorData.name + Environment.NewLine + arg);
                    }
                    //this.GetClipInfo(workerSpineAnimatorData);
                }
                if (dictionary3.Count > 0)
                {
                    foreach (KeyValuePair<KeyValuePairSS, object> keyValuePair in dictionary3)
                    {
                        char[] separator = new char[]
                        {
                            '_'
                        };
                        string text = keyValuePair.Key.value.Split(separator)[0];
                        if (this.nameDic.ContainsKey(text))
                        {
                            WorkerSpineAnimatorData workerSpineAnimatorData2 = this.nameDic[text];
                            WorkerSpineAnimatorData workerSpineAnimatorData3 = new WorkerSpineAnimatorData(workerSpineAnimatorData2.id + 10000000, keyValuePair.Key.value, workerSpineAnimatorData2.animatorSrc, workerSpineAnimatorData2.skeletonSrc);
                            workerSpineAnimatorData3.LoadData();
                            DirectoryInfo directoryInfo3 = new DirectoryInfo(dictionary4[keyValuePair.Key]);
                            List<Texture2D> list = new List<Texture2D>();
                            foreach (FileInfo fileInfo in directoryInfo3.GetFiles())
                            {
                                if (fileInfo.Name.Contains(".png"))
                                {
                                    byte[] data2 = File.ReadAllBytes(directoryInfo3.FullName + "/" + fileInfo.Name);
                                    Texture2D texture2D = new Texture2D(2, 2);
                                    texture2D.LoadImage(data2);
                                    texture2D.name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                                    list.Add(texture2D);
                                }
                            }
                            string atlasText = File.ReadAllText(directoryInfo3.FullName + "/atlas.txt");
                            Shader shader = null;
                            Spine.Unity.AtlasAsset atlasAsset = Spine.Unity.AtlasAsset.CreateRuntimeInstance(atlasText, list.ToArray(), shader, true);
                            new Spine.AtlasAttachmentLoader(new Spine.Atlas[]
                            {
                                atlasAsset.GetAtlas()
                            });
                            Spine.Unity.SkeletonDataAsset skeletonData;
                            if (keyValuePair.Value is string)
                            {
                                skeletonData = Spine.Unity.SkeletonDataAsset.CreateRuntimeInstance((string)keyValuePair.Value, atlasAsset, true, workerSpineAnimatorData3.skeletonData.scale);
                            }
                            else
                            {
                                skeletonData = Spine.Unity.SkeletonDataAsset.CreateRuntimeInstance((byte[])keyValuePair.Value, atlasAsset, true, workerSpineAnimatorData3.skeletonData.scale);
                            }
                            workerSpineAnimatorData3.skeletonData = skeletonData;
                            this.nameDicMod.Add(keyValuePair.Key, workerSpineAnimatorData3);
                        }
                        else if (text == "Custom")
                        {
                            WorkerSpineAnimatorData workerSpineAnimatorData4 = new WorkerSpineAnimatorData(keyValuePair.Key.GetHashCode(), keyValuePair.Key.value);
                            DirectoryInfo directoryInfo4 = new DirectoryInfo(dictionary4[keyValuePair.Key]);
                            List<Texture2D> list2 = new List<Texture2D>();
                            foreach (FileInfo fileInfo2 in directoryInfo4.GetFiles())
                            {
                                if (fileInfo2.Name.Contains(".png"))
                                {
                                    byte[] data3 = File.ReadAllBytes(directoryInfo4.FullName + "/" + fileInfo2.Name);
                                    Texture2D texture2D2 = new Texture2D(2, 2);
                                    texture2D2.LoadImage(data3);
                                    texture2D2.name = Path.GetFileNameWithoutExtension(fileInfo2.Name);
                                    list2.Add(texture2D2);
                                }
                            }
                            string atlasText2 = File.ReadAllText(directoryInfo4.FullName + "/atlas.txt");
                            Shader shader2 = null;
                            Spine.Unity.AtlasAsset atlasAsset2 = Spine.Unity.AtlasAsset.CreateRuntimeInstance(atlasText2, list2.ToArray(), shader2, true);
                            new Spine.AtlasAttachmentLoader(new Spine.Atlas[]
                            {
                                atlasAsset2.GetAtlas()
                            });
                            Spine.Unity.SkeletonDataAsset skeletonDataAsset;
                            if (keyValuePair.Value is string)
                            {
                                skeletonDataAsset = Spine.Unity.SkeletonDataAsset.CreateRuntimeInstance((string)keyValuePair.Value, atlasAsset2, true, 0.01f);
                            }
                            else
                            {
                                skeletonDataAsset = Spine.Unity.SkeletonDataAsset.CreateRuntimeInstance((byte[])keyValuePair.Value, atlasAsset2, true, 0.01f);
                            }
                            workerSpineAnimatorData4.skeletonData = skeletonDataAsset;
                            if (skeletonDataAsset.controller != null)
                            {
                                File.WriteAllText(Application.dataPath + "/BaseMods/controller.txt", "");
                            }
                            this.nameDicMod.Add(keyValuePair.Key, workerSpineAnimatorData4);
                        }
                    }
                }
                this._isLoaded = true;
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/BaseMods/error2.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }



        [MemberAlias("FNSS_skel", typeof(WorkerSpineAnimatorManager))]
        public void FNSS_skel(WorkerSpineAnimatorData data, byte[] nskel)
        {
        }
        [MemberAlias("FNSS_skel", typeof(WorkerSpineAnimatorManager))]
        public void FNSS_skel(WorkerSpineAnimatorData data, string nskel)
        {
        }
        [MemberAlias("FNSS_skin",typeof(WorkerSpineAnimatorManager))]
        public void FNSS_skin(WorkerSpineAnimatorData data, string dir)
        {
        }

        [NewMember]
        private Dictionary<KeyValuePairSS, WorkerSpineAnimatorData> nameDicMod = new Dictionary<KeyValuePairSS, WorkerSpineAnimatorData>();



        [MemberAlias("_isLoaded", typeof(WorkerSpineAnimatorManager))]
        private bool _isLoaded;
        [MemberAlias("nameDic", typeof(WorkerSpineAnimatorManager))]
        private Dictionary<string, WorkerSpineAnimatorData> nameDic = new Dictionary<string, WorkerSpineAnimatorData>();
        [MemberAlias("idDic", typeof(WorkerSpineAnimatorManager))]
        private Dictionary<int, WorkerSpineAnimatorData> idDic = new Dictionary<int, WorkerSpineAnimatorData>();
    }
    [ModifiesType("WeaponSetter")]
    public class WeaponSetter_patch
    {
        [ModifiesMember("SetWeapon")]
        public void SetWeapon_patch(global::WeaponModel weapon)
        {
            KeyValuePairSS SS = new KeyValuePairSS(EquipmentTypeInfo_patch.GetLcId(weapon.metaInfo).packageId, weapon.metaInfo.sprite);
            global::WeaponClassType weaponClassType = weapon.metaInfo.weaponClassType;
            int num = (int)weaponClassType;
            if (weaponClassType != global::WeaponClassType.SPECIAL)
            {
                if (weaponClassType == global::WeaponClassType.OFFICER)
                {
                    this.get_animator().SetBool("UniqueBattleMove", false);
                    this.get_animator().SetInteger("WeaponId", num);
                    this.get_animator().SetBool("TwoHanded", false);
                    this.setter.SetRightWeapon(global::WeaponClassType.OFFICER, null);
                    if (weapon.metaInfo.sprite != string.Empty)
                    {
                        Sprite weaponSprite = ((WorkerSpriteManager_patch)(object)WorkerSpriteManager.instance).GetWeaponSprite_Mod(weaponClassType, SS);
                        this.setter.SetRightWeapon(weaponClassType, weaponSprite);
                    }
                }
                else if (weaponClassType == global::WeaponClassType.FIST)
                {
                    int id = (int)float.Parse(weapon.metaInfo.sprite);
                    Sprite[] fistSprite = global::WorkerSprite.WorkerSprite_WorkerSpriteManager.instance.GetFistSprite(id);
                    if (fistSprite[0] == null || fistSprite[1] == null)
                    {
                        return;
                    }
                    this.isTwohanded = true;
                    num -= 4;
                    this.get_animator().SetBool("UniqueBattleMove", true);
                    this.setter.SetLeftWeapon(global::WeaponClassType.FIST, fistSprite[0]);
                    this.setter.SetRightWeapon(global::WeaponClassType.FIST, fistSprite[1]);
                    this.weaponId = num;
                    this.uniqueMovement = (num >= 3);
                    this.get_animator().SetInteger("WeaponId", this.weaponId);
                    this.get_animator().SetBool("TwoHanded", this.isTwohanded);
                    return;
                }
                else
                {
                    if (num >= 4)
                    {
                        num -= 4;
                        this.isTwohanded = true;
                        this.uniqueMovement = (num >= 3);
                        if (num >= 3)
                        {
                            this.get_animator().SetBool("UniqueBattleMove", true);
                        }
                        else
                        {
                            this.get_animator().SetBool("UniqueBattleMove", false);
                        }
                    }
                    else
                    {
                        this.isTwohanded = false;
                    }
                    this.weaponId = num;
                    this.get_animator().SetInteger("WeaponId", this.weaponId);
                    this.get_animator().SetBool("TwoHanded", this.isTwohanded);
                    if (weapon.metaInfo.sprite != string.Empty)
                    {
                        Sprite weaponSprite2 = ((WorkerSpriteManager_patch)(object)WorkerSpriteManager.instance).GetWeaponSprite_Mod(weaponClassType, SS);
                        this.setter.SetRightWeapon(weaponClassType, weaponSprite2);
                    }
                }
            }
        }


        [MemberAlias("get_animator", typeof(WeaponSetter))]
        private Animator get_animator()
        {
            return null;
        }


        [MemberAlias("weaponId", typeof(WeaponSetter))]
        public int weaponId;
        [MemberAlias("isTwohanded", typeof(WeaponSetter))]
        public bool isTwohanded;
        [MemberAlias("MoveWeapon", typeof(WeaponSetter))]
        public SpriteRenderer MoveWeapon;
        [MemberAlias("changer", typeof(WeaponSetter))]
        private WorkerSpine.AgentSpriteChanger changer;
        [MemberAlias("setter", typeof(WeaponSetter))]
        private global::WorkerSprite.WorkerSpriteSetter setter;
        [MemberAlias("uniqueMovement",typeof(WeaponSetter))]
        private bool uniqueMovement;
    }
    [ModifiesType("WeaponModel")]
    public class WeaponModel_patch
    {
        [ModifiesMember("OnAttack")]
        public string OnAttack_patch(global::UnitModel actor, global::UnitModel target)
        {
            this.fireEffectRunned = false;
            this.remainDelay = 8f;
            this.currentTarget = target;
            global::EquipmentScriptBase.WeaponDamageInfo weaponDamageInfo = this.script.OnAttackStart(actor, target);
            this._currentDamageInfos = new Queue<global::DamageInfo>(weaponDamageInfo.dmgs);
            if (weaponDamageInfo.dmgs[0].soundInfo != null && weaponDamageInfo.dmgs[0].soundInfo.soundType == global::DamageInfo_EffectType.ANIM_START)
            {
                ((SoundInfo_patch)(object)weaponDamageInfo.dmgs[0].soundInfo).PlaySound_Mod(EquipmentTypeInfo_patch.GetLcId(metaInfo).packageId, target.GetCurrentViewPosition());
            }
            if (weaponDamageInfo.dmgs[0].effectInfos.Count > 0)
            {
                foreach (global::EffectInfo effectInfo in weaponDamageInfo.dmgs[0].effectInfos)
                {
                    if (effectInfo.effectType == global::DamageInfo_EffectType.ANIM_START)
                    {
                        if (effectInfo.invokedUnit == global::EffectInvokedUnit.OWNER)
                        {
                            if (!this.fireEffectRunned)
                            {
                                effectInfo.MakeEffect(actor.GetMovableNode());
                                this.fireEffectRunned = true;
                            }
                        }
                        else
                        {
                            effectInfo.MakeEffect(target.GetMovableNode());
                        }
                    }
                }
            }
            return weaponDamageInfo.animationName;
        }
        [ModifiesMember("InvokeEffect")]
        public void InvokeEffect_patch(global::UnitModel unit, global::DamageInfo damageInfo, global::UnitDirection dir)
        {
            RwbpType type = damageInfo.type;
            DefenseInfo defense = unit.defense;
            if (damageInfo.soundInfo != null && damageInfo.soundInfo.soundType == global::DamageInfo_EffectType.DAMAGE_INVOKED)
            {
                ((SoundInfo_patch)(object)damageInfo.soundInfo).PlaySound_Mod(EquipmentTypeInfo_patch.GetLcId(metaInfo).packageId, unit.GetCurrentViewPosition());
            }
            if (damageInfo.effectInfos.Count > 0)
            {
                foreach (global::EffectInfo effectInfo in damageInfo.effectInfos)
                {
                    if (effectInfo.effectType == global::DamageInfo_EffectType.DAMAGE_INVOKED)
                    {
                        if (effectInfo.invokedUnit == global::EffectInvokedUnit.OWNER)
                        {
                            if (!effectInfo.invokeOnce)
                            {
                                this.fireEffectRunned = false;
                            }
                            if (!this.fireEffectRunned)
                            {
                                effectInfo.MakeEffect(get_owner().GetMovableNode());
                                this.fireEffectRunned = true;
                            }
                        }
                        else
                        {
                            effectInfo.MakeEffect(unit.GetMovableNode());
                        }
                    }
                }
            }
            global::DamageParticleEffect damageParticleEffect = global::DamageParticleEffect.Invoker(unit, type, defense, dir);
        }

        [MemberAlias("get_owner", typeof(EquipmentModel))]
        public global::UnitModel get_owner()
        {
            return null;
        }
        [MemberAlias("metaInfo", typeof(EquipmentModel))]
        public global::EquipmentTypeInfo metaInfo;
        [MemberAlias("script", typeof(EquipmentModel))]
        public global::EquipmentScriptBase script;
        [MemberAlias("currentTarget", typeof(EquipmentModel))]
        public global::UnitModel currentTarget;




        [MemberAlias("remainDelay", typeof(WeaponModel))]
        public float remainDelay;
        [MemberAlias("fireEffectRunned", typeof(WeaponModel))]
        private bool fireEffectRunned;
        [MemberAlias("_currentDamageInfos",typeof(WeaponModel))]
        private Queue<DamageInfo> _currentDamageInfos = new Queue<DamageInfo>();
    }
    [ModifiesType("WorkerSprite.WorkerSpriteSetter")]
    public class WorkerSpriteSetter_patch
    {
        [ModifiesMember("UpdateAttachment")]
        public void UpdateAttachment_patch()
        {
            List<SpineChangeData> list = new List<SpineChangeData>();
            using (Dictionary<global::EGOgiftAttachRegion, EGOGiftRenderData>.ValueCollection.Enumerator enumerator = this.replaceGiftData.Values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    EGOGiftRenderData r = enumerator.Current;
                    SpineChangeData spineChangeData = new SpineChangeData
                    {
                        sprite = r.Sprite,
                        slot = r.slot,
                        attachmentName = r.attachmentName
                    };
                    try
                    {
                        if (r.region == global::EGOgiftAttachRegion.RIGHTHAND && r.attachType == global::EGOgiftAttachType.REPLACE)
                        {
                            global::EGOgiftModel egogiftModel = this.get_Model().Equipment.gifts.replacedGifts.Find((global::EGOgiftModel x) => EquipmentTypeInfo_patch.GetLcId(x.metaInfo) == EGOGiftRenderData_patch.GetLcId(r));
                            if (egogiftModel != null && !this.get_Model().Equipment.gifts.GetDisplayState(egogiftModel))
                            {
                                spineChangeData.sprite = global::WorkerSpriteManager.instance.righthand;
                                list.Add(spineChangeData);
                                continue;
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    if (r.region == global::EGOgiftAttachRegion.MOUTH || r.region == global::EGOgiftAttachRegion.EYE)
                    {
                        if (r.region == global::EGOgiftAttachRegion.EYE)
                        {
                            this.get_workerSpriteData().replaced.Eye = r.Sprite;
                            this.currentSpriteSet.Eye = r.Sprite;
                            this.get_workerSpriteData().EyeColor = Color.white;
                            this.EyeApply();
                        }
                        else if (r.region == global::EGOgiftAttachRegion.MOUTH)
                        {
                            this.MouthApply();
                        }
                    }
                    else
                    {
                        list.Add(spineChangeData);
                    }
                }
            }
            this.Apply(list);
        }
        [ModifiesMember("AddGiftModel")]
        public void AddGiftModel_patch(global::EGOgiftModel gift)
        {
            if (!this.currentGift.Contains(gift))
            {
                this.currentGift.Add(gift);
            }
            EGOGiftRenderData egogiftRenderData = null;
            global::EGOgiftAttachRegion egogiftAttachRegion = global::EGOgiftAttachRegion.EYE;
            string sprite = gift.metaInfo.sprite;
            string empty = string.Empty;
            string empty2 = string.Empty;
            if (global::EGOGiftRegionKey.ParseRegion(gift.metaInfo.attachPos, out egogiftAttachRegion))
            {
                Sprite attachmentSprite = global::WorkerSpriteManager.instance.GetAttachmentSprite(egogiftAttachRegion, sprite);
                if (attachmentSprite == null)
                {
                    Debug.LogError("Couldn't find : " + gift.metaInfo.Name + " Searched as " + sprite);
                    return;
                }
                if (gift.metaInfo.attachType == global::EGOgiftAttachType.ADD || gift.metaInfo.attachType == global::EGOgiftAttachType.SPECIAL_ADD)
                {
                    if (this.attachGiftData.TryGetValue(global::UnitEGOgiftSpace.GetRegionId(gift.metaInfo), out egogiftRenderData))
                    {
                        egogiftRenderData.Sprite = attachmentSprite;
                        egogiftRenderData.DataName = gift.metaInfo.Name;
                        egogiftRenderData.metaId = (long)gift.metaInfo.id;
                        ((EGOGiftRenderData_patch)(object)egogiftRenderData).modid = EquipmentTypeInfo_patch.GetLcId(gift.metaInfo).packageId;
                        this.AddGift(egogiftRenderData);
                        return;
                    }
                    if (!global::EGOGiftRegionKey.GetRegionKey(egogiftAttachRegion, out empty, out empty2))
                    {
                        Debug.LogError("Error " + egogiftAttachRegion);
                        return;
                    }
                    egogiftRenderData = new EGOGiftRenderData
                    {
                        Sprite = attachmentSprite,
                        slot = empty,
                        attachmentName = empty2,
                        DataName = gift.metaInfo.Name,
                        region = egogiftAttachRegion,
                        attachType = gift.metaInfo.attachType,
                        metaId = (long)gift.metaInfo.id
                    };
                    ((EGOGiftRenderData_patch)(object)egogiftRenderData).modid = EquipmentTypeInfo_patch.GetLcId(gift.metaInfo).packageId;
                    this.attachGiftData.Add(global::UnitEGOgiftSpace.GetRegionId(gift.metaInfo), egogiftRenderData);
                    this.AddGift(egogiftRenderData);
                    return;
                }
                else
                {
                    if (egogiftAttachRegion == global::EGOgiftAttachRegion.BACK)
                    {
                        return;
                    }
                    if (egogiftAttachRegion == global::EGOgiftAttachRegion.BACK2)
                    {
                        return;
                    }
                    if (egogiftAttachRegion == global::EGOgiftAttachRegion.HEADBACK)
                    {
                        return;
                    }
                    if (this.replaceGiftData.TryGetValue(egogiftAttachRegion, out egogiftRenderData))
                    {
                        egogiftRenderData.Sprite = attachmentSprite;
                        egogiftRenderData.DataName = gift.metaInfo.Name;
                        this.ReplaceGift(egogiftRenderData);
                        return;
                    }
                    if (!global::EGOGiftRegionKey.GetRegionKey(egogiftAttachRegion, out empty, out empty2))
                    {
                        Debug.LogError("Error " + egogiftAttachRegion);
                        return;
                    }
                    egogiftRenderData = new EGOGiftRenderData
                    {
                        Sprite = attachmentSprite,
                        slot = empty,
                        attachmentName = empty2,
                        DataName = gift.metaInfo.Name,
                        region = egogiftAttachRegion,
                        attachType = gift.metaInfo.attachType,
                        metaId = (long)gift.metaInfo.id
                    };
                    ((EGOGiftRenderData_patch)(object)egogiftRenderData).modid = EquipmentTypeInfo_patch.GetLcId(gift.metaInfo).packageId;
                    this.replaceGiftData.Add(egogiftAttachRegion, egogiftRenderData);
                    this.ReplaceGift(egogiftRenderData);
                }
            }
        }
        [NewMember]
        public void ArmorEquip_Mod(LcId armorId)
        {
            this.armorId = armorId.id;
            ((WorkerSpriteManager_patch)(object)WorkerSpriteManager.instance).GetArmorData_Mod(armorId, ref get_Model().spriteData);
            this.UpdateArmorSpriteSet();
            this.ArmorApply();
        }
        [ModifiesMember("ArmorEquip")]
        public void ArmorEquip_patch(int armorId)
        {
            ArmorEquip_Mod(new LcId(armorId));
        }





        [MemberAlias("Apply", typeof(WorkerSpriteSetter))]
        public void Apply(List<SpineChangeData> data)
        {
        }
        [MemberAlias("MouthApply", typeof(WorkerSpriteSetter))]
        public void MouthApply()
        {
        }
        [MemberAlias("EyeApply", typeof(WorkerSpriteSetter))]
        public void EyeApply()
        {
        }
        [MemberAlias("get_workerSpriteData", typeof(WorkerSpriteSetter))]
        public WorkerSprite.WorkerSprite get_workerSpriteData()
        {
            return this._model.spriteData;
        }
        [MemberAlias("ReplaceGift", typeof(WorkerSpriteSetter))]
        public void ReplaceGift(EGOGiftRenderData renderData)
        {
        }
        [MemberAlias("AddGift", typeof(WorkerSpriteSetter))]
        public void AddGift(EGOGiftRenderData renderData)
        {
        }
        [MemberAlias("ArmorApply", typeof(WorkerSpriteSetter))]
        public void ArmorApply()
        {
            
        }
        [MemberAlias("UpdateArmorSpriteSet", typeof(WorkerSpriteSetter))]
        public void UpdateArmorSpriteSet()
        {
        }
        [MemberAlias("get_Model", typeof(WorkerSpriteSetter))]
        public global::WorkerModel get_Model()
        {
            return null;
        }




        [MemberAlias("HeadRegion", typeof(WorkerSpriteSetter))]
        private const string HeadRegion = "Head";
        [MemberAlias("AddObjectSrc", typeof(WorkerSpriteSetter))]
        private const string AddObjectSrc = "Slot/WorkerAttachment";
        [MemberAlias("_model", typeof(WorkerSpriteSetter))]
        private global::WorkerModel _model;
        [MemberAlias("GiftPos", typeof(WorkerSpriteSetter))]
        public Transform[] GiftPos;
        [MemberAlias("currentSpriteSet", typeof(WorkerSpriteSetter))]
        public WorkerCurrentSpriteSet currentSpriteSet;
        [MemberAlias("attachGiftData", typeof(WorkerSpriteSetter))]
        private Dictionary<int, EGOGiftRenderData> attachGiftData = new Dictionary<int, EGOGiftRenderData>();
        [MemberAlias("replaceGiftData", typeof(WorkerSpriteSetter))]
        private Dictionary<global::EGOgiftAttachRegion, EGOGiftRenderData> replaceGiftData = new Dictionary<global::EGOgiftAttachRegion, EGOGiftRenderData>();
        [MemberAlias("separator", typeof(WorkerSpriteSetter))]
        private Spine.Unity.Modules.SkeletonRenderSeparator separator;
        [MemberAlias("currentWeaponType", typeof(WorkerSpriteSetter))]
        private global::WeaponClassType currentWeaponType = global::WeaponClassType.AXE;
        [MemberAlias("faceType", typeof(WorkerSpriteSetter))]
        public WorkerFaceType faceType;
        [MemberAlias("hairColor", typeof(WorkerSpriteSetter))]
        public Color hairColor;
        [MemberAlias("eyeColor", typeof(WorkerSpriteSetter))]
        public Color eyeColor;
        [MemberAlias("EyeRenderer", typeof(WorkerSpriteSetter))]
        public SpriteRenderer EyeRenderer;
        [MemberAlias("EyebrowRenderer", typeof(WorkerSpriteSetter))]
        public SpriteRenderer EyebrowRenderer;
        [MemberAlias("MouthRenderer", typeof(WorkerSpriteSetter))]
        public SpriteRenderer MouthRenderer;
        [MemberAlias("SymbolRenderer", typeof(WorkerSpriteSetter))]
        public SpriteRenderer SymbolRenderer;
        [MemberAlias("MouthReplaceGiftRender", typeof(WorkerSpriteSetter))]
        public SpriteRenderer MouthReplaceGiftRender;
        [MemberAlias("WeaponRenderer", typeof(WorkerSpriteSetter))]
        public SpriteRenderer WeaponRenderer;
        [MemberAlias("SetHeadSprite", typeof(WorkerSpriteSetter))]
        public bool SetHeadSprite;
        [MemberAlias("HeadSprite", typeof(WorkerSpriteSetter))]
        public Sprite HeadSprite;
        [MemberAlias("TransparentSprite", typeof(WorkerSpriteSetter))]
        public Sprite TransparentSprite;
        [MemberAlias("repack", typeof(WorkerSpriteSetter))]
        public bool repack = true;
        [MemberAlias("repackedShader", typeof(WorkerSpriteSetter))]
        public Shader repackedShader;
        [MemberAlias("NoteRenderer", typeof(WorkerSpriteSetter))]
        public SpriteRenderer NoteRenderer;
        [MemberAlias("runtimeAtlas", typeof(WorkerSpriteSetter))]
        public Texture2D runtimeAtlas;
        [MemberAlias("runtimeMaterial", typeof(WorkerSpriteSetter))]
        public Material runtimeMaterial;
        [MemberAlias("currentSkin", typeof(WorkerSpriteSetter))]
        public Spine.Skin currentSkin;
        [MemberAlias("panicRenderer", typeof(WorkerSpriteSetter))]
        public SpriteRenderer panicRenderer;
        [MemberAlias("_weaponPosition", typeof(WorkerSpriteSetter))]
        private Vector2 _weaponPosition = Vector2.zero;
        [MemberAlias("_weaponRotation", typeof(WorkerSpriteSetter))]
        private float _weaponRotation;
        [MemberAlias("_initWeaponData", typeof(WorkerSpriteSetter))]
        private bool _initWeaponData;
        [MemberAlias("debugCheck", typeof(WorkerSpriteSetter))]
        public bool debugCheck;
        [MemberAlias("armorId", typeof(WorkerSpriteSetter))]
        public int armorId = 1;
        [MemberAlias("_armorColored", typeof(WorkerSpriteSetter))]
        private bool _armorColored;
        [MemberAlias("currentGift", typeof(WorkerSpriteSetter))]
        private List<global::EGOgiftModel> currentGift = new List<global::EGOgiftModel>();
    }
    [ModifiesType("WorkerModel")]
    public class WorkerModel_patch
    {
        [ModifiesMember("GetWeaponSprite")]
        public Sprite GetWeaponSprite_patch()
        {
            Sprite result = null;
            global::WeaponClassType weaponClassType = get_Equipment().weapon.metaInfo.weaponClassType;
            if (weaponClassType == global::WeaponClassType.FIST)
            {
                int id = (int)float.Parse(get_Equipment().weapon.metaInfo.sprite);
                Sprite[] fistSprite = global::WorkerSprite.WorkerSprite_WorkerSpriteManager.instance.GetFistSprite(id);
                if (fistSprite[0] == null || fistSprite[1] == null)
                {
                    return result;
                }
                result = fistSprite[1];
            }
            else
            {
                KeyValuePairSS SS = new KeyValuePairSS(EquipmentTypeInfo_patch.GetLcId(get_Equipment().weapon.metaInfo).packageId, get_Equipment().weapon.metaInfo.sprite);
                Sprite weaponSprite = ((WorkerSpriteManager_patch)(object)WorkerSpriteManager.instance).GetWeaponSprite_Mod(weaponClassType, SS);
                result = weaponSprite;
            }
            return result;
        }

        [ModifiesMember("OnSetArmor")]
        protected void OnSetArmor_patch()
        {
            OnSetArmor();
            ((WorkerSpriteManager_patch)(object)WorkerSpriteManager.instance).GetArmorData_Mod(new LcId(EquipmentTypeInfo_patch.GetLcId(get_Equipment().armor.metaInfo).packageId, get_Equipment().armor.metaInfo.armorId), ref this.spriteData);
        }


        [MemberAlias("get_Equipment", typeof(UnitModel))]
        public global::UnitEquipSpace get_Equipment()
        {
            return null;
        }
        [MemberAlias("OnSetArmor",typeof(UnitModel))]
        protected void OnSetArmor()
        {
        }




        [MemberAlias("commandQueue", typeof(WorkerModel))]
        protected global::WorkerCommandQueue commandQueue;
        [MemberAlias("workerClass", typeof(WorkerModel))]
        public global::WorkerClass workerClass;
        [MemberAlias("isRealWorker", typeof(WorkerModel))]
        public bool isRealWorker = true;
        [MemberAlias("name", typeof(WorkerModel))]
        public string name;
        [MemberAlias("gender", typeof(WorkerModel))]
        public string gender;
        [MemberAlias("_currentSefira", typeof(WorkerModel))]
        private string _currentSefira;
        [MemberAlias("currentSefiraEnum", typeof(WorkerModel))]
        public global::SefiraEnum currentSefiraEnum = global::SefiraEnum.DUMMY;
        [MemberAlias("_revivalHp", typeof(WorkerModel))]
        protected bool _revivalHp;
        [MemberAlias("_revivalMental", typeof(WorkerModel))]
        protected bool _revivalMental;
        [MemberAlias("_revivaledHp", typeof(WorkerModel))]
        protected bool _revivaledHp;
        [MemberAlias("_revivaledMental", typeof(WorkerModel))]
        protected bool _revivaledMental;
        [MemberAlias("revivalProb", typeof(WorkerModel))]
        private const float revivalProb = 0.25f;
        [MemberAlias("movementMul", typeof(WorkerModel))]
        public float movementMul = 1f;
        [MemberAlias("panicValue", typeof(WorkerModel))]
        public int panicValue;
        [MemberAlias("invincible", typeof(WorkerModel))]
        public bool invincible;
        [MemberAlias("blockRecover", typeof(WorkerModel))]
        public bool blockRecover;
        [MemberAlias("stunTime", typeof(WorkerModel))]
        public float stunTime;
        [MemberAlias("haltUpdate", typeof(WorkerModel))]
        public bool haltUpdate;
        [MemberAlias("returnPanic", typeof(WorkerModel))]
        public bool returnPanic;
        [MemberAlias("willDead", typeof(WorkerModel))]
        public bool willDead;
        [MemberAlias("_isDead", typeof(WorkerModel))]
        protected bool _isDead;
        [MemberAlias("speechTable", typeof(WorkerModel))]
        public Dictionary<string, string> speechTable = new Dictionary<string, string>();
        [MemberAlias("target", typeof(WorkerModel))]
        public global::CreatureModel target;
        [MemberAlias("targetWorker", typeof(WorkerModel))]
        public global::WorkerModel targetWorker;
        [MemberAlias("targetObject", typeof(WorkerModel))]
        public global::StandingItemModel targetObject;
        [MemberAlias("currentPanicAction", typeof(WorkerModel))]
        private global::PanicAction currentPanicAction;
        [MemberAlias("unconAction", typeof(WorkerModel))]
        public global::UncontrollableAction unconAction;
        [MemberAlias("_recentlyAttacked", typeof(WorkerModel))]
        private global::CreatureModel _recentlyAttacked;
        [MemberAlias("animationMessageRecevied", typeof(WorkerModel))]
        public global::CreatureBase animationMessageRecevied;
        [MemberAlias("visible", typeof(WorkerModel))]
        public bool visible = true;
        [MemberAlias("waitTimer", typeof(WorkerModel))]
        public float waitTimer;
        [MemberAlias("OnWorkEndFlag", typeof(WorkerModel))]
        public bool OnWorkEndFlag;
        [MemberAlias("puppetChanged", typeof(WorkerModel))]
        public bool puppetChanged;
        [MemberAlias("lastestMoveTarget", typeof(WorkerModel))]
        public global::MapNode lastestMoveTarget;
        [MemberAlias("_attackTargetWorker", typeof(WorkerModel))]
        private global::WorkerModel _attackTargetWorker;
        [MemberAlias("_specialDeadScene", typeof(WorkerModel))]
        private bool _specialDeadScene;
        [MemberAlias("deadSceneName", typeof(WorkerModel))]
        protected string deadSceneName;
        [MemberAlias("seperator", typeof(WorkerModel))]
        protected bool seperator = true;
        [MemberAlias("hasUniqueFace", typeof(WorkerModel))]
        protected bool hasUniqueFace;
        [MemberAlias("hairSprite", typeof(WorkerModel))]
        public Sprite hairSprite;
        [MemberAlias("faceSprite", typeof(WorkerModel))]
        public Sprite faceSprite;
        [MemberAlias("stunEffect", typeof(WorkerModel))]
        public GameObject stunEffect;
        [MemberAlias("spriteData", typeof(WorkerModel))]
        public global::WorkerSprite.WorkerSprite spriteData;
        [MemberAlias("_panicData", typeof(WorkerModel))]
        protected global::PanicData _panicData;
        [MemberAlias("isChangeableAnimator", typeof(WorkerModel))]
        public bool isChangeableAnimator = true;
        [MemberAlias("_deadType", typeof(WorkerModel))]
        private global::DeadType _deadType;
    }
    [ModifiesType("WorkerSpriteManager")]
    public class WorkerSpriteManager_patch
    {

        [NewMember]
        public void LoadModSprites(DirectoryInfo dir)
        {
            CurModPath = dir.FullName;
            DirectoryInfo directoryInfo = new DirectoryInfo(ModFolderSrc);
            DirectoryInfo directoryInfo4 = new DirectoryInfo(ModEyeSrc);
            DirectoryInfo directoryInfo5 = new DirectoryInfo(ModPanicEyeSrc);
            DirectoryInfo directoryInfo6 = new DirectoryInfo(ModDeadEyeSrc);
            DirectoryInfo directoryInfo7 = new DirectoryInfo(ModEyebrowSrc);
            DirectoryInfo directoryInfo8 = new DirectoryInfo(ModBattleEyebrowSrc);
            DirectoryInfo directoryInfo9 = new DirectoryInfo(ModPanicEyebrowSrc);
            DirectoryInfo directoryInfo10 = new DirectoryInfo(ModMouthSrc);
            DirectoryInfo directoryInfo11 = new DirectoryInfo(ModBattleMouthSrc);
            DirectoryInfo directoryInfo12 = new DirectoryInfo(ModFrontHairSrc);
            DirectoryInfo directoryInfo13 = new DirectoryInfo(ModRearHairSrc);
            if (!directoryInfo.Exists)
            {
                return;
            }
            if (directoryInfo4.Exists)
            {
                this.LoadCustomSprite(directoryInfo4, BasicSpriteRegion.EYE_DEFAULT, WorkerSpriteManager.SizeRef.Eye());
            }
            if (directoryInfo5.Exists)
            {
                this.LoadCustomSprite(directoryInfo5, BasicSpriteRegion.EYE_PANIC, WorkerSpriteManager.SizeRef.Eye());
            }
            if (directoryInfo6.Exists)
            {
                this.LoadCustomSprite(directoryInfo6, BasicSpriteRegion.EYE_DEAD, WorkerSpriteManager.SizeRef.Eye());
            }
            if (directoryInfo7.Exists)
            {
                this.LoadCustomSprite(directoryInfo7, BasicSpriteRegion.EYEBROW, WorkerSpriteManager.SizeRef.Eyebrow());
            }
            if (directoryInfo8.Exists)
            {
                this.LoadCustomSprite(directoryInfo8, BasicSpriteRegion.EYEBROW_BATTLE, WorkerSpriteManager.SizeRef.Eyebrow());
            }
            if (directoryInfo9.Exists)
            {
                this.LoadCustomSprite(directoryInfo9, BasicSpriteRegion.EYEBROW_PANIC, WorkerSpriteManager.SizeRef.Eyebrow());
            }
            if (directoryInfo10.Exists)
            {
                this.LoadCustomSprite(directoryInfo10, BasicSpriteRegion.MOUTH, WorkerSpriteManager.SizeRef.Mouth());
            }
            if (directoryInfo11.Exists)
            {
                this.LoadCustomSprite(directoryInfo11, BasicSpriteRegion.MOUTH_BATTLE, WorkerSpriteManager.SizeRef.Mouth());
            }
            if (directoryInfo12.Exists)
            {
                this.LoadCustomSprite(directoryInfo12, BasicSpriteRegion.HAIR_FRONT, WorkerSpriteManager.SizeRef.FrontHair());
            }
            if (directoryInfo13.Exists)
            {
                this.LoadCustomSprite(directoryInfo13, BasicSpriteRegion.HAIR_REAR, WorkerSpriteManager.SizeRef.RearHair());
            }
        }
        [ModifiesMember("LoadCustomSprites")]
        public void LoadCustomSprites_Patch()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(get_CustomFolderSrc());
            DirectoryInfo directoryInfo2 = new DirectoryInfo(get_CustomFaceSrc());
            DirectoryInfo directoryInfo3 = new DirectoryInfo(get_CustomHairSrc());
            DirectoryInfo directoryInfo4 = new DirectoryInfo(get_CustomEyeSrc());
            DirectoryInfo directoryInfo5 = new DirectoryInfo(get_CustomPanicEyeSrc());
            DirectoryInfo directoryInfo6 = new DirectoryInfo(get_CustomDeadEyeSrc());
            DirectoryInfo directoryInfo7 = new DirectoryInfo(get_CustomEyebrowSrc());
            DirectoryInfo directoryInfo8 = new DirectoryInfo(get_CustomBattleEyebrowSrc());
            DirectoryInfo directoryInfo9 = new DirectoryInfo(get_CustomPanicEyebrowSrc());
            DirectoryInfo directoryInfo10 = new DirectoryInfo(get_CustomMouthSrc());
            DirectoryInfo directoryInfo11 = new DirectoryInfo(get_CustomBattleMouthSrc());
            DirectoryInfo directoryInfo12 = new DirectoryInfo(get_CustomFrontHairSrc());
            DirectoryInfo directoryInfo13 = new DirectoryInfo(get_CustomRearHairSrc());
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
                Debug.Log("Not Exist CustomFoldoer So Make Directory : " + get_CustomFolderSrc());
                directoryInfo2.Create();
                directoryInfo3.Create();
                directoryInfo4.Create();
                directoryInfo5.Create();
                directoryInfo6.Create();
                directoryInfo7.Create();
                directoryInfo8.Create();
                directoryInfo9.Create();
                directoryInfo10.Create();
                directoryInfo11.Create();
                directoryInfo12.Create();
                directoryInfo13.Create();
            }
            if (!directoryInfo2.Exists)
            {
                directoryInfo2.Create();
                Debug.Log("Not Exist CustomFaceFolder So Make Directory : " + get_CustomFaceSrc());
            }
            if (!directoryInfo3.Exists)
            {
                directoryInfo3.Create();
                if (!directoryInfo12.Exists)
                {
                    directoryInfo12.Create();
                }
                if (!directoryInfo13.Exists)
                {
                    directoryInfo13.Create();
                }
                Debug.Log("Not Exist CustomHairFolder So Make Directory : " + get_CustomHairSrc());
            }
            if (!directoryInfo4.Exists)
            {
                directoryInfo4.Create();
            }
            if (!directoryInfo5.Exists)
            {
                directoryInfo5.Create();
            }
            if (!directoryInfo6.Exists)
            {
                directoryInfo6.Create();
            }
            if (!directoryInfo7.Exists)
            {
                directoryInfo7.Create();
            }
            if (!directoryInfo8.Exists)
            {
                directoryInfo8.Create();
            }
            if (!directoryInfo9.Exists)
            {
                directoryInfo9.Create();
            }
            if (!directoryInfo10.Exists)
            {
                directoryInfo10.Create();
            }
            if (!directoryInfo11.Exists)
            {
                directoryInfo11.Create();
            }
            if (!directoryInfo12.Exists)
            {
                directoryInfo12.Create();
            }
            if (!directoryInfo13.Exists)
            {
                directoryInfo13.Create();
            }
            this.LoadCustomSprite(directoryInfo4, BasicSpriteRegion.EYE_DEFAULT, WorkerSpriteManager.SizeRef.Eye());
            this.LoadCustomSprite(directoryInfo5, BasicSpriteRegion.EYE_PANIC, WorkerSpriteManager.SizeRef.Eye());
            this.LoadCustomSprite(directoryInfo6, BasicSpriteRegion.EYE_DEAD, WorkerSpriteManager.SizeRef.Eye());
            this.LoadCustomSprite(directoryInfo7, BasicSpriteRegion.EYEBROW, WorkerSpriteManager.SizeRef.Eyebrow());
            this.LoadCustomSprite(directoryInfo8, BasicSpriteRegion.EYEBROW_BATTLE, WorkerSpriteManager.SizeRef.Eyebrow());
            this.LoadCustomSprite(directoryInfo9, BasicSpriteRegion.EYEBROW_PANIC, WorkerSpriteManager.SizeRef.Eyebrow());
            this.LoadCustomSprite(directoryInfo10, BasicSpriteRegion.MOUTH, WorkerSpriteManager.SizeRef.Mouth());
            this.LoadCustomSprite(directoryInfo11, BasicSpriteRegion.MOUTH_BATTLE, WorkerSpriteManager.SizeRef.Mouth());
            this.LoadCustomSprite(directoryInfo12, BasicSpriteRegion.HAIR_FRONT, WorkerSpriteManager.SizeRef.FrontHair());
            this.LoadCustomSprite(directoryInfo13, BasicSpriteRegion.HAIR_REAR, WorkerSpriteManager.SizeRef.RearHair());

            foreach (DirectoryInfo dir in Add_On.instance.DirList)
            {
                LoadModSprites(dir);
            }
           
        }
        [NewMember]
        public string ModFolderSrc
        {
            get
            {
                return CurModPath + "/BaseModCustomData";
            }
        }
        [NewMember]
        public string CurModPath;
        [NewMember]
        public string ModHairSrc
        {
            get
            {
                return this.ModFolderSrc + "/Hair";
            }
        }
        [NewMember]
        public string ModFaceSrc
        {
            get
            {
                return this.ModFolderSrc + "/Face";
            }
        }
        [NewMember]
        public string ModFrontHairSrc
        {
            get
            {
                return this.ModHairSrc + "/Front";
            }
        }
        [NewMember]
        public string ModRearHairSrc
        {
            get
            {
                return this.ModHairSrc + "/Rear";
            }
        }
        [NewMember]
        public string ModEyeSrc
        {
            get
            {
                return this.ModFaceSrc + "/Eye_Default";
            }
        }
        [NewMember]
        public string ModPanicEyeSrc
        {
            get
            {
                return this.ModFaceSrc + "/Eye_Panic";
            }
        }
        [NewMember]
        public string ModDeadEyeSrc
        {
            get
            {
                return this.ModFaceSrc + "/Eye_Dead";
            }
        }
        [NewMember]
        public string ModMouthSrc
        {
            get
            {
                return this.ModFaceSrc + "/Mouth_Default";
            }
        }
        [NewMember]
        public string ModBattleMouthSrc
        {
            get
            {
                return this.ModFaceSrc + "/Mouth_Battle";
            }
        }
        [NewMember]
        public string ModEyebrowSrc
        {
            get
            {
                return this.ModFaceSrc + "/Eyebrow_Default";
            }
        }
        [NewMember]
        public string ModBattleEyebrowSrc
        {
            get
            {
                return this.ModFaceSrc + "/Eyebrow_Battle";
            }
        }
        [NewMember]
        public string ModPanicEyebrowSrc
        {
            get
            {
                return this.ModFaceSrc + "/Eyebrow_Panic";
            }
        }
        [NewMember]
        public bool GetUniqueWeaponSpriteInfo_Mod(KeyValuePairSS SS, out UniqueWeaponSpriteUnit unit)
        {
            if (SS.key == String.Empty)
            {
                GetWeaponSprite_patch(WeaponClassType.SPECIAL, SS.value);
                return _uniqueWeaponDic.TryGetValue(SS.value + "_AbcdcodeMade", out unit);
            } else
            {
                GetWeaponSprite_Mod(WeaponClassType.SPECIAL, SS);
                return uniqueWeaponDicMod.TryGetValue(SS, out unit);
            }
        }
        [ModifiesMember("GetUniqueWeaponSpriteInfo")]
        public bool GetUniqueWeaponSpriteInfo_patch(string name, out UniqueWeaponSpriteUnit unit)
        {
            return GetUniqueWeaponSpriteInfo_Mod(new KeyValuePairSS(String.Empty, name), out unit);
        }
        [NewMember]
        public bool SpecialFindDir_Texture_Mod(string modid, string name, ref DirectoryInfo info, ref Texture2D tex)
        {
            bool flag = false;
            if (modid == String.Empty)
            {
                foreach (DirectoryInfo dir in global::Add_On.instance.DirList)
                {
                    DirectoryInfo directoryInfo = global::EquipmentDataLoader.CheckNamedDir(dir, "Equipment");
                    if (directoryInfo != null)
                    {
                        if (!Directory.Exists(directoryInfo.FullName + "/Sprite/Weapon"))
                        {
                            continue;
                        }
                        DirectoryInfo directoryInfo2 = new DirectoryInfo(directoryInfo.FullName + "/Sprite/Weapon");
                        if (directoryInfo2.GetFiles().Length != 0)
                        {
                            FileInfo[] files = directoryInfo2.GetFiles();
                            for (int i = 0; i < files.Length; i++)
                            {
                                if (files[i].Name == name + ".png")
                                {
                                    info = directoryInfo2;
                                    tex.LoadImage(File.ReadAllBytes(files[i].FullName));
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
                return flag;
            } else
            {
                ModInfo_patch pmodinfo = (ModInfo_patch)((Add_On_patch)(object)Add_On.instance).ModList.Find(x => ((ModInfo_patch)(object)x).modid == modid);
                if(pmodinfo != null)
                {
                    DirectoryInfo directoryInfo = global::EquipmentDataLoader.CheckNamedDir(pmodinfo.modpath, "Equipment");
                    if (Directory.Exists(directoryInfo.FullName + "/Sprite/Weapon"))
                    {
                        DirectoryInfo directoryInfo2 = new DirectoryInfo(directoryInfo.FullName + "/Sprite/Weapon");
                        if (directoryInfo2.GetFiles().Length != 0)
                        {
                            FileInfo[] files = directoryInfo2.GetFiles();
                            for (int i = 0; i < files.Length; i++)
                            {
                                if (files[i].Name == name + ".png")
                                {
                                    info = directoryInfo2;
                                    tex.LoadImage(File.ReadAllBytes(files[i].FullName));
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return flag;
        }
        [ModifiesMember("SpecialFindDir_Texture")]
        public bool SpecialFindDir_Texture_patch(string name, ref DirectoryInfo info, ref Texture2D tex)
        {
            return SpecialFindDir_Texture_Mod(string.Empty, name, ref info, ref tex);
        }
        [NewMember]
        public Sprite GetWeaponSprite_Mod(global::WeaponClassType type, KeyValuePairSS SS)
        {
            Sprite result = null;

            if (SS.key == String.Empty)
            {
                try
                {
                    global::WorkerSprite.WorkerEquipmentSprite workerEquipmentSprite = null;
                    if (type == global::WeaponClassType.SPECIAL)
                    {
                        if (this._uniqueWeaponDic.ContainsKey(SS.value))
                        {
                            global::WorkerSprite.UniqueWeaponSpriteUnit uniqueWeaponSpriteUnit = null;
                            if (this._uniqueWeaponDic.TryGetValue(SS.value + "_AbcdcodeMade", out uniqueWeaponSpriteUnit))
                            {
                                result = uniqueWeaponSpriteUnit.CommonSprite;
                            }
                            else
                            {
                                Texture2D texture = new Texture2D(256, 256);
                                DirectoryInfo dir = null;
                                bool flag = this.SpecialFindDir_Texture_patch(SS.value, ref dir, ref texture);
                                this._uniqueWeaponDic.Add(SS.value + "_AbcdcodeMade", this._uniqueWeaponDic[SS.value].GetCopy());
                                if (!flag)
                                {
                                    result = this._uniqueWeaponDic[SS.value + "_AbcdcodeMade"].CommonSprite;
                                    return result;
                                }
                                this.MakeNewUniqueWeaponSpriteUnit(SS.value, this._uniqueWeaponDic[SS.value + "_AbcdcodeMade"], dir, texture, out result);
                            }
                        }
                        else
                        {
                            char[] separator = new char[]
                            {
                    '_'
                            };
                            string key = SS.value.Split(separator)[0];
                            if (this._uniqueWeaponDic.ContainsKey(key))
                            {
                                Texture2D texture2 = new Texture2D(256, 256);
                                DirectoryInfo dir2 = null;
                                bool flag2 = this.SpecialFindDir_Texture_patch(SS.value, ref dir2, ref texture2);
                                this._uniqueWeaponDic.Add(SS.value, this._uniqueWeaponDic[key].GetCopy());
                                this._uniqueWeaponDic.Add(SS.value + "_AbcdcodeMade", this._uniqueWeaponDic[key].GetCopy());
                                if (!flag2)
                                {
                                    result = this._uniqueWeaponDic[SS.value + "_AbcdcodeMade"].CommonSprite;
                                    return result;
                                }
                                this.MakeNewUniqueWeaponSpriteUnit(SS.value, this._uniqueWeaponDic[SS.value + "_AbcdcodeMade"], dir2, texture2, out result);
                            }
                        }
                    }
                    else if (type != global::WeaponClassType.FIST)
                    {
                        if (this.equipData.GetData(global::WorkerSprite.EquipmentSpriteRegion.WEAPON, out workerEquipmentSprite))
                        {
                            global::WorkerSprite.WorkerWeaponSprite workerWeaponSprite = workerEquipmentSprite as global::WorkerSprite.WorkerWeaponSprite;
                            FieldInfo field = typeof(global::WorkerSprite.WorkerWeaponSprite.WeaponDatabase).GetField("data", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                            Dictionary<string, Sprite> dictionary = (Dictionary<string, Sprite>)field.GetValue(workerWeaponSprite.GetDb((int)type));
                            Sprite sprite = null;
                            try
                            {
                                if (!dictionary.ContainsKey(SS.value))
                                {
                                    if (type == global::WeaponClassType.SPEAR)
                                    {
                                        workerWeaponSprite.GetDb((int)type).GetSprite("Spear_Set_01_0", out sprite);
                                    }
                                    if (type == global::WeaponClassType.PISTOL)
                                    {
                                        workerWeaponSprite.GetDb((int)type).GetSprite("Pistol_Set_01_0", out sprite);
                                    }
                                    if (type == global::WeaponClassType.HAMMER)
                                    {
                                        workerWeaponSprite.GetDb((int)type).GetSprite("Hammer_Set_01_0", out sprite);
                                    }
                                    if (type == global::WeaponClassType.BOWGUN)
                                    {
                                        workerWeaponSprite.GetDb((int)type).GetSprite("BowGun_Set_01_0", out sprite);
                                    }
                                    if (type == global::WeaponClassType.CANNON)
                                    {
                                        workerWeaponSprite.GetDb((int)type).GetSprite("Cannon_Set_01_0", out sprite);
                                    }
                                    if (type == global::WeaponClassType.AXE)
                                    {
                                        workerWeaponSprite.GetDb((int)type).GetSprite("Axe_Set_01_0", out sprite);
                                    }
                                    if (type == global::WeaponClassType.KNIFE)
                                    {
                                        workerWeaponSprite.GetDb((int)type).GetSprite("Knife_Set_01_0", out sprite);
                                    }
                                    if (type == global::WeaponClassType.MACE)
                                    {
                                        workerWeaponSprite.GetDb((int)type).GetSprite("Mace_Set_01_0", out sprite);
                                    }
                                    if (type == global::WeaponClassType.RIFLE)
                                    {
                                        workerWeaponSprite.GetDb((int)type).GetSprite("Rifle_Set_01_0", out sprite);
                                    }
                                    Texture2D texture2D = new Texture2D(sprite.texture.width, sprite.texture.height);
                                    foreach (DirectoryInfo dir3 in global::Add_On.instance.DirList)
                                    {
                                        bool flag3 = false;
                                        DirectoryInfo directoryInfo = global::EquipmentDataLoader.CheckNamedDir(dir3, "Equipment");
                                        if (directoryInfo != null)
                                        {
                                            if (!Directory.Exists(directoryInfo.FullName + "/Sprite/Weapon"))
                                            {
                                                continue;
                                            }
                                            DirectoryInfo directoryInfo2 = new DirectoryInfo(directoryInfo.FullName + "/Sprite/Weapon");
                                            if (directoryInfo2.GetFiles().Length != 0)
                                            {
                                                FileInfo[] files = directoryInfo2.GetFiles();
                                                for (int i = 0; i < files.Length; i++)
                                                {
                                                    if (files[i].Name == SS.value + ".png")
                                                    {
                                                        texture2D.LoadImage(File.ReadAllBytes(files[i].FullName));
                                                        flag3 = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        if (flag3)
                                        {
                                            break;
                                        }
                                    }
                                    Vector2 vector = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);
                                    Sprite sprite2 = Sprite.Create(texture2D, sprite.rect, vector, sprite.pixelsPerUnit, 0U, SpriteMeshType.FullRect, sprite.border);
                                    sprite2.name = SS.value;
                                    dictionary.Add(SS.value, sprite2);
                                    field.SetValue(workerWeaponSprite.GetDb((int)type), dictionary);
                                }
                                if (workerWeaponSprite.GetDb((int)type).GetSprite(SS.value, out sprite))
                                {
                                    return sprite;
                                }
                            }
                            catch (Exception message)
                            {
                                Debug.LogError(message);
                                return null;
                            }
                        }
                        result = null;
                    }
                    else
                    {
                        Sprite[] fistSprite = this.GetFistSprite((int)float.Parse(SS.value));
                        if (fistSprite.Length <= 1)
                        {
                            result = null;
                        }
                        else
                        {
                            result = fistSprite[1];
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModDebug.Log(" GetWeaponSprite error - " + ex.Message + Environment.NewLine + ex.StackTrace);
                    File.WriteAllText(Application.dataPath + "/BaseMods/error.txt", ex.Message + Environment.NewLine + ex.StackTrace);
                    result = null;
                }
                return result;
            } else
            {
                try
                {
                    global::WorkerSprite.WorkerEquipmentSprite workerEquipmentSprite = null;
                if (uniqueWeaponDicMod == null) uniqueWeaponDicMod = new Dictionary<KeyValuePairSS, UniqueWeaponSpriteUnit>();
                if (CommonWeaponSpriteMod == null) CommonWeaponSpriteMod = new Dictionary<KeyValuePairSS, Sprite>();
                if (type == global::WeaponClassType.SPECIAL)
                {
                    if (uniqueWeaponDicMod.ContainsKey(SS))
                    {
                        return uniqueWeaponDicMod[SS].CommonSprite;
                    }
                    if (this._uniqueWeaponDic.ContainsKey(SS.value))
                    {
                        global::WorkerSprite.UniqueWeaponSpriteUnit uniqueWeaponSpriteUnit = null;

                        Texture2D texture = new Texture2D(256, 256);
                        DirectoryInfo dir = null;
                        bool flag = this.SpecialFindDir_Texture_Mod(SS.key, SS.value, ref dir, ref texture);
                        uniqueWeaponDicMod[SS] = this._uniqueWeaponDic[SS.value].GetCopy();
                        if (!flag)
                        {
                            result = uniqueWeaponDicMod[SS].CommonSprite;
                            return result;
                        }
                        this.MakeNewUniqueWeaponSpriteUnit(SS.value, uniqueWeaponDicMod[SS], dir, texture, out result);
                    }
                    else
                    {
                        char[] separator = new char[]
                        {
                    '_'
                        };
                        string key = SS.value.Split(separator)[0];
                        if (this._uniqueWeaponDic.ContainsKey(key))
                        {
                            Texture2D texture2 = new Texture2D(256, 256);
                            DirectoryInfo dir2 = null;
                            bool flag2 = this.SpecialFindDir_Texture_Mod(SS.key, SS.value, ref dir2, ref texture2);
                            uniqueWeaponDicMod[SS] = this._uniqueWeaponDic[key].GetCopy();
                            if (!flag2)
                            {
                                result = uniqueWeaponDicMod[SS].CommonSprite;
                                return result;
                            }
                            this.MakeNewUniqueWeaponSpriteUnit(SS.value, uniqueWeaponDicMod[SS], dir2, texture2, out result);
                        }
                    }
                }
                else if (type != global::WeaponClassType.FIST)
                {
                    if (CommonWeaponSpriteMod.ContainsKey(SS))
                    {
                        return CommonWeaponSpriteMod[SS];
                    }
                    if (this.equipData.GetData(global::WorkerSprite.EquipmentSpriteRegion.WEAPON, out workerEquipmentSprite))
                    {
                        global::WorkerSprite.WorkerWeaponSprite workerWeaponSprite = workerEquipmentSprite as global::WorkerSprite.WorkerWeaponSprite;
                        FieldInfo field = typeof(global::WorkerSprite.WorkerWeaponSprite.WeaponDatabase).GetField("data", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                        Dictionary<string, Sprite> dictionary = (Dictionary<string, Sprite>)field.GetValue(workerWeaponSprite.GetDb((int)type));
                        Sprite sprite = null;

                            if (!dictionary.ContainsKey(SS.value))
                            {
                                if (type == global::WeaponClassType.SPEAR)
                                {
                                    workerWeaponSprite.GetDb((int)type).GetSprite("Spear_Set_01_0", out sprite);
                                }
                                if (type == global::WeaponClassType.PISTOL)
                                {
                                    workerWeaponSprite.GetDb((int)type).GetSprite("Pistol_Set_01_0", out sprite);
                                }
                                if (type == global::WeaponClassType.HAMMER)
                                {
                                    workerWeaponSprite.GetDb((int)type).GetSprite("Hammer_Set_01_0", out sprite);
                                }
                                if (type == global::WeaponClassType.BOWGUN)
                                {
                                    workerWeaponSprite.GetDb((int)type).GetSprite("BowGun_Set_01_0", out sprite);
                                }
                                if (type == global::WeaponClassType.CANNON)
                                {
                                    workerWeaponSprite.GetDb((int)type).GetSprite("Cannon_Set_01_0", out sprite);
                                }
                                if (type == global::WeaponClassType.AXE)
                                {
                                    workerWeaponSprite.GetDb((int)type).GetSprite("Axe_Set_01_0", out sprite);
                                }
                                if (type == global::WeaponClassType.KNIFE)
                                {
                                    workerWeaponSprite.GetDb((int)type).GetSprite("Knife_Set_01_0", out sprite);
                                }
                                if (type == global::WeaponClassType.MACE)
                                {
                                    workerWeaponSprite.GetDb((int)type).GetSprite("Mace_Set_01_0", out sprite);
                                }
                                if (type == global::WeaponClassType.RIFLE)
                                {
                                    workerWeaponSprite.GetDb((int)type).GetSprite("Rifle_Set_01_0", out sprite);
                                }
                                Texture2D texture2D = new Texture2D(sprite.texture.width, sprite.texture.height);
                                ModInfo_patch pmodinfo = (ModInfo_patch)((Add_On_patch)(object)Add_On.instance).ModList.Find(x => ((ModInfo_patch)(object)x).modid == SS.key);
                                if (pmodinfo != null)
                                {
                                    DirectoryInfo directoryInfo = global::EquipmentDataLoader.CheckNamedDir(pmodinfo.modpath, "Equipment");
                                    if (directoryInfo != null)
                                    {
                                        if (Directory.Exists(directoryInfo.FullName + "/Sprite/Weapon"))
                                        {

                                            DirectoryInfo directoryInfo2 = new DirectoryInfo(directoryInfo.FullName + "/Sprite/Weapon");
                                            if (directoryInfo2.GetFiles().Length != 0)
                                            {
                                                FileInfo[] files = directoryInfo2.GetFiles();
                                                for (int i = 0; i < files.Length; i++)
                                                {
                                                    if (files[i].Name == SS.value + ".png")
                                                    {
                                                        texture2D.LoadImage(File.ReadAllBytes(files[i].FullName));
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                Vector2 vector = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);
                                Sprite sprite2 = Sprite.Create(texture2D, sprite.rect, vector, sprite.pixelsPerUnit, 0U, SpriteMeshType.FullRect, sprite.border);
                                sprite2.name = SS.value;
                                CommonWeaponSpriteMod[SS] = sprite2;
                                return CommonWeaponSpriteMod[SS];
                            }
                        }

                    }
                    result = null;
                }
                else
                {
                    Sprite[] fistSprite = this.GetFistSprite((int)float.Parse(SS.value));
                    if (fistSprite.Length <= 1)
                    {
                        result = null;
                    }
                    else
                    {
                        result = fistSprite[1];
                    }
                }
                return result;
            }
                                        catch (Exception message)
                {
                    ModDebug.Log("GetWeaponSpriteMod error - " + message.Message + Environment.NewLine + message.StackTrace);
                    return null;
                }
            }
        }
        

        [ModifiesMember("GetWeaponSprite")]
        public Sprite GetWeaponSprite_patch(global::WeaponClassType type, string name)
        {
            return GetWeaponSprite_Mod(type, new KeyValuePairSS(string.Empty, name));
        }
        [NewMember]
        public void GetArmorData_Mod(LcId id, ref WorkerSprite.WorkerSprite set)
        {
           if(id.packageId == String.Empty)
            {
                GetArmorData_patch(id.id, ref set);
                return;
            }
            if (ClothesSetMod == null) ClothesSetMod = new Dictionary<LcId, SpriteResourceLoadData>();

            if(ClothesSetMod.ContainsKey(id))
            {
                set.Armor.SetSprite(ClothesSetMod[id]);
            }
            foreach (ModInfo info in ((Add_On_patch)(object)Add_On.instance).ModList)
            {
                ModInfo_patch modinfo = ((ModInfo_patch)(object)info);
                if (modinfo.modid == id.packageId)
                {
                    Texture2D texture2D = new Texture2D(2, 2);
                    bool flag = false;
                    DirectoryInfo directoryInfo = global::EquipmentDataLoader.CheckNamedDir(modinfo.modpath, "Equipment");
                    if (directoryInfo != null)
                    {
                        if (!Directory.Exists(directoryInfo.FullName + "/Sprite/Armor"))
                        {
                            continue;
                        }
                        DirectoryInfo directoryInfo2 = new DirectoryInfo(directoryInfo.FullName + "/Sprite/Armor");
                        if (directoryInfo2.GetFiles().Length != 0)
                        {
                            FileInfo[] files = directoryInfo2.GetFiles();
                            for (int j = 0; j < files.Length; j++)
                            {
                                if (files[j].Name == id.id.ToString()+ ".png")
                                {
                                    texture2D.LoadImage(File.ReadAllBytes(files[j].FullName));
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }
                if(flag)
                    {
                        global::WorkerSprite.SpriteResourceLoadData clothesSet = this.GetClothesSet(0);
                        global::WorkerSprite.AtlasLoadData atlasLoadData = new global::WorkerSprite.AtlasLoadData();
                        atlasLoadData.sprites.Clear();
                        for (int i = 0; i < (clothesSet as global::WorkerSprite.AtlasLoadData).sprites.Count; i++)
                        {
                            Sprite sprite = (clothesSet as global::WorkerSprite.AtlasLoadData).sprites[i];
                            Vector2 vector = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);
                            Sprite sprite2 = Sprite.Create(Add_On.duplicateTexture(texture2D), sprite.rect, vector, sprite.pixelsPerUnit, 0U, SpriteMeshType.FullRect, sprite.border, true);
                            sprite2.name = sprite.name;
                            atlasLoadData.sprites.Add(sprite2);
                            atlasLoadData.count++;
                        }
                        ClothesSetMod[id] = atlasLoadData;
                        set.Armor.SetSprite(ClothesSetMod[id]);
                        return;
                    }
                }
            }

            ModDebug.Log("GetArmorData_Mod No Match! Id : " + id.ToString());

        }
        [ModifiesMember("GetArmorData")]
        public void GetArmorData_patch(int id, ref global::WorkerSprite.WorkerSprite set)
        {
            WorkerEquipmentSprite workerEquipmentSprite = null;
            this.equipData.GetData(EquipmentSpriteRegion.CLOTHES, out workerEquipmentSprite);
            if (id < 100000000)
            {
                id += 100000000;
            }
            if (!(workerEquipmentSprite as global::WorkerSprite.WorkerClothesSprite).lib.ContainsKey(id))
            {
                global::WorkerSprite.SpriteResourceLoadData clothesSet = this.GetClothesSet(0);
                global::WorkerSprite.AtlasLoadData atlasLoadData = new global::WorkerSprite.AtlasLoadData();
                atlasLoadData.sprites.Clear();
                bool flag = false;
                for (int i = 0; i < (clothesSet as global::WorkerSprite.AtlasLoadData).sprites.Count; i++)
                {
                    Texture2D texture2D = new Texture2D(2, 2);
                    foreach (DirectoryInfo dir in global::Add_On.instance.DirList)
                    {
                        flag = false;
                        DirectoryInfo directoryInfo = global::EquipmentDataLoader.CheckNamedDir(dir, "Equipment");
                        if (directoryInfo != null)
                        {
                            if (!Directory.Exists(directoryInfo.FullName + "/Sprite/Armor"))
                            {
                                continue;
                            }
                            DirectoryInfo directoryInfo2 = new DirectoryInfo(directoryInfo.FullName + "/Sprite/Armor");
                            if (directoryInfo2.GetFiles().Length != 0)
                            {
                                FileInfo[] files = directoryInfo2.GetFiles();
                                for (int j = 0; j < files.Length; j++)
                                {
                                    if (files[j].Name == (id - 100000000).ToString() + ".png")
                                    {
                                        texture2D.LoadImage(File.ReadAllBytes(files[j].FullName));
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
                    if (flag)
                    {
                        Sprite sprite = (clothesSet as global::WorkerSprite.AtlasLoadData).sprites[i];
                        Vector2 vector = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);
                        Sprite sprite2 = Sprite.Create(texture2D, sprite.rect, vector, sprite.pixelsPerUnit, 0U, SpriteMeshType.FullRect, sprite.border, true);
                        sprite2.name = sprite.name;
                        atlasLoadData.sprites.Add(sprite2);
                        atlasLoadData.count++;
                    }
                }
                if (flag)
                {
                    (workerEquipmentSprite as global::WorkerSprite.WorkerClothesSprite).lib.Add(id, atlasLoadData);
                }
                else
                {
                    (workerEquipmentSprite as global::WorkerSprite.WorkerClothesSprite).lib.Add(id, (workerEquipmentSprite as global::WorkerSprite.WorkerClothesSprite).lib[id - 100000000]);
                }
            }
            global::WorkerSprite.SpriteResourceLoadData clothesSet2 = this.GetClothesSet(id);
            if (clothesSet2 != null)
            {
                set.Armor.SetSprite(clothesSet2);
            }
        }
        [ModifiesMember("GetClothesSet")]
        public global::WorkerSprite.SpriteResourceLoadData GetClothesSet(int id)
        {
            global::WorkerSprite.SpriteResourceLoadData result = null;
            global::WorkerSprite.WorkerEquipmentSprite workerEquipmentSprite = null;
            if (this.equipData.GetData(global::WorkerSprite.EquipmentSpriteRegion.CLOTHES, out workerEquipmentSprite))
            {
                global::WorkerSprite.WorkerClothesSprite workerClothesSprite = workerEquipmentSprite as global::WorkerSprite.WorkerClothesSprite;
                if ((result = workerClothesSprite.GetData(id)) == null)
                {
                    Debug.Log("Finding clothes set " + id);
                    result = workerClothesSprite.lib[0];
                }
            }
            return result;
        }




        [MemberAlias("LoadCustomSprite", typeof(WorkerSpriteManager))]
        public void LoadCustomSprite(DirectoryInfo di, BasicSpriteRegion region, WorkerSpriteManager.SizeRef size)
        {
            
        }
        [MemberAlias("get_CustomBattleEyebrowSrc", typeof(WorkerSpriteManager))]
        public string get_CustomBattleEyebrowSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomBattleMouthSrc", typeof(WorkerSpriteManager))]
        public string get_CustomBattleMouthSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomDeadEyeSrc", typeof(WorkerSpriteManager))]
        public string get_CustomDeadEyeSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomEyebrowSrc", typeof(WorkerSpriteManager))]
        public string get_CustomEyebrowSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomEyeSrc", typeof(WorkerSpriteManager))]
        public string get_CustomEyeSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomFaceSrc", typeof(WorkerSpriteManager))]
        public string get_CustomFaceSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomFolderSrc", typeof(WorkerSpriteManager))]
        public string get_CustomFolderSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomFrontHairSrc", typeof(WorkerSpriteManager))]
        public string get_CustomFrontHairSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomHairSrc", typeof(WorkerSpriteManager))]
        public string get_CustomHairSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomMouthSrc", typeof(WorkerSpriteManager))]
        public string get_CustomMouthSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomPanicEyebrowSrc", typeof(WorkerSpriteManager))]
        public string get_CustomPanicEyebrowSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomPanicEyeSrc", typeof(WorkerSpriteManager))]
        public string get_CustomPanicEyeSrc()
        {
            return null;
        }
        [MemberAlias("get_CustomRearHairSrc", typeof(WorkerSpriteManager))]
        public string get_CustomRearHairSrc()
        {
            return null;
        }

        [MemberAlias("MakeNewUniqueWeaponSpriteUnit", typeof(WorkerSpriteManager))]
        public void MakeNewUniqueWeaponSpriteUnit(string name, global::WorkerSprite.UniqueWeaponSpriteUnit unit, DirectoryInfo dir, Texture2D texture, out Sprite result)
        {
            Sprite commonSprite = unit.CommonSprite;
            Vector2 vector = new Vector2(commonSprite.pivot.x / commonSprite.rect.width, commonSprite.pivot.y / commonSprite.rect.height);
            Sprite sprite = Sprite.Create(texture, commonSprite.rect, vector, commonSprite.pixelsPerUnit, 0U, SpriteMeshType.FullRect, commonSprite.border);
            sprite.name = name;
            result = sprite;
            if (unit.sprites != null && unit.sprites.Count > 0)
            {
                int num = 0;
                foreach (global::WorkerSprite.UniqueWeaponSprite uniqueWeaponSprite in unit.sprites)
                {
                    Sprite sprite2 = uniqueWeaponSprite.sprite;
                    Texture2D texture2D = new Texture2D(2, 2);
                    byte[] data = File.ReadAllBytes(string.Concat(new object[]
                    {
                dir.FullName,
                "/",
                name,
                "_",
                num,
                ".png"
                    }));
                    texture2D.LoadImage(data);
                    Vector2 vector2 = new Vector2(sprite2.pivot.x / sprite2.rect.width, sprite2.pivot.y / sprite2.rect.height);
                    Sprite sprite3 = Sprite.Create(texture2D, sprite2.rect, vector2, sprite2.pixelsPerUnit, 0U, SpriteMeshType.FullRect, sprite2.border);
                    unit.sprites[num].sprite = sprite3;
                    num++;
                }
            }
            unit.GetType().GetField("_commonSprite", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).SetValue(unit, sprite);
        }
        [MemberAlias("GetFistSprite",typeof(WorkerSpriteManager))]
        public Sprite[] GetFistSprite(int id)
        {
            return null;
        }

        [NewMember]
        [NonSerialized]
        public Dictionary<KeyValuePairSS, Sprite> CommonWeaponSpriteMod = new Dictionary<KeyValuePairSS, Sprite>();

        [NewMember]
        [NonSerialized]
        public Dictionary<KeyValuePairSS, UniqueWeaponSpriteUnit> uniqueWeaponDicMod = new Dictionary<KeyValuePairSS, UniqueWeaponSpriteUnit>();

        [NewMember]
        [NonSerialized]
        public Dictionary<LcId, SpriteResourceLoadData> ClothesSetMod = new Dictionary<LcId, SpriteResourceLoadData>();


       






        [MemberAlias("custom_128", typeof(WorkerSpriteManager))]
        private static float custom_128 = 128f;
        [MemberAlias("custom_256", typeof(WorkerSpriteManager))]
        private static float custom_256 = 256f;
        [MemberAlias("custom_512", typeof(WorkerSpriteManager))]
        private static float custom_512 = 512f;
        [MemberAlias("pivot", typeof(WorkerSpriteManager))]
        private static Vector2 pivot = new Vector2(0.5f, 0.5f);
        [MemberAlias("custom_x_std_hair", typeof(WorkerSpriteManager))]
        private const float custom_x_std_hair = 256f;
        [MemberAlias("custom_x_std_face", typeof(WorkerSpriteManager))]
        private const float custom_x_std_face = 256f;
        [MemberAlias("facePath", typeof(WorkerSpriteManager))]
        public const string facePath = "Sprites/Agent/Face";
        [MemberAlias("bothHairPath", typeof(WorkerSpriteManager))]
        public const string bothHairPath = "Sprites/Agent/Hair/Both";
        [MemberAlias("femaleHairPath", typeof(WorkerSpriteManager))]
        public const string femaleHairPath = "Sprites/Agent/Hair/Female";
        [MemberAlias("maleHairPath", typeof(WorkerSpriteManager))]
        public const string maleHairPath = "Sprites/Agent/Hair/Male";
        [MemberAlias("righthand", typeof(WorkerSpriteManager))]
        public Sprite righthand;
        [MemberAlias("basicData", typeof(WorkerSpriteManager))]
        public global::WorkerSprite.WorkerBasicSpriteController basicData;
        [MemberAlias("equipData", typeof(WorkerSpriteManager))]
        public global::WorkerSprite.WorkerEquipmentSpriteController equipData;
        [MemberAlias("workerColor", typeof(WorkerSpriteManager))]
        public List<WorkerSpine.WorkerColorPreset> workerColor;
        [MemberAlias("SefiraSymbol", typeof(WorkerSpriteManager))]
        public List<Sprite> SefiraSymbol;
        [MemberAlias("SefiraSymbol2", typeof(WorkerSpriteManager))]
        public List<Sprite> SefiraSymbol2;
        [MemberAlias("SefiraSymbol3", typeof(WorkerSpriteManager))]
        public List<Sprite> SefiraSymbol3;
        [MemberAlias("SefiraSymbol4", typeof(WorkerSpriteManager))]
        public List<Sprite> SefiraSymbol4;
        [MemberAlias("WorkNote", typeof(WorkerSpriteManager))]
        public List<Sprite> WorkNote;
        [MemberAlias("UniqueWeapon", typeof(WorkerSpriteManager))]
        public List<Sprite> UniqueWeapon;
        [MemberAlias("PanicShadow", typeof(WorkerSpriteManager))]
        public List<Sprite> PanicShadow;
        [MemberAlias("uniqueWeaponSprites", typeof(WorkerSpriteManager))]
        public List<global::WorkerSprite.UniqueWeaponSpriteUnit> uniqueWeaponSprites;
        [MemberAlias("_uniqueWeaponDic", typeof(WorkerSpriteManager))]
        private Dictionary<string, global::WorkerSprite.UniqueWeaponSpriteUnit> _uniqueWeaponDic;
        [MemberAlias("ModifyBases", typeof(WorkerSpriteManager))]
        private static string ModifyBases;
        [MemberAlias("Modifys", typeof(WorkerSpriteManager))]
        public List<int> Modifys;
        [MemberAlias("Special_Basic", typeof(WorkerSpriteManager))]
        public static Sprite Special_Basic;
    }
    [ModifiesType("WorkerPortraitSetter")]
    public class WorkerPortraitSetter_patch
    {
        [ModifiesMember("SetWeapon")]
        public void SetWeapon_patch(global::WeaponModel weapon)
        {
          
            if (!this.WeaponSet)
            {
                return;
            }
            if (weapon == null)
            {
                this.OneHandedRenderer.enabled = false;
                this.TwoHandedRenderer.enabled = false;
                this.FistRenderer.enabled = false;
                this.ClearAddedWeapon();
                return;
            }
            KeyValuePairSS SS = new KeyValuePairSS(EquipmentTypeInfo_patch.GetLcId(weapon.metaInfo).packageId, weapon.metaInfo.sprite);
            global::WeaponClassType weaponClassType = weapon.metaInfo.weaponClassType;
            if (weaponClassType == global::WeaponClassType.SPECIAL)
            {
                global::WorkerSprite.UniqueWeaponSpriteUnit uniqueWeaponSpriteUnit = null;
                if (((WorkerSpriteManager_patch)(object)WorkerSpriteManager.instance).GetUniqueWeaponSpriteInfo_Mod(SS, out uniqueWeaponSpriteUnit) && uniqueWeaponSpriteUnit != this._currentUnit)
                {
                    this._currentUnit = uniqueWeaponSpriteUnit;
                    this.FistRenderer.enabled = false;
                    this.OneHandedRenderer.enabled = false;
                    this.TwoHandedRenderer.enabled = false;
                    this.SetUniqueWeapon(uniqueWeaponSpriteUnit);
                }
                return;
            }
            if (this._currentUnit != null)
            {
                this.ClearAddedWeapon();
                this._currentUnit = null;
            }
            if (weaponClassType == global::WeaponClassType.FIST)
            {
                int id = (int)float.Parse(weapon.metaInfo.sprite);
                Sprite[] fistSprite = global::WorkerSprite.WorkerSprite_WorkerSpriteManager.instance.GetFistSprite(id);
                if (fistSprite[0] == null || fistSprite[1] == null)
                {
                    return;
                }
                this.FistRenderer.sprite = fistSprite[1];
                this.OneHandedRenderer.enabled = false;
                this.TwoHandedRenderer.enabled = false;
                this.FistRenderer.enabled = true;
            }
            else
            {
                bool flag = global::WeaponSetter.IsTwohanded(weapon);
                Sprite weaponSprite = ((WorkerSpriteManager_patch)(object)WorkerSpriteManager.instance).GetWeaponSprite_Mod(weaponClassType, SS);
                if (flag)
                {
                    this.TwoHandedRenderer.sprite = weaponSprite;
                    this.OneHandedRenderer.enabled = false;
                    this.TwoHandedRenderer.enabled = true;
                    this.FistRenderer.enabled = false;
                }
                else
                {
                    this.OneHandedRenderer.sprite = weaponSprite;
                    this.OneHandedRenderer.enabled = true;
                    this.TwoHandedRenderer.enabled = false;
                    this.FistRenderer.enabled = false;
                }
            }
        }
        [ModifiesMember("SetArmor")]
        public void SetArmor_patch(global::EquipmentTypeInfo armor)
        {
            LcId lcid = EquipmentTypeInfo_patch.GetLcId(armor);
            int armorId = armor.armorId;
            if (this._armor != null && lcid == EquipmentTypeInfo_patch.GetLcId(this._armor))
            {
                return;
            }
            this._armor = armor;
            global::WorkerSprite.WorkerSprite workerSprite = new global::WorkerSprite.WorkerSprite();
            ((WorkerSpriteManager_patch)(object)WorkerSpriteManager.instance).GetArmorData_Mod(new LcId(lcid.packageId, armorId), ref workerSprite);
            this.Eye.gameObject.SetActive(false);
            this.Eyebrow.gameObject.SetActive(false);
            this.Mouth.gameObject.SetActive(false);
            this.FrontHair.gameObject.SetActive(false);
            this.RearHair.gameObject.SetActive(false);
            this.Symbol.gameObject.SetActive(false);
            this.SetSprite(this.Body, workerSprite.Armor.Body);
            this.SetSprite(this.LeftUpArm, workerSprite.Armor.Arm_Left_Up);
            this.SetSprite(this.LeftDownArm, workerSprite.Armor.Arm_Left_Down);
            this.SetSprite(this.RightUpArm, workerSprite.Armor.Arm_Right_Up);
            this.SetSprite(this.RightDownArm, workerSprite.Armor.Arm_Right_Down);
            this.SetSprite(this.LeftUpLeg, workerSprite.Armor.Leg_Left_Up);
            this.SetSprite(this.LeftDownLeg, workerSprite.Armor.Leg_Left_Down);
            this.SetSprite(this.RightUpLeg, workerSprite.Armor.Leg_Right_Up);
            this.SetSprite(this.RightDownLeg, workerSprite.Armor.Leg_Right_Down);
            this.SetSprite(this.LeftHand, workerSprite.Armor.Left_Hand);
            this.SetSprite(this.RightHand, workerSprite.Armor.Right_Hand);
            this.SetSprite(this.CoatBack, workerSprite.Armor.Coat_Back);
            this.SetSprite(this.CoatLeft, workerSprite.Armor.Coat_Right);
            this.SetSprite(this.CoatRight, workerSprite.Armor.Coat_Left);
            this.SetWeapon_patch(null);
            this.OnSetAmror_patch(armor);
        }
        [ModifiesMember("OnSetAmror")]
        private void OnSetAmror_patch(global::EquipmentTypeInfo model)
        {
            if(attachmentsMod == null)
            {
                attachmentsMod = new Dictionary<LcId, WorkerPortraitAttachment>();
            }
            LcId lcid = EquipmentTypeInfo_patch.GetLcId(model);
            if (model.script == "MagicalGirlArmor")
            {
                foreach (KeyValuePair<LcId, global::WorkerPortraitAttachment> keyValuePair in this.attachmentsMod)
                {
                    if (keyValuePair.Value.isUnique)
                    {
                        return;
                    }
                }
                GameObject gameObject = global::Prefab.LoadPrefab("UIComponent/WorkerPortraitAttachment");
                global::WorkerPortraitAttachment component = gameObject.GetComponent<global::WorkerPortraitAttachment>();
                component.SetMagicalGirlArmor();
                this.attachmentsMod.Add(lcid, component);
                component.RectTransform.SetParent(get_transform());
                component.RectTransform.localScale = Vector3.one;
                component.RectTransform.localPosition = Vector3.zero;
                component.Image.SetNativeSize();
                component.RectTransform.anchoredPosition = new Vector2(2.7f, -29.3f);
            }
            else
            {
                foreach (KeyValuePair<LcId, global::WorkerPortraitAttachment> keyValuePair2 in this.attachmentsMod)
                {
                    if (keyValuePair2.Value.isUnique)
                    {
                        UnityEngine.Object.Destroy(keyValuePair2.Value.gameObject);
                        this.attachmentsMod.Remove(keyValuePair2.Key);
                        break;
                    }
                }
            }
        }
        [ModifiesMember("DestroyGifts")]
        private void DestroyGifts_patch()
        {
            if(attachmentsMod == null)
            {
                attachmentsMod = new Dictionary<LcId, WorkerPortraitAttachment>();
                return;
            }
            foreach (KeyValuePair<LcId, global::WorkerPortraitAttachment> keyValuePair in this.attachmentsMod)
            {
                UnityEngine.Object.Destroy(keyValuePair.Value.gameObject);
            }
            this.attachmentsMod.Clear();
        }
        [ModifiesMember("RemoveDisabled")]
        private void RemoveDisabled_patch(global::EGOgiftModel model)
        {
            LcId lcid = EquipmentTypeInfo_patch.GetLcId(model.metaInfo);
            if (this.attachmentsMod.ContainsKey(lcid))
            {
                global::WorkerPortraitAttachment workerPortraitAttachment = this.attachmentsMod[lcid];
                UnityEngine.Object.Destroy(workerPortraitAttachment.gameObject);
                this.attachmentsMod.Remove(lcid);
            }
        }
        [ModifiesMember("ContainsGift")]
        private bool ContainsGift_patch(global::EGOgiftModel model)
        {
            LcId lcid = EquipmentTypeInfo_patch.GetLcId(model.metaInfo);
            return this.attachmentsMod.ContainsKey(lcid);
        }
        [ModifiesMember("CheckGifts")]
        private void CheckGifts_patch()
        {
            List<global::EGOgiftModel> addedGifts = this.model.Equipment.gifts.addedGifts;
            List<global::EGOgiftModel> replacedGifts = this.model.Equipment.gifts.replacedGifts;
            List<global::EGOgiftModel> list = new List<global::EGOgiftModel>();
            list.AddRange(addedGifts);
            list.AddRange(replacedGifts);
            List<global::EGOgiftModel> list2 = new List<global::EGOgiftModel>();
            List<global::EGOgiftModel> list3 = new List<global::EGOgiftModel>();
            foreach (KeyValuePair<LcId, global::WorkerPortraitAttachment> keyValuePair in this.attachmentsMod)
            {
                global::EGOgiftModel egogiftModel = keyValuePair.Value.gift as global::EGOgiftModel;
                if (egogiftModel != null)
                {
                    if (!list.Contains(egogiftModel))
                    {
                        list3.Add(egogiftModel);
                    }
                    else if (!this.model.Equipment.gifts.GetDisplayState(egogiftModel))
                    {
                        this.model.Equipment.gifts.GetDisplayState(egogiftModel);
                        list3.Add(egogiftModel);
                    }
                }
            }
            foreach (global::EGOgiftModel egogiftModel2 in list)
            {
                bool flag = this.ContainsGift_patch(egogiftModel2);
                if (!flag && this.model.Equipment.gifts.GetDisplayState(egogiftModel2))
                {
                    list2.Add(egogiftModel2);
                }
                if (flag && egogiftModel2.metaInfo.attachType == global::EGOgiftAttachType.REPLACE)
                {
                    if (egogiftModel2.metaInfo.AttachRegion == global::EGOgiftAttachRegion.MOUTH)
                    {
                        this._mouthReplace = true;
                    }
                    if (egogiftModel2.metaInfo.AttachRegion == global::EGOgiftAttachRegion.RIGHTHAND)
                    {
                        this._handReplace = true;
                    }
                }
            }
            foreach (global::EGOgiftModel egogiftModel3 in list3)
            {
                this.RemoveDisabled_patch(egogiftModel3);
                if (egogiftModel3.metaInfo.attachType == global::EGOgiftAttachType.REPLACE)
                {
                    if (egogiftModel3.metaInfo.AttachRegion == global::EGOgiftAttachRegion.MOUTH)
                    {
                        this._mouthReplace = false;
                    }
                    if (egogiftModel3.metaInfo.AttachRegion == global::EGOgiftAttachRegion.RIGHTHAND)
                    {
                        this._handReplace = false;
                    }
                }
            }
            foreach (global::EGOgiftModel egogiftModel4 in list2)
            {
                this.AddNewAttach_patch(egogiftModel4);
            }
            if (this._mouthReplace)
            {
                this.Mouth.color = transparentColor;
            }
            else
            {
                this.Mouth.color = Color.white;
            }
            if (this._handReplace)
            {
                this.RightHand.color = transparentColor;
            }
            else
            {
                this.RightHand.color = Color.white;
            }
        }

        [ModifiesMember("Start")]
        private void Start_patch()
        {
            if(attachmentsMod == null)
            {
                attachmentsMod = new Dictionary<LcId, WorkerPortraitAttachment>();
            }
            this.Head.material = null;
            this.Eye.material = null;
            this.Mouth.material = null;
            this.Eyebrow.material = null;
            this.FrontHair.material = null;
            this.RearHair.material = null;
            this.CoatBack.material = null;
            this.CoatRight.material = null;
            this.CoatLeft.material = null;
            this.LeftUpLeg.material = null;
            this.LeftDownLeg.material = null;
            this.RightUpLeg.material = null;
            this.RightDownLeg.material = null;
            this.LeftUpArm.material = null;
            this.LeftDownArm.material = null;
            this.LeftHand.material = null;
            this.Body.material = null;
            this.RightUpArm.material = null;
            this.RightDownArm.material = null;
            this.RightHand.material = null;
            this.Symbol.material = null;
            this.SetWeapon_patch(null);
            if (this._armor == null && this.model == null)
            {
                this.SetAgentArmor();
                this.Eye.gameObject.SetActive(true);
                this.Eyebrow.gameObject.SetActive(true);
                this.Mouth.gameObject.SetActive(true);
                this.FrontHair.gameObject.SetActive(true);
                this.RearHair.gameObject.SetActive(true);
                this.Symbol.gameObject.SetActive(true);
            }
        }

        [ModifiesMember("AddNewAttach")]
        private void AddNewAttach_patch(global::EGOgiftModel model)
        {
            LcId lcid = EquipmentTypeInfo_patch.GetLcId(model.metaInfo);
            GameObject gameObject = global::Prefab.LoadPrefab("UIComponent/WorkerPortraitAttachment");
            global::WorkerPortraitAttachment component = gameObject.GetComponent<global::WorkerPortraitAttachment>();
            component.SetGift(model);
            this.attachmentsMod.Add(lcid, component);
            component.RectTransform.SetParent(get_transform());
            component.RectTransform.localScale = Vector3.one;
            component.RectTransform.localPosition = Vector3.zero;
            component.Image.SetNativeSize();
            global::EGOgiftAttachRegion region = component.region;
            bool flag = model.metaInfo.attachType == global::EGOgiftAttachType.REPLACE;
            switch (region)
            {
                case global::EGOgiftAttachRegion.HEAD:
                    component.RectTransform.anchoredPosition = PositionFix_Head;
                    break;
                case global::EGOgiftAttachRegion.EYE:
                    component.RectTransform.anchoredPosition = PositionFix_Eye;
                    component.RectTransform.SetSiblingIndex(this.Mouth.transform.GetSiblingIndex() + 1);
                    if (component.Image.sprite.name.Contains("Mask"))
                    {
                        float x = -15.71f;
                        if (lcid == 400052)
                        {
                            x = -28.4f;
                        }
                        component.RectTransform.anchoredPosition = new Vector2(x, PositionFix_Eye.y);
                    }
                    break;
                case global::EGOgiftAttachRegion.MOUTH:
                    component.RectTransform.anchoredPosition = this.Mouth.rectTransform.anchoredPosition;
                    if (lcid == 400032 || lcid == 400018)
                    {
                        component.RectTransform.anchoredPosition = new Vector2(-13f, 9f);
                        component.RectTransform.SetSiblingIndex(this.FrontHair.transform.GetSiblingIndex() + 2);
                    }
                    else
                    {
                        component.RectTransform.anchoredPosition = new Vector2(-34f, 0f);
                        component.RectTransform.SetSiblingIndex(this.FrontHair.transform.GetSiblingIndex() + 1);
                    }
                    break;
                case global::EGOgiftAttachRegion.HAIR:
                    component.RectTransform.SetAsLastSibling();
                    component.RectTransform.anchoredPosition = new Vector2(16f, 108f);
                    component.RectTransform.localScale = Vector3.one * 1.2f;
                    break;
                case global::EGOgiftAttachRegion.RIGHTHAND:
                    component.RectTransform.SetParent(this.RightHand.transform.parent);
                    component.RectTransform.SetSiblingIndex(this.RightHand.transform.GetSiblingIndex() + 1);
                    component.RectTransform.anchoredPosition = new Vector2(16.5f, -91.1f);
                    break;
                case global::EGOgiftAttachRegion.BODY_UP:
                    component.RectTransform.anchoredPosition = new Vector2(-6.5f, -49.7f);
                    break;
                case global::EGOgiftAttachRegion.RIBBORN:
                    component.RectTransform.anchoredPosition = new Vector2(-6.3f, -27.8f);
                    component.RectTransform.localScale = Vector3.one;
                    component.RectTransform.SetAsLastSibling();
                    break;
                case global::EGOgiftAttachRegion.RIGHTCHEEK:
                    {
                        Vector2 anchoredPosition = new Vector2(36.6f, 24.3f);
                        component.RectTransform.anchoredPosition = anchoredPosition;
                        break;
                    }
                case global::EGOgiftAttachRegion.FACE:
                    component.RectTransform.anchoredPosition = this.Head.rectTransform.anchoredPosition;
                    component.RectTransform.SetSiblingIndex(this.Mouth.rectTransform.GetSiblingIndex() + 1);
                    break;
                case global::EGOgiftAttachRegion.BACK:
                    component.RectTransform.SetSiblingIndex(this.RearHair.rectTransform.GetSiblingIndex() + 1);
                    if (lcid == 400043)
                    {
                        component.RectTransform.anchoredPosition = new Vector2(17f, -94.8f);
                    }
                    else
                    {
                        component.RectTransform.anchoredPosition = new Vector2(104.2f, -94.8f);
                    }
                    break;
                case global::EGOgiftAttachRegion.HEADBACK:
                    component.RectTransform.anchoredPosition = PositionFix_HeadBack;
                    component.RectTransform.localScale = Vector3.one * 0.65f;
                    component.RectTransform.SetAsFirstSibling();
                    break;
                case global::EGOgiftAttachRegion.BACK2:
                    component.RectTransform.SetSiblingIndex(this.RearHair.rectTransform.GetSiblingIndex() + 1);
                    component.RectTransform.anchoredPosition = new Vector2(-74.5f, -94.8f);
                    break;
                case global::EGOgiftAttachRegion.LEFTHAND:
                    component.RectTransform.anchoredPosition = this.LeftHand.rectTransform.anchoredPosition;
                    break;
            }
        }


       
        [MemberAlias("get_transform", typeof(Component))]
        public Transform get_transform()
        {
            return null;
        }

        [MemberAlias("SetUniqueWeapon", typeof(WorkerPortraitSetter))]
        public void SetUniqueWeapon(global::WorkerSprite.UniqueWeaponSpriteUnit unit)
        {
            
        }
        [MemberAlias("ClearAddedWeapon", typeof(WorkerPortraitSetter))]
        private void ClearAddedWeapon()
        {
            if (this.weaponAdded.Count == 0)
            {
                return;
            }
            foreach (GameObject obj in this.weaponAdded)
            {
                UnityEngine.Object.Destroy(obj);
            }
            this.weaponAdded.Clear();
        }
        [MemberAlias("SetSprite", typeof(WorkerPortraitSetter))]
        public void SetSprite(Image region, Sprite sprite)
        {
        }
        [MemberAlias("SetAgentArmor", typeof(WorkerPortraitSetter))]
        public void SetAgentArmor()
        {
        }














        [NewMember]
        [NonSerialized]
        public Dictionary<LcId, global::WorkerPortraitAttachment> attachmentsMod = new Dictionary<LcId, global::WorkerPortraitAttachment>();




        [MemberAlias("weaponUnit", typeof(WorkerPortraitSetter))]
        private const string weaponUnit = "UIComponent/PortraitWeapon";
        [MemberAlias("attachSrc", typeof(WorkerPortraitSetter))]
        private const string attachSrc = "UIComponent/WorkerPortraitAttachment";
        [MemberAlias("transparentColor", typeof(WorkerPortraitSetter))]
        private static Color transparentColor = new Color(1f, 1f, 1f, 0f);
        [MemberAlias("PositionFix_Head", typeof(WorkerPortraitSetter))]
        private static Vector2 PositionFix_Head = new Vector2(5.1f, 121.4f);
        [MemberAlias("PositionFix_HeadBack", typeof(WorkerPortraitSetter))]
        private static Vector2 PositionFix_HeadBack = new Vector2(0f, 160f);
        [MemberAlias("PositionFix_Eye", typeof(WorkerPortraitSetter))]
        private static Vector2 PositionFix_Eye = new Vector2(-2.4f, 63f);
        [MemberAlias("Head", typeof(WorkerPortraitSetter))]
        public Image Head;
        [MemberAlias("Eye", typeof(WorkerPortraitSetter))]
        public Image Eye;
        [MemberAlias("Mouth", typeof(WorkerPortraitSetter))]
        public Image Mouth;
        [MemberAlias("Eyebrow", typeof(WorkerPortraitSetter))]
        public Image Eyebrow;
        [MemberAlias("FrontHair", typeof(WorkerPortraitSetter))]
        public Image FrontHair;
        [MemberAlias("RearHair", typeof(WorkerPortraitSetter))]
        public Image RearHair;
        [MemberAlias("CoatBack", typeof(WorkerPortraitSetter))]
        public Image CoatBack;
        [MemberAlias("CoatRight", typeof(WorkerPortraitSetter))]
        public Image CoatRight;
        [MemberAlias("CoatLeft", typeof(WorkerPortraitSetter))]
        public Image CoatLeft;
        [MemberAlias("LeftUpLeg", typeof(WorkerPortraitSetter))]
        public Image LeftUpLeg;
        [MemberAlias("LeftDownLeg", typeof(WorkerPortraitSetter))]
        public Image LeftDownLeg;
        [MemberAlias("RightUpLeg", typeof(WorkerPortraitSetter))]
        public Image RightUpLeg;
        [MemberAlias("RightDownLeg", typeof(WorkerPortraitSetter))]
        public Image RightDownLeg;
        [MemberAlias("LeftUpArm", typeof(WorkerPortraitSetter))]
        public Image LeftUpArm;
        [MemberAlias("LeftDownArm", typeof(WorkerPortraitSetter))]
        public Image LeftDownArm;
        [MemberAlias("LeftHand", typeof(WorkerPortraitSetter))]
        public Image LeftHand;
        [MemberAlias("Body", typeof(WorkerPortraitSetter))]
        public Image Body;
        [MemberAlias("RightUpArm", typeof(WorkerPortraitSetter))]
        public Image RightUpArm;
        [MemberAlias("RightDownArm", typeof(WorkerPortraitSetter))]
        public Image RightDownArm;
        [MemberAlias("RightHand", typeof(WorkerPortraitSetter))]
        public Image RightHand;
        [MemberAlias("Symbol", typeof(WorkerPortraitSetter))]
        public Image Symbol;
        [MemberAlias("WeaponMask", typeof(WorkerPortraitSetter))]
        public GameObject[] WeaponMask;
        [MemberAlias("TwoHandedRenderer", typeof(WorkerPortraitSetter))]
        public Image TwoHandedRenderer;
        [MemberAlias("OneHandedRenderer", typeof(WorkerPortraitSetter))]
        public Image OneHandedRenderer;
        [MemberAlias("FistRenderer", typeof(WorkerPortraitSetter))]
        public Image FistRenderer;
        [MemberAlias("_mouthReplace", typeof(WorkerPortraitSetter))]
        private bool _mouthReplace;
        [MemberAlias("_handReplace", typeof(WorkerPortraitSetter))]
        private bool _handReplace;
        [MemberAlias("WeaponSet", typeof(WorkerPortraitSetter))]
        public bool WeaponSet;
        [MemberAlias("model", typeof(WorkerPortraitSetter))]
        private global::WorkerModel model;
        [MemberAlias("_armor", typeof(WorkerPortraitSetter))]
        private global::EquipmentTypeInfo _armor;
        [MemberAlias("log", typeof(WorkerPortraitSetter))]
        public bool log;
        [MemberAlias("attachedGifts", typeof(WorkerPortraitSetter))]
        private List<global::EGOgiftModel> attachedGifts = new List<global::EGOgiftModel>();
        //[MemberAlias("attachments", typeof(WorkerPortraitSetter))]
        //private Dictionary<long, global::WorkerPortraitAttachment> attachments = new Dictionary<long, global::WorkerPortraitAttachment>();
        [MemberAlias("weaponAdded", typeof(WorkerPortraitSetter))]
        private List<GameObject> weaponAdded = new List<GameObject>();
        [MemberAlias("_currentUnit", typeof(WorkerPortraitSetter))]
        private global::WorkerSprite.UniqueWeaponSpriteUnit _currentUnit;
    }
    [ModifiesType("CreatureInfo.WeaponSlot")]
    public class WeaponSlot_patch
    {
        [ModifiesMember("SetModel")]
        public void SetModel_patch(global::EquipmentTypeInfo info)
        {
            SetModel(info);
            string empty = string.Empty;
            string empty2 = string.Empty;
            Inventory.InventoryItemDescGetter.GetWeaponDesc(info, out empty2, out empty);
            this.ItemGrade.text = info.Grade.ToString();
            Inventory.InventoryItemController.SetGradeText(info.Grade, this.ItemGrade);
            this.DamageRange.text = (int)info.damageInfo.min + "-" + (int)info.damageInfo.max;
            this.AttackSpeed.text = empty2;
            this.AttackRange.text = empty;
            this.ItemName.text = info.Name;
            global::RwbpType type = info.damageInfo.type;
            Color white = Color.white;
            Color white2 = Color.white;
            global::UIColorManager.instance.GetRWBPTypeColor(type, out white, out white2);
            switch (type)
            {
                case global::RwbpType.R:
                    this.TypeText.text = "RED";
                    break;
                case global::RwbpType.W:
                    this.TypeText.text = "WHITE";
                    break;
                case global::RwbpType.B:
                    this.TypeText.text = "BLACK";
                    break;
                case global::RwbpType.P:
                    this.TypeText.text = "PALE";
                    break;
                default:
                    this.TypeText.text = "NONE";
                    break;
            }
            this.TypeText.color = white;
            this.TypeFill.color = Color.white;
            this.TypeFill.sprite = global::IconManager.instance.DamageIcon[type - global::RwbpType.R];
            Sprite weaponSprite = ((WorkerSpriteManager_patch)(object)WorkerSpriteManager.instance).GetWeaponSprite_Mod(info.weaponClassType, new KeyValuePairSS(EquipmentTypeInfo_patch.GetLcId(info).packageId, info.sprite));
            this.ItemImage.sprite = weaponSprite;
            this.ItemImage.SetNativeSize();
            if (weaponSprite == null)
            {
                this.ItemImage.enabled = false;
            }
            else
            {
                this.ItemImage.enabled = true;
            }
            this.CheckMakeCount_patch();
        }
        [ModifiesMember("CheckMakeCount")]
        public void CheckMakeCount_patch()
        {
            int num = 0;
            int num2 = 0;
            if (((InventoryModel_patch)(object)InventoryModel.Instance).GetEquipCount_Mod(EquipmentTypeInfo_patch.GetLcId(get_Info()), out num, out num2))
            {
                this.cost = num + "/" + num2;
                this.MakeCount.text = this.cost;
            }
        }

        [MemberAlias("get_Info", typeof(EquipSlot))]
        public global::EquipmentTypeInfo get_Info()
        {
            return null;
        }
        [MemberAlias("SetModel", typeof(EquipSlot))]
        public void SetModel(global::EquipmentTypeInfo info)
        {
        }

        [MemberAlias("ItemName", typeof(EquipSlot))]
        public Text ItemName;


        [MemberAlias("MakeWeaponTooltip", typeof(WeaponSlot))]
        public global::TooltipMouseOver MakeWeaponTooltip;
        [MemberAlias("ItemImage", typeof(WeaponSlot))]
        public Image ItemImage;
        [MemberAlias("ItemGrade", typeof(WeaponSlot))]
        public Text ItemGrade;
        [MemberAlias("DamageRange", typeof(WeaponSlot))]
        public Text DamageRange;
        [MemberAlias("AttackSpeed", typeof(WeaponSlot))]
        public Text AttackSpeed;
        [MemberAlias("AttackRange", typeof(WeaponSlot))]
        public Text AttackRange;
        [MemberAlias("MakeCount", typeof(WeaponSlot))]
        public Text MakeCount;
        [MemberAlias("TypeFill", typeof(WeaponSlot))]
        public Image TypeFill;
        [MemberAlias("TypeText", typeof(WeaponSlot))]
        public Text TypeText;
        [MemberAlias("BuildButton", typeof(WeaponSlot))]
        public Button BuildButton;
        [MemberAlias("cost", typeof(WeaponSlot))]
        private string cost = string.Empty;
        [MemberAlias("Cost", typeof(WeaponSlot))]
        public int Cost;
        [MemberAlias("currentCreature",typeof(WeaponSlot))]
        public global::CreatureModel currentCreature;
    }
}
