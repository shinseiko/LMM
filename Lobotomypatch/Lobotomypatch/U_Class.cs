using Harmony;
using LobotomyBaseMod;
using Patchwork.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static UnitEGOgiftSpace;
using LobotomyBaseModLib;

namespace Lobotomypatch
{
    [ModifiesType("UnitBuf")]
    public class UnitBuf_patch
    {
        [NewMember]
        public virtual float OnGiveDamageMult(UnitModel target, DamageInfo dmg)
        {
            return 1;
        }
        [NewMember]
        public virtual void OnSuppressed(CreatureModel model)
        {

        }
    }
    [ModifiesType("UnitEquipSpace")]
    public class UnitEquipSpace_patch
    {
        [NewMember]
        public bool HasEquipment_Mod(LcId id)
        {
            return (this.weapon != null && EquipmentTypeInfo_patch.GetLcId(this.weapon.metaInfo) == id) || (this.armor != null && EquipmentTypeInfo_patch.GetLcId(this.armor.metaInfo) == id) || ((UnitEGOgiftSpace_patch)(object)this.gifts).HasEquipment_Mod(id);
        }
        [ModifiesMember("HasEquipment")]
        public bool HasEquipment_patch(int id)
        {
            return HasEquipment_Mod(new LcId(id));
        }




