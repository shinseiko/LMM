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
using System.Security.Policy;
using System.Xml.Linq;
using LobotomyBaseModLib;
using NAudio.Wave;
using System.Xml.Serialization;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;

[assembly: PatchAssembly]

namespace LobotomyBaseMod
{
	[NewType]
	public class GlobalNoticeBox : MonoBehaviour
	{
		public static GlobalNoticeBox Instance
		{
			get
			{
				if (instance != null)
				{
					return instance;
				}
				instance = GetBox();
				return instance;
			}
		}
		private static GlobalNoticeBox instance;
		private static GlobalNoticeBox GetBox()
		{
			Image back = ExtenionUtil.CreateImage($"{Application.dataPath}//Managed/BaseMod/Image/Desc.png", GlobalGameManager.instance.gameObject.transform.GetChild(0));
			back.rectTransform.sizeDelta = new Vector2(2000, 2000);
			back.sprite = null;
			back.color = new Color(0, 0, 0, 0.5f);

			var result = back.gameObject.AddComponent<GlobalNoticeBox>();

			Image msgbox = ExtenionUtil.CreateImage($"{Application.dataPath}//Managed/BaseMod/Image/Desc.png", back.transform);
			msgbox.rectTransform.sizeDelta = new Vector2(800, 600);
			GameObject obj = new GameObject();
			obj.transform.SetParent(msgbox.transform);
			obj.transform.localPosition = new Vector3(0, 0);
			Text txt = obj.AddComponent<Text>();
			txt.fontSize = 25;
			txt.font = GlobalGameManager.instance.GetLanguageFont("kr").GetFont(FontType.CONTEXT).font;
			txt.color = new Color(0.2509804f, 1f, 0.654902f);
			txt.alignment = TextAnchor.MiddleCenter;
			txt.rectTransform.sizeDelta = new Vector2(760, 400);
			result.noticeText = txt;

			Image del = ExtenionUtil.CreateImage($"{Application.dataPath}//Managed/BaseMod/Image/DelIcon.png", msgbox.transform);
			del.rectTransform.sizeDelta = new Vector2(100, 100);
			del.transform.localPosition = new Vector3(350, 250);
			Button btn = del.gameObject.AddComponent<Button>();
			btn.targetGraphic = del;
			btn.onClick = new Button.ButtonClickedEvent();
			btn.onClick.AddListener(result.Close);
			//del.gameObject.AddComponent<FrameDummy>();

			instance = result;

			return result;
		}

		public void OpenBox(string text)
		{
			this.gameObject.SetActive(true);
			noticeText.text = text;
		}
		public void Close()
		{
			GlobalNoticeBox.Instance.gameObject.SetActive(false);
		}
		public Text noticeText;
	}
	[NewType]
	public class ModSaveUtil
	{
		public static Dictionary<string, T1> ConvertDic<T1,T2>(Dictionary<string,T2> dic)
		{
			Dictionary<string, T1> result = new Dictionary<string, T1>();
			foreach (var pair in dic)
			{
				result[pair.Key] = (T1)(object)pair.Value;
			}
			return result;
		}
		public static Dictionary<string, object> ReadSerializableFile(string fileName)
		{
			Dictionary<string, object> dictionary;

			BinaryFormatter binaryFormatter;
			binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(fileName, FileMode.Open);
			try
			{
				dictionary = (Dictionary<string, object>)binaryFormatter.Deserialize(fileStream);
				fileStream.Close();
			}
			catch (Exception ex2)
			{
				ModDebug.Log("save load error - " + ex2.Message + Environment.NewLine + ex2.StackTrace);
				return null;
			}

			return dictionary;
		}

