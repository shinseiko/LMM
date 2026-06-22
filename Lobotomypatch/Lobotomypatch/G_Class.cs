using LobotomyBaseMod;
using Patchwork.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Lobotomypatch
{
    [ModifiesType("GameManager")]
    public class GameManager_patch
    {
        [ModifiesMember("ReturnToCheckPoint")]
        public void ReturnToCheckPoint_patch()
        {
            if (global::GlobalGameManager.instance.ExistSaveData())
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                this.EndGame();
                this.Release();
                LcIdLong num;
                while (((PlayerModel_patch)(object)PlayerModel.instance).GetWaitingCreature_Mod(out num))
                {
                }
                global::GlobalGameManager.instance.sceneDataSaver.currentBgmVolume = global::BgmManager.instance.currentBgmVolume;
                global::GlobalGameManager.instance.sceneDataSaver.currentVolume = global::BgmManager.instance.currentMasterVolume;
                global::GlobalGameManager.instance.SaveGlobalData();
                global::GlobalGameManager.instance.LoadData(global::SaveType.CHECK_POINT);
                CreatureGenerate.CreatureGenerateInfoManager.Instance.Init();
                global::GlobalGameManager.instance.lastLoaded = true;
                global::GlobalGameManager.instance.loadingScene = "StoryEndScene";
                global::GlobalGameManager.instance.loadingScreen.LoadScene("Main");
                return;
            }
            Debug.LogError("save file not found");
        }

        [MemberAlias("Release", typeof(GameManager))]
        private void Release()
        {
        }
        [MemberAlias("EndGame", typeof(GameManager))]
        public void EndGame()
        {
        }

    }
    [ModifiesType("GlobalGameManager")]
    public class GlobalGameManager_patch
    {
        [ModifiesMember("OnApplicationQuit")]
        private void OnApplicationQuit()
        {
            this.SaveLogs();
            this.SaveStateData();

            //string backuppath = Application.dataPath + "/Managed/BaseMod/BackUp.dll";
            //System.IO.File.Copy(backuppath, Application.dataPath + "/Managed/Assembly-CSharp.dll", true);
        }

        [ModifiesMember("GetCurrentLanguage")]
        public string GetCurrentLanguage()
        {
            return language;
        }
        [NewMember]
        public void ChangeLanguage_new(string value)
        {
            this.language = value;
            GameStaticDataLoader.ReloadData();
            this.SetLanguageFont();
            Notice.instance.Send(NoticeName.LanaguageChange, new object[0]);
            ManualUI.Instance.Reload();
        }
        [ModifiesMember("LoadEtcFile")]
        public Dictionary<string, object> LoadEtcFile_patch()
        {
            Dictionary<string, object> dictionary = global::SaveUtil.ReadSerializableFile(this.saveEtcFileName);
            try
            {
                List<KeyValuePair<string, long>> modlist = new List<KeyValuePair<string, long>>();
                bool added = false;
                if (GameUtil.TryGetValue<List<KeyValuePair<string, long>>>(dictionary, "waitingCreatureMod", ref modlist))
                {

                    foreach (KeyValuePair<string, long> id in modlist)
                    {
                        LcIdLong lcid = new LcIdLong(id.Key, id.Value);
                        if (!((PlayerModel_patch)(object)PlayerModel.instance).IsWaitingCreature_Mod(lcid))
                        {
                            ((PlayerModel_patch)(object)PlayerModel.instance).AddWaitingCreature_Mod(lcid);
                            added = true;
                        }
                    }
                }
                if (!added)
                {
                    List<long> list = new List<long>();
                    if (GameUtil.TryGetValue<List<long>>(dictionary, "waitingCreature", ref list))
                    {
                        foreach (long id in list)
                        {
                            if (!PlayerModel.instance.IsWaitingCreature(id))
                            {
                                PlayerModel.instance.AddWaitingCreature(id);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return dictionary;
        }
        [ModifiesMember("SaveEtcData")]
        public void SaveEtcData_patch()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            try
            {
                global::SefiraBossManager.Instance.SaveBossSessionData(dictionary);
            }
            catch
            {
            }
            try
            {
                List<long> list = new List<long>();
                if (PlayerModel.instance.IsWaitingCreatureExist())
                {
                    foreach (LcIdLong item in ((PlayerModel_patch)(object)PlayerModel.instance).addedCreatureMod)
                    {
                        list.Add(item.id);
                    }
                }
                dictionary.Add("waitingCreature", list);

                List<KeyValuePair<string, long>> modlist = new List<KeyValuePair<string, long>>();
                if (PlayerModel.instance.IsWaitingCreatureExist())
                {
                    foreach (LcIdLong item in ((PlayerModel_patch)(object)PlayerModel.instance).addedCreatureMod)
                    {
                        modlist.Add(new KeyValuePair<string, long>(item.packageId, item.id));
                    }
                }
                dictionary.Add("waitingCreatureMod", modlist);
            }
            catch
            {
            }
            global::SaveUtil.WriteSerializableFile(this.saveEtcFileName, dictionary);
        }



        [MemberAlias("SetLanguageFont", typeof(GlobalGameManager))]
        public void SaveStateData()
        {
        }
        [MemberAlias("SaveLogs", typeof(GlobalGameManager))]
        private void SaveLogs()
        {
        }
        [MemberAlias("SetLanguageFont", typeof(GlobalGameManager))]
        public void SetLanguageFont()
        {
        }


        [MemberAlias("language", typeof(GlobalGameManager))]
        public string language = "en";
        [MemberAlias("saveEtcFileName", typeof(GlobalGameManager))]
        private string saveEtcFileName;
    }
}
