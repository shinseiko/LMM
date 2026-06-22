using Inventory;
using LobotomyBaseMod;
using Patchwork.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Harmony;
using LobotomyBaseModLib;

namespace Lobotomypatch
{
    [ModifiesType("Inventory.InventoryWeaponSlot")]
    public class InventoryWeaponSlot_patch
    {
        [ModifiesMember("SetWeapon")]
        public void SetWeapon(global::WeaponModel weapon)
        {
            global::UnitModel owner = weapon.owner;
            this.Name.text = weapon.metaInfo.Name;
            string text = (int)weapon.GetDamage(owner).min + "-" + (int)weapon.GetDamage(owner).max;
            global::RwbpType type = weapon.GetDamage(owner).type;
            Color color;
            Color color2;
            global::UIColorManager.instance.GetRWBPTypeColor(type, out color, out color2);
            this.Type.text = type.ToString();
            this.Type.color = color;
            string empty = string.Empty;
            string empty2 = string.Empty;
            InventoryItemDescGetter.GetWeaponDesc(weapon, out empty2, out empty);
            this.Range.text = empty2;
            this.AttackSpeed.text = empty;
            this.DamageRange.text = text;
            InventoryItemController.SetGradeText(weapon.metaInfo.Grade, this.Grade);
            this.Grade.text = weapon.metaInfo.Grade.ToString();
            this.SetEquipmentText();
            this.TooltipButton.interactable = true;
            Sprite sprite;
            if (weapon.metaInfo.weaponClassType == global::WeaponClassType.FIST)
            {
                int id = (int)float.Parse(weapon.metaInfo.sprite);
                Sprite[] fistSprite = global::WorkerSprite.WorkerSprite_WorkerSpriteManager.instance.GetFistSprite(id);
                if (fistSprite[0] == null || fistSprite[1] == null)
                {
                    return;
                }
                sprite = fistSprite[1];
            }
            else
            {
                KeyValuePairSS SS = new KeyValuePairSS(EquipmentTypeInfo_patch.GetLcId(weapon.metaInfo).packageId, weapon.metaInfo.sprite);
                sprite = ((WorkerSpriteManager_patch)(object)WorkerSpriteManager.instance).GetWeaponSprite_Mod(weapon.metaInfo.weaponClassType, SS);
            }
            Debug.Log("Weapon sprite " + sprite);
            this.Icon.sprite = sprite;
            this.Icon.SetNativeSize();
            this.Icon.preserveAspect = true;
            if (sprite == null)
            {
                this.Icon.gameObject.SetActive(false);
            }
            else
            {
                this.Icon.gameObject.SetActive(true);
            }
            this.RequireInit(weapon.metaInfo);
        }
        [ModifiesMember("ApplyPortrait")]
        public void ApplyPortrait_patch()
        {
            Sprite sprite;
            if (get_Info().weaponClassType == global::WeaponClassType.FIST)
            {
                int id = (int)float.Parse(get_Info().sprite);
                Sprite[] fistSprite = global::WorkerSprite.WorkerSprite_WorkerSpriteManager.instance.GetFistSprite(id);
                if (fistSprite[0] == null || fistSprite[1] == null)
                {
                    return;
                }
                sprite = fistSprite[1];
            }
            else
            {
                KeyValuePairSS SS = new KeyValuePairSS(EquipmentTypeInfo_patch.GetLcId(get_Info()).packageId, get_Info().sprite);
                sprite = ((WorkerSpriteManager_patch)(object)WorkerSpriteManager.instance).GetWeaponSprite_Mod(get_Info().weaponClassType, SS);
            }
            this.Icon.sprite = sprite;
            this.Icon.preserveAspect = true;
            this.Icon.SetNativeSize();
            if (sprite == null)
            {
                this.Icon.gameObject.SetActive(false);
            }
            else
            {
                this.Icon.gameObject.SetActive(true);
            }
        }

        [ModifiesMember("UpdateUI")]
        public void UpdateUI_patch()
        {
            UpdateUI();
            string text = string.Format("{0}-{1}", (int)get_Info().damageInfo.min, (int)get_Info().damageInfo.max);
            global::RwbpType type = get_Info().damageInfo.type;
            Color color = Color.white;
            color = global::UIColorManager.instance.GetRWBPTypeColor(type);

            LcId lcid = EquipmentTypeInfo_patch.GetLcId(get_Info());
            if (lcid == 200038 || lcid == 200004)
            {
                this.Type.text = "???";
                this.Type.color = Color.grey;
                this.DamageTypeImage.sprite = global::IconManager.instance.DamageIcon[type - global::RwbpType.R];
                this.DamageTypeImage.enabled = false;
                this.DamageTypeImage.color = Color.white;
            }
            else
            {
                this.Type.text = Assets.Scripts.UI.Utils.EnumTextConverter.GetRwbpType(type).ToUpper();
                this.Type.color = color;
                this.DamageTypeImage.enabled = true;
                this.DamageTypeImage.sprite = global::IconManager.instance.DamageIcon[type - global::RwbpType.R];
                this.DamageTypeImage.color = Color.white;
            }
            this.DamageRange.text = text;
            string empty = string.Empty;
            string empty2 = string.Empty;
            InventoryItemDescGetter.GetWeaponDesc(get_Info(), out empty, out empty2);
            this.Range.text = empty2;
            this.AttackSpeed.text = empty;
            this.TooltipButton.interactable = true;
            this.TooltipButton.OnPointerExit(null);
        }



        [MemberAlias("RequireInit", typeof(InventorySlot))]
        public void RequireInit(global::EquipmentTypeInfo info = null)
        {
        }
        [MemberAlias("get_Info", typeof(InventorySlot))]
        public global::EquipmentTypeInfo get_Info()
        {
            return null;
        }
        [MemberAlias("UpdateUI", typeof(InventorySlot))]
        public  void UpdateUI()
        {
        }



        [MemberAlias("SetEquipmentText", typeof(InventoryWeaponSlot))]
        public void SetEquipmentText()
        {
        }


