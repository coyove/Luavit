using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class RoomFromToAssociationsList : MultiPrimaryKeyObjectList
	{
		private System.Collections.Generic.IEnumerable<Phase> m_phases;

		public override void InitializeList()
		{
			this.m_phases = APIObjectList.ActiveDocument.Phases.OfType<Phase>();
			foreach (FamilyInstance current in base.GetFamilyInstance(new BuiltInCategory[]
			{
				(BuiltInCategory)(-2000023),
                (BuiltInCategory)(-2000014)
			}))
			{
				if (current != null)
				{
					foreach (Phase current2 in this.m_phases)
					{
						ElementOnPhaseStatus phaseStatus = current.GetPhaseStatus(current2.Id);
						if (phaseStatus == (ElementOnPhaseStatus)4 || phaseStatus == (ElementOnPhaseStatus)2)
						{
							object obj = this.CreateListItem(current, current2);
							if (obj != null)
							{
								this.m_list.Add(obj);
							}
						}
					}
				}
			}
		}

		protected override void PopulateDbRow(object element, DataRow row)
		{
			object[] array = element as object[];
			if (array == null || array.Length < 5)
			{
				return;
			}
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_Id"), array[0]);
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_PhaseId"), array[1]);
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_DesignOptionId"), array[2]);
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_FromRoom"), APIObjectList.GetIdDbValue((int)array[3]));
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_ToRoom"), APIObjectList.GetIdDbValue((int)array[4]));
		}

		public override object GetRevitObject(DataRow dataRow)
		{
			int id = System.Convert.ToInt32(dataRow[APIObjectList.ColumnRes(base.TableInfo.PrimaryKeys[0])]);
			System.Convert.ToInt32(dataRow[APIObjectList.ColumnRes(base.TableInfo.PrimaryKeys[1])]);
			if (!(APIObjectList.GetElementById(id) is FamilyInstance))
			{
				return null;
			}
			return dataRow.ItemArray;
		}

		protected override object[] GetPrimaryKeyValues(object obj)
		{
			object[] array = obj as object[];
			if (array == null)
			{
				return null;
			}
			return new object[]
			{
				array[0],
				array[1],
				array[2]
			};
		}

		private object CreateListItem(FamilyInstance familyInstance, Phase phase)
		{
			Room toRoom = this.GetToRoom(familyInstance, phase);
			Room fromRoom = this.GetFromRoom(familyInstance, phase);
			if (toRoom != null || fromRoom != null)
			{
				return new object[]
				{
					familyInstance.Id.IntegerValue,
					phase.Id.IntegerValue,
					APIObjectList.GetElementId(familyInstance.DesignOption),
					APIObjectList.GetElementId(fromRoom),
					APIObjectList.GetElementId(toRoom)
				};
			}
			return null;
		}

		private Room GetToRoom(FamilyInstance familyInstance, Phase phase)
		{
			if (familyInstance == null)
			{
				return null;
			}
			try
			{
				return familyInstance.get_ToRoom(phase);
			}
			catch (System.Exception value)
			{
				Log.WriteLine("FamilyInstance.get_ToRoom Id:{0}", new object[]
				{
					APIObjectList.GetElementId(familyInstance)
				});
				Log.WriteLine(value);
			}
			return null;
		}

		private Room GetFromRoom(FamilyInstance familyInstance, Phase phase)
		{
			if (familyInstance == null)
			{
				return null;
			}
			try
			{
				return familyInstance.get_FromRoom(phase);
			}
			catch (System.Exception value)
			{
				Log.WriteLine("FamilyInstance.get_FromRoom Id:{0}", new object[]
				{
					APIObjectList.GetElementId(familyInstance)
				});
				Log.WriteLine(value);
			}
			return null;
		}
	}
}
