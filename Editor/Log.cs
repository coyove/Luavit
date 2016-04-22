using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Revit.Addon.RevitDBLink.CS
{
	public class Log
	{
		private static int mCurrentLevel = 0;

		public static string Version = "0.0.0.0";

		private static string CurrentTime
		{
			get
			{
				return string.Format("[{0}] ", System.DateTime.Now.ToString("HH:mm:ss.ffff"));
			}
		}

		public static string LogFilename
		{
			get
			{
				return Command.RecordingJournalFile;
			}
		}

		public static bool HasError
		{
			get;
			set;
		}

		public static bool HasWarning
		{
			get;
			set;
		}

		public static void Start()
		{
			Log.Reset();
		}

		private static void Reset()
		{
			Log.mCurrentLevel = 0;
			Log.HasError = (Log.HasWarning = false);
		}

		public static void Stop()
		{
			try
			{
				Log.Flush();
				Trace.Close();
			}
			catch (System.Exception ex)
			{
				TaskDialog.Show("Log Error", "Log stop error: " + ex.ToString());
			}
		}

		private static void Flush()
		{
			Trace.Flush();
		}

		private static void TraceWriteLine(string msg)
		{
			Trace.WriteLine(msg);
			try
			{
				if (Command.App != null)
				{
					Command.App.WriteJournalComment("[RDBL] " + msg, false);
				}
			}
			catch (System.Exception)
			{
			}
		}

		public static void WriteLine(string format, params object[] args)
		{
			string text = string.Format(format, args);
			text = text.Replace(System.Environment.NewLine, System.Environment.NewLine + Log.IndentWithTimeString(Log.mCurrentLevel));
			Log.TraceWriteLine(Log.CurrentTime + Log.Indent(Log.mCurrentLevel) + text);
			Log.Flush();
		}

		public static void WriteLine(string value)
		{
			Log.TraceWriteLine(Log.CurrentTime + Log.Indent(Log.mCurrentLevel) + value);
			Log.Flush();
		}

		public static void WriteLine(System.Collections.Generic.List<string> value)
		{
			Log.TraceWriteLine(Log.CurrentTime);
			foreach (string current in value)
			{
				Log.TraceWriteLine(Log.IndentWithTimeString(Log.mCurrentLevel) + current);
			}
			Log.Flush();
		}

		public static void WriteLine(System.Text.StringBuilder value)
		{
			value.Replace(System.Environment.NewLine, System.Environment.NewLine + Log.IndentWithTimeString(Log.mCurrentLevel));
			Log.TraceWriteLine(Log.CurrentTime + Log.Indent(Log.mCurrentLevel) + value);
			Log.Flush();
		}

		public static void WriteWarning(string format, params object[] args)
		{
			Log.HasWarning = true;
			Log.WriteLine("[----Warning----] " + format, args);
		}

		public static void WriteWarning(string value)
		{
			Log.HasWarning = true;
			Log.WriteLine("[----Warning----] " + value);
		}

		public static void WriteError(string format, params object[] args)
		{
			Log.HasError = true;
			Log.WriteLine("[----Error----] " + format, args);
		}

		public static void WriteError(string value)
		{
			Log.HasError = true;
			Log.WriteLine("[----Error----] " + value);
		}

		public static void WriteNotice(string format, params object[] args)
		{
			Log.WriteLine("[----Notice----] " + format, args);
		}

		public static void WriteNotice(string value)
		{
			Log.WriteLine("[----Notice----] " + value);
		}

		public static void WriteLine(object value)
		{
			Log.WriteLine(value.ToString());
		}

		public static void Indent()
		{
			Log.mCurrentLevel++;
		}

		public static string Indent(int level)
		{
			return new string(' ', level * 4);
		}

		public static string IndentWithTimeString(int level)
		{
			return new string(' ', level * 4 + Log.CurrentTime.Length);
		}

		public static void Unindent()
		{
			if (Log.mCurrentLevel > 0)
			{
				Log.mCurrentLevel--;
			}
		}
	}
}
