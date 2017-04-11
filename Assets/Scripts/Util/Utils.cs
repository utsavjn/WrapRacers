using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System.Net;
using System.Runtime.InteropServices;

namespace Htc.Util
{
	public static class Utils
	{
		private const Char LIST_SPRITER = ',';
		private const Char KEY_VALUE_SPRITER = ':';
		private const Char MAP_SPRITER = ',';

		public static object GetValue(String value, Type type)
		{
			if (type == null)
				return null;
			else if (type == typeof(string))
				return value;
			else if (type == typeof(Int32))
				return Convert.ToInt32(Convert.ToDouble(value));
			else if (type == typeof(float))
				return float.Parse(value);
			else if (type == typeof(byte))
				return Convert.ToByte(Convert.ToDouble(value));
			else if (type == typeof(sbyte))
				return Convert.ToSByte(Convert.ToDouble(value));
			else if (type == typeof(UInt32))
				return Convert.ToUInt32(Convert.ToDouble(value));
			else if (type == typeof(Int16))
				return Convert.ToInt16(Convert.ToDouble(value));
			else if (type == typeof(Int64))
				return Convert.ToInt64(Convert.ToDouble(value));
			else if (type == typeof(UInt16))
				return Convert.ToUInt16(Convert.ToDouble(value));
			else if (type == typeof(UInt64))
				return Convert.ToUInt64(Convert.ToDouble(value));
			else if (type == typeof(double))
				return double.Parse(value);
			else if (type == typeof(bool))
			{
				if (value == "0")
					return false;
				else if (value == "1")
					return true;
				else
					return bool.Parse(value);
			}
			else if (type.BaseType == typeof(Enum))
				return GetValue(value, Enum.GetUnderlyingType(type));
			else if (type == typeof(Vector3))
			{
				Vector3 result;
				ParseVector3(value, out result);
				return result;
			}
			else if (type == typeof(Quaternion))
			{
				Quaternion result;
				ParseQuaternion(value, out result);
				return result;
			}
			else if (type == typeof(Color))
			{
				Color result;
				ParseColor(value, out result);
				return result;
			}
			else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
			{
				Type[] types = type.GetGenericArguments();
				var map = ParseMap(value);
				var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
				foreach (var item in map)
				{
					var key = GetValue(item.Key, types[0]);
					var v = GetValue(item.Value, types[1]);
					type.GetMethod("Add").Invoke(result, new object[] { key, v });
				}
				return result;
			}
			else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
			{
				Type t = type.GetGenericArguments()[0];
				var list = ParseList(value);
				var result = type.GetConstructor(Type.EmptyTypes).Invoke(null);
				foreach (var item in list)
				{
					var v = GetValue(item, t);
					type.GetMethod("Add").Invoke(result, new object[] { v });
				}
				return result;
			}
			else
				return null;
		}

		public static bool ParseVector3(string _inputString, out Vector3 result)
		{
			string trimString = _inputString.Trim();
			result = new Vector3();
			if (trimString.Length < 7)
			{
				return false;
			}
			//if (trimString[0] != '(' || trimString[trimString.Length - 1] != ')')
			//{
			//    return false;
			//}
			try
			{
				string[] _detail = trimString.Split(LIST_SPRITER);//.Substring(1, trimString.Length - 2)
				if (_detail.Length != 3)
				{
					return false;
				}
				result.x = float.Parse(_detail[0]);
				result.y = float.Parse(_detail[1]);
				result.z = float.Parse(_detail[2]);
				return true;
			}
			catch (Exception e)
			{
				Debug.LogError("Parse Vector3 error: " + trimString + e.ToString());
				return false;
			}
		}