        [MemberAlias("Grade", typeof(InventorySlot))]
        public Text Grade;
        [MemberAlias("Name", typeof(InventorySlot))]
        public Text Name;
        [MemberAlias("Icon", typeof(InventorySlot))]
        public Image Icon;
        [MemberAlias("TooltipButton", typeof(InventorySlot))]
        public Button TooltipButton;



        [MemberAlias("Type", typeof(InventoryWeaponSlot))]
        public Text Type;
        [MemberAlias("DamageRange", typeof(InventoryWeaponSlot))]
        public Text DamageRange;
        [MemberAlias("AttackSpeed", typeof(InventoryWeaponSlot))]
        public Text AttackSpeed;
        [MemberAlias("Range", typeof(InventoryWeaponSlot))]
        public Text Range;
        [MemberAlias("DamageTypeImage", typeof(InventoryWeaponSlot))]
        public Image DamageTypeImage;
        [MemberAlias("Newline", typeof(InventoryWeaponSlot))]
        public static string Newline = Environment.NewLine;
        [MemberAlias("oldText",typeof(InventoryWeaponSlot))]
        private string oldText = string.Empty;
    }
    [ModifiesType("Inventory.InventoryRequireLayout")]
    public class InventoryRequireLayout_patch
    {
        [ModifiesMember("Init")]
        public void Init_patch(global::EquipmentTypeInfo info)
        {
            IEnumerator enumerator = this.parent.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    object obj = enumerator.Current;
                    Transform transform = (Transform)obj;
                    UnityEngine.Object.Destroy(transform.gameObject);
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
            if (info.requires.Count == 0)
            {
                string text = global::LocalizeTextDataModel.instance.GetText("Inventory_NoRequire");
                string empty = string.Empty;
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.unit);
                global::InventoryRequireUnit component = gameObject.GetComponent<global::InventoryRequireUnit>();
                gameObject.transform.SetParent(this.parent);
                gameObject.transform.localScale = Vector3.one;
                LcId lcid = EquipmentTypeInfo_patch.GetLcId(info);
                if (lcid == 300034 || lcid == 200034)
                {
                    text = global::LocalizeTextDataModel.instance.GetText("Bald");
                    component.SetText(text);
                }
                else
                {
                    component.SetText(text, empty, 30);
                }
                return;
            }
            foreach (global::EgoRequire egoRequire in info.requires)
            {
                string statType = string.Empty;
                string grade = string.Empty;
                try
                {
                    int gradeFontSize = 30;
                    statType = global::LocalizeTextDataModel.instance.GetText(statName[(int)egoRequire.type]);
                    if (egoRequire.value >= 6)
                    {
                        grade = "EX";
                        gradeFontSize = 22;
                    }
                    else
                    {
                        grade = global::AgentModel.GetLevelGradeText(egoRequire.value);
                    }
                    GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.unit);
                    global::InventoryRequireUnit component2 = gameObject2.GetComponent<global::InventoryRequireUnit>();
                    gameObject2.transform.SetParent(this.parent);
                    gameObject2.transform.localScale = Vector3.one;
                    component2.SetText(statType, grade, gradeFontSize);
                }
                catch (Exception message)
                {
                    Debug.LogError(message);
                }
            }
        }



        [MemberAlias("statName", typeof(InventoryRequireLayout))]
        private static string[] statName = new string[]
{
            "Inventory_Level",
            "Rstat",
            "Wstat",
            "Bstat",
            "Pstat"
};
        [MemberAlias("ActiveControl", typeof(InventoryRequireLayout))]
        public GameObject ActiveControl;
        [MemberAlias("parent", typeof(InventoryRequireLayout))]
        public RectTransform parent;
        [MemberAlias("unit",typeof(InventoryRequireLayout))]
        public GameObject unit;
    }
    [ModifiesType("Inventory.InventoryItemController")]
    public class InventoryItemController_patch
    {
        [ModifiesMember("SetList")]
        public void SetList_patch()
        {
            List<InventorySlot> list = new List<InventorySlot>();
            global::RiskLevel riskLevel = global::RiskLevel.ZAYIN;
            if (this.selectedLevel != -1)
            {
                riskLevel = (global::RiskLevel)this.selectedLevel;
            }
            foreach (InventorySlot inventorySlot in this.slotDicMod.Values)
            {
                if (this._currentWeaponType == InventoryItemType.WEAPON)
                {
                    if (inventorySlot.Info.type != global::EquipmentTypeInfo.EquipmentType.WEAPON)
                    {
                        inventorySlot.gameObject.SetActive(false);
                        continue;
                    }
                }
                else if (inventorySlot.Info.type != global::EquipmentTypeInfo.EquipmentType.ARMOR)
                {
                    inventorySlot.gameObject.SetActive(false);
                    continue;
                }
                if (this.selectedLevel == -1)
                {
                    list.Add(inventorySlot);
                    inventorySlot.gameObject.SetActive(true);
                }
                else if (riskLevel == this.GetRiskLevel(inventorySlot))
                {
                    list.Add(inventorySlot);
                    inventorySlot.gameObject.SetActive(true);
                }
                else
                {
                    inventorySlot.gameObject.SetActive(false);
                }
            }
            List<InventorySlot> list2 = list;
            Comparison<InventorySlot> compare = new Comparison<InventorySlot>(InventorySlot.SortCompare);
            list2.Sort(compare);
            this.SortList(list);
            this.CurrentDisplayed = list;
        }
        [ModifiesMember("CheckAgentContains")]
        public void CheckAgentContains_patch(global::AgentModel target, Color c)
        {
            foreach (InventorySlot inventorySlot in this.slotDicMod.Values)
            {
                int agentSlotIndex = inventorySlot.GetAgentSlotIndex(target);
                if (agentSlotIndex != -1)
                {
                    inventorySlot.ownerSlot[agentSlotIndex].SetTextureColor(c);
                    break;
                }
            }
        }
        [ModifiesMember("OnClickDetailInfo")]
        public void OnClickDetailInfo_patch(global::EquipmentModel equipment)
        {
            OnClickDetailInfo_patch(equipment.metaInfo);
        }
        [ModifiesMember("CloseTooltip")]
        public void CloseTooltip_patch()
        {
            if (_currentDetailMod != -1)
            {
                this.OnClickDetailInfo_patch(EquipmentTypeList_patch.instance.GetData_Mod(_currentDetailMod));
            }
            this._currentDetailMod = new LcId(-1);
        }
        [ModifiesMember("OnEquipAction")]
        public void OnEquipAction_patch(EquipmentModel equipment, AgentModel agent = null)
        {
            InventorySlot inventorySlot = null;
            if (!this.GetSlot_patch(equipment, out inventorySlot))
            {
                Debug.LogError("Couldn't find slot about " + EquipmentTypeInfo_patch.GetLcId(equipment.metaInfo));
                return;
            }
            if (agent == null)
            {
                if (equipment.metaInfo.type == global::EquipmentTypeInfo.EquipmentType.ARMOR)
                {
                    if (equipment.owner != null)
                    {
                        equipment.owner.ReleaseArmor();
                    }
                }
                else if (equipment.metaInfo.type == global::EquipmentTypeInfo.EquipmentType.WEAPON && equipment.owner != null)
                {
                    equipment.owner.ReleaseWeaponV2();
                }
                inventorySlot.CheckOwner();
                return;
            }
            if (!equipment.CheckRequire(agent))
            {
                return;
            }
            global::EquipmentModel equipmentModel = null;
            if (equipment.metaInfo.type == global::EquipmentTypeInfo.EquipmentType.ARMOR)
            {
                equipmentModel = agent.Equipment.armor;
                if (equipment.owner != null)
                {
                    equipment.owner.ReleaseArmor();
                }
                if (equipmentModel != null)
                {
                    if (equipmentModel.instanceId == equipment.instanceId)
                    {
                        agent.ReleaseArmor();
                    }
                    else
                    {
                        agent.SetArmor(equipment as global::ArmorModel);
                    }
                }
                else
                {
                    agent.SetArmor(equipment as global::ArmorModel);
                }
            }
            else if (equipment.metaInfo.type == global::EquipmentTypeInfo.EquipmentType.WEAPON)
            {
                equipmentModel = agent.Equipment.weapon;
                if (equipment.owner != null)
                {
                    equipment.owner.ReleaseWeaponV2();
                }
                if (equipmentModel != null)
                {
                    if (equipmentModel.instanceId == equipment.instanceId)
                    {
                        agent.ReleaseWeaponV2();
                    }
                    else
                    {
                        agent.SetWeapon(equipment as global::WeaponModel);
                    }
                }
                else
                {
                    agent.SetWeapon(equipment as global::WeaponModel);
                }
            }
            if (equipmentModel != null)
            {
                InventorySlot inventorySlot2 = null;
                if (this.GetSlot_patch(equipmentModel, out inventorySlot2))
                {
                    inventorySlot2.CheckOwner();
                }
            }
            inventorySlot.CheckOwner();
        }
        [ModifiesMember("OnClickDetailInfo")]
        public void OnClickDetailInfo_patch(global::EquipmentTypeInfo info)
        {
            
            InventorySlot inventorySlot = null;
            InventoryUI.CurrentWindow.audioClipPlayer.OnPlayInList(3);
            this.TooltipDesc.text = info.Description;
            this.TooltipPosSet();
            if (!this.GetSlot_patch(info, out inventorySlot))
            {
                return;
            }
            if (_currentDetailMod == EquipmentTypeInfo_patch.GetLcId(info))
            {
                this.ToolTipControl.gameObject.SetActive(false);
                _currentDetailMod = new LcId(-1);
                inventorySlot.TooltipButton.interactable = true;
                inventorySlot.TooltipButton.OnPointerExit(null);
                return;
            }
            InventorySlot inventorySlot2 = null;
            if (this.slotDicMod.TryGetValue(_currentDetailMod, out inventorySlot2))
            {
                inventorySlot2.TooltipButton.interactable = true;
                inventorySlot2.TooltipButton.OnPointerExit(null);
            }
            if (!this.ToolTipControl.gameObject.activeInHierarchy)
            {
                this.ToolTipControl.gameObject.SetActive(true);
            }
            this.TooltipTitle_ItemName.text = info.Name;
            string specialDesc = info.SpecialDesc;
            if (info.GetLocalizedText("specialDesc", out specialDesc))
            {
                if (specialDesc == "UNKOWN")
                {
                    this.MiddleActive.SetActive(false);
                }
                else
                {
                    this.Tooltip_Middle.text = info.SpecialDesc;
                    this.MiddleActive.SetActive(true);
                }
            }
            else
            {
                this.MiddleActive.SetActive(false);
            }
            int siblingIndex = this.ToolTipControl.GetSiblingIndex();
            int num = inventorySlot.RectTransform.GetSiblingIndex();
            if (num > siblingIndex)
            {
                num--;
            }
            this.ToolTipControl.SetSiblingIndex(num + 1);
            _currentDetailMod = EquipmentTypeInfo_patch.GetLcId(info);
            this.ToolTipControl.transform.parent.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            this.ToolTipControl.transform.parent.GetComponent<ContentSizeFitter>().enabled = false;
            this.ToolTipControl.transform.parent.GetComponent<ContentSizeFitter>().enabled = true;
        }
        [ModifiesMember("Init")]
        public void Init_patch()
        {
            if (this.slotDicMod == null)
            {
                this.slotDicMod = new Dictionary<LcId, InventorySlot>();
            }
            _currentDetailMod = new LcId(-1);
            Dictionary<LcId, List<global::EquipmentModel>> equipmentListByTypeInfo = ((InventoryModel_patch)(object)InventoryModel.Instance).GetEquipmentListByTypeInfo_Mod();
            if (equipmentListByTypeInfo == null)
            {
                Debug.LogError("inventory is null");
                return;
            }
            List<LcId> list = new List<LcId>();
            foreach (KeyValuePair<LcId, List<global::EquipmentModel>> keyValuePair in equipmentListByTypeInfo)
            {
                global::EquipmentTypeInfo data = EquipmentTypeList_patch.instance.GetData_Mod(keyValuePair.Key);
                if (data.type != global::EquipmentTypeInfo.EquipmentType.SPECIAL)
                {
                    if (EquipmentTypeInfo_patch.GetLcId(data) != 2)
                    {
                        InventorySlot inventorySlot = null;
                        if (!this.slotDicMod.TryGetValue(keyValuePair.Key, out inventorySlot))
                        {
                            if (data.type == global::EquipmentTypeInfo.EquipmentType.WEAPON)
                            {
                                GameObject gameObject = global::Prefab.LoadPrefab("UIComponent/Inventory/EquipmentSlot_Weapon");
                                inventorySlot = gameObject.GetComponent<InventoryWeaponSlot>();
                                inventorySlot.RectTransform.SetParent(this.WeaponListParent);
                            }
                            else
                            {
                                GameObject gameObject = global::Prefab.LoadPrefab("UIComponent/Inventory/EquipmentSlot_Armor");
                                inventorySlot = gameObject.GetComponent<InventoryArmorSlot>();
                                inventorySlot.RectTransform.SetParent(this.ArmorListParent);
                            }
                            inventorySlot.RectTransform.localScale = Vector3.one;
                            inventorySlot.SetModel(data, keyValuePair.Value);
                            this.slotDicMod.Add(keyValuePair.Key, inventorySlot);
                        }
                        else if (keyValuePair.Value.Count == 0)
                        {
                            list.Add(keyValuePair.Key);
                        }
                        else
                        {
                            inventorySlot.UpdateList(keyValuePair.Value);
                        }
                    }
                }
            }
            foreach (LcId key in list)
            {
                InventorySlot inventorySlot2 = null;
                if (this.slotDicMod.TryGetValue(key, out inventorySlot2))
                {
                    UnityEngine.Object.Destroy(inventorySlot2.gameObject);
                }
                this.slotDicMod.Remove(key);
            }
            this.OnClickButton((int)this._currentWeaponType);
            if (this.selectedLevel == -1)
            {
                this.ClearButtonRankColor();
            }
            else
            {
                this.SetButtonRankColor();
            }
        }
        [ModifiesMember("GetSlot")]
        public bool GetSlot_patch(global::EquipmentModel equipment, out InventorySlot slot)
        {
            LcId id = EquipmentTypeInfo_patch.GetLcId(equipment.metaInfo);
            return this.slotDicMod.TryGetValue(id, out slot);
        }
        [ModifiesMember("GetSlot")]
        public bool GetSlot_patch(global::EquipmentTypeInfo info, out InventorySlot slot)
        {
            LcId id = EquipmentTypeInfo_patch.GetLcId(info);
            return this.slotDicMod.TryGetValue(id, out slot);
        }






        [MemberAlias("GetRiskLevel", typeof(InventoryItemController))]
        private global::RiskLevel GetRiskLevel(InventorySlot slot)
        {
            return (global::RiskLevel)(int.Parse(slot.Info.grade) - 1);
        }
        [MemberAlias("SortList", typeof(InventoryItemController))]
        private void SortList(List<InventorySlot> sorted)
        {
        }
        [MemberAlias("TooltipPosSet", typeof(InventoryItemController))]
        public void TooltipPosSet()
        {
        }
        [MemberAlias("ClearButtonRankColor", typeof(InventoryItemController))]
        private void ClearButtonRankColor()
        {
        }
        [MemberAlias("SetButtonRankColor", typeof(InventoryItemController))]
        private void SetButtonRankColor()
        {
        }
        [MemberAlias("OnClickButton", typeof(InventoryItemController))]
        public void OnClickButton(int index)
        { 
        }


        [NewMember]
        [NonSerialized]
        private LcId _currentDetailMod;
        [NewMember]
        [NonSerialized]
        private Dictionary<LcId, InventorySlot> slotDicMod;


        [MemberAlias("_armorSlot", typeof(InventoryItemController))]
        private const string _armorSlot = "UIComponent/Inventory/EquipmentSlot_Armor";
        [MemberAlias("_weaponSlot", typeof(InventoryItemController))]
        private const string _weaponSlot = "UIComponent/Inventory/EquipmentSlot_Weapon";
        [MemberAlias("WeaponButton", typeof(InventoryItemController))]
        public Button WeaponButton;
        [MemberAlias("ArmorButton", typeof(InventoryItemController))]
        public Button ArmorButton;
        [MemberAlias("WeaponControl", typeof(InventoryItemController))]
        public RectTransform WeaponControl;
        [MemberAlias("ArmorControl", typeof(InventoryItemController))]
        public RectTransform ArmorControl;
        [MemberAlias("WeaponListParent", typeof(InventoryItemController))]
        public RectTransform WeaponListParent;
        [MemberAlias("ArmorListParent", typeof(InventoryItemController))]
        public RectTransform ArmorListParent;
        [MemberAlias("WeaponScroll", typeof(InventoryItemController))]
        public ScrollRect WeaponScroll;
        [MemberAlias("ArmorScroll", typeof(InventoryItemController))]
        public ScrollRect ArmorScroll;
        [MemberAlias("ToolTipControl", typeof(InventoryItemController))]
        public RectTransform ToolTipControl;
        [MemberAlias("TooltipTitle_ItemName", typeof(InventoryItemController))]
        public Text TooltipTitle_ItemName;
        [MemberAlias("Tooltip_Middle", typeof(InventoryItemController))]
        public Text Tooltip_Middle;
        [MemberAlias("MiddleActive", typeof(InventoryItemController))]
        public GameObject MiddleActive;
        [MemberAlias("TooltipDesc", typeof(InventoryItemController))]
        public Text TooltipDesc;
        [MemberAlias("tooltipRect", typeof(InventoryItemController))]
        public RectTransform tooltipRect;
        [MemberAlias("gradeColor", typeof(InventoryItemController))]
        public Color[] gradeColor;
        [MemberAlias("sortButton", typeof(InventoryItemController))]
        public Button[] sortButton;
        [MemberAlias("rankButton", typeof(InventoryItemController))]
        public InventoryRankButton[] rankButton;
        [MemberAlias("FailEqiup", typeof(InventoryItemController))]
        public Color FailEqiup;
        [MemberAlias("weaponDic", typeof(InventoryItemController))]
        private Dictionary<long, InventoryWeaponSlot> weaponDic = new Dictionary<long, InventoryWeaponSlot>();
        [MemberAlias("armorDic", typeof(InventoryItemController))]
        private Dictionary<long, InventoryArmorSlot> armorDic = new Dictionary<long, InventoryArmorSlot>();
        //[MemberAlias("slotDic", typeof(InventoryItemController))]
        //private Dictionary<int, InventorySlot> slotDic = new Dictionary<int, InventorySlot>();
        [MemberAlias("CurrentDisplayed", typeof(InventoryItemController))]
        private List<InventorySlot> CurrentDisplayed = new List<InventorySlot>();
        [MemberAlias("_currentWeaponType", typeof(InventoryItemController))]
        private InventoryItemType _currentWeaponType;
        //[MemberAlias("_currentDetail", typeof(InventoryItemController))]
        //private int _currentDetail = -1;
        [MemberAlias("selectedLevel",typeof(InventoryItemController))]
        private int selectedLevel = -1;
    }
    [ModifiesType("Inventory.InventoryAgentController")]
    public class InventoryAgentController_patch
    {
        [ModifiesMember("SetUI")]
        public void SetUI_patch()
        {
            if (this.get_CurrentAgent() == null)
            {
                return;
            }
            this.AgentSlot.SetAgent(this.get_CurrentAgent());
            try
            {
                global::DamageInfo damage = this.get_CurrentAgent().Equipment.weapon.GetDamage(this.get_CurrentAgent());
                if (EquipmentTypeInfo_patch.GetLcId(this.get_CurrentAgent().Equipment.weapon.metaInfo) != 200038 && EquipmentTypeInfo_patch.GetLcId(this.get_CurrentAgent().Equipment.weapon.metaInfo) != 200004)
                {
                    global::RwbpType type = damage.type;
                    Color color;
                    Color color2;
                    global::UIColorManager.instance.GetRWBPTypeColor(type, out color, out color2);
                    this.TypeFill.enabled = true;
                    this.TypeFill.color = Color.white;
                    this.TypeFill.sprite = global::IconManager.instance.DamageIcon[type - global::RwbpType.R];
                    this.TypeText.color = color;
                    this.TypeText.text = Assets.Scripts.UI.Utils.EnumTextConverter.GetRwbpType(type).ToUpper();
                    string text = string.Format("{0}-{1}", (int)damage.min, (int)damage.max);
                    this.DamageText.text = text;
                }
                else
                {
                    this.TypeFill.color = Color.white;
                    this.TypeFill.enabled = false;
                    this.TypeText.color = Color.gray;
                    this.TypeText.text = "???";
                    string text2 = string.Format("{0}-{1}", (int)damage.min, (int)damage.max);
                    this.DamageText.text = text2;
                }
                global::WorkerPrimaryStatBonus titleBonus = this.get_CurrentAgent().titleBonus;
                int num = this.get_CurrentAgent().primaryStat.maxHP + titleBonus.maxHP;
                int num2 = this.get_CurrentAgent().primaryStat.maxMental + titleBonus.maxMental;
                int num3 = this.get_CurrentAgent().primaryStat.workProb + titleBonus.workProb;
                int num4 = this.get_CurrentAgent().primaryStat.cubeSpeed + titleBonus.cubeSpeed;
                int num5 = this.get_CurrentAgent().primaryStat.attackSpeed + titleBonus.attackSpeed;
                int num6 = this.get_CurrentAgent().primaryStat.movementSpeed + titleBonus.movementSpeed;
                int num7 = this.get_CurrentAgent().maxHp - num;
                int num8 = this.get_CurrentAgent().maxMental - num2;
                int num9 = this.get_CurrentAgent().workProb - num3;
                int num10 = this.get_CurrentAgent().workSpeed - num4;
                int num11 = (int)this.get_CurrentAgent().attackSpeed - num5;
                int num12 = (int)this.get_CurrentAgent().movement - num6;
                if (num7 > 0)
                {
                    this.Stats[0].slots[0].SetText(num + string.Empty, "+" + num7);
                }
                else if (num7 < 0)
                {
                    this.Stats[0].slots[0].SetText(num + string.Empty, "-" + -num7);
                }
                else
                {
                    this.Stats[0].slots[0].SetText(num + string.Empty);
                }
                if (num8 > 0)
                {
                    this.Stats[1].slots[0].SetText(num2 + string.Empty, "+" + num8);
                }
                else if (num8 < 0)
                {
                    this.Stats[1].slots[0].SetText(num2 + string.Empty, "-" + -num8);
                }
                else
                {
                    this.Stats[1].slots[0].SetText(num2 + string.Empty);
                }
                if (num9 > 0)
                {
                    this.Stats[2].slots[0].SetText(num3 + string.Empty, "+" + num9);
                }
                else if (num9 < 0)
                {
                    this.Stats[2].slots[0].SetText(num3 + string.Empty, "-" + -num9);
                }
                else
                {
                    this.Stats[2].slots[0].SetText(num3 + string.Empty);
                }
                if (num10 > 0)
                {
                    this.Stats[2].slots[1].SetText(num4 + string.Empty, "+" + num10);
                }
                else if (num10 < 0)
                {
                    this.Stats[2].slots[1].SetText(num4 + string.Empty, "-" + -num10);
                }
                else
                {
                    this.Stats[2].slots[1].SetText(num4 + string.Empty);
                }
                if (num12 > 0)
                {
                    this.Stats[3].slots[0].SetText(num6 + string.Empty, "+" + num12);
                }
                else if (num12 < 0)
                {
                    this.Stats[3].slots[0].SetText(num6 + string.Empty, "-" + -num12);
                }
                else
                {
                    this.Stats[3].slots[0].SetText(num6 + string.Empty);
                }
                if (num11 > 0)
                {
                    this.Stats[3].slots[1].SetText(num5 + string.Empty, "+" + num11);
                }
                else if (num11 < 0)
                {
                    this.Stats[3].slots[1].SetText(num5 + string.Empty, "-" + -num11);
                }
                else
                {
                    this.Stats[3].slots[1].SetText(num5 + string.Empty);
                }
                this.Stats[0].Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Rstat"), global::AgentModel.GetLevelGradeText(this.get_CurrentAgent().Rstat));
                this.Stats[1].Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Wstat"), global::AgentModel.GetLevelGradeText(this.get_CurrentAgent().Wstat));
                this.Stats[2].Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Bstat"), global::AgentModel.GetLevelGradeText(this.get_CurrentAgent().Bstat));
                this.Stats[3].Fill_Inner.text = string.Format("{0} {1}", global::LocalizeTextDataModel.instance.GetText("Pstat"), global::AgentModel.GetLevelGradeText(this.get_CurrentAgent().Pstat));
                global::DefenseInfo defense = this.get_CurrentAgent().defense;
                global::UIUtil.DefenseSetOnlyText(defense, this.DefenseType);
                global::UIUtil.DefenseSetFactor(defense, this.DefenseFactor, true);
                string name = this.get_CurrentAgent().Equipment.weapon.metaInfo.Name;
                if (name == "UNKNOWN")
                {
                    this.WeaponTitle.text = global::LocalizeTextDataModel.instance.GetText("Inventory_WeaponTitle");
                }
                else
                {
                    this.WeaponTitle.text = name;
                }
                string name2 = this.get_CurrentAgent().Equipment.armor.metaInfo.Name;
                if (name2 == "UNKNOWN")
                {
                    this.ArmorTitle.text = global::LocalizeTextDataModel.instance.GetText("Inventory_ArmorTitle");
                }
                else
                {
                    this.ArmorTitle.text = name2;
                }
                this.SubEquipTitle.text = global::LocalizeTextDataModel.instance.GetText("Inventory_GiftTitle");
                IEnumerator enumerator = this.SubEquipListParent.transform.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object obj = enumerator.Current;
                        Transform transform = (Transform)obj;
                        UnityEngine.Object.Destroy(transform.gameObject);
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
                foreach (global::EquipmentModel equipmentModel in this.get_CurrentAgent().Equipment.gifts.addedGifts)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.attachUnit);
                    global::InventoryAttachmentUnit component = gameObject.GetComponent<global::InventoryAttachmentUnit>();
                    component.text.text = equipmentModel.metaInfo.Name + " : " + equipmentModel.metaInfo.Description;
                    gameObject.transform.SetParent(this.SubEquipListParent.transform);
                    gameObject.transform.localScale = Vector3.one;
                }
                foreach (global::EquipmentModel equipmentModel2 in this.get_CurrentAgent().Equipment.gifts.replacedGifts)
                {
                    GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.attachUnit);
                    global::InventoryAttachmentUnit component2 = gameObject2.GetComponent<global::InventoryAttachmentUnit>();
                    component2.text.text = equipmentModel2.metaInfo.Name + " : " + equipmentModel2.metaInfo.Description;
                    gameObject2.transform.SetParent(this.SubEquipListParent.transform);
                    gameObject2.transform.localScale = Vector3.one;
                }
                InventoryItemController.SetGradeText(this.get_CurrentAgent().Equipment.weapon.metaInfo.Grade, this.WeaponGrade);
                InventoryItemController.SetGradeText(this.get_CurrentAgent().Equipment.armor.metaInfo.Grade, this.ArmorGrade);
                this.WeaponImage.sprite = this.get_CurrentAgent().GetWeaponSprite();
                this.ArmorImage.SetArmor(this.get_CurrentAgent().Equipment.armor.metaInfo);
                VerticalLayoutGroup component3 = this.SubEquipListParent.transform.GetComponent<VerticalLayoutGroup>();
                component3.enabled = false;
                component3.enabled = true;
            }
            catch (Exception message)
            {
                Debug.LogError(message);
            }
        }

        [MemberAlias("get_CurrentAgent", typeof(InventoryAgentController))]
        public global::AgentModel get_CurrentAgent()
        {
            return this._currentAgent;
        }

        [MemberAlias("AgentSlot", typeof(InventoryAgentController))]
        public InventoryAgentSlot AgentSlot;
        [MemberAlias("SubEquipTitle", typeof(InventoryAgentController))]
        public Text SubEquipTitle;
        [MemberAlias("SubEquipListParent", typeof(InventoryAgentController))]
        public RectTransform SubEquipListParent;
        [MemberAlias("attachUnit", typeof(InventoryAgentController))]
        public GameObject attachUnit;
        [MemberAlias("WeaponTitle", typeof(InventoryAgentController))]
        public Text WeaponTitle;
        [MemberAlias("TypeFill", typeof(InventoryAgentController))]
        public Image TypeFill;
        [MemberAlias("TypeText", typeof(InventoryAgentController))]
        public Text TypeText;
        [MemberAlias("DamageText", typeof(InventoryAgentController))]
        public Text DamageText;
        [MemberAlias("WeaponGrade", typeof(InventoryAgentController))]
        public Text WeaponGrade;
        [MemberAlias("WeaponImage", typeof(InventoryAgentController))]
        public Image WeaponImage;
        [MemberAlias("ArmorTitle", typeof(InventoryAgentController))]
        public Text ArmorTitle;
        [MemberAlias("DefenseType", typeof(InventoryAgentController))]
        public Text[] DefenseType;
        [MemberAlias("DefenseFactor", typeof(InventoryAgentController))]
        public Text[] DefenseFactor;
        [MemberAlias("ArmorGrade", typeof(InventoryAgentController))]
        public Text ArmorGrade;
        [MemberAlias("ArmorImage", typeof(InventoryAgentController))]
        public global::WorkerPortraitSetter ArmorImage;
        [MemberAlias("Stats", typeof(InventoryAgentController))]
        public global::AgentInfoWindow.StatObject[] Stats;
        [MemberAlias("_currentAgent",typeof(InventoryAgentController))]
        private global::AgentModel _currentAgent;
    }
    [ModifiesType("InventoryModel")]
