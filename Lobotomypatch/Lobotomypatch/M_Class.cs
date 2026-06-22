using LobotomyBaseMod;
using Patchwork.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using LobotomyBaseModLib;
using Assets.Scripts.UI.Utils;

namespace Lobotomypatch
{
    [ModifiesType()]
    public class ModInfo_patch : ModInfo
    {

        public ModInfo_patch(DirectoryInfo dir) : base(dir)
        {

        }
        [NewMember()]
        public void Init(DirectoryInfo dir)
        {
            this.foldername = dir.Name;
            this.modpath = dir;
            options = new List<ModOptionData>();
           string smodinfo = string.Empty;
            string smodid = string.Empty;
            string smoddesc = string.Empty;
            modid = string.Empty;
            string lang = GlobalGameManager.instance.GetCurrentLanguage();
            bool flag = File.Exists(dir.FullName + "/Info/" + lang + "/info.xml");
            if (!flag)
            {
                lang = "en";
                flag = File.Exists(dir.FullName + "/Info/" + lang + "/info.xml");
            }
            if(!flag)
            {
                lang = "kr";
                flag = File.Exists(dir.FullName + "/Info/" + lang + "/info.xml");
            }
            if (flag)
            {
                string xml = File.ReadAllText(dir.FullName + "/Info/" + lang + "/info.xml");
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                XmlNode xmlNode = xmlDocument.SelectSingleNode("/info/name");
                XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("/info/descs").SelectNodes("desc");
                this.modname = xmlNode.InnerText;
                smodinfo = string.Concat(new string[]
                {
                "Folder : ",
                this.foldername,
                Environment.NewLine,
                "Name : ",
                this.modname,
                Environment.NewLine
                });
                xmlNode = xmlDocument.SelectSingleNode("/info/ID");

                if (xmlNode != null)
                {
                    smodid = "ID : " + xmlNode.InnerText + Environment.NewLine;
                    modid = xmlNode.InnerText;
                }
                foreach (object obj in xmlNodeList)
                {
                    XmlNode xmlNode2 = (XmlNode)obj;
                    smoddesc = smoddesc + Environment.NewLine + xmlNode2.InnerText;
                }

                if (File.Exists(dir.FullName + "/Info/GlobalInfo.xml"))
                {
                    xml = File.ReadAllText(dir.FullName + "/Info/GlobalInfo.xml");
                    xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(xml);

                    xmlNode = xmlDocument.SelectSingleNode("/info/ID");
                    if (xmlNode != null)
                    {
                        smodid = "ID : " + xmlNode.InnerText + Environment.NewLine;
                        modid = xmlNode.InnerText;

                        var opts = xmlDocument.SelectNodes("/info/Option");
                    
                        if (opts != null && opts.Count > 0)
                        {
                            ModDebug.Log("Check1");
                            for (int i = 0; i < opts.Count; i++)
                            {
                                ModDebug.Log("Check2");
                                var opt = opts[i];
                                var type = opt.SelectSingleNode("Type").InnerText;
                                if (type == null) continue;
                                ModDebug.Log($"Check3 {type}");
                                if (type == "Toggle")
                                {
                                    ModDebug.Log($"Check4-1");
                                    ModOptionData md = new ModOptionData();
                                    md.optionName = opt.SelectSingleNode("Name").InnerText;
                                    var v = opt.SelectSingleNode("DefValue").InnerText;
                                    md.bvalue = v == "true";
                                    md.type = ModOptionJsonType.Toggle;
                                    options.Add(md);
                                    ModDebug.Log($"Check4-2 { md.optionName}");
                                }
                                 if (type == "Slider")
                                {
                                    ModDebug.Log($"Check5-1 {type}");
                                    ModOptionData md = new ModOptionData();
                                    md.optionName = opt.SelectSingleNode("Name").InnerText;
                                    var v = opt.SelectSingleNode("DefValue").InnerText;
                                    md.fvalue = float.Parse(v);
                                    md.type = ModOptionJsonType.Slider;
                                    options.Add(md);
                                    ModDebug.Log($"Check5-2 { md.optionName}");
                                }
                            }
                        }
                    }

                  
                }
                this.modinfo = smodinfo + smodid + smoddesc;
                return;
            }
            this.modinfo = "UnKnown";
            this.modname = this.foldername;

        }

        [NewMember]
        public string modid;

        [NewMember]
        public DirectoryInfo modpath;