		public static bool ParseQuaternion(string _inputString, out Quaternion result)
		{
			string trimString = _inputString.Trim();
			result = new Quaternion();
			if (trimString.Length < 9)
			{
				return false;
			}
			//if (trimString[0] != '(' || trimString[trimString.Length - 1] != ')')
			//{
			//    return false;
			//}
			try
			{
				string[] _detail = trimString.Split(LIST_SPRITER);//.Substring(1, trimString.Length - 2)
				if (_detail.Length != 4)
				{
					return false;
				}
				result.x = float.Parse(_detail[0]);
				result.y = float.Parse(_detail[1]);
				result.z = float.Parse(_detail[2]);
				result.w = float.Parse(_detail[3]);
				return true;
			}
			catch (Exception e)
			{
				Debug.LogError("Parse Quaternion error: " + trimString + e.ToString());
				return false;
			}
		}

		public static bool ParseColor(string _inputString, out Color result)
		{
			string trimString = _inputString.Trim();
			result = Color.clear;
			if (trimString.Length < 9)
			{
				return false;
			}
			//if (trimString[0] != '(' || trimString[trimString.Length - 1] != ')')
			//{
			//    return false;
			//}
			try
			{
				string[] _detail = trimString.Split(LIST_SPRITER);//.Substring(1, trimString.Length - 2)
				if (_detail.Length != 4)
				{
					return false;
				}
				result = new Color(float.Parse(_detail[0]) / 255, float.Parse(_detail[1]) / 255, float.Parse(_detail[2]) / 255, float.Parse(_detail[3]) / 255);
				return true;
			}
			catch (Exception e)
			{
				Debug.LogError("Parse Color error: " + trimString + e.ToString());
				return false;
			}
		}

		public static Dictionary<String, String> ParseMap(this String strMap, Char keyValueSpriter = KEY_VALUE_SPRITER, Char mapSpriter = MAP_SPRITER)
		{
			Dictionary<String, String> result = new Dictionary<String, String>();
			if (String.IsNullOrEmpty(strMap))
			{
				return result;
			}

			var map = strMap.Split(mapSpriter);//根据字典项分隔符分割字符串，获取键值对字符串
			for (int i = 0; i < map.Length; i++)
			{
				if (String.IsNullOrEmpty(map[i]))
				{
					continue;
				}

				var keyValuePair = map[i].Split(keyValueSpriter);//根据键值分隔符分割键值对字符串
				if (keyValuePair.Length == 2)
				{
					if (!result.ContainsKey(keyValuePair[0]))
						result.Add(keyValuePair[0], keyValuePair[1]);
					else
						Debug.LogWarning(String.Format("Key {0} already exist, index {1} of {2}.", keyValuePair[0], i, strMap));
				}
				else
				{
					Debug.LogWarning(String.Format("KeyValuePair are not match: {0}, index {1} of {2}.", map[i], i, strMap));
				}
			}
			return result;
		}

		public static List<String> ParseList(this String strList, Char listSpriter = LIST_SPRITER)
		{
			var result = new List<String>();
			if (String.IsNullOrEmpty(strList))
				return result;

			var trimString = strList.Trim();
			if (String.IsNullOrEmpty(strList))
			{
				return result;
			}
			var detials = trimString.Split(listSpriter);//.Substring(1, trimString.Length - 2)
			foreach (var item in detials)
			{
				if (!String.IsNullOrEmpty(item))
					result.Add(item.Trim());
			}

			return result;
		}

		public static string GetDirectoryName(string fileName)
		{
			return fileName.Substring(0, fileName.LastIndexOf('/'));
		}

		public static String LoadFile(String fileName)
		{
			if (File.Exists(fileName))
				using (StreamReader sr = File.OpenText(fileName))
					return sr.ReadToEnd();
			else
				return String.Empty;
		}

		public static String LoadResource(String fileName)
		{
			var text = Resources.Load(fileName);
			if (text != null)
			{
				var result = text.ToString();
				Resources.UnloadAsset(text);
				return result;
			}
			else
				return String.Empty;
		}

		public static string GetFilePathWithoutExtention(string fileName)
		{
			return fileName.Substring(0, fileName.LastIndexOf('.'));
		}
	}
}