        [MemberAlias("weapon", typeof(UnitEquipSpace))]
        public global::WeaponModel weapon;
        [MemberAlias("armor", typeof(UnitEquipSpace))]
        public global::ArmorModel armor;
        [MemberAlias("gifts", typeof(UnitEquipSpace))]
        public global::UnitEGOgiftSpace gifts = new global::UnitEGOgiftSpace();
        [MemberAlias("kitCreature",typeof(UnitEquipSpace))]
        public global::CreatureModel kitCreature;
    }
    [ModifiesType("UnitEGOgiftSpace")]
    public class UnitEGOgiftSpace_patch
    {
        [NewMember]
        public bool HasEquipment_Mod(LcId id)
        {
            foreach (global::EGOgiftModel egogiftModel in this.replacedGifts)
            {
                if (EquipmentTypeInfo_patch.GetLcId(egogiftModel.metaInfo) == id)
                {
                    return true;
                }
            }
            foreach (global::EGOgiftModel egogiftModel2 in this.addedGifts)
            {
                if (EquipmentTypeInfo_patch.GetLcId(egogiftModel2.metaInfo) == id)
                {
                    return true;
                }
            }
            return false;
        }
        [ModifiesMember("HasEquipment")]
        public bool HasEquipment_patch(int id)
        {
            return HasEquipment_Mod(new LcId(id));
        }
        [ModifiesMember("ReleaseGift")]
        public void ReleaseGift_patch(global::EGOgiftModel gift)
        {
            this.displayState.Remove(UnitEGOgiftSpace.GetRegionId(gift.metaInfo));
            this.lockState.Remove(UnitEGOgiftSpace.GetRegionId(gift.metaInfo));
        }
        [ModifiesMember("LoadDataAndAttach")]
        public void LoadDataAndAttach_patch(global::UnitModel owner, Dictionary<string, object> dic)
        {
            new Dictionary<string, object>();
            List<int> list = new List<int>();
            List<string> modlist = new List<string>();
            global::GameUtil.TryGetValue<List<int>>(dic, "giftTypeIdList", ref list);
            bool modlistinit = global::GameUtil.TryGetValue<List<string>>(dic, "giftTypeIdListMod", ref modlist);
            try
            {
                global::GameUtil.TryGetValue<Dictionary<int, global::UnitEGOgiftSpace.GiftLockState>>(dic, "lockState", ref this.lockState);
                global::GameUtil.TryGetValue<Dictionary<int, bool>>(dic, "displayState", ref this.displayState);

                Dictionary<int, string> modlock = new Dictionary<int, string>();
                GameUtil.TryGetValue<Dictionary<int, string>>(dic, "lockStateMod", ref modlock);
                foreach (KeyValuePair<int, UnitEGOgiftSpace.GiftLockState> pair in lockState)
                {
                    if(modlock.ContainsKey(pair.Key))
                    {
                        ((UnitEGOgiftSpace_GiftLockState_patch)pair.Value).modid = modlock[pair.Key];
                    } else
                    {
                        ((UnitEGOgiftSpace_GiftLockState_patch)pair.Value).modid = String.Empty;
                    } 
                }
            }
            catch (Exception)
            {
                this.lockState.Clear();
                this.displayState.Clear();
            }
            int i = 0;
            foreach (int num in list)
            {
                if(modlistinit)
                {
                    EquipmentTypeInfo data = EquipmentTypeList_patch.instance.GetData_Mod(new LcId(modlist[i],num));
                    if (data != null)
                    {
                        if (data.type != global::EquipmentTypeInfo.EquipmentType.SPECIAL)
                        {
                            ModDebug.Log("id : " + num + " is not gift");
                        }
                        else
                        {
                            global::EGOgiftModel gift = global::EGOgiftModel.MakeGift(data);
                            owner.AttachEGOgift(gift);
                        }
                    }
                } else
                {
                    EquipmentTypeInfo data = global::EquipmentTypeList.instance.GetData(num);
                    if (data != null)
                    {
                        if (data.type != global::EquipmentTypeInfo.EquipmentType.SPECIAL)
                        {
                            ModDebug.Log("id : " + num + " is not gift");
                        }
                        else
                        {
                            global::EGOgiftModel gift = global::EGOgiftModel.MakeGift(data);
                            owner.AttachEGOgift(gift);
                        }
                    }
                }
                i++;
                
            }
        }
        [ModifiesMember("GetSaveData")]
        public Dictionary<string, object> GetSaveData_patch()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            List<int> list = new List<int>();
            List<string> modlist = new List<string>();
            foreach (global::EGOgiftModel egogiftModel in this.replacedGifts)
            {
                list.Add(egogiftModel.metaInfo.id);
                modlist.Add(EquipmentTypeInfo_patch.GetLcId(egogiftModel.metaInfo).packageId);
            }
            foreach (global::EGOgiftModel egogiftModel2 in this.addedGifts)
            {
                list.Add(egogiftModel2.metaInfo.id);
                modlist.Add(EquipmentTypeInfo_patch.GetLcId(egogiftModel2.metaInfo).packageId);
            }
            Dictionary<int, string> modlock = new Dictionary<int, string>();
            foreach(KeyValuePair<int,UnitEGOgiftSpace.GiftLockState> pair in lockState)
            {
                modlock[pair.Key] = ((UnitEGOgiftSpace_GiftLockState_patch)pair.Value).modid;
            }
            dictionary.Add("giftTypeIdList", list);
            dictionary.Add("giftTypeIdListMod", modlist);
            dictionary.Add("lockState", this.lockState);
            dictionary.Add("lockStateMod", modlock);
            dictionary.Add("displayState", this.displayState);
            return dictionary;
        }
        [ModifiesMember("GetLockStateUI")]
        public bool GetLockStateUI_patch(global::EquipmentTypeInfo info)
        {
            if (this.lockState.ContainsKey(global::UnitEGOgiftSpace.GetRegionId(info)))
            {
                return this.lockState[global::UnitEGOgiftSpace.GetRegionId(info)].state;
            }
            this.lockState.Add(global::UnitEGOgiftSpace.GetRegionId(info), new global::UnitEGOgiftSpace.GiftLockState
            {
                id = (long)info.id,
                state = false
            });
            ((UnitEGOgiftSpace_GiftLockState_patch)this.lockState[UnitEGOgiftSpace.GetRegionId(info)]).modid = EquipmentTypeInfo_patch.GetLcId(info).packageId;
            return false;
        }
        [ModifiesMember("AttachGift")]
        public void AttachGift_patch(global::UnitModel owner, global::EGOgiftModel model)
        {
            if (model.metaInfo.attachType == global::EGOgiftAttachType.ADD)
            {
                global::EGOgiftModel egogiftModel = this.addedGifts.Find((global::EGOgiftModel x) => x.metaInfo.attachType == global::EGOgiftAttachType.ADD && x.metaInfo.attachPos == model.metaInfo.attachPos);
                if (egogiftModel != null)
                {
                    egogiftModel.OnRelease();
                    this.addedGifts.Remove(egogiftModel);
                    global::Notice.instance.Send(global::NoticeName.OnChangeGift, new object[]
                    {
                    owner
                    });
                }
                this.addedGifts.Add(model);
                model.OnEquip(owner);
            }
            else if (model.metaInfo.attachType == global::EGOgiftAttachType.SPECIAL_ADD)
            {
                global::EGOgiftModel egogiftModel2 = this.addedGifts.Find((global::EGOgiftModel x) => x.metaInfo.attachType == global::EGOgiftAttachType.SPECIAL_ADD && x.metaInfo.attachPos == model.metaInfo.attachPos);
                if (egogiftModel2 != null)
                {
                    egogiftModel2.OnRelease();
                    this.addedGifts.Remove(egogiftModel2);
                    global::Notice.instance.Send(global::NoticeName.OnChangeGift, new object[]
                    {
                    owner
                    });
                }
                this.addedGifts.Add(model);
                model.OnEquip(owner);
            }
            else if (model.metaInfo.attachType == global::EGOgiftAttachType.REPLACE)
            {
                global::EGOgiftModel egogiftModel3 = this.replacedGifts.Find((global::EGOgiftModel x) => x.metaInfo.attachType == global::EGOgiftAttachType.REPLACE && x.metaInfo.attachPos == model.metaInfo.attachPos);
                if (egogiftModel3 != null)
                {
                    egogiftModel3.OnRelease();
                    this.replacedGifts.Remove(egogiftModel3);
                    global::Notice.instance.Send(global::NoticeName.OnChangeGift, new object[]
                    {
                    owner
                    });
                }
                this.replacedGifts.Add(model);
                model.OnEquip(owner);
            }
            if (!this.displayState.ContainsKey(global::UnitEGOgiftSpace.GetRegionId(model.metaInfo)))
            {
                this.displayState.Add(global::UnitEGOgiftSpace.GetRegionId(model.metaInfo), true);
            }
            if (!this.lockState.ContainsKey(global::UnitEGOgiftSpace.GetRegionId(model.metaInfo)))
            {
                this.lockState.Add(global::UnitEGOgiftSpace.GetRegionId(model.metaInfo), new global::UnitEGOgiftSpace.GiftLockState
                {
                    id = (long)model.metaInfo.id,
                    state = false
                });
                ((UnitEGOgiftSpace_GiftLockState_patch)this.lockState[UnitEGOgiftSpace.GetRegionId(model.metaInfo)]).modid = EquipmentTypeInfo_patch.GetLcId(model.metaInfo).packageId;
            }
        }
        [ModifiesMember("SetLockState")]
        public void SetLockState_patch(global::EGOgiftModel model, bool state)
        {
            if (this.lockState.ContainsKey(global::UnitEGOgiftSpace.GetRegionId(model.metaInfo)))
            {
                UnitEGOgiftSpace_GiftLockState_patch giftLockState = (UnitEGOgiftSpace_GiftLockState_patch)this.lockState[global::UnitEGOgiftSpace.GetRegionId(model.metaInfo)];
                giftLockState.id = (long)model.metaInfo.id;
                giftLockState.modid = EquipmentTypeInfo_patch.GetLcId(model.metaInfo).packageId;
                giftLockState.state = state;
                this.lockState[global::UnitEGOgiftSpace.GetRegionId(model.metaInfo)] = giftLockState;
            }
            else
            {
                this.lockState.Add(global::UnitEGOgiftSpace.GetRegionId(model.metaInfo), new UnitEGOgiftSpace.GiftLockState
                {
                    id = (long)model.metaInfo.id,
                    state = false
            });
                ((UnitEGOgiftSpace_GiftLockState_patch)this.lockState[UnitEGOgiftSpace.GetRegionId(model.metaInfo)]).modid = EquipmentTypeInfo_patch.GetLcId(model.metaInfo).packageId;
            }
        }
        [ModifiesMember("GetLockState")]
        public bool GetLockState_patch(global::EquipmentTypeInfo info)
        {
            if (this.lockState.ContainsKey(global::UnitEGOgiftSpace.GetRegionId(info)))
            {
                global::UnitEGOgiftSpace.GiftLockState giftLockState = this.lockState[global::UnitEGOgiftSpace.GetRegionId(info)];
                LcId lcid = UnitEGOgiftSpace_GiftLockState_patch.GetLcId(giftLockState);
                return EquipmentTypeInfo_patch.GetLcId(info) != lcid && giftLockState.state;
            }
            this.lockState.Add(global::UnitEGOgiftSpace.GetRegionId(info), new UnitEGOgiftSpace.GiftLockState
            {
                id = (long)info.id,
                state = false
            });
            ((UnitEGOgiftSpace_GiftLockState_patch)this.lockState[UnitEGOgiftSpace.GetRegionId(info)]).modid = EquipmentTypeInfo_patch.GetLcId(info).packageId;
            return false;
        }





