using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Diagnostics;
using Htc.Util;

namespace GameData
{
	public abstract class GameData
	{
		public int id { get; protected set; }

		protected static Dictionary<int, T> GetDataMap<T>()
		{
			Dictionary<int, T> dataMap;
			Stopwatch sw = new Stopwatch();
			sw.Start();
			var type = typeof(T);
			var fileNameField = type.GetField("fileName");
			if (fileNameField != null)
			{
				var fileName = fileNameField.GetValue(null) as String;
				var result = GameDataController.Instance.FormatData(fileName, typeof(Dictionary<int, T>), type);
				dataMap = result as Dictionary<int, T>;
			}
			else
			{
				dataMap = new Dictionary<int, T>();
			}
			sw.Stop();
			return dataMap;
		}
	}

	public abstract class GameData<T> : GameData where T : GameData<T>
	{
		private static Dictionary<int, T> m_dataMap;

		public static Dictionary<int, T> dataMap
		{
			get
			{
				if (m_dataMap == null)
					m_dataMap = GetDataMap<T>();
				return m_dataMap;
			}
			set { m_dataMap = value; }
		}
	}

	public class GameDataController : DataLoader
	{
		private List<Type> m_defaultData = new List<Type>() { typeof(PlayerData), typeof(ObstacleData)};

		private static GameDataController m_instance;

		public static GameDataController Instance
		{
			get { return m_instance; }
		}

		static GameDataController()
		{
			m_instance = new GameDataController();
		}

		public static void Init(Action<int, int> progress = null, Action finished = null)
		{
			m_instance.LoadData(m_instance.m_defaultData, m_instance.FormatXMLData, null);
			Action action = () => { m_instance.InitAsynData(m_instance.FormatXMLData, progress, finished); };
			action.BeginInvoke(null, null);
		}

		private void InitAsynData(Func<string, Type, Type, object> formatData, Action<int, int> progress, Action finished)
		{
			try
			{
				Stopwatch sw = new Stopwatch();
				sw.Start();
				List<Type> gameDataType = new List<Type>();
				Assembly ass = typeof(GameDataController).Assembly;
				var types = ass.GetTypes();
				foreach (var item in types)
				{
					if (item.Namespace == "Htc.GameData")
					{
						var type = item.BaseType;
						while (type != null)
						{
							if (type == typeof(GameData) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(GameData<>)))//type == typeof(GameData) || 
								//#else
								//if ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof(GameData<>)))
								//#endif
							{
								if (!m_defaultData.Contains(item))
									gameDataType.Add(item);
								break;
							}
							else
							{
								type = type.BaseType;
							}
						}
					}
				}
				LoadData(gameDataType, formatData, progress);
				sw.Stop();
				GC.Collect();
				if (finished != null)
					finished();
			}
			catch (Exception ex)
			{
			}
		}

		private void LoadData(List<Type> gameDataType, Func<string, Type, Type, object> formatData, Action<int, int> progress)
		{
			var count = gameDataType.Count;
			var i = 1;
			foreach (var item in gameDataType)
			{
				var p = item.GetProperty("dataMap", ~BindingFlags.DeclaredOnly);
				var fileNameField = item.GetField("fileName");
				if (p != null && fileNameField != null)
				{
					var fileName = fileNameField.GetValue(null) as String;
					var result = formatData(String.Concat(m_resourcePath, fileName, m_fileExtention), p.PropertyType, item);
					p.GetSetMethod().Invoke(null, new object[] { result });
				}
				if (progress != null)
					progress(i, count);
				i++;
			}
		}

		public object FormatData(string fileName, Type dicType, Type type)
		{
			return FormatXMLData(String.Concat(m_resourcePath, fileName, m_fileExtention), dicType, type);
		}

		#region xml

		private object FormatXMLData(string fileName, Type dicType, Type type)
		{
			object result = null;
			try
			{
				result = dicType.GetConstructor(Type.EmptyTypes).Invoke(null);
				Dictionary<Int32, Dictionary<String, String>> map;
				if (XMLParser.LoadIntMap(fileName, m_isUseOutterConfig, out map))
				{
					var props = type.GetProperties();
					foreach (var item in map)
					{
						var t = type.GetConstructor(Type.EmptyTypes).Invoke(null);
						foreach (var prop in props)
						{
							if (prop.Name == "id")
							{
								prop.SetValue(t, item.Key, null);
							}
							else
							{
								if (item.Value.ContainsKey(prop.Name))
								{
									var value = Utils.GetValue(item.Value[prop.Name], prop.PropertyType);
									prop.SetValue(t, value, null);
								}
							}
						}
						dicType.GetMethod("Add").Invoke(result, new object[] { item.Key, t });
					}
				}
			}
			catch (Exception ex)
			{
				
			}

			return result;
		}

		#endregion
	}
}

public abstract class DataLoader
{
	protected readonly String m_resourcePath;
	protected readonly String m_fileExtention;
	protected readonly bool m_isUseOutterConfig;
	protected Action<int, int> m_progress;
	protected Action m_finished;

	protected DataLoader()
	{
		m_isUseOutterConfig = GameConfig.IsUseOutterConfig;
		if (m_isUseOutterConfig) {
			m_resourcePath = String.Concat (GameConfig.OutterPath, GameConfig.CONFIG_SUB_FOLDER);
			m_fileExtention = GameConfig.XML;
		} else {
			m_resourcePath = GameConfig.CONFIG_SUB_FOLDER;
			m_fileExtention = GameConfig.CONFIG_FILE_EXTENSION;
		}
	}
}