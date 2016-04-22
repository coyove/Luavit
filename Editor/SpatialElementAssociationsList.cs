using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public abstract class SpatialElementAssociationsList : MultiPrimaryKeyObjectList
	{
		private System.Collections.Generic.IEnumerable<Phase> m_phases;

		public override void InitializeList()
		{
			this.m_phases = APIObjectList.ActiveDocument.Phases.OfType<Phase>();
			System.Collections.Generic.IEnumerable<FamilyInstance> familyInstance = base.GetFamilyInstance(new BuiltInCategory[]
			{
				(BuiltInCategory)(-2001000),
				(BuiltInCategory)(-2001040),
				(BuiltInCategory)(-2001060),
				(BuiltInCategory)(-2000080),
				(BuiltInCategory)(-2001100),
				(BuiltInCategory)(-2000151),
				(BuiltInCategory)(-2001120),
				(BuiltInCategory)(-2001140),
				(BuiltInCategory)(-2001160),
				(BuiltInCategory)(-2001350),
				(BuiltInCategory)(-2008013),
				(BuiltInCategory)(-2008075),
				(BuiltInCategory)(-2008077),
				(BuiltInCategory)(-2008079),
				(BuiltInCategory)(-2008081),
				(BuiltInCategory)(-2008083),
				(BuiltInCategory)(-2008085),
				(BuiltInCategory)(-2008087),
                (BuiltInCategory)(-2008099)
			});
			foreach (FamilyInstance current in familyInstance)
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
								if (Command.ConfigFile.Debug)
								{
									object[] array = obj as object[];
									Log.WriteLine("Id: {0}, Phase: {1}, SpatialEle: {2}, DesignOption: {3}", new object[]
									{
										array[0],
										array[1],
										array[2],
										array[3]
									});
								}
								this.m_list.Add(obj);
							}
						}
					}
				}
			}
		}

		public override object GetRevitObject(DataRow dataRow)
		{
			int num = System.Convert.ToInt32(dataRow[base.TableInfo[base.TableInfo.PrimaryKeys[0]].Name]);
			int num2 = System.Convert.ToInt32(dataRow[base.TableInfo[base.TableInfo.PrimaryKeys[1]].Name]);
			if (!(APIObjectList.GetElementById(num) is FamilyInstance))
			{
				return null;
			}
			Phase phase = APIObjectList.GetElementById(num2) as Phase;
			if (phase == null || !phase.IsValidObject)
			{
				Log.WriteLine("Got invalid phase! {0}, Phase: {1}", new object[]
				{
					num,
					num2
				});
				return null;
			}
			return dataRow.ItemArray;
		}

		private object CreateListItem(FamilyInstance familyInstance, Phase phase)
		{
			SpatialElement spatialElement = this.GetSpatialElement(familyInstance, phase);
			if (spatialElement != null)
			{
				return new object[]
				{
					familyInstance.Id.IntegerValue,
					phase.Id.IntegerValue,
					spatialElement.Id.IntegerValue,
					APIObjectList.GetElementId(familyInstance.DesignOption)
				};
			}
			return null;
		}

		protected override void PopulateDbRow(object element, DataRow row)
		{
			object[] array = element as object[];
			if (array == null || array.Length < 4)
			{
				return;
			}
			int num = (int)array[0];
			int num2 = (int)array[1];
			int num3 = (int)array[2];
			int num4 = (int)array[3];
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_Id"), num);
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_PhaseId"), num2);
			Utility.Assign(row, APIObjectList.ColumnRes("ColN_CST_DesignOptionId"), num4);
			Utility.Assign(row, APIObjectList.ColumnRes(this.GetSpatialElementColumnKey()), num3);
		}

		protected override object[] GetPrimaryKeyValues(object obj)
		{
			object[] array = obj as object[];
			if (array == null)
			{
				return null;
			}
			int num = (int)array[0];
			int num2 = (int)array[1];
			int arg_26_0 = (int)array[2];
			int num3 = (int)array[3];
			return new object[]
			{
				num,
				num2,
				num3
			};
		}

		protected abstract string GetSpatialElementColumnKey();

		protected abstract SpatialElement GetSpatialElement(FamilyInstance familyInstance, Phase phase);
	}
}
