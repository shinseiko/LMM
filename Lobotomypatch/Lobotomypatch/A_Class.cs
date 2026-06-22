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
using LobotomyBaseMod;




namespace Lobotomypatch
{
    [ModifiesType("AlterTitleController")]
    public class AlterTitleController_patch
    {
        [ModifiesMember("OnSetLanguage")]
        public void OnSetLanguage_patch(string ln)
        {
            this.GetPlayer().OnPlayInList(1);
            GlobalGameManager.instance.ForceTypeChange<GlobalGameManager_patch>().ChangeLanguage_new(ln);
            this.get__languageCTRL().Hide();
            this.get__buttonCTRL().Show();
            this.ForceTypeChange<MonoBehaviour>().StartCoroutine(this.Reload());
        }

        [ModifiesMember("Start")]
        private void Start_patch()
        {
            this.LoadBackgroundImage();
            get__logoCTRL().Show();
            this.CheckSaveState();
            this.GameVersionChecker.rectTransform.sizeDelta = this.GameVersionChecker.rectTransform.sizeDelta + new Vector2(0f, 50f);
            this.GameVersionChecker.rectTransform.localPosition = this.GameVersionChecker.rectTransform.localPosition + new Vector3(0f, -25f, 0f);
            this.GameVersionChecker.text = string.Concat(new object[]
            {
            global::GlobalGameManager.instance.BuildVer,
            "\nBaseMod ",
            global::Add_On.version
            });
            this.LanguageText.text = global::SupportedLanguage.GetCurrentLanguageName(get_CurrentLanguage());
            int num = global::GlobalGameManager.instance.PreLoadData() + 1;
            string text3 = "Day " + num;
            Text challengeDayText = this.ChallengeDayText;
            string text2 = text3;
            this.ContinueDayText.text = text2;
            challengeDayText.text = text2;
            this.ResetButton.interactable = true;
        }



        [MemberAlias("Reload", typeof(AlterTitleController))]
        private IEnumerator Reload()
        {
            yield return true;
        }
        [MemberAlias("get__buttonCTRL", typeof(AlterTitleController))]
        private UIController get__buttonCTRL()
        {
            return null;
        }
        [MemberAlias("get__languageCTRL", typeof(AlterTitleController))]
        private UIController get__languageCTRL()
        {
            return null;
        }
        [MemberAlias("GetPlayer", typeof(AlterTitleController))]
        public AudioClipPlayer GetPlayer()
        {
            return AlterTitleController.Controller.GetComponent<AudioClipPlayer>();
        }
        [MemberAlias("get_CurrentLanguage", typeof(AlterTitleController))]
        private string get_CurrentLanguage()
        {
            return null;
        }
        [MemberAlias("CheckSaveState", typeof(AlterTitleController))]
        private void CheckSaveState()
        {
        }
        [MemberAlias("get__logoCTRL", typeof(AlterTitleController))]
        private UIController get__logoCTRL()
        {
            return null;
        }
        [MemberAlias("LoadBackgroundImage",typeof(AlterTitleController))]
        public void LoadBackgroundImage()
        {
        }


        [MemberAlias("ResetButton", typeof(AlterTitleController))]
        public Button ResetButton;
        [MemberAlias("ChallengeDayText", typeof(AlterTitleController))]
        public Text ChallengeDayText;
        [MemberAlias("ContinueDayText", typeof(AlterTitleController))]
        public Text ContinueDayText;
        [MemberAlias("LanguageText", typeof(AlterTitleController))]
        public Text LanguageText;
        [MemberAlias("GameVersionChecker", typeof(AlterTitleController))]
        public Text GameVersionChecker;
    }
    [ModifiesType]
    public class Add_On_patch : Add_On
    {
        // Token: 0x06000009 RID: 9 RVA: 0x00002578 File Offset: 0x00000778
        /*[ModifiesMember("SaveBackUp", ModificationScope.All)]
        public void SaveBackUp_patch()
        {
            DirectoryInfo backDir = Add_On.GetBackDir();
            DirectoryInfo parent = new DirectoryInfo(Application.persistentDataPath).Parent;
            Add_On.UpdatingBackUps(backDir, parent);
            UnityEngine.Debug.Log("Create backup");
        }*/


