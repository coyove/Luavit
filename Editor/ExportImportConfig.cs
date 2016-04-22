using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class ExportImportConfig
	{
		public System.Collections.Generic.List<string> IgnoreTables = new System.Collections.Generic.List<string>();

		public bool IgnoreSharedParameters;

		public System.Collections.Generic.List<string> IgnoreSharedParametersExceptions = new System.Collections.Generic.List<string>();

		public void Initialize(string ignoreTablesFile, string docName, bool ignoreSpecificTables = false)
		{
			this.IgnoreTables.Clear();
			try
			{
				if (!System.IO.File.Exists(ignoreTablesFile))
				{
					Log.WriteLine("Config file not found");
				}
				else
				{
					Log.WriteLine("Config file found");
					using (System.IO.StreamReader streamReader = new System.IO.StreamReader(ignoreTablesFile, true))
					{
						XElement xElement = XElement.Load(streamReader);
						XElement xElement2 = null;
						System.Collections.Generic.IEnumerable<XElement> source = from t in xElement.Elements("Tables")
						where t.Attribute("file") != null && t.Attribute("file").Value == docName
						select t;
						if (source.Count<XElement>() > 0)
						{
							xElement2 = source.First<XElement>();
							if (xElement2 != null)
							{
								foreach (XElement current in xElement2.Elements("Table"))
								{
									this.IgnoreTables.Add(current.Attribute("name").Value);
									Log.WriteLine("Config: Not supported table: " + current.Attribute("name").Value);
								}
								System.Collections.Generic.IEnumerable<XElement> source2 = xElement2.Elements("SharedParameters");
								if (source2.Count<XElement>() > 0)
								{
									this.IgnoreSharedParameters = true;
									XElement xElement3 = source2.First<XElement>();
									System.Collections.Generic.IEnumerable<XAttribute> source3 = xElement3.Attributes("ignore");
									if (source3.Count<XAttribute>() > 0)
									{
										bool ignoreSharedParameters;
										if (!bool.TryParse(source3.First<XAttribute>().Value, out ignoreSharedParameters))
										{
											ignoreSharedParameters = true;
										}
										this.IgnoreSharedParameters = ignoreSharedParameters;
									}
									System.Collections.Generic.IEnumerable<XElement> enumerable = xElement3.Elements("Except");
									foreach (XElement current2 in enumerable)
									{
										System.Collections.Generic.IEnumerable<XAttribute> source4 = current2.Attributes("name");
										if (source4.Count<XAttribute>() > 0)
										{
											this.IgnoreSharedParametersExceptions.Add(source4.First<XAttribute>().Value);
										}
									}
								}
								Log.WriteLine("IgnoreSharedParameters: " + this.IgnoreSharedParameters);
								if (this.IgnoreSharedParametersExceptions.Count > 0)
								{
									Log.WriteLine("Except: ");
								}
								foreach (string current3 in this.IgnoreSharedParametersExceptions)
								{
									Log.WriteLine("\t" + current3);
								}
							}
						}
					}
					if (ignoreSpecificTables)
					{
						this.IgnoreTables.Add("ProjectInformation");
						this.IgnoreTables.Add("ElementLevel");
						this.IgnoreTables.Add("ElementPhase");
						this.IgnoreTables.Add("RoomTags");
						this.IgnoreTables.Add("CurtainWallMullions");
						this.IgnoreTables.Add("ElementPhase");
						this.IgnoreTables.Add("MaterialQuantities");
						this.IgnoreTables.Add("DoorWall");
						this.IgnoreTables.Add("CurtainWallPanelOnWall");
						this.IgnoreTables.Add("WindowWall");
						this.IgnoreTables.Add("MechanicalEquipmentOnWall");
						this.IgnoreTables.Add("PlumbingFixtureOnWall");
						this.IgnoreTables.Add("LightingFixtureOnWall");
						this.IgnoreTables.Add("OpeningOnWall");
						this.IgnoreTables.Add("GenericModelOnWall");
						this.IgnoreTables.Add("CaseworkOnWall");
					}
				}
			}
			catch (System.Exception ex)
			{
				Log.WriteLine(ex.ToString());
			}
		}
	}
}
