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
using UnityEngine.Events;
using UnityEngine.SceneManagement;


namespace Lobotomypatch
{
    [ModifiesType("NewTitleScript")]
    public class NewTitleScript_patch
    {
        [ModifiesMember("OnSetLanguage")]
        public void OnSetLanguage_patch(string language)
        {
            GlobalGameManager.instance.ForceTypeChange<GlobalGameManager_patch>().ChangeLanguage_new(language);
            if (GlobalEtcDataModel.instance.trueEndingDone)
            {
                SceneManager.LoadSceneAsync("AlterTitleScene");
                return;
            }
            SceneManager.LoadSceneAsync("NewTitleScene");
        }
        [ModifiesMember("Start")]
        private void Start_patch()
        {
            this.OnExitCredit();
            this.StartNewSceneControl.enabled = false;
            this.MenuPanelAnim.enabled = false;
            this.set_IsOpenedOption(false);
            this.ButtonAreaGroup.alpha = 0f;
            this.ButtonAreaGroup.interactable = false;
            this.ButtonAreaGroup.blocksRaycasts = false;
            this.DeleteMaxObserveData();
            bool flag = false;
            this.hiddenRoot.SetActive(false);
            this.defRoot.SetActive(true);
            this.TitleBgm.volume = global::GlobalGameManager.instance.sceneDataSaver.currentBgmVolume;
            Image[] array = this.hiddenOverlay;
            for (int i = 0; i < array.Length; i++)
            {
                array[i].enabled = false;
            }
            if (!global::GlobalGameManager.instance.IsPlaying())
            {
                if (!global::GlobalGameManager.instance.ExistSaveData())
                {
                    Debug.Log("No data");
                    this.contMenuActivated = false;
                    this.continueText.color = this.Disabled;
                }
                else
                {
                    flag = true;
                }
                if (!global::GlobalGameManager.instance.ExistUnlimitData())
                {
                    this.chalMenuActivated = false;
                    this.challengeText.color = this.Disabled;
                }
            }
            if (!flag)
            {
                this.Init();
            }
            this.newGameTooltip.SetActive(false);
            this.GameVersionChecker.rectTransform.sizeDelta = this.GameVersionChecker.rectTransform.sizeDelta + new Vector2(0f, 50f);
            this.GameVersionChecker.rectTransform.localPosition = this.GameVersionChecker.rectTransform.localPosition + new Vector3(0f, -25f, 0f);
            this.GameVersionChecker.text = string.Concat(new object[]
            {
            global::GlobalGameManager.instance.BuildVer,
            "\nBaseMod ",
            global::Add_On.version
            });
            this.newGameObject.GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnClickExitGame));
        }


        [MemberAlias("OnClickExitGame", typeof(NewTitleScript))]
        public void OnClickExitGame()
        {
        }
        [MemberAlias("Init", typeof(NewTitleScript))]
        private void Init()
        {
        }
        [MemberAlias("DeleteMaxObserveData", typeof(NewTitleScript))]
        private void DeleteMaxObserveData()
        {
        }
        [MemberAlias("set_IsOpenedOption", typeof(NewTitleScript))]
        public void set_IsOpenedOption(bool value)
        {
        }
        [MemberAlias("OnExitCredit",typeof(NewTitleScript))]
        public void OnExitCredit()
        {
        }




        [MemberAlias("newGameObject", typeof(NewTitleScript))]
        public GameObject newGameObject;
        [MemberAlias("GameVersionChecker", typeof(NewTitleScript))]
        public Text GameVersionChecker;
        [MemberAlias("newGameTooltip", typeof(NewTitleScript))]
        public GameObject newGameTooltip;
        [MemberAlias("Disabled", typeof(NewTitleScript))]
        public Color Disabled;
        [MemberAlias("challengeText", typeof(NewTitleScript))]
        public Text challengeText;
        [MemberAlias("chalMenuActivated", typeof(NewTitleScript))]
        private bool chalMenuActivated = true;
        [MemberAlias("continueText", typeof(NewTitleScript))]
        public Text continueText;
        [MemberAlias("contMenuActivated", typeof(NewTitleScript))]
        private bool contMenuActivated = true;
        [MemberAlias("hiddenOverlay", typeof(NewTitleScript))]
        public Image[] hiddenOverlay;
        [MemberAlias("TitleBgm", typeof(NewTitleScript))]
        public AudioSource TitleBgm;
        [MemberAlias("defRoot", typeof(NewTitleScript))]
        public GameObject defRoot;
        [MemberAlias("hiddenRoot", typeof(NewTitleScript))]
        public GameObject hiddenRoot;
        [MemberAlias("ButtonAreaGroup", typeof(NewTitleScript))]
        public CanvasGroup ButtonAreaGroup;
        [MemberAlias("MenuPanelAnim", typeof(NewTitleScript))]
        public Animator MenuPanelAnim;
        [MemberAlias("StartNewSceneControl", typeof(NewTitleScript))]
        public Animator StartNewSceneControl;
    }
}