        [NewMember]
        public List<ModOptionData> options = new List<ModOptionData>();
    }
    [ModifiesType()]
    public class ModList_patch : ModList
    {
        [NewMember]
        public void OnClickModOption()
        {
            if (optionPanel.activeSelf)
            {
                optionPanel.SetActive(false);
            }
            else
            {
                optionPanel.SetActive(true);
                optionCanvas.CreateOptions(curinfo);
            }
        }
        [NewMember]
        public GameObject MakeModOptionPanel()
        {
            GameObject gameObject = new GameObject("BackGround_Option");
            Image image = gameObject.AddComponent<Image>();
            image.transform.SetParent(base.gameObject.transform);
            Texture2D texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(File.ReadAllBytes(Application.dataPath + "/Managed/BaseMod/Image/Desc.png"));
            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
            image.sprite = sprite;
            image.rectTransform.sizeDelta = new Vector2((float)texture2D.width, (float)texture2D.height);
            gameObject.transform.localScale = new Vector3(1f, 1f);
            gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            //var setting = gameObject.AddComponent<VerticalLayoutGroup>();
            //setting.childForceExpandWidth = false;
            //setting.childForceExpandHeight = false;
            //var csf = gameObject.AddComponent<ContentSizeFitter>();
            //csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            //csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            gameObject.SetActive(true);
            optionCanvas = gameObject.AddComponent<ModOptionListUI>();
            optionCanvas.Init(optionCanvas.transform as RectTransform);
            return gameObject;
        }
        [NewMember]
        public GameObject MakeModOptionBtn()
        {
            GameObject gameObject = new GameObject("Config");
            Image image = gameObject.AddComponent<Image>();
            //gameObject.AddComponent<FrameDummy>();
            gameObject.transform.SetParent(base.gameObject.transform);
            Texture2D texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(File.ReadAllBytes(Application.dataPath + "/Managed/BaseMod/Image/Config.png"));
            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
            image.sprite = sprite;
            image.rectTransform.sizeDelta = new Vector2(70, 70);
            gameObject.transform.localScale = new Vector3(1f, 1f);
            Button button = gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(delegate
            {
                this.OnClickModOption();
            });
            return gameObject;
        }
        [ModifiesMember("OnClickModInfo", ModificationScope.All)]
        public void OnClickModInfo_patch(int i)
        {
            if (this.Modlist.Count >= i + this.Page * 5)
            {
                curinfo = (ModInfo_patch)this.Modlist[this.Page * 5 + i];
                this.Desc.text = curinfo.modinfo;
                optionPanel.SetActive(false);
                modOptionBtn.SetActive(curinfo.options.Count > 0);
            }
        }
        [ModifiesMember("MakeModDesc", ModificationScope.All)]
        public GameObject MakeModDesc_patch(global::ModInfo info)
        {
            GameObject gameObject = new GameObject("BackGround");
            Image image = gameObject.AddComponent<Image>();
            image.transform.SetParent(base.gameObject.transform);
            Texture2D texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(File.ReadAllBytes(Application.dataPath + "/Managed/BaseMod/Image/Desc.png"));
            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f));
            image.sprite = sprite;
            image.rectTransform.sizeDelta = new Vector2((float)texture2D.width, (float)texture2D.height);
            gameObject.transform.localScale = new Vector3(1f, 1f);
            gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
            //var setting = gameObject.AddComponent<VerticalLayoutGroup>();
            //setting.childForceExpandWidth = false;
            //setting.childForceExpandHeight = false;
            //var csf = gameObject.AddComponent<ContentSizeFitter>();
            //csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            //csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            gameObject.SetActive(true);
            return gameObject;
        }
        [ModifiesMember("Awake", ModificationScope.All)]
        public void Awake_patch()
        {
            Instance = this;
            try
            {
                this.Modlist = ((Add_On_patch)Add_On.instance).ModList;
                this.Page = 0;
                if (!this.init)
                {
                    this.PanelList = new List<GameObject>();
                    this.PanelTextList = new List<Text>();
                    for (int i = 0; i < 5; i++)
                    {
                        GameObject gameObject;
                        if (this.Modlist.Count > i)
                        {
                            gameObject = this.MakeModInfo(this.Modlist[i], i);
                            this.MakeModInfo2(this.Modlist[i], gameObject);
                        }
                        else
                        {
                            gameObject = this.MakeModInfo(null, i);
                            this.MakeModInfo2(null, gameObject);
                        }
                        this.PanelList.Add(gameObject);
                        gameObject.transform.localPosition = new Vector2(-800f, (float)(255 - i * 150));
                    }
                    this.DescPanel = this.MakeModDesc(null);
                    this.MakeModDesc2(null);
                    this.DescPanel.transform.localPosition = new Vector2(160f, -75f);
                    this.init = true;
                    this.Down = this.MakeDownButton();
                    this.Down.transform.localPosition = new Vector2(-795f, -445f);
                    this.Up = this.MakeUpButton();
                    this.Up.transform.localPosition = new Vector2(-795f, 355f);
                    modOptionBtn = this.MakeModOptionBtn();
                    modOptionBtn.transform.localPosition = new Vector3(-640, -470);
                    optionPanel = MakeModOptionPanel();
                    optionPanel.transform.localPosition = new Vector2(160f, -75f);
                    optionPanel.SetActive(false);
                    this.UpdatePage();
                }
                optionPanel.SetActive(false);
                this.Desc.text = string.Empty;
                curinfo = null;
                modOptionBtn.SetActive(false);
            }
            catch (Exception ex)
            {
                ModDebug.Log("AWKerror - " + ex.Message + Environment.NewLine + ex.StackTrace);
                //File.WriteAllText(Add_On.Error_Report + "AWKerror.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
        [NewMember]
        public ModInfo_patch curinfo;
        [NewMember]
        public static GameObject optionPanel;
        [NewMember]
        public static ModList Instance;
        [NewMember]
        public ModOptionListUI optionCanvas;
        [NewMember]
        public GameObject modOptionBtn;
        [NewType]
        public class ModOptionListUI : MonoBehaviour
        {
            public void Init(RectTransform parent)
            {
                canvas = parent;
                options = new List<ModOptionUI>();
            }
            public void CreateOptions(ModInfo_patch curinfo)
            {
                Clear();
                this.curinfo = curinfo;
                foreach (var opt in curinfo.options)
                {
                    if (opt.type == ModOptionJsonType.Slider)
                    {
                        AddOptionTMP_Slider(opt);
                    }
                    if (opt.type == ModOptionJsonType.Toggle)
                    {
                        AddOptionTMP_Toggle(opt);
                    }
                }
			}
            public void AddOptionTMP_Slider(ModOptionData md)
            {
               string name = LocalizeTextDataModel.instance.GetText(md.optionName);
                var option = ModOptionUI.CreateSliderUI(canvas, name == "UNKNOWN" ? md.optionName : name , curinfo.modid, md.optionName);
                //option.gameObject.AddComponent<FrameDummy>();
                option.transform.localPosition = new Vector3(-240, 420 - options.Count * OptionSize);
                options.Add(option);
            }
            public void AddOptionTMP_Toggle(ModOptionData md)
            {
                string name = LocalizeTextDataModel.instance.GetText(md.optionName);
                var option = ModOptionUI.CreateToggleUI(canvas, name == "UNKNOWN" ? md.optionName : name, curinfo.modid, md.optionName);
                option.transform.localPosition = new Vector3(-550, 385 - options.Count * OptionSize);
                options.Add(option);
            }
            public void Clear()
            {
                foreach (var opt in options)
                {
                    UnityEngine.Object.Destroy(opt.gameObject);
                }
                options = new List<ModOptionUI>();
            }
            public ModInfo_patch curinfo;
            public List<ModOptionUI> options;
            public RectTransform canvas;
            public static int OptionSize = 150;
        }
        [NewType]
        public class ModOptionUI : MonoBehaviour
        {
            public static ModOptionUI CreateSliderUI(Transform parent, string tmptext, string modid, string id)
            {
                var obj = UnityEngine.Object.Instantiate(OptionUI.Instance.Opt_MasterVolume.transform.parent.gameObject);
                var slider = obj.transform.GetChild(2).gameObject.GetComponent<Slider>();
              

                var text = obj.transform.GetChild(0).gameObject.GetComponent<Text>();
                text.text = tmptext;
                UnityEngine.Object.Destroy(obj.transform.GetChild(0).gameObject.GetComponent<LocalizeTextLoadScript>());

                var icon = obj.transform.GetChild(4).gameObject.GetComponent<Image>();
                icon.enabled = false;

                obj.transform.SetParent(parent);
                var result = obj.AddComponent<ModOptionUI>();
                result.type = OptionType.Slider;
                result.sliderFill = obj.transform.GetChild(3).gameObject.GetComponent<Image>();
                result.modid = modid;
                result.optid = id;


                slider.onValueChanged = new Slider.SliderEvent();

                slider.value = ModOptionManager.Instance.GetSliderValue(modid, id);
                result.sliderFill.fillAmount = slider.value;


                slider.onValueChanged.AddListener(result.SliderTest);
                slider.gameObject.SetActive(true);
                return result;
            }
            public static ModOptionUI CreateToggleUI(Transform parent,string tmptext, string modid, string id)
            {
                var obj = UnityEngine.Object.Instantiate(OptionUI.Instance.Opt_Dlc.transform.parent.gameObject);
                var tog = obj.transform.GetChild(1).gameObject.GetComponent<Toggle>();
               

                var txt = obj.transform.GetChild(0).gameObject.GetComponent<Text>();
                txt.text = tmptext;
                UnityEngine.Object.Destroy(obj.transform.GetChild(0).gameObject.GetComponent<LocalizeTextLoadScript>());
               
                obj.transform.SetParent(parent);
                var result = obj.AddComponent<ModOptionUI>();
                result.type = OptionType.Toggle;
                result.modid = modid;
                result.optid = id;

                tog.onValueChanged = new Toggle.ToggleEvent();
               

                tog.isOn = ModOptionManager.Instance.GetToggleValue(modid, id);

                tog.onValueChanged.AddListener(result.ToggleTest);
                tog.gameObject.SetActive(true);
                return result;
            }

            public  void SliderTest(float value)
            {
                //ModDebug.Log($"Slider {value}");
                sliderFill.fillAmount = value;

                ModOptionManager.Instance.SetSliderValue(modid, optid, value);
            }
            public void ToggleTest(bool value)
            {
                //ModDebug.Log($"Toggle {value}");
                ModOptionManager.Instance.SetToggleValue(modid, optid, value);
            }

            public string modid;

            public string optid;

            public Image sliderFill;

            public OptionType type;
            [NewType]
            public enum OptionType
            {
                None,
                Toggle,
                Slider
            }
        }
    }
}