        [MemberAlias("replacedGifts", typeof(UnitEGOgiftSpace))]
        public List<global::EGOgiftModel> replacedGifts;
        [MemberAlias("addedGifts", typeof(UnitEGOgiftSpace))]
        public List<global::EGOgiftModel> addedGifts;
        [MemberAlias("displayState", typeof(UnitEGOgiftSpace))]
        public Dictionary<int, bool> displayState = new Dictionary<int, bool>();
        [MemberAlias("lockState", typeof(UnitEGOgiftSpace))]
        public Dictionary<int, global::UnitEGOgiftSpace.GiftLockState> lockState = new Dictionary<int, global::UnitEGOgiftSpace.GiftLockState>();
    }
    [ModifiesType]
    public class UnitEGOgiftSpace_GiftLockState_patch : UnitEGOgiftSpace.GiftLockState
    {
        [NewMember]
        public static LcId GetLcId(UnitEGOgiftSpace.GiftLockState LockState)
        {
            if(((UnitEGOgiftSpace_GiftLockState_patch)LockState).modid == null)
            {
                return new LcId((int)LockState.id);
            }
            return new LcId(((UnitEGOgiftSpace_GiftLockState_patch)LockState).modid, (int)LockState.id);
        }
        [NewMember]
        [NonSerialized]
        public string modid;
    }
    [ModifiesType("UnitModel")]
    public class UnitModel_patch
    {

      
        [NewMember]
        public bool HasEquipment_Mod(LcId id)
        {
            return ((UnitEquipSpace_patch)(object)this._equipment).HasEquipment_Mod(id);
        }
        [ModifiesMember("GetBufDamageMultiplier")]
        public virtual float GetBufDamageMultiplier(global::UnitModel attacker, global::DamageInfo damage)
        {
            float num = 1f;
            foreach (global::UnitBuf unitBuf in this._bufList)
            {
                num *= unitBuf.OnTakeDamage(attacker, damage);
            }
            if (attacker != null)
            {
                foreach (var unitBuf in attacker.GetUnitBufList())
                {
                    num *= unitBuf.ForceTypeChange<UnitBuf_patch>().OnGiveDamageMult(attacker, damage);
                }
            }
            return num;
        }
        [ModifiesMember("HasEquipment")]
        public bool HasEquipment_patch(int id)
        {
            return HasEquipment_Mod(new LcId(id));
        }
        [ModifiesMember("AttachEGOgift")]
        public void AttachEGOgift_patch(global::EGOgiftModel gift)
        {
            LcId lcid = EquipmentTypeInfo_patch.GetLcId(gift.metaInfo);
            if (!global::UnitEGOgiftSpace.IsUniqueLock((long)gift.metaInfo.id) || lcid.packageId != string.Empty)
            {
                if (this._equipment.gifts.GetLockState(gift.metaInfo))
                {
                    return;
                }
            }
            else
            {
                this._equipment.gifts.SetLockState(gift, false);
            }
            this._equipment.gifts.AttachGift((UnitModel)(object)this, gift);
            ModDebug.Log("AttachGift - get_maxHp : " + get_maxHp().ToString());
            if (this.hp > (float)this.get_maxHp())
            {
                this.hp = (float)this.get_maxHp();
            }
            if (this.hp < 1f)
            {
                this.hp = 1f;
            }
            ModDebug.Log("AttachGift - get_maxMp : " + get_maxMental().ToString());
            if (this.mental > (float)this.get_maxMental())
            {
                this.mental = (float)this.get_maxMental();
            }
            this.OnChangeGift();
        }
        [NewMember]
        public bool ReleaseEGOGift_Mod(LcId id)
        {
            List<EGOgiftModel> addedGifts = _equipment.gifts.addedGifts;
            EGOgiftModel eGOgiftModel = addedGifts.Find((EGOgiftModel x) => EquipmentTypeInfo_patch.GetLcId(x.metaInfo) == id);
            if (eGOgiftModel != null)
            {
                eGOgiftModel.OnRelease();
                addedGifts.Remove(eGOgiftModel);
                OnChangeGift();
                _equipment.gifts.ReleaseGift(eGOgiftModel);
                return true;
            }

            List<EGOgiftModel> replacedGifts = _equipment.gifts.replacedGifts;
            EGOgiftModel eGOgiftModel2 = replacedGifts.Find((EGOgiftModel x) => EquipmentTypeInfo_patch.GetLcId(x.metaInfo) == id);
            if (eGOgiftModel2 != null)
            {
                eGOgiftModel2.OnRelease();
                replacedGifts.Remove(eGOgiftModel2);
                OnChangeGift();
                _equipment.gifts.ReleaseGift(eGOgiftModel2);
                return true;
            }

            return false;
        }


        [ModifiesMember("ReleaseEGOgift")]
        public void ReleaseEGOgift_patch(global::EGOgiftModel model)
        {
            ReleaseEGOGift_Mod(EquipmentTypeInfo_patch.GetLcId(model.metaInfo));
        }
        [ModifiesMember("ReleaseEGOGift")]
        public bool ReleaseEGOGift_patch(int id)
        {
            return ReleaseEGOGift_Mod(new LcId(id));
        }





        [MemberAlias("get_maxMental", typeof(UnitModel), AliasCallMode.Virtual)]
        public int get_maxMental()
        {
            return 0;
        }
        [MemberAlias("get_maxHp", typeof(UnitModel),AliasCallMode.Virtual)]
        public int get_maxHp()
        {
            return 0;
        }
        [MemberAlias("OnChangeGift", typeof(UnitModel), AliasCallMode.Virtual)]
        protected virtual void OnChangeGift()
        {
        }


        [MemberAlias("stunCriteria", typeof(UnitModel))]
        public const float stunCriteria = 2f;
        [MemberAlias("defaultStunEffectSrc", typeof(UnitModel))]
        public const string defaultStunEffectSrc = "Effect/Stun";
        [MemberAlias("instanceId", typeof(UnitModel))]
        public long instanceId;
        [MemberAlias("movableNode", typeof(UnitModel))]
        protected global::MovableObjectNode movableNode;
        [MemberAlias("shield", typeof(UnitModel))]
        public global::UnitShieldEquipment shield;
        [MemberAlias("_equipment", typeof(UnitModel))]
        protected global::UnitEquipSpace _equipment = new global::UnitEquipSpace();
        [MemberAlias("tempAnim", typeof(UnitModel))]
        public global::DummyAttackAnimator tempAnim = new global::DummyAttackAnimator();
        [MemberAlias("factionTypeInfo", typeof(UnitModel))]
        protected global::FactionTypeInfo factionTypeInfo;
        [MemberAlias("stunTimer", typeof(UnitModel))]
        protected global::AutoTimer stunTimer = new global::AutoTimer();
        [MemberAlias("hp", typeof(UnitModel))]
        public float hp;
        [MemberAlias("mental", typeof(UnitModel))]
        public float mental;
        [MemberAlias("baseMaxHp", typeof(UnitModel))]
        public int baseMaxHp;
        [MemberAlias("baseMaxMental", typeof(UnitModel))]
        public int baseMaxMental;
        [MemberAlias("baseMovement", typeof(UnitModel))]
        public float baseMovement;
        [MemberAlias("baseRegeneration", typeof(UnitModel))]
        public int baseRegeneration;
        [MemberAlias("baseRegenerationDelay", typeof(UnitModel))]
        public float baseRegenerationDelay = 5f;
        [MemberAlias("additionalDef", typeof(UnitModel))]
        public global::DefenseInfo additionalDef = new global::DefenseInfo();
        [MemberAlias("superArmorMax", typeof(UnitModel))]
        public float superArmorMax;
        [MemberAlias("superArmor", typeof(UnitModel))]
        public float superArmor;
        [MemberAlias("superArmorDefense", typeof(UnitModel))]
        public float superArmorDefense;
        [MemberAlias("remainMoveDelay", typeof(UnitModel))]
        public float remainMoveDelay;
        [MemberAlias("remainAttackDelay", typeof(UnitModel))]
        public float remainAttackDelay;
        [MemberAlias("isStun", typeof(UnitModel))]
        protected bool isStun;
        [MemberAlias("damageTransform", typeof(UnitModel))]
        public global::StatTransform damageTransform = new global::IdentityTransform();
        [MemberAlias("basePhysicalDefense", typeof(UnitModel))]
        public int basePhysicalDefense;
        [MemberAlias("baseMentalDefense", typeof(UnitModel))]
        public int baseMentalDefense;
        [MemberAlias("encounteredWorker", typeof(UnitModel))]
        public List<global::WorkerModel> encounteredWorker = new List<global::WorkerModel>();
        [MemberAlias("_bufList", typeof(UnitModel),AliasCallMode.Virtual)]
        protected List<UnitBuf> _bufList = new List<UnitBuf>();
        [MemberAlias("_statBufList", typeof(UnitModel), AliasCallMode.Virtual)]
        protected List<global::UnitStatBuf> _statBufList = new List<global::UnitStatBuf>();
        [MemberAlias("_barrierBufList", typeof(UnitModel), AliasCallMode.Virtual)]
        protected List<global::BarrierBuf> _barrierBufList = new List<global::BarrierBuf>();
    }
}