		// Token: 0x06005B33 RID: 23347 RVA: 0x00212888 File Offset: 0x00210A88
		public static void WriteSerializableFile(string fileName, Dictionary<string, object> dic)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(fileName,FileMode.Create);
			binaryFormatter.Serialize(fileStream, dic);
			fileStream.Close();
		}
	}

	[NewType]
	public class ModOptionManager : Singleton<ModOptionManager>
	{
		public void Init()
		{
			dic = new Dictionary<string, List<ModOptionData>>();
			foreach (ModInfo_patch info in ((Add_On_patch)(object)Add_On.instance).ModList)
			{
				if (info.modid == string.Empty) continue;
				if (info.options == null || info.options.Count == 0) continue;
				dic[info.modid] = new List<ModOptionData>(info.options);
			}
			LoadData();
			SaveData();
		}
		public void LoadData()
		{
			if (!File.Exists(path)) return;

			var sdic = ModSaveUtil.ReadSerializableFile(path);

			// var ldic = JsonUtility.FromJson<Dictionary<string, List<ModOptionJson>>>(File.ReadAllText(path));
			Dictionary<string, List<ModOptionData>> ldic = ModSaveUtil.ConvertDic<List<ModOptionData>,object>(sdic);

			foreach (var pair in ldic)
			{
				if (!dic.ContainsKey(pair.Key)) continue;
				foreach (var lmd in pair.Value)
				{
					var md = dic[pair.Key].Find(x => x.optionName == lmd.optionName);
					if (md == null) continue;
					md.CopyValue(lmd);
				}
			}

		}
		public void SaveData()
		{
			ModSaveUtil.WriteSerializableFile(path, ModSaveUtil.ConvertDic<object, List<ModOptionData>>(dic));
			//var json = dic.JsonSerialize();
			//File.WriteAllText(path, json);

		}
		public float GetSliderValue(string modid, string name)
		{
			if (!dic.ContainsKey(modid)) return -1;
			var md = dic[modid].Find(x => x.optionName == name);
			if (md == null || md.type != ModOptionJsonType.Slider) return -1;
			return md.fvalue;
		}
		public void SetSliderValue(string modid, string name, float value)
		{
			if (!dic.ContainsKey(modid)) return;
			var md = dic[modid].Find(x => x.optionName == name);
			if (md == null || md.type != ModOptionJsonType.Slider) return;
			md.fvalue = value;
			SaveData();
		}
		public bool GetToggleValue(string modid, string name)
		{
			if (!dic.ContainsKey(modid)) return false;
			var md = dic[modid].Find(x => x.optionName == name);
			if (md == null || md.type != ModOptionJsonType.Toggle) return false;
			return md.bvalue;
		}
		public void SetToggleValue(string modid, string name, bool value)
		{
			if (!dic.ContainsKey(modid)) return;
			var md = dic[modid].Find(x => x.optionName == name);
			if (md == null || md.type != ModOptionJsonType.Toggle) return;
			md.bvalue = value;
			SaveData();
		}
		public Dictionary<string, List<ModOptionData>> dic;

		public string path = $"{Application.persistentDataPath}/LobotomyBaseMod/ModOptionData.dat";
	}

	[NewType]
	[Serializable]
	public class ModOptionData
	{
		public virtual void CopyValue(ModOptionData md)
		{
			if (type != md.type || md.optionName != optionName) return;
			fvalue = md.fvalue;
			bvalue = md.bvalue;
		}
		public ModOptionJsonType type;
		public float fvalue;
		public bool bvalue;
		public string optionName;
	}
	[NewType]
	public enum ModOptionJsonType
	{
		Toggle,
		Slider
	}
	[NewType]
	public class FrameDummy : MonoBehaviour
	{
		public void Update()
		{
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				base.gameObject.transform.localPosition += new Vector3(-1f, 0f);
				ModDebug.Log($"cur pos : {gameObject.transform.localPosition}");
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				base.gameObject.transform.localPosition += new Vector3(1f, 0f);
				ModDebug.Log($"cur pos : {gameObject.transform.localPosition}");
			}
			if (Input.GetKey(KeyCode.UpArrow))
			{
				base.gameObject.transform.localPosition += new Vector3(0f, 1f);
				ModDebug.Log($"cur pos : {gameObject.transform.localPosition}");
			}
			if (Input.GetKey(KeyCode.DownArrow))
			{
				base.gameObject.transform.localPosition += new Vector3(0f, -1f);
				ModDebug.Log($"cur pos : {gameObject.transform.localPosition}");
			}
		}
	}
	[NewType]
	public class ModInitializer
	{
		public virtual void OnInitialize()
		{

		}
		public string MODID;
	}
	[NewType]
	public class ModListXml
	{
		public static void SerializeData(ModListXml data, string path)
		{
			using (StreamWriter streamWriter = new StreamWriter(path))
			{
				new XmlSerializer(typeof(ModListXml)).Serialize(streamWriter, data);
			}
		}
		public static ModListXml LoadData(string path)
		{
			ModListXml result;
			using (StringReader stringReader = new StringReader(File.ReadAllText(path)))
			{
				result = (ModListXml)new XmlSerializer(typeof(ModListXml)).Deserialize(stringReader);
			}
			return result;
		}
		public List<ModInfoXml> list = new List<ModInfoXml>();
	}
	[NewType]
	public class ModInfoXml
	{
		public string modfoldername = string.Empty;
		public bool Useit = true;
		public bool IsWorkShop = false;
	}
	[NewType]
	public class ModAssetBundleManager : Singleton<ModAssetBundleManager>
	{
		public void Init()
		{
			bundles = new Dictionary<string, List<AssetBundle>>();
			GObjList = new Dictionary<string, List<GameObjectBundleCache>>();
			GetBundles();
		}
		public void GetBundles()
		{
			bundles = new Dictionary<string, List<AssetBundle>>();
			foreach (ModInfo info in ((Add_On_patch)(object)Add_On.instance).ModList)
			{
				ModInfo_patch pinfo = (ModInfo_patch)info;
				DirectoryInfo directoryInfo = pinfo.modpath.CheckNamedDir(PathDir);// EquipmentDataLoader.CheckNamedDir(pinfo.modpath, "BaseModArtWork");
				if (directoryInfo != null)
				{
					if (!bundles.ContainsKey(pinfo.modid))
					{
						bundles[pinfo.modid] = new List<AssetBundle>();
						GObjList[pinfo.modid] = new List<GameObjectBundleCache>();
					}
					GetBundles(pinfo.modid, directoryInfo);
				}
			}
		}
		public void GetBundles(string modid, DirectoryInfo curdic)
		{
			foreach (DirectoryInfo dir in curdic.GetDirectories())
			{
				GetBundles(modid, dir);
			}
			foreach (FileInfo file in curdic.GetFiles())
			{
				AssetBundle bundle = AssetBundle.LoadFromFile(file.FullName);
				if (bundle != null)
				{
					bundle.name = Path.GetFileNameWithoutExtension(file.FullName);
					bundles[modid].Add(bundle);
				}
				else
				{
					ModDebug.Log("Can't load asset - " + Path.GetFileNameWithoutExtension(file.FullName));
				}

			}
		}
		public GameObject LoadAssetEachScale(Transform parent, Vector3 scale, Vector3 position, KeyValuePairSS name, string bundlename = "")
		{
			GameObject obj = ModAssetBundleManager.Instance.GetAsset(name, bundlename);
			//obj.LocalScalingAll(scale.x, scale.y);
			obj.LocalEachScalingAll(scale.x, scale.y, scale.z);
			obj.transform.parent = parent;
			obj.transform.localPosition = position;
			obj.SetActive(true);
			return obj;
		}
		public GameObject LoadAssetEachScale(Transform parent, Vector3 scale, Vector3 position, string name, string bundlename = "")
		{
			return LoadAssetEachScale(parent, scale, position, new KeyValuePairSS(string.Empty, name), bundlename);
		}
		public GameObject GetAsset(string name, string bundlename = "")
		{
			return GetAsset(new KeyValuePairSS(string.Empty, name), bundlename);
		}
		public GameObject GetAsset(KeyValuePairSS name, Vector3 pos, string bundlename = "")
		{
			return GetAsset(name, pos, new Quaternion(0, 0, 0, 0), bundlename);
		}
		public GameObject GetAsset(KeyValuePairSS name, string bundlename = "")
		{
			return GetAsset(name, new Vector3(0, 0), new Quaternion(0, 0, 0, 0), bundlename);
		}
		public GameObject GetAsset(KeyValuePairSS name, Vector3 pos, Quaternion rot, string bundlename = "")
		{
			if (GObjList.ContainsKey(name.key))
			{
				GameObjectBundleCache cache = GObjList[name.key].Find((GameObjectBundleCache x) => x.objname == name.value && (bundlename == "" || x.BundleName == bundlename));
				if (cache != null)
				{
					return UnityEngine.Object.Instantiate(cache.obj, pos, rot);
				}
				foreach (AssetBundle bundle in bundles[name.key])
				{
					GameObject Gobj = bundle.LoadAsset<GameObject>(name.value);
					if (Gobj != null)
					{
						GameObjectBundleCache cache2 = new GameObjectBundleCache()
						{
							BundleName = bundle.name,
							objname = name.value,
							obj = Gobj
						};
						GObjList[name.key].Add(cache2);
						return UnityEngine.Object.Instantiate(Gobj, pos, rot);
					}
				}
				return null;
			}
			else
			{
				ModDebug.Log("AssetBundleLoader - Wrong modid : " + name.key);
				return null;
			}
		}
		public Dictionary<string, List<AssetBundle>> bundles;
		public Dictionary<string, List<GameObjectBundleCache>> GObjList;
		[NewType]
		public class GameObjectBundleCache
		{
			public string objname;
			public string BundleName;
			public GameObject obj;
		}
		public static string PathDir = "BaseModAssetBundle";
	}
	[NewType]
	public class ModAudioClipManager : Singleton<ModAudioClipManager>
	{
		public void Init()
		{
			AudioClipPathCaching();
			AudioClipDic = new CacheDic<KeyValuePairSS, AudioClip>(AudioClipFinding);
		}
		public AudioClip GetAudioClip(string modid, string spritename)
		{
			return GetAudioClip(new KeyValuePairSS(modid, spritename));
		}
		public AudioClip GetAudioClip(KeyValuePairSS SS)
		{
			return AudioClipDic[SS];
		}

		private AudioClip AudioClipFinding(KeyValuePairSS id)
		{
			if (AudioClipPathCache.ContainsKey(id))
			{
				try
				{
					string path = AudioClipPathCache[id];
					string extension = Path.GetExtension(path);
					AudioClip result = null;
					if (extension == ".wav")
					{
						result = WavtoAudioClip(path);
					}
					else if (extension == ".mp3")
					{
						result = mp3toAudioClip(path);
					}
					else
					{
						ModDebug.Log("GetAudioClip error path : " + AudioClipPathCache[id]);
						ModDebug.Log("None Support Extension");
						AudioClipPathCache.Remove(id);
						return null;
					}
					AudioClipPathCache.Remove(id);
					return result;
				}
				catch (Exception e)
				{
					ModDebug.Log("GetAudioClip error path : " + AudioClipPathCache[id]);
					ModDebug.Log("GetAudioClip error - " + e.Message + Environment.NewLine + e.StackTrace);
					AudioClipPathCache.Remove(id);
					return null;

				}
			}
			return null;
		}
		private void AudioClipPathCaching()
		{
			AudioClipPathCache = new Dictionary<KeyValuePairSS, string>();
			foreach (ModInfo info in ((Add_On_patch)(object)Add_On.instance).ModList)
			{
				ModInfo_patch pinfo = (ModInfo_patch)info;
				DirectoryInfo directoryInfo = pinfo.modpath.CheckNamedDir(PathDir);// EquipmentDataLoader.CheckNamedDir(pinfo.modpath, "BaseModArtWork");
				if (directoryInfo != null)
				{
					AudioClipPathCaching(pinfo.modid, directoryInfo);
				}
			}
		}
		private void AudioClipPathCaching(string modid, DirectoryInfo curdic)
		{
			foreach (DirectoryInfo dir in curdic.GetDirectories())
			{
				AudioClipPathCaching(modid, dir);
			}
			foreach (FileInfo file in curdic.GetFiles())
			{
				string filename = Path.GetFileNameWithoutExtension(file.FullName);
				AudioClipPathCache[new KeyValuePairSS(modid, filename)] = file.FullName;
			}
		}

		public static AudioClip mp3toAudioClip(string path)
		{
			Mp3FileReader sourceProvider = new Mp3FileReader(path);
			WaveFileWriter.CreateWaveFile(path + ".wav", sourceProvider);
			AudioClip audioClip = WavtoAudioClip(path + ".wav");
			File.Delete(path + ".wav");
			return audioClip;
		}
		public static AudioClip WavtoAudioClip(string path)
		{
			byte[] array = File.ReadAllBytes(path);
			WAV wav = new WAV(array);
			AudioClip audioClip = AudioClip.Create("Default", wav.SampleCount, 1, wav.Frequency, false);
			audioClip.SetData(wav.LeftChannel, 0);
			return audioClip;
		}
		public Dictionary<KeyValuePairSS, string> AudioClipPathCache;
		public CacheDic<KeyValuePairSS, AudioClip> AudioClipDic;

		public static string PathDir = "BaseModAudioClip";
	}
	[NewType]
	public class ModArtWorkManager : Singleton<ModArtWorkManager>
	{
		public void Init()
		{
			ArtWorkPathCaching();
			ArtWorkDic = new CacheDic<KeyValuePairSS, Sprite>(ArtWorkFinding);
		}
		public Sprite GetArtWork(string modid, string spritename)
		{
			return GetArtWork(new KeyValuePairSS(modid, spritename));
		}
		public Sprite GetArtWork(KeyValuePairSS SS)
		{
			return ArtWorkDic[SS];
		}
		private Sprite ArtWorkFinding(KeyValuePairSS id)
		{
			if (ArtWorkPathCache.ContainsKey(id))
			{
				try
				{
					Sprite result = ExtenionUtil.CreateSpriteByPng(ArtWorkPathCache[id]);
					ArtWorkPathCache.Remove(id);
					return result;
				}
				catch (Exception e)
				{
					ModDebug.Log("GetArtWork error path : " + ArtWorkPathCache[id]);
					ModDebug.Log("GetArtWork error - " + e.Message + Environment.NewLine + e.StackTrace);
					ArtWorkPathCache.Remove(id);
					return null;

				}
			}
			return null;
		}
		private void ArtWorkPathCaching()
		{
			ArtWorkPathCache = new Dictionary<KeyValuePairSS, string>();
			foreach (ModInfo info in ((Add_On_patch)(object)Add_On.instance).ModList)
			{
				ModInfo_patch pinfo = (ModInfo_patch)info;
				DirectoryInfo directoryInfo = pinfo.modpath.CheckNamedDir(PathDir);// EquipmentDataLoader.CheckNamedDir(pinfo.modpath, "BaseModArtWork");
				if (directoryInfo != null)
				{
					ArtWorkPathCaching(pinfo.modid, directoryInfo);
				}
			}
		}
		private void ArtWorkPathCaching(string modid, DirectoryInfo curdic)
		{
			foreach (DirectoryInfo dir in curdic.GetDirectories())
			{
				ArtWorkPathCaching(modid, dir);
			}
			foreach (FileInfo file in curdic.GetFiles())
			{
				string filename = Path.GetFileNameWithoutExtension(file.FullName);
				ArtWorkPathCache[new KeyValuePairSS(modid, filename)] = file.FullName;
			}
		}

		public Dictionary<KeyValuePairSS, string> ArtWorkPathCache;

		public CacheDic<KeyValuePairSS, Sprite> ArtWorkDic;

		public static string PathDir = "BaseModArtWork";
	}

	[NewType]
	public class WAV
	{
		private static float bytesToFloat(byte firstByte, byte secondByte)
		{
			short num = (short)((int)secondByte << 8 | (int)firstByte);
			return (float)num / 32768f;
		}

		private static int bytesToInt(byte[] bytes, int offset = 0)
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				num |= (int)bytes[offset + i] << i * 8;
			}
			return num;
		}

		private static byte[] GetBytes(string filename)
		{
			return File.ReadAllBytes(filename);
		}

		public float[] LeftChannel { get; internal set; }

		public float[] RightChannel { get; internal set; }

		public int ChannelCount { get; internal set; }

		public int SampleCount { get; internal set; }

		public int Frequency { get; internal set; }

		public WAV(string filename) : this(WAV.GetBytes(filename))
		{
		}

		public WAV(byte[] wav)
		{
			this.ChannelCount = (int)wav[22];
			this.Frequency = WAV.bytesToInt(wav, 24);
			int i = 12;
			while (wav[i] != 100 || wav[i + 1] != 97 || wav[i + 2] != 116 || wav[i + 3] != 97)
			{
				i += 4;
				int num = (int)wav[i] + (int)wav[i + 1] * 256 + (int)wav[i + 2] * 65536 + (int)wav[i + 3] * 16777216;
				i += 4 + num;
			}
			i += 8;
			this.SampleCount = (wav.Length - i) / 2;
			bool flag = this.ChannelCount == 2;
			if (flag)
			{
				this.SampleCount /= 2;
			}
			this.LeftChannel = new float[this.SampleCount];
			bool flag2 = this.ChannelCount == 2;
			if (flag2)
			{
				this.RightChannel = new float[this.SampleCount];
			}
			else
			{
				this.RightChannel = null;
			}
			int num2 = 0;
			while (i < wav.Length)
			{
				this.LeftChannel[num2] = WAV.bytesToFloat(wav[i], wav[i + 1]);
				i += 2;
				bool flag3 = this.ChannelCount == 2;
				if (flag3)
				{
					this.RightChannel[num2] = WAV.bytesToFloat(wav[i], wav[i + 1]);
					i += 2;
				}
				num2++;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002288 File Offset: 0x00000488
		public override string ToString()
		{
			return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", new object[]
			{
				this.LeftChannel,
				this.RightChannel,
				this.ChannelCount,
				this.SampleCount,
				this.Frequency
			});
		}
	}
	[NewType]
	public class CustomBuf : UnitBuf
	{
		public CustomBuf()
		{
			type = UnitBufType.ADD_SUPERARMOR;
		}
		public CustomBuf(float buftime)
		{
			type = UnitBufType.ADD_SUPERARMOR;
			remainTime = buftime;
		}
	}
	[NewType]
	public class KeyValuePairSS
	{
		public KeyValuePairSS(string k, string v)
		{
			key = k;
			value = v;
		}
		public string key;

		public string value;

		public override bool Equals(object obj)
		{
			if (obj is KeyValuePairSS)
			{
				KeyValuePairSS o = (KeyValuePairSS)obj;
				return o.key.Equals(this.key) && o.value.Equals(this.value);
			}
			return base.Equals(obj);
		}
		public override int GetHashCode()
		{
			return (key + value).GetHashCode();
		}
	}
	[NewType]
	public static class ExtenionUtil
	{
		public static Image CreateImage(string imagepath, Transform parent)
		{
			if (!File.Exists(imagepath))
			{
				ModDebug.Log("CreateImage error - No File in Path");
				return null;
			}
			return CreateImage(CreateSpriteByPng(imagepath), parent);
		}
		public static Image CreateImage(Sprite sprite, Transform parent)
		{
			GameObject gameObject = new GameObject("BackGround");
			Image image = gameObject.AddComponent<Image>();
			image.transform.SetParent(parent);
			image.sprite = sprite;
			image.rectTransform.sizeDelta = new Vector2(sprite.texture.width, (float)sprite.texture.height);
			gameObject.transform.localScale = new Vector3(1f, 1f);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			gameObject.SetActive(true);
			return image;
		}
		public static Sprite CreateSpriteByPng(string filepath)
		{
			if (File.Exists(filepath))
			{
				Texture2D texture2D = new Texture2D(2, 2);
				texture2D.LoadImage(File.ReadAllBytes(filepath));
				Sprite value = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f), 100, 0, SpriteMeshType.FullRect);
				return value;
			}
			return null;
		}
		public static T GetTypeInstance<T>(string typename)
		{
			object obj = null;
			foreach (Assembly assembly in Add_On.instance.AssemList)
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.Name == typename)
					{
						obj = Activator.CreateInstance(type);
						return (T)obj;
					}
				}
			}
			if (obj == null)
			{
				obj = Activator.CreateInstance(Type.GetType(typename));
			}
			return (T)obj;
		}
		public static Type GetType(string typename)
		{
			foreach (Assembly assembly in Add_On.instance.AssemList)
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.Name == typename)
					{
						return type;
					}
				}
			}
			return Type.GetType(typename);
		}
		public static bool TryGetValue<T>(Dictionary<string, object> dic, string name, ref T field)
		{
			object obj;
			if (dic.TryGetValue(name, out obj) && obj is T)
			{
				field = (T)((object)obj);
				return true;
			}
			return false;
		}

	}
	[NewType]
	public class CreatureInfoCodex_SortData_Mod
	{
		public int index = -1;

		public LcIdLong id;

		public static int Compare(CreatureInfoCodex_SortData_Mod a, CreatureInfoCodex_SortData_Mod b)
		{
			if (a.index == b.index)
			{
				return a.id.CompareTo(b.id);
			}

			return a.index.CompareTo(b.index);
		}
	}
	[NewType]
	public class ConsoleCommand_Mod
	{
		public static void RemoveGift_Mod(long id, LcId equipid)
		{
			global::AgentModel agent = global::AgentManager.instance.GetAgent(id);
			global::EGOgiftModel egogiftModel = null;
			foreach (global::EGOgiftModel egogiftModel2 in agent.GetAllGifts())
			{
				bool flag = EquipmentTypeInfo_patch.GetLcId(egogiftModel2.metaInfo) == equipid;
				if (flag)
				{
					egogiftModel = egogiftModel2;
					break;
				}
			}
			bool flag2 = egogiftModel != null;
			if (flag2)
			{
				agent.ReleaseEGOgift(egogiftModel);
			}
		}
		public static void AddGift_Mod(long id, LcId equipid)
		{
			EquipmentModel equipmentModel = InventoryModel.Instance.ForceTypeChange<InventoryModel_patch>().CreateEquipmentForcely_Mod(equipid);
			AgentModel agent = AgentManager.instance.GetAgent(id);
			agent.AttachEGOgift(equipmentModel as EGOgiftModel);
		}
		public static void AddGift(long id, int equipid)
		{
			AddGift_Mod(id, new LcId(equipid));
		}
		public static void GenerateEquipment(string modid, int id)
		{
			try
			{
				((InventoryModel_patch)(object)InventoryModel.Instance).CreateEquipment_Mod(new LcId(modid, id));
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}
		public static void AddWaitingGenCreature(string modid, long id)
		{
			try
			{

				((PlayerModel_patch)(object)PlayerModel.instance).AddWaitingCreature_Mod(new LcIdLong(modid, id));
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}
	}
	[NewType]
	public class LcIdLong : IEquatable<LcIdLong>, IEquatable<long>, IComparable<LcIdLong>
	{
		public readonly long id;

		public readonly string packageId;

		public static readonly LcIdLong None = new LcIdLong(-1);

		public LcIdLong(long id)
		{
			this.id = id;
			packageId = "";
		}

		public LcIdLong(string packageId, long id)
		{
			this.packageId = packageId;
			this.id = id;
			if (packageId == null)
			{
				Debug.LogError("error");
			}
		}

		public bool IsBasic()
		{
			return !IsWorkshop();
		}

		public bool IsWorkshop()
		{
			return !IsBasicId(packageId);
		}

		public bool IsNone()
		{
			return id < 0;
		}

		public override bool Equals(object obj)
		{
			LcIdLong other;
			if ((object)(other = obj as LcIdLong) != null)
			{
				return Equals(other);
			}

			return false;
		}

		public bool Equals(LcIdLong other)
		{
			if (id == other.id)
			{
				return packageId == other.packageId;
			}

			return false;
		}

		public bool Equals(long other)
		{
			if (id == other)
			{
				return IsBasic();
			}

			return false;
		}

		public override int GetHashCode()
		{
			if (packageId == null)
			{
				Debug.LogError("error");
			}

			return id.GetHashCode() + packageId.GetHashCode();
		}

		public static bool operator ==(LcIdLong lhs, long rhs)
		{
			if ((object)lhs == null)
			{
				lhs = None;
			}

			return lhs.Equals(rhs);
		}

		public static bool operator !=(LcIdLong lhs, long rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator ==(LcIdLong lhs, LcIdLong rhs)
		{
			if ((object)lhs == null)
			{
				lhs = None;
			}

			if ((object)rhs == null)
			{
				rhs = None;
			}

			return lhs.Equals(rhs);
		}

		public static bool operator !=(LcIdLong lhs, LcIdLong rhs)
		{
			return !(lhs == rhs);
		}

		public int CompareTo(LcIdLong other)
		{
			int num = id.CompareTo(other.id);
			if (num == 0)
			{
				return packageId.CompareTo(other.packageId);
			}

			return num;
		}

		public static bool IsModId(string packageId)
		{
			return !IsBasicId(packageId);
		}

		public static bool IsBasicId(string packageId)
		{
			return string.IsNullOrEmpty(packageId);
		}

		public override string ToString()
		{
			return "LcIdLong(" + packageId + ":" + id + ")";
		}
	}
	[NewType]
	public class LcId : IEquatable<LcId>, IEquatable<int>, IComparable<LcId>
	{
		public readonly int id;

		public readonly string packageId;

		public static readonly LcId None = new LcId(-1);

		public LcId(int id)
		{
			this.id = id;
			packageId = "";
		}

		public LcId(string packageId, int id)
		{
			this.packageId = packageId;
			this.id = id;
			if (packageId == null)
			{
				Debug.LogError("error");
			}
		}

		public bool IsBasic()
		{
			return !IsWorkshop();
		}

		public bool IsWorkshop()
		{
			return !IsBasicId(packageId);
		}

		public bool IsNone()
		{
			return id < 0;
		}

		public override bool Equals(object obj)
		{
			LcId other;
			if ((object)(other = obj as LcId) != null)
			{
				return Equals(other);
			}

			return false;
		}

		public bool Equals(LcId other)
		{
			if (id == other.id)
			{
				return packageId == other.packageId;
			}

			return false;
		}

		public bool Equals(int other)
		{
			if (id == other)
			{
				return IsBasic();
			}

			return false;
		}

		public override int GetHashCode()
		{
			if (packageId == null)
			{
				Debug.LogError("error");
			}

			return id.GetHashCode() + packageId.GetHashCode();
		}

		public static bool operator ==(LcId lhs, int rhs)
		{
			if ((object)lhs == null)
			{
				lhs = None;
			}

			return lhs.Equals(rhs);
		}

		public static bool operator !=(LcId lhs, int rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator ==(LcId lhs, LcId rhs)
		{
			if ((object)lhs == null)
			{
				lhs = None;
			}

			if ((object)rhs == null)
			{
				rhs = None;
			}

			return lhs.Equals(rhs);
		}

		public static bool operator !=(LcId lhs, LcId rhs)
		{
			return !(lhs == rhs);
		}

		public int CompareTo(LcId other)
		{
			int num = id.CompareTo(other.id);
			if (num == 0)
			{
				return packageId.CompareTo(other.packageId);
			}

			return num;
		}

		public static bool IsModId(string packageId)
		{
			return !IsBasicId(packageId);
		}

		public static bool IsBasicId(string packageId)
		{
			return string.IsNullOrEmpty(packageId);
		}

		public override string ToString()
		{
			return "LorId(" + packageId.ToString() + ":" + id + ")";
		}
	}
	[NewType()]
	public class ModDebug
	{
		public static bool CheckLogFileExist()
		{
			return File.Exists(LogFilePath);
		}
		public static void FileInit()
		{
			if (!Inited)
			{
				HarmonyInstance harmonyInstance = HarmonyInstance.Create("Lobotomy.abcdcode.MidnightEGO");
				MethodInfo method = typeof(ModDebug).GetMethod("Debug_Log", AccessTools.all);
				harmonyInstance.Patch(typeof(UnityEngine.Debug).GetMethod("Log", AccessTools.all, null, new Type[] { typeof(object) }, null), null, new HarmonyMethod(method), null);
				method = typeof(ModDebug).GetMethod("Debug_LogError", AccessTools.all);
				harmonyInstance.Patch(typeof(UnityEngine.Debug).GetMethod("LogError", AccessTools.all, null, new Type[] { typeof(object) }, null), null, new HarmonyMethod(method), null);

				method = typeof(ModDebug).GetMethod("Debug_LogException", AccessTools.all);
				harmonyInstance.Patch(typeof(UnityEngine.Debug).GetMethod("LogException", AccessTools.all, null, new Type[] { typeof(Exception) }, null), null, new HarmonyMethod(method), null);

				Inited = true;
			}
			if (!Directory.Exists(BaseModFolderPath))
			{
				Directory.CreateDirectory(BaseModFolderPath);
			}
			File.WriteAllText(LogFilePath, "");
		}
		public static void Debug_LogException(Exception exception)
		{
			string msg = exception.Message + Environment.NewLine + exception.StackTrace;
			Log(msg);
		}
		public static void Debug_LogError(object message)
		{
			string str = message.ToString();
			Log(str + Environment.NewLine + Environment.StackTrace);
		}
		public static void Debug_Log(object message)
		{
			if (message == null)
			{
				Log("NULL OBJ" + Environment.NewLine + Environment.StackTrace);
				return;
			}
			string str = message.ToString();
			Log(str + Environment.NewLine + Environment.StackTrace);
		}
		public static void Log(string msg)
		{
			if (!CheckLogFileExist())
			{
				FileInit();
			}
			using (var writer = new StreamWriter(LogFilePath, append: true))
			{
				writer.WriteLine(msg);
			}
		}
		public static string LogFilePath
		{
			get
			{
				return BaseModFolderPath + "/Log.txt";
			}
		}
		public static string BaseModFolderPath
		{
			get
			{
				return Application.persistentDataPath + "/LobotomyBaseMod";
			}
		}
		public static bool Inited = false;
	}
}