        [NewMember]
        public static Sprite GetPortrait_Mod(string modid, string portraitSrc)
        {
            if (PortraitsMod == null) PortraitsMod = new Dictionary<KeyValuePairSS, Sprite>();
            KeyValuePairSS keyvalue = new KeyValuePairSS(modid, portraitSrc);
            if (PortraitsMod.ContainsKey(keyvalue))
            {
                return PortraitsMod[keyvalue];
            }
            string[] array = portraitSrc.Split(new char[]
            {
            '/'
            });
            Sprite result = null;
            Sprite sprite = Resources.Load<Sprite>("Sprites/Unit/creature/AuthorNote");
            if (modid == string.Empty)
            {
                if (array[0] == "Custom")
                {
                    foreach (DirectoryInfo directoryInfo in Add_On.instance.DirList)
                    {
                        string path = directoryInfo.FullName + "/Creature/Portrait/" + array[1] + ".png";
                        if (File.Exists(path))
                        {
                            byte[] data = File.ReadAllBytes(path);
                            Texture2D texture2D = new Texture2D(2, 2);
                            texture2D.LoadImage(data);
                            PortraitsMod[keyvalue] = Sprite.Create(texture2D, sprite.rect, sprite.pivot, sprite.pixelsPerUnit, 0U, SpriteMeshType.Tight, sprite.border);
                            return PortraitsMod[keyvalue];
                        }
                    }
                    return result;
                }
                result = Resources.Load<Sprite>(portraitSrc);
                return result;
            }
            if (array[0] == "Custom")
            {

                foreach (DirectoryInfo directoryInfo in Add_On.instance.DirList)
                {
                    string path = directoryInfo.FullName + "/Creature/Portrait/" + array[1] + ".png";
                    if (File.Exists(path))
                    {
                        byte[] data = File.ReadAllBytes(path);
                        Texture2D texture2D = new Texture2D(2, 2);
                        texture2D.LoadImage(data);
                        PortraitsMod[keyvalue] = Sprite.Create(texture2D, sprite.rect, sprite.pivot, sprite.pixelsPerUnit, 0U, SpriteMeshType.FullRect, sprite.border);
                        return PortraitsMod[keyvalue];
                    }
                }
                return result;
            }
            Sprite s = ModArtWorkManager.Instance.GetArtWork(keyvalue);
            if (s != null)
            {
                PortraitsMod[keyvalue] = Sprite.Create(s.texture, sprite.rect, sprite.pivot, sprite.pixelsPerUnit, 0U, SpriteMeshType.FullRect, sprite.border);
                return PortraitsMod[keyvalue];
            }
            return null;
        }

        [ModifiesMember("GetPortrait")]
        public static Sprite GetPortrait(string portraitSrc)
        {
            return GetPortrait_Mod(String.Empty,portraitSrc);
        }

        [NewMember]
        private static Dictionary<KeyValuePairSS, Sprite> PortraitsMod;

        [NewMember]
        public void LoadMod(DirectoryInfo dir)
        {
            this.DirList.Add(dir);
            if (Directory.Exists(dir.FullName + "/CustomEffect"))
            {
                foreach (DirectoryInfo directoryInfo in new DirectoryInfo(dir.FullName + "/CustomEffect").GetDirectories())
                {
                    try
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
                        string atlasText = File.ReadAllText(directoryInfo.FullName + "/atlas.txt");
                        Shader shader = null;
                        AtlasAsset atlasAsset = AtlasAsset.CreateRuntimeInstance(atlasText, list.ToArray(), shader, true);
                        SkeletonDataAsset value = SkeletonDataAsset.CreateRuntimeInstance(File.ReadAllText(directoryInfo.FullName + "/json.txt"), atlasAsset, true, 0.01f);
                        this.EffectList.Add(directoryInfo.Name, value);
                    }
                    catch (Exception ex2)
                    {
                        ModDebug.Log("Initerror - " + ex2.Message + Environment.NewLine + ex2.StackTrace);
                        //File.WriteAllText(Application.dataPath + "/BaseMods/Initerror.txt", ex2.Message + Environment.NewLine + ex2.StackTrace);
                    }
                }
            }
        }
        [NewMember]
        public string GetWorkshopDirPath()
        {
            /*string text = SteamApps.AppInstallDir(default(AppId));
            return string.Format(text.Remove(text.Length - "common\\LobotomyCorp".Length) + "workshop\\content\\{0}\\", 2531580);*/


            DirectoryInfo d = new DirectoryInfo(Application.dataPath);
            string path = d.Parent.Parent.Parent.FullName + "/workshop/content/2531580";
            return path;
        }
        [ModifiesMember("init", ModificationScope.All)]
        public void init_patch()
        {

            try
            {
                ModDebug.FileInit();
                ModDebug.Log("Start Add_On Patch");

                    //SteamClient.Init(568220, true);
                    //SteamId steamId = SteamClient.SteamId;
                /* try
                 {
                     Add_On.SaveBackUp();
                 }
                 catch (Exception ex)
                 {
                     File.WriteAllText(Application.dataPath + "/BaseMods/BackUperror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
                 }*/
                this.AssemList = new List<Assembly>();
                this.EffectList = new Dictionary<string, SkeletonDataAsset>();
                DirectoryInfo[] directories = new DirectoryInfo(Application.dataPath + "/BaseMods").GetDirectories();
                this.DirList = new List<DirectoryInfo>();
                this.ModList = new List<ModInfo>();

                List<DirectoryInfo> dirlist = directories.ToList();
                List<DirectoryInfo> dirlist_workshop = null;
                if(Directory.Exists(GetWorkshopDirPath()))
                {
                    dirlist_workshop = new DirectoryInfo(GetWorkshopDirPath()).GetDirectories().ToList();
                }
                if (File.Exists(Application.dataPath + "/BaseMods/BaseModList_v2.xml"))
                {
                    ModListXml xml = ModListXml.LoadData(Application.dataPath + "/BaseMods/BaseModList_v2.xml");
                    foreach(ModInfoXml info in xml.list)
                    {
                        DirectoryInfo dir = null;
                        if(info.IsWorkShop)
                        {
                            if (dirlist_workshop != null) { 

                                dir = dirlist_workshop.Find(x => x.Name == info.modfoldername);
                                dirlist_workshop.Remove(dir);
                            }
                        }    else
                        {
                            dir = dirlist.Find(x => x.Name == info.modfoldername);
                            dirlist.Remove(dir);
                        }
                       
                        if(dir != null)
                        {
                            if(info.Useit)
                            {
                                LoadMod(dir);
                            }
                        }
                    }
                }
                foreach(DirectoryInfo dir in dirlist)
                {
                    LoadMod(dir);
                }
                foreach (DirectoryInfo dir in this.DirList)
                {
                    ModInfo item = new ModInfo(dir);
                    ((ModInfo_patch)item).Init(dir);
                    //ExtensionUtil.NewModInfoInit(item, dir);
                    this.ModList.Add(item);
                }

                ModArtWorkManager.Instance.Init();
                ModAudioClipManager.Instance.Init();
                ModAssetBundleManager.Instance.Init();
                ModOptionManager.Instance.Init();
                bool isError = false;
                foreach (ModInfo modinfo in this.ModList)
                {
                    ModInfo_patch pmodinfo = modinfo.ForceTypeChange<ModInfo_patch>();
                    foreach (FileInfo fileInfo2 in pmodinfo.modpath.GetFiles())
                    {
                        if (fileInfo2.Name.Contains(".dll"))
                        {
                            foreach (Type type in Assembly.LoadFile(fileInfo2.FullName).GetTypes())
                            {
                                if (type.Name == "Harmony_Patch")
                                {
                                    try
                                    {

                                        Activator.CreateInstance(type);
                                    }
                                    catch (Exception ex3)
                                    {
                                        ModDebug.Log($"Herror - {pmodinfo.modname} / {fileInfo2.Name}" + ex3.Message + Environment.NewLine + ex3.StackTrace);
                                        isError = true;
                                        //File.WriteAllText(Application.dataPath + "/BaseMods/Herror.txt", ex3.Message + Environment.NewLine + ex3.StackTrace);
                                    }
                                }
                                try
                                {

                                    if (type.IsSubclassOf(typeof(ModInitializer)))
                                    {
                                        ModInitializer init = Activator.CreateInstance(type) as ModInitializer;
                                        init.MODID = pmodinfo.modid;
                                        init.OnInitialize();
                                    }
                                }
                                catch (Exception e)
                                {
                                    ModDebug.Log($"Herror - {pmodinfo.modname} / {fileInfo2.Name}" + e.Message + Environment.NewLine + e.StackTrace);
                                    isError = true;
                                }
                            }
                            this.AssemList.Add(Assembly.LoadFile(fileInfo2.FullName));
                        }
                    }
                }
              
                ModDebug.Log("End Add_On Patch");
                Add_On.instance = this;
                Add_On.version = "1.3.9";
                if (isError)
                {
                    GlobalNoticeBox.Instance.OpenBox("An error occurred while loading the mod. \nGameplay may not be possible.");
                }
            }
            catch (Exception e)
            {
                ModDebug.Log("AddOn Init error - " + e.Message + Environment.NewLine + e.StackTrace);
                GlobalNoticeBox.Instance.OpenBox("An error occurred while loading the mod. \nGameplay may not be possible.");
                Add_On.version = "1.3.9";
            }

           
        }