public class InventoryModel_patch
    {
        [ModifiesMember("RemoveAllDlcEquipment")]
        public bool RemoveAllDlcEquipment_patch()
        {
            bool result = false;
            foreach (long id in CreatureGenerateInfo.creditCreatures)
            {
                global::CreatureTypeInfo data = CreatureTypeList_patch.instance.GetData_Mod(new LcIdLong(id));
                if (data != null)
                {
                    using (List<global::CreatureEquipmentMakeInfo>.Enumerator enumerator = data.equipMakeInfos.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            global::CreatureEquipmentMakeInfo makeInfo = enumerator.Current;
                            if (this._equipList.RemoveAll((global::EquipmentModel x) => EquipmentTypeInfo_patch.GetLcId(x.metaInfo) == EquipmentTypeInfo_patch.GetLcId(makeInfo.equipTypeInfo)) > 0)
                            {
                                result = true;
                            }
                        }
                    }
                }
            }
            return result;
        }
        [ModifiesMember("LoadGlobalData")]
        public void LoadGlobalData_patch(Dictionary<string, object> dic)
        {
            try
            {
                this._equipList.Clear();
                List<global::InventoryModel.EquipmentSaveData> list = new List<global::InventoryModel.EquipmentSaveData>();
                List<string> modid = new List<string>();
                global::GameUtil.TryGetValue<List<global::InventoryModel.EquipmentSaveData>>(dic, "equips", ref list);
                global::GameUtil.TryGetValue<long>(dic, "nextInstanceId", ref this._nextInstanceId);
               bool result = GameUtil.TryGetValue<List<string>>(dic, "equipsMod", ref modid);
                this._nextInstanceId += 1L;
                foreach (global::InventoryModel.EquipmentSaveData equipmentSaveData in list)
                {
                    int equipTypeId = equipmentSaveData.equipTypeId;
                    if(result)
                    {
                        int index = list.IndexOf(equipmentSaveData);
                        LcId lcid = new LcId(modid[index], equipTypeId);
                        if (EquipmentTypeList_patch.instance.GetData_Mod(lcid) != null)
                        {
                            this.CreateEquipment_Mod(lcid, equipmentSaveData.equipInstanceId);
                        } else if (modid[index] == String.Empty)
                        {
                            foreach (ModInfo info in ((Add_On_patch)(object)Add_On.instance).ModList)
                            {
                                ModInfo_patch pinfo = info.ForceTypeChange<ModInfo_patch>();
                                if (pinfo.modid != String.Empty)
                                {
                                    lcid = new LcId(pinfo.modid, equipTypeId);
                                    if(InventoryModel.Instance.equipList.FindAll((global::EquipmentModel x) => EquipmentTypeInfo_patch.GetLcId(x.metaInfo) == lcid).Count >= 1)
                                    {
                                        continue;
                                    }
                                    if (this.CreateEquipment_Mod(lcid, equipmentSaveData.equipInstanceId) != null)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (global::EquipmentTypeList.instance.GetData(equipTypeId) != null)
                    {
                        this.CreateEquipment_patch(equipTypeId, equipmentSaveData.equipInstanceId);
                    }
                    else
                    {
                        foreach (ModInfo info in ((Add_On_patch)(object)Add_On.instance).ModList)
                        {
                            ModInfo_patch pinfo = info.ForceTypeChange<ModInfo_patch>();
                            if (pinfo.modid != String.Empty)
                            {
                                LcId lcid = new LcId(pinfo.modid, equipTypeId);
                                if (this.CreateEquipment_Mod(lcid, equipmentSaveData.equipInstanceId) != null)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModDebug.Log("InventoryModel.LoadGlobalData error - " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        [ModifiesMember("GetGlobalSaveData")]
        public Dictionary<string, object> GetGlobalSaveData_patch()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            List<global::InventoryModel.EquipmentSaveData> list = new List<global::InventoryModel.EquipmentSaveData>();
            List<string> modid = new List<string>();
            foreach (global::EquipmentModel equipmentModel in this._equipList)
            {
                list.Add(new global::InventoryModel.EquipmentSaveData
                {
                    equipTypeId = equipmentModel.metaInfo.id,
                    equipInstanceId = equipmentModel.instanceId
                });
                modid.Add(((EquipmentTypeInfo_patch)(object)equipmentModel.metaInfo).modid);
            }
            dictionary.Add("equips", list);
            dictionary.Add("equipsMod", modid);
            dictionary.Add("nextInstanceId", this._nextInstanceId);
            return dictionary;
        }
        [NewMember]
        public global::EquipmentModel CreateEquipmentForcely_Mod(LcId id)
        {
            global::EquipmentModel result;
            try
            {
                global::EquipmentTypeInfo data = EquipmentTypeList_patch.instance.GetData_Mod(id);
                this._equipList.FindAll((global::EquipmentModel x) => EquipmentTypeInfo_patch.GetLcId(x.metaInfo) == id);
                global::EquipmentModel equipmentModel = null;
                global::EquipmentTypeInfo.EquipmentType type = data.type;
                if (type != global::EquipmentTypeInfo.EquipmentType.ARMOR)
                {
                    if (type != global::EquipmentTypeInfo.EquipmentType.WEAPON)
                    {
                        equipmentModel = new global::EGOgiftModel();
                    }
                    else
                    {
                        equipmentModel = new global::WeaponModel();
                    }
                }
                else
                {
                    equipmentModel = new global::ArmorModel();
                }
                equipmentModel.instanceId = this._nextInstanceId;
                equipmentModel.metaInfo = data;
                object obj = null;
                try
                {
                    foreach (Assembly assembly in global::Add_On.instance.AssemList)
                    {
                        foreach (Type type2 in assembly.GetTypes())
                        {
                            if (type2.Name == data.script)
                            {
                                obj = Activator.CreateInstance(type2);
                            }
                        }
                    }
                    if (obj == null)
                    {
                        obj = Activator.CreateInstance(Type.GetType(data.script));
                    }
                }
                catch (ArgumentNullException)
                {
                    obj = Activator.CreateInstance(Type.GetType("EquipmentScriptBase"));
                }
                if (obj is global::EquipmentScriptBase)
                {
                    equipmentModel.script = (global::EquipmentScriptBase)obj;
                    equipmentModel.script.SetModel(equipmentModel);
                }
                this._equipList.Add(equipmentModel);
                global::Notice.instance.Send(global::NoticeName.MakeEquipment, new object[]
                {
                equipmentModel
                });
                if (equipmentModel != null)
                {
                    this._nextInstanceId += 1L;
                }
                result = equipmentModel;
            }
            catch (Exception ex)
            {
                ModDebug.Log("CreateEquipmentForcely error - " + ex.Message + Environment.NewLine + ex.StackTrace);
                result = null;
            }
            return result;
        }
        [ModifiesMember("CreateEquipmentForcely")]
        public global::EquipmentModel CreateEquipmentForcely_patch(int id)
        {
            return CreateEquipmentForcely_Mod(new LcId(id));
        }
        [NewMember]
        public Dictionary<LcId, List<global::EquipmentModel>> GetEquipmentListByTypeInfo_Mod()
        {
            Dictionary<LcId, List<global::EquipmentModel>> dictionary = new Dictionary<LcId, List<global::EquipmentModel>>();
            foreach (global::EquipmentModel equipmentModel in this._equipList)
            {
                LcId id = EquipmentTypeInfo_patch.GetLcId(equipmentModel.metaInfo);
                List<global::EquipmentModel> list = null;
                if (dictionary.TryGetValue(id, out list))
                {
                    if (!list.Contains(equipmentModel))
                    {
                        list.Add(equipmentModel);
                    }
                }
                else
                {
                    list = new List<global::EquipmentModel>
                {
                    equipmentModel
                };
                    dictionary.Add(id, list);
                }
            }
            return dictionary;
        }
        [NewMember]
        public global::EquipmentModel CreateEquipment_Mod(LcId id)
        {
            global::EquipmentModel equipmentModel = this.CreateEquipment_Mod(id, this._nextInstanceId);
            if (equipmentModel != null)
            {
                this._nextInstanceId += 1L;
            }
            return equipmentModel;
        }
        [ModifiesMember("CreateEquipment")]
        public global::EquipmentModel CreateEquipment_patch(int id)
        {
            return CreateEquipment_Mod(new LcId(id));
        }
        [NewMember]
        public bool CheckEquipmentCount_Mod(LcId id)
        {
            global::EquipmentTypeInfo data = EquipmentTypeList_patch.instance.GetData_Mod(id);
            return this._equipList.FindAll((EquipmentModel x) => EquipmentTypeInfo_patch.GetLcId(x.metaInfo) == id).Count < data.MaxNum;
        }
        [ModifiesMember("CheckEquipmentCount")]
        public bool CheckEquipmentCount_patch(int id)
        {
            return CheckEquipmentCount_Mod(new LcId(id));
        }
        [NewMember]
        public global::EquipmentModel CreateEquipment_Mod(LcId id, long instanceId)
        {
            try
            {
                ModDebug.Log("Try Make - " + id);
                global::EquipmentTypeInfo data = EquipmentTypeList_patch.instance.GetData_Mod(id);
                if(data == null)
                {
                    ModDebug.Log("Fail Make(Null)- " + id);
                    return null;
                }
                if (this.get_equipList().FindAll((global::EquipmentModel x) => EquipmentTypeInfo_patch.GetLcId(x.metaInfo) == id).Count >= data.MaxNum)
                {
                    ModDebug.Log("Fail Make(Full)- " + id);
                    return null;
                }
                global::EquipmentModel equipmentModel = null;
                global::EquipmentTypeInfo.EquipmentType type = data.type;
                if (type != global::EquipmentTypeInfo.EquipmentType.ARMOR)
                {
                    if (type != global::EquipmentTypeInfo.EquipmentType.WEAPON)
                    {
                        equipmentModel = new global::EGOgiftModel();
                    }
                    else
                    {
                        equipmentModel = new global::WeaponModel();
                    }
                }
                else
                {
                    equipmentModel = new global::ArmorModel();
                }
                equipmentModel.instanceId = instanceId;
                equipmentModel.metaInfo = data;
                object obj = null;
                foreach (Assembly assembly in global::Add_On.instance.AssemList)
                {
                    foreach (Type type2 in assembly.GetTypes())
                    {
                        if (type2.Name == data.script)
                        {
                            obj = Activator.CreateInstance(type2);
                        }
                    }
                }
                if (obj == null)
                {
                    try
                    {
                        obj = Activator.CreateInstance(Type.GetType(data.script));
                    }
                    catch (ArgumentNullException)
                    {
                        obj = Activator.CreateInstance(Type.GetType("EquipmentScriptBase"));
                    }
                }
                if (obj is global::EquipmentScriptBase)
                {
                    equipmentModel.script = (global::EquipmentScriptBase)obj;
                    equipmentModel.script.SetModel(equipmentModel);
                }
                this.get_equipList().Add(equipmentModel);
                global::Notice.instance.Send(global::NoticeName.MakeEquipment, new object[]
                {
            equipmentModel
                });
                ModDebug.Log("Success Make - " + id);
                return equipmentModel;
            } catch(Exception e)
            {
                ModDebug.Log("error make - " + e.Message+Environment.NewLine+e.StackTrace);
                return null;
            }
        }
        [ModifiesMember("CreateEquipment")]
        public global::EquipmentModel CreateEquipment_patch(int id, long instanceId)
        {
            
            return CreateEquipment_Mod(new LcId(id),instanceId);
        }


        [NewMember]
        public bool GetEquipCount_Mod(LcId id, out int current, out int max)
        {
            try
            {
                global::EquipmentTypeInfo data = EquipmentTypeList_patch.instance.GetData_Mod(id);
                List<global::EquipmentModel> list = this._equipList.FindAll((EquipmentModel x) => EquipmentTypeInfo_patch.GetLcId(x.metaInfo) == id);
                current = list.Count;
                max = data.MaxNum;
            }
            catch (Exception)
            {
                current = 0;
                max = 0;
                return false;
            }
            return true;
        }
        [ModifiesMember("GetEquipCount")]
        public bool GetEquipCount_patch(int id, out int current, out int max)
        {
            return GetEquipCount_Mod(new LcId(id), out current, out max);
        }



        [MemberAlias("get_equipList", typeof(InventoryModel))]
        public List<global::EquipmentModel> get_equipList()
        {
            return this._equipList;
        }

        [MemberAlias("_equipList", typeof(InventoryModel))]
        private List<global::EquipmentModel> _equipList = new List<global::EquipmentModel>();
        [MemberAlias("_nextInstanceId",typeof(InventoryModel))]
        public long _nextInstanceId = 1L;
    }
}
