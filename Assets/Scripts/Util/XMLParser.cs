using System;
using System.IO;
using System.Security;
using System.Collections.Generic;
using Mono.Xml;

namespace Game.Util
{
	public class XMLParser
	{
		public Dictionary<String, Dictionary<String, String>> LoadMap(String fileName, out String key)
		{
			key = Path.GetFileNameWithoutExtension(fileName);
			var xml = Load(fileName);
			return LoadMap(xml);
		}

		public Boolean LoadMap(String fileName, out Dictionary<String, Dictionary<String, String>> map)
		{
			try
			{
				var xml = Load(fileName);
				map = LoadMap(xml);
				return true;
			}
			catch (Exception ex)
			{
				map = null;
				return false;
			}
		}

		public static Boolean LoadIntMap(String fileName, bool isForceOutterRecoure, out Dictionary<Int32, Dictionary<String, String>> map)
		{
			try
			{
				SecurityElement xml;
				if (isForceOutterRecoure)
				{
					xml = LoadOutter(fileName);
				}
				else
					xml = Load(fileName);
				if (xml == null)
				{
					map = null;
					return false;
				}
				else
				{
					map = LoadIntMap(xml, fileName);
					return true;
				}
			}
			catch (Exception ex)
			{
				map = null;
				return false;
			}
		}

		public static Dictionary<Int32, Dictionary<String, String>> LoadIntMap(SecurityElement xml, string source)
		{
			var result = new Dictionary<Int32, Dictionary<String, String>>();

			var index = 0;
			foreach (SecurityElement subMap in xml.Children)
			{
				index++;
				if (subMap.Children == null || subMap.Children.Count == 0)
				{
					continue;
				}
				Int32 key = Int32.Parse((subMap.Children[0] as SecurityElement).Text);
				if (result.ContainsKey(key))
				{
					continue;
				}

				var children = new Dictionary<String, String>();
				result.Add(key, children);
				for (int i = 1; i < subMap.Children.Count; i++)
				{
					var node = subMap.Children[i] as SecurityElement;

					string tag;
					if (node.Tag.Length < 3)
					{
						tag = node.Tag;
					}
					else
					{
						var tagTial = node.Tag.Substring(node.Tag.Length - 2, 2);
						if (tagTial == "_i" || tagTial == "_s" || tagTial == "_f" || tagTial == "_l" || tagTial == "_k" || tagTial == "_m")
							tag = node.Tag.Substring(0, node.Tag.Length - 2);
						else
							tag = node.Tag;
					}

					if (node != null && !children.ContainsKey(tag))
					{
						if (String.IsNullOrEmpty(node.Text))
							children.Add(tag, "");
						else
							children.Add(tag, node.Text.Trim());
					}
				}
			}
			return result;
		}

		public static Dictionary<String, Dictionary<String, String>> LoadMap(SecurityElement xml)
		{
			var result = new Dictionary<String, Dictionary<String, String>>();

			foreach (SecurityElement subMap in xml.Children)
			{
				String key = (subMap.Children[0] as SecurityElement).Text.Trim();
				if (result.ContainsKey(key))
				{
					continue;
				}

				var children = new Dictionary<string, string>();
				result.Add(key, children);
				for (int i = 1; i < subMap.Children.Count; i++)
				{
					var node = subMap.Children[i] as SecurityElement;
					if (node != null && !children.ContainsKey(node.Tag))
					{
						if (String.IsNullOrEmpty(node.Text))
							children.Add(node.Tag, "");
						else
							children.Add(node.Tag, node.Text.Trim());
					}
				}
			}
			return result;
		}

		public static SecurityElement Load(String fileName)
		{
			String xmlText = LoadText(fileName);
			if (String.IsNullOrEmpty(xmlText))
				return null;
			else
				return LoadXML(xmlText);
		}

		public static SecurityElement LoadOutter(String fileName)
		{
			String xmlText = Utils.LoadFile(fileName.Replace('\\', '/'));;
			if (String.IsNullOrEmpty(xmlText))
				return null;
			else
				return LoadXML(xmlText);
		}

		public static String LoadText(String fileName)
		{
			try
			{
				return Utils.LoadResource(fileName);
			}
			catch (Exception ex)
			{
				return "";
			}
		}

		public static SecurityElement LoadXML(String xml)
		{
			try
			{
				SecurityParser securityParser = new SecurityParser();
				securityParser.LoadXml(xml);
				return securityParser.ToXml();
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public static void SaveBytes(String fileName, byte[] buffer)
		{
			if (!Directory.Exists(Utils.GetDirectoryName(fileName)))
			{
				Directory.CreateDirectory(Utils.GetDirectoryName(fileName));
			}
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			using (FileStream fs = new FileStream(fileName, FileMode.Create))
			{
				using (BinaryWriter sw = new BinaryWriter(fs))
				{
					sw.Write(buffer);
					sw.Flush();
					sw.Close();
				}
				fs.Close();
			}
		}

		public static void SaveText(String fileName, String text)
		{
			if (!Directory.Exists(Utils.GetDirectoryName(fileName)))
			{
				Directory.CreateDirectory(Utils.GetDirectoryName(fileName));
			}
			if (File.Exists(fileName))
			{
				File.Delete(fileName);
			}
			using (FileStream fs = new FileStream(fileName, FileMode.Create))
			{
				using (StreamWriter sw = new StreamWriter(fs))
				{
					sw.Write(text);
					sw.Flush();
					sw.Close();
				}
				fs.Close();
			}
		}
	}
}