        [NewMember]
        public List<ModInfo> ModList;

    }
    [ModifiesType("AgentUnit")]
    public class AgentUnit_patch
    {
        [ModifiesMember("OnChangeArmor")]
        public void OnChangeArmor_patch()
        {
            if (this.model == null)
            {
                return;
            }
            if (this.model.Equipment.armor == null)
            {
                return;
            }
            ((WorkerSpriteSetter_patch)(object)this.spriteSetter).ArmorEquip_Mod(new LcId(EquipmentTypeInfo_patch.GetLcId(this.model.Equipment.armor.metaInfo).packageId,this.model.Equipment.armor.metaInfo.armorId));
        }



        [MemberAlias("spriteSetter", typeof(WorkerUnit))]
        public global::WorkerSprite.WorkerSpriteSetter spriteSetter;
        [MemberAlias("model",typeof(AgentUnit))]
        public global::AgentModel model;
    }
    [ModifiesType("CreatureInfo.ArmorSlot")]
    public class ArmorSlot_patch
    {
        [ModifiesMember("CheckMakeCount")]
        public void CheckMakeCount_patch()
        {
            int num = 0;
            int num2 = 0;
            if (((InventoryModel_patch)(object)InventoryModel.Instance).GetEquipCount_Mod(EquipmentTypeInfo_patch.GetLcId(get_Info()), out num, out num2))
            {
                string text = num + "/" + num2;
                this.MakeCount.text = text;
                this.cost = text;
            }
        }


        [MemberAlias("get_Info",typeof(EquipSlot))]
        public global::EquipmentTypeInfo get_Info()
        {
            return null;
        }


        [MemberAlias("MakeArmorTooltip", typeof(ArmorSlot))]
        public global::TooltipMouseOver MakeArmorTooltip;
        [MemberAlias("GradeText", typeof(ArmorSlot))]
        public Text GradeText;
        [MemberAlias("TypeText", typeof(ArmorSlot))]
        public Text[] TypeText;
        [MemberAlias("RWBP_Defense", typeof(ArmorSlot))]
        public Text[] RWBP_Defense;
        [MemberAlias("MakeCount", typeof(ArmorSlot))]
        public Text MakeCount;
        [MemberAlias("portrait", typeof(ArmorSlot))]
        public global::WorkerPortraitSetter portrait;
        [MemberAlias("BuildButton", typeof(ArmorSlot))]
        public Button BuildButton;
        [MemberAlias("Cost", typeof(ArmorSlot))]
        public int Cost;
        [MemberAlias("cost", typeof(ArmorSlot))]
        private string cost;
        [MemberAlias("currentCreature",typeof(ArmorSlot))]
        public global::CreatureModel currentCreature;
    }
    [ModifiesType("CommandWindow.AgentEquipmentSlot")]
    public class AgentEquipmentSlot_patch
    {
        [ModifiesMember("SetData")]
        public void SetData_patch(global::AgentModel agent)
        {
            this.WeaponName.text = agent.Equipment.weapon.metaInfo.Name;
            global::DamageInfo damage = agent.Equipment.weapon.GetDamage(agent);
            if (EquipmentTypeInfo_patch.GetLcId(agent.Equipment.weapon.metaInfo) != 200038 && EquipmentTypeInfo_patch.GetLcId(agent.Equipment.weapon.metaInfo) != 200004)
            {
                global::RwbpType type = damage.type;
                Color color;
                Color color2;
                global::UIColorManager.instance.GetRWBPTypeColor(type, out color, out color2);
                this.TypeFill.color = Color.white;
                this.TypeText.text = Assets.Scripts.UI.Utils.EnumTextConverter.GetRwbpType(type).ToUpper();
                this.TypeText.color = color;
                this.TypeText.resizeTextForBestFit = true;
                this.TypeFill.enabled = true;
                this.TypeFill.sprite = global::IconManager.instance.DamageIcon[type - global::RwbpType.R];
            }
            else
            {
                this.TypeFill.enabled = false;
                this.TypeText.text = "???";
                this.TypeText.color = Color.gray;
            }
            Inventory.InventoryItemController.SetGradeText(agent.Equipment.weapon.metaInfo.Grade, this.WeaponGrade);
            Inventory.InventoryItemController.SetGradeText(agent.Equipment.armor.metaInfo.Grade, this.ArmorGrade);
            string grade = agent.Equipment.weapon.metaInfo.grade;
            foreach (Text text in this.Vanlia)
            {
                text.text = grade;
            }
            this.DualValue.SetActive(false);
            global::DefenseInfo defense = agent.defense;
            global::UIUtil.DefenseSetOnlyText(defense, this.DefenseType);
            global::UIUtil.SetDefenseTypeIcon(defense, this.DefenseFactorRenderer);
            if (agent.Equipment.armor != null)
            {
                this.ArmorName.text = agent.Equipment.armor.metaInfo.Name;
            }
        }


        [MemberAlias("ActiveControl", typeof(CommandWindow.AgentEquipmentSlot))]
        public GameObject ActiveControl;
        [MemberAlias("WeaponName", typeof(CommandWindow.AgentEquipmentSlot))]
        public Text WeaponName;
        [MemberAlias("TypeFill", typeof(CommandWindow.AgentEquipmentSlot))]
        public Image TypeFill;
        [MemberAlias("TypeText", typeof(CommandWindow.AgentEquipmentSlot))]
        public Text TypeText;
        [MemberAlias("WeaponGrade", typeof(CommandWindow.AgentEquipmentSlot))]
        public Text WeaponGrade;
        [MemberAlias("SingleValue", typeof(CommandWindow.AgentEquipmentSlot))]
        public GameObject SingleValue;
        [MemberAlias("DualValue", typeof(CommandWindow.AgentEquipmentSlot))]
        public GameObject DualValue;
        [MemberAlias("Vanlia", typeof(CommandWindow.AgentEquipmentSlot))]
        public List<Text> Vanlia;
        [MemberAlias("Additional", typeof(CommandWindow.AgentEquipmentSlot))]
        public Text Additional;
        [MemberAlias("ArmorName", typeof(CommandWindow.AgentEquipmentSlot))]
        public Text ArmorName;
        [MemberAlias("DefenseType", typeof(CommandWindow.AgentEquipmentSlot))]
        public Text[] DefenseType;
        [MemberAlias("ArmorGrade", typeof(CommandWindow.AgentEquipmentSlot))]
        public Text ArmorGrade;
        [MemberAlias("DefenseFactorRenderer",typeof(CommandWindow.AgentEquipmentSlot))]
        public Image[] DefenseFactorRenderer;
    }
    [ModifiesType("AgentManager")]
    public class AgentManager_patch
    {
        [ModifiesMember("RemoveAllDlcEquipment")]
        public bool RemoveAllDlcEquipment_patch()
        {
            bool result = false;
            List<global::AgentModel> list = new List<global::AgentModel>(this.agentList);
            list.AddRange(this.agentListSpare);
            foreach (long id in global::CreatureGenerateInfo.creditCreatures)
            {
                foreach (global::AgentModel agentModel in list)
                {
                    global::CreatureTypeInfo data = global::CreatureTypeList.instance.GetData(id);
                    if (data != null)
                    {
                        foreach (global::CreatureEquipmentMakeInfo creatureEquipmentMakeInfo in data.equipMakeInfos)
                        {

                            if (agentModel.Equipment.weapon != null && EquipmentTypeInfo_patch.GetLcId(creatureEquipmentMakeInfo.equipTypeInfo) == EquipmentTypeInfo_patch.GetLcId(agentModel.Equipment.weapon.metaInfo))
                            {

                                agentModel.ReleaseWeaponV2();
                                result = true;
                            }
                            else if (agentModel.Equipment.armor != null && EquipmentTypeInfo_patch.GetLcId(creatureEquipmentMakeInfo.equipTypeInfo) == EquipmentTypeInfo_patch.GetLcId(agentModel.Equipment.armor.metaInfo))
                            {
                                agentModel.ReleaseArmor();
                                result = true;
                            }
                            else if (((UnitModel_patch)(object)agentModel).ReleaseEGOGift_Mod(EquipmentTypeInfo_patch.GetLcId(creatureEquipmentMakeInfo.equipTypeInfo)))
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            return result;
        }
        [MemberAlias("agentListSpare", typeof(AgentManager))]
        public List<global::AgentModel> agentListSpare;
        [MemberAlias("agentList", typeof(AgentManager))]
        private List<AgentModel> agentList;
    }
    [ModifiesType("CreatureGenerate.ActivateStateModel")]
    public class ActivateStateModel_patch
    {
        [NewMember]
        public static LcIdLong GetLcId(ActivateStateModel model)
        {
            return new LcIdLong(((ActivateStateModel_patch)(object)model).modid, model.id);
        }

        [NewMember]
        public string modid;


    }

    [ModifiesType("CreatureGenerate.ActivateStateList")]
    public class ActivateStateList_patch
    {
        [ModifiesMember("GetUsableCreatures")]
        public List<ActivateStateModel> GetUsableCreatures()
        {
            List<ActivateStateModel> list = new List<ActivateStateModel>();
            int currentDay = this.get_CurrentDay();
            bool genKit = CreatureGenerateInfoManager.Instance.GenKit;
            List<LcIdLong> list2 = PlayerModel.instance.ForceTypeChange<PlayerModel_patch>().CopyWaitingCreatures_Mod();
            foreach (ActivateStateModel activateStateModel in this.list)
            {
                ActivateStateModel_patch Pmodel = activateStateModel.ForceTypeChange<ActivateStateModel_patch>();
                LcIdLong lcid = new LcIdLong(Pmodel.modid, activateStateModel.id);
                if (list2.Contains(lcid))
                {
                    activateStateModel.isUsed = true;
                }
                if (!activateStateModel.isUsed && !activateStateModel.isRemoved)
                {
                    if (genKit && activateStateModel.isKit)
                    {
                        if (lcid != 300109L || global::CreatureSelectUI.CheckCreatureExisting(100104L))
                        {
                            list.Add(activateStateModel);
                        }
                    }
                    else if (!genKit && !activateStateModel.isKit)
                    {
                        if (lcid != 100014L || !global::CreatureSelectUI.CheckCreatureExisting(100015L))
                        {
                            if (lcid != 100015L)
                            {
                                list.Add(activateStateModel);
                            }
                        }
                    }
                }
            }
            return list;
        }
        [ModifiesMember("DayUpdate")]
        public void DayUpdate_patch()
        {
            List<LcIdLong> list = new List<LcIdLong>();
            List<LcIdLong> list2 = ((PlayerModel_patch)(object)PlayerModel.instance).CopyWaitingCreatures_Mod();
            foreach (global::CreatureModel creatureModel in global::CreatureManager.instance.GetCreatureList())
            {
                list.Add(CreatureTypeInfo_patch.GetLcId(creatureModel.metaInfo));
            }
            foreach (ActivateStateModel activateStateModel in this.list)
            {
                ActivateStateModel_patch Pmodel = activateStateModel.ForceTypeChange<ActivateStateModel_patch>();

                LcIdLong lcid = new LcIdLong(Pmodel.modid, activateStateModel.id); 

                activateStateModel.isRemoved = false;
                if (!activateStateModel.isUsed)
                {
                    if (list.Contains(lcid))
                    {
                        activateStateModel.isUsed = true;
                    }
                    else if (list2.Contains(lcid))
                    {
                        activateStateModel.isUsed = true;
                    }
                }
            }
        }
        [NewMember]
        public void OnUsed_Mod(LcIdLong id)
        {
            foreach (ActivateStateModel activateStateModel in this.list)
            {
                if (activateStateModel.id == id.id && ((ActivateStateModel_patch)(object)activateStateModel).modid == id.packageId)
                {
                    activateStateModel.isUsed = true;
                    break;
                }
            }
        }
        [NewMember]
        public void RemoveAction_Mod(LcIdLong id)
        {
            foreach (ActivateStateModel activateStateModel in this.list)
            {
                ActivateStateModel_patch ASM = (ActivateStateModel_patch)(object)activateStateModel;
                LcIdLong aid = new LcIdLong(ASM.modid, activateStateModel.id);
                if (aid == id)
                {
                    activateStateModel.isRemoved = true;
                    break;
                }
            }
        }
        [ModifiesMember("RemoveAction")]
        public void RemoveAction_patch(long id)
        {
            RemoveAction_Mod(new LcIdLong(id));
        }


        [MemberAlias("get_CurrentDay", typeof(ActivateStateList))]
        private int get_CurrentDay()
        {
            return CreatureGenerateInfoManager.Instance.GenDay;
        }



        [MemberAlias("list", typeof(ActivateStateList))]
        public List<ActivateStateModel> list;
    }
    [ModifiesType]
    public class AgentInfoWindow_UIComponent_patch : AgentInfoWindow.UIComponent
    {
        [ModifiesMember("SetData")]
        public void SetData_patch(global::AgentModel agent)
        {
            if (agent == null)
            {
                return;
            }
            this.SetColorData();
            this.AgentTitle.enabled = true;
            this.GradeImage.sprite = global::DeployUI.GetAgentGradeSprite(agent);
            this.AgentName.text = agent.GetUnitName();
            this.AgentTitle.text = agent.GetTitle();
            this.portrait.SetWorker(agent);
            global::WorkerPrimaryStatBonus titleBonus = agent.titleBonus;
            int originFortitudeStat = agent.originFortitudeStat;
            int originPrudenceStat = agent.originPrudenceStat;
            int originTemperanceStat = agent.originTemperanceStat;
            int originTemperanceStat2 = agent.originTemperanceStat;
            int originJusticeStat = agent.originJusticeStat;
            int originJusticeStat2 = agent.originJusticeStat;
            int num = agent.maxHp - originFortitudeStat;
            int num2 = agent.maxMental - originPrudenceStat;
            int num3 = agent.workProb - originTemperanceStat;
            int num4 = agent.workSpeed - originTemperanceStat2;
            int num5 = (int)agent.attackSpeed - originJusticeStat;
            int num6 = (int)agent.movement - originJusticeStat2;
            if (num > 0)
            {
                this.Stat_R.slots[0].SetText(originFortitudeStat + string.Empty, "+" + num);
            }
            else if (num < 0)
            {
                this.Stat_R.slots[0].SetText(originFortitudeStat + string.Empty, "-" + -num);
            }
            else
            {
                this.Stat_R.slots[0].SetText(originFortitudeStat + string.Empty);
            }
            if (num2 > 0)
            {
                this.Stat_W.slots[0].SetText(originPrudenceStat + string.Empty, "+" + num2);
            }
            else if (num2 < 0)
            {
                this.Stat_W.slots[0].SetText(originPrudenceStat + string.Empty, "-" + -num2);
            }
            else
            {
                this.Stat_W.slots[0].SetText(originPrudenceStat + string.Empty);
            }
            if (num3 > 0)
            {
                this.Stat_B.slots[0].SetText(originTemperanceStat + string.Empty, "+" + num3);
            }
            else if (num3 < 0)
            {
                this.Stat_B.slots[0].SetText(originTemperanceStat + string.Empty, "-" + -num3);
            }
            else
            {
                this.Stat_B.slots[0].SetText(originTemperanceStat + string.Empty);
            }
            if (num4 > 0)
            {
                this.Stat_B.slots[1].SetText(originTemperanceStat2 + string.Empty, "+" + num4);
            }
            else if (num4 < 0)
            {
                this.Stat_B.slots[1].SetText(originTemperanceStat2 + string.Empty, "-" + -num4);
            }
            else
            {
                this.Stat_B.slots[1].SetText(originTemperanceStat2 + string.Empty);
            }
            if (num5 > 0)
            {
                this.Stat_P.slots[0].SetText(originJusticeStat + string.Empty, "+" + num5);
            }
            else if (num5 < 0)
            {
                this.Stat_P.slots[0].SetText(originJusticeStat + string.Empty, "-" + -num5);
            }
            else
            {
                this.Stat_P.slots[0].SetText(originJusticeStat + string.Empty);
            }
            if (num6 > 0)
            {
                this.Stat_P.slots[1].SetText(originJusticeStat2 + string.Empty, "+" + num6);
            }
            else if (num6 < 0)
            {
                this.Stat_P.slots[1].SetText(originJusticeStat2 + string.Empty, "-" + -num6);
            }
            else
            {
                this.Stat_P.slots[1].SetText(originJusticeStat2 + string.Empty);
            }
            this.Stat_R.Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Rstat"), global::AgentModel.GetLevelGradeText(agent.Rstat));
            this.Stat_W.Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Wstat"), global::AgentModel.GetLevelGradeText(agent.Wstat));
            this.Stat_B.Fill_Inner.text = string.Format("{0}{2}{1}", global::LocalizeTextDataModel.instance.GetText("Bstat"), global::AgentModel.GetLevelGradeText(agent.Bstat), "\n");
            this.Stat_P.Fill_Inner.text = string.Format("{0}{2}{1}", global::LocalizeTextDataModel.instance.GetText("Pstat"), global::AgentModel.GetLevelGradeText(agent.Pstat), "\n");
            this.Weapon.StatName.text = agent.Equipment.weapon.metaInfo.Name;
            if (EquipmentTypeInfo_patch.GetLcId(agent.Equipment.weapon.metaInfo) == 200038 || EquipmentTypeInfo_patch.GetLcId(agent.Equipment.weapon.metaInfo) == 200004)
            {
                global::DamageInfo damage = agent.Equipment.weapon.GetDamage(agent);
                global::RwbpType type = damage.type;
                this.Weapon.Fill_Inner.text = "???";
                this.Weapon.Fill_Inner.color = Color.gray;
                this.Weapon.Fill.color = Color.white;
                this.Weapon.Fill.enabled = false;
                string text = string.Format("{0}-{1}", (int)damage.min, (int)damage.max);
                this.Weapon.slots[0].SetText(text);
            }
            else
            {
                this.Weapon.Fill.enabled = true;
                global::DamageInfo damage2 = agent.Equipment.weapon.GetDamage(agent);
                global::RwbpType type2 = damage2.type;
                this.Weapon.Fill_Inner.text = Assets.Scripts.UI.Utils.EnumTextConverter.GetRwbpType(type2).ToUpper();
                Color color;
                Color color2;
                global::UIColorManager.instance.GetRWBPTypeColor(type2, out color, out color2);
                this.Weapon.Fill_Inner.color = color;
                this.Weapon.Fill.color = Color.white;
                this.Weapon.Fill.sprite = global::IconManager.instance.DamageIcon[type2 - global::RwbpType.R];
                string text2 = string.Format("{0}-{1}", (int)damage2.min, (int)damage2.max);
                this.Weapon.slots[0].SetText(text2);
            }
            global::DefenseInfo defense = agent.defense;
            global::UIUtil.DefenseSetOnlyText(defense, this.DefenseType);
            global::UIUtil.SetDefenseTypeIcon(defense, this.DefenseTypeRenderer);
            if (agent.Equipment.armor != null)
            {
                this.ArmorName.text = agent.Equipment.armor.metaInfo.Name;
            }
            else
            {
                this.ArmorName.text = "Armor is missing";
            }
            Inventory.InventoryItemController.SetGradeText(agent.Equipment.weapon.metaInfo.Grade, this.WeaponGrade);
            Inventory.InventoryItemController.SetGradeText(agent.Equipment.armor.metaInfo.Grade, this.ArmorGrade);
            for (int i = 0; i < this.StatTooltips.Length; i++)
            {
                string text3 = global::LocalizeTextDataModel.instance.GetText(this.StatTooltips[i].ID);
                string arg = "?";
                switch (i)
                {
                    case 0:
                        arg = agent.fortitudeLevel.ToString();
                        break;
                    case 1:
                        arg = agent.prudenceLevel.ToString();
                        break;
                    case 2:
                        arg = agent.temperanceLevel.ToString();
                        break;
                    case 3:
                        arg = agent.justiceLevel.ToString();
                        break;
                    case 4:
                        arg = (agent.workProb / 5).ToString();
                        break;
                    case 5:
                        arg = (agent.workSpeed / 5).ToString();
                        break;
                    case 6:
                        arg = (agent.attackSpeed / 5f).ToString();
                        break;
                    case 7:
                        arg = (agent.movement / 5f).ToString();
                        break;
                }
                string dynamicTooltip = string.Format(text3, arg);
                this.StatTooltips[i].SetDynamicTooltip(dynamicTooltip);
            }
            for (int j = 0; j < this.DefenseTooltips.Length; j++)
            {
                string text4 = global::LocalizeTextDataModel.instance.GetText(this.DefenseTooltips[j].ID);
                string defenseTypeText = this.GetDefenseTypeText(agent.defense, j + global::RwbpType.R);
                string dynamicTooltip2 = string.Format(text4, defenseTypeText);
                this.DefenseTooltips[j].SetDynamicTooltip(dynamicTooltip2);
            }
        }

        [MemberAlias("GetDefenseTypeText", typeof(AgentInfoWindow.UIComponent))]
        private string GetDefenseTypeText(global::DefenseInfo def, global::RwbpType t)
        {
            global::DefenseInfo.Type defenseType = def.GetDefenseType(t);
            string result = "?";
            switch (defenseType)
            {
                case global::DefenseInfo.Type.NONE:
                    result = global::LocalizeTextDataModel.instance.GetText("DefenseType_None");
                    break;
                case global::DefenseInfo.Type.WEAKNESS:
                    result = global::LocalizeTextDataModel.instance.GetText("DefenseType_Weak");
                    break;
                case global::DefenseInfo.Type.SUPER_WEAKNESS:
                    result = global::LocalizeTextDataModel.instance.GetText("DefenseType_SuperWeak");
                    break;
                case global::DefenseInfo.Type.ENDURE:
                    result = global::LocalizeTextDataModel.instance.GetText("DefenseType_Endure");
                    break;
                case global::DefenseInfo.Type.RESISTANCE:
                    result = global::LocalizeTextDataModel.instance.GetText("DefenseType_Resist");
                    break;
                case global::DefenseInfo.Type.IMMUNE:
                    result = global::LocalizeTextDataModel.instance.GetText("DefenseType_Immune");
                    break;
            }
            return result;
        }

    }
    [ModifiesType]
    public class AgentInfoWindow_InGameModeComponent_patch : AgentInfoWindow.InGameModeComponent
    {


        [ModifiesMember("SetUI")]
        public void SetUI_patch(AgentModel agent)
        {
            this.AgentTitle.enabled = true;
            this.GradeImage.sprite = global::DeployUI.GetAgentGradeSprite(agent);
            this.AgentName.text = agent.GetUnitName();
            string str = string.Empty;
            global::Sefira sefira = global::SefiraManager.instance.GetSefira(agent.lastServiceSefira);
            if (sefira != null)
            {
                global::SefiraEnum sefiraEnum = sefira.sefiraEnum;
                if (sefiraEnum == global::SefiraEnum.TIPERERTH2)
                {
                    sefiraEnum = global::SefiraEnum.TIPERERTH1;
                }
                str = string.Format(global::LocalizeTextDataModel.instance.GetText("continous_service_ability_cur_title2"), global::LocalizeTextDataModel.instance.GetTextAppend(new string[]
                {
                global::SefiraName.GetSefiraByEnum(sefiraEnum),
                "Name"
                }), agent.continuousServiceDay) + " ";
            }
            this.AgentTitle.text = str + global::LocalizeTextDataModel.instance.GetText("continous_service_ability_cur_blank") + agent.GetTitle();
            this.portrait.SetWorker(agent);
            global::AgentInfoWindow.WorkerPrimaryStatUI[] array = this.statUI;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].SetStat(agent);
            }
            this.Weapon.StatName.text = agent.Equipment.weapon.metaInfo.Name;
            global::DamageInfo damage = agent.Equipment.weapon.GetDamage(agent);
            if (EquipmentTypeInfo_patch.GetLcId(agent.Equipment.weapon.metaInfo) == 200038 || EquipmentTypeInfo_patch.GetLcId(agent.Equipment.weapon.metaInfo) == 200004)
            {
                this.Weapon.Fill.enabled = false;
                this.Weapon.Fill_Inner.text = "???";
                this.Weapon.Fill_Inner.color = Color.gray;
                float num = agent.GetDamageFactorByEquipment();
                num *= agent.GetDamageFactorBySefiraAbility();
                float reinforcementDmg = agent.Equipment.weapon.script.GetReinforcementDmg();
                string text = string.Format("{0}-{1}", (int)(damage.min * num * reinforcementDmg), (int)(damage.max * num * reinforcementDmg));
                this.Weapon.slots[0].SetText(text);
            }
            else
            {
                this.Weapon.Fill.enabled = true;
                global::RwbpType type = damage.type;
                this.Weapon.Fill_Inner.text = Assets.Scripts.UI.Utils.EnumTextConverter.GetRwbpType(type).ToUpper();
                Color color;
                Color color2;
                global::UIColorManager.instance.GetRWBPTypeColor(type, out color, out color2);
                this.Weapon.Fill_Inner.color = color;
                this.Weapon.Fill.color = Color.white;
                this.Weapon.Fill.sprite = global::IconManager.instance.DamageIcon[type - global::RwbpType.R];
                float num2 = agent.GetDamageFactorByEquipment();
                num2 *= agent.GetDamageFactorBySefiraAbility();
                float reinforcementDmg2 = agent.Equipment.weapon.script.GetReinforcementDmg();
                string text2 = string.Format("{0}-{1}", (int)(damage.min * num2 * reinforcementDmg2), (int)(damage.max * num2 * reinforcementDmg2));
                this.Weapon.slots[0].SetText(text2);
            }
            DefenseInfo defense = agent.defense;
            UIUtil.DefenseSetFactor(defense, this.DefenseType, true);
            UIUtil.SetDefenseTypeIcon(defense, this.DefenseIcon);
            if (agent.Equipment.armor != null)
            {
                this.ArmorName.text = agent.Equipment.armor.metaInfo.Name;
            }
            else
            {
                this.ArmorName.text = "Armor is missing";
            }
            Inventory.InventoryItemController.SetGradeText(agent.Equipment.weapon.metaInfo.Grade, this.WeaponGrade);
            Inventory.InventoryItemController.SetGradeText(agent.Equipment.armor.metaInfo.Grade, this.ArmorGrade);
            for (int j = 0; j < this.StatTooltips.Length; j++)
            {
                string text3 = global::LocalizeTextDataModel.instance.GetText(this.StatTooltips[j].ID);
                string arg = "?";
                switch (j)
                {
                    case 0:
                        arg = agent.fortitudeLevel.ToString();
                        break;
                    case 1:
                        arg = agent.prudenceLevel.ToString();
                        break;
                    case 2:
                        arg = agent.temperanceLevel.ToString();
                        break;
                    case 3:
                        arg = agent.justiceLevel.ToString();
                        break;
                    case 4:
                        arg = (agent.workProb / 5).ToString();
                        break;
                    case 5:
                        arg = (agent.workSpeed / 5).ToString();
                        break;
                    case 6:
                        arg = (agent.attackSpeed / 5f).ToString();
                        break;
                    case 7:
                        arg = (agent.movement / 5f).ToString();
                        break;
                }
                string dynamicTooltip = string.Format(text3, arg);
                this.StatTooltips[j].SetDynamicTooltip(dynamicTooltip);
            }
            for (int k = 0; k < this.DefenseTooltips.Length; k++)
            {
                string text4 = global::LocalizeTextDataModel.instance.GetText(this.DefenseTooltips[k].ID);
                string defenseTypeText = this.GetDefenseTypeText(agent.defense, k + global::RwbpType.R);
                string dynamicTooltip2 = string.Format(text4, defenseTypeText);
                this.DefenseTooltips[k].SetDynamicTooltip(dynamicTooltip2);
            }
        }








        [MemberAlias("StatTooltips", typeof(AgentInfoWindow.InGameModeComponent))]
        public TooltipMouseOver[] StatTooltips;
        [MemberAlias("DefenseTooltips", typeof(AgentInfoWindow.InGameModeComponent))]
        public TooltipMouseOver[] DefenseTooltips;
        [MemberAlias("GradeImage", typeof(AgentInfoWindow.InGameModeComponent))]
        public Image GradeImage;
        [MemberAlias("AgentName", typeof(AgentInfoWindow.InGameModeComponent))]
        public Text AgentName;
        [MemberAlias("AgentTitle", typeof(AgentInfoWindow.InGameModeComponent))]
        public Text AgentTitle;
        [MemberAlias("portrait", typeof(AgentInfoWindow.InGameModeComponent))]
        public WorkerPortraitSetter portrait;
        [MemberAlias("statUI", typeof(AgentInfoWindow.InGameModeComponent))]
        public AgentInfoWindow.WorkerPrimaryStatUI[] statUI;
        [MemberAlias("Weapon", typeof(AgentInfoWindow.InGameModeComponent))]
        public global::AgentInfoWindow.StatObject Weapon;
        [MemberAlias("WeaponGrade", typeof(AgentInfoWindow.InGameModeComponent))]
        public Text WeaponGrade;
        [MemberAlias("ArmorName", typeof(AgentInfoWindow.InGameModeComponent))]
        public Text ArmorName;
        [MemberAlias("ArmorGrade", typeof(AgentInfoWindow.InGameModeComponent))]
        public Text ArmorGrade;
        [MemberAlias("DefenseType", typeof(AgentInfoWindow.InGameModeComponent))]
        public Text[] DefenseType;
        [MemberAlias("DefenseIcon", typeof(AgentInfoWindow.InGameModeComponent))]
        public Image[] DefenseIcon;

    }
}
