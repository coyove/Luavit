using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ConfigFile
	{
		private string m_revitIniFileName;

		public bool DebugProgressBar
		{
			get;
			private set;
		}

		public bool DebugSQL
		{
			get;
			private set;
		}

		public bool Debug
		{
			get;
			private set;
		}

		public bool IgnoreSpecificTables
		{
			get;
			private set;
		}

		public string GetCultureString()
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			int size = 1024;
			ConfigFile.GetPrivateProfileString("Language", "Select", null, stringBuilder, size, this.m_revitIniFileName);
			return stringBuilder.ToString();
		}

		public ConfigFile()
		{
			this.m_revitIniFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(ConfigFile).Assembly.Location), RDBResource.GetString("ConfigurationFileName"));
			this.DebugProgressBar = this.GetBoolean("DebugMode", "DebugProgressBar");
			this.DebugSQL = this.GetBoolean("DebugMode", "DebugSQL");
			this.Debug = this.GetBoolean("DebugMode", "Debug");
			this.IgnoreSpecificTables = this.GetBoolean("DebugMode", "IgnoreSpecificTables");
		}

		private bool GetBoolean(string section, string key)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			int size = 1024;
			ConfigFile.GetPrivateProfileString(section, key, null, stringBuilder, size, this.m_revitIniFileName);
			bool result;
			try
			{
				result = bool.Parse(stringBuilder.ToString());
			}
			catch (System.Exception)
			{
				result = false;
			}
			return result;
		}

		[System.Runtime.InteropServices.DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int WritePrivateProfileString(string section, string key, string value, string fileName);

		[System.Runtime.InteropServices.DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int WritePrivateProfileString(string section, string key, int value, string fileName);

		[System.Runtime.InteropServices.DllImport("kernel32", CharSet = CharSet.Auto)]
		private static extern int GetPrivateProfileString(string section, string key, string defaultValue, System.Text.StringBuilder result, int size, string fileName);

		[System.Runtime.InteropServices.DllImport("kernel32", CharSet = CharSet.Auto)]
		private static extern int GetPrivateProfileInt(string section, string key, int defaultValue, string filePath);
	}
}
