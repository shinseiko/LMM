using LobotomyBaseMod;
using Patchwork.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lobotomypatch
{
    [ModifiesType("PlayerModel")]
    public class PlayerModel_patch
    {
        [NewMember]
        public List<LcIdLong> CopyWaitingCreatures_Mod()
        {
            List<LcIdLong> list = new List<LcIdLong>();
            foreach (LcIdLong item in this.addedCreatureMod)
            {
                list.Add(item);
            }
            return list;
        }
        [NewMember]
        public bool GetWaitingCreature_Mod(out LcIdLong id)
        {
            id = new LcIdLong(-1L);
            if (this.addedCreatureMod.Count == 0)
            {
                return false;
            }
            id = this.addedCreatureMod.Dequeue();
            if (global::GlobalGameManager.instance.ExistEtcData())
            {
                global::GlobalGameManager.instance.SaveEtcData();
            }
            return true;
        }
        [NewMember]
        public bool IsWaitingCreature_Mod(LcIdLong id)
        {
            return this.addedCreatureMod.Contains(id);
        }
        [ModifiesMember("IsWaitingCreature")]
        public bool IsWaitingCreature_patch(long id)
        {
            return IsWaitingCreature_Mod(new LcIdLong(id));
        }
        [ModifiesMember("IsWaitingCreatureExist")]
        public bool IsWaitingCreatureExist_patch()
        {
            if (this.day >= 20 && this.day < 25)
            {
                return this.addedCreatureMod.Count >= 2;
            }
            if (this.day >= 45 && this.day < 50)
            {
                return this.addedCreatureMod.Count >= 2;
            }
            return this.addedCreatureMod.Count >= 1;
        }
        [NewMember]
        public void InitAddingCreatures_Mod()
        {
            addedCreatureMod.Clear();
        }
        [ModifiesMember("InitAddingCreatures")]
        public void InitAddingCreatures_patch()
        {
            InitAddingCreatures_Mod();
        }
        [NewMember]
        public void AddWaitingCreature_Mod(LcIdLong id)
        {
            addedCreatureMod.Enqueue(id);
        }
        [ModifiesMember("AddWaitingCreature")]
        public void AddWaitingCreature_patch(long id)
        {
            AddWaitingCreature_Mod(new LcIdLong(id));
        }
        [ModifiesMember(".ctor")]
        public void Ctor()
        {
            addedCreatureMod = new Queue<LcIdLong>();
            addedCreature = new Queue<long>();
        }

        [NewMember]
        public Queue<LcIdLong> addedCreatureMod;


        [MemberAlias("day", typeof(PlayerModel))]
        private int day;
        [MemberAlias("addedCreature", typeof(PlayerModel))]
        public Queue<long> addedCreature;
    }
}
