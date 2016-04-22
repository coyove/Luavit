using Autodesk.Revit.DB;
using System;
using System.Data;
using System.Linq;

namespace Revit.Addon.RevitDBLink.CS
{
	public class SymbolList : ElementList
	{
		private int m_numberSuffix = 2;

		protected BuiltInCategory m_category;

		protected ElementTypeEnum m_instanceOrSymbol;

		public override bool SupportCreate
		{
			get
			{
				return base.Elements.Size != 0;
			}
		}

		public override bool SupportImport
		{
			get
			{
				return true;
			}
		}

		public SymbolList(BuiltInCategory builtInCategory)
		{
			this.m_category = builtInCategory;
			this.m_instanceOrSymbol = ElementTypeEnum.SYMBOL;
		}

		public override void InitializeList()
		{
			base.AddRange(APIObjectList.GetElements(true, new BuiltInCategory[]
			{
				this.m_category
			}));
		}

		protected override void PopulateDbRow(Element element, DataRow row)
		{
			ParameterMap parameterMap = null;
			try
			{
				parameterMap = element.ParametersMap;
			}
			catch (System.Exception value)
			{
				Log.WriteLine("Element Id {0}", new object[]
				{
					element.Id.IntegerValue
				});
				Log.WriteLine("Element.ParametersMap");
				Log.WriteLine(value);
			}
			foreach (ColumnInfo current in base.TableInfo.Values)
			{
				if (current.BuiltInParameter != (BuiltInParameter)(- 1))
				{
					BuiltInParameter builtInParameter = current.BuiltInParameter;
					Parameter parameter = element.get_Parameter(builtInParameter);
					if (parameter != null)
					{
						Utility.Assign(row, current.Name, base.GetParameterDdValue(parameter));
					}
				}
				else if (parameterMap != null)
				{
					Parameter parameterByDefinitionName = APIObjectList.GetParameterByDefinitionName(parameterMap, current.Name);
					Utility.Assign(row, current.Name, base.GetParameterDdValue(parameterByDefinitionName));
				}
				else
				{
					Utility.Assign(row, current.Name, System.DBNull.Value);
				}
			}
		}

		public override ErrorTable ImportFromDataTable(DataTable dataTable)
		{
			ErrorTable errorTable = new ErrorTable(base.TableInfo.TableId, base.TableInfo.Name);
			foreach (DataRow dataRow in dataTable.Rows)
			{
				if (dataRow.RowState != DataRowState.Deleted)
				{
					Element element = this.GetRevitObject(dataRow) as Element;
					ErrorRow errorRow;
					if (element == null)
					{
						element = this.CreateNewElement(dataRow);
						if (element == null)
						{
							errorRow = new ErrorRow(dataRow);
							errorRow.State = DataRowState.Deleted;
							errorRow.HasChange = true;
							for (int i = 0; i < dataTable.Columns.Count; i++)
							{
								string columnName = dataTable.Columns[i].ColumnName;
								errorRow.Cells.Add(columnName, new ErrorCell(columnName, UpdateResult.Equals));
							}
						}
						else
						{
							errorRow = this.UpdateOneElement(element, dataRow);
							errorRow.State = DataRowState.Added;
							errorRow.HasChange = true;
						}
					}
					else
					{
						errorRow = this.UpdateOneElement(element, dataRow);
					}
					errorTable.Rows.Add(errorRow);
					if (!errorTable.HasChange && errorRow.HasChange)
					{
						errorTable.HasChange = true;
					}
				}
			}
			return errorTable;
		}

		protected virtual Element CreateNewElement(DataRow row)
		{
            // *** Coyove Patched ***

			Element result;
			try
			{
				string text = APIObjectList.ColumnRes((BuiltInParameter)(-1002002));
				string text2 = APIObjectList.ColumnRes((BuiltInParameter)(-1002001));
				string text3 = row[text].ToString();
				string text4 = row[text2].ToString();
				if (text3 == null || text4 == null || text3 == string.Empty || text4 == string.Empty)
				{
					result = null;
				}
				else
				{
					string arg = text4;
					while (true)
					{
						string text5 = text4.Replace("'", "''");
						string text6 = text3.Replace("'", "''");
                        // string filterExpression = string.Format("[{0}] = '{1}' and [{2}] = '{3}'", new object[]
                        string filterExpression = string.Format("\"{0}\" = '{1}' and \"{2}\" = '{3}'", new object[]
                        {
							text,
							text6,
							text2,
							text5
						});
						if (row.Table.Select(filterExpression).Length <= 1)
						{
							break;
						}
						text4 = arg + " " + this.m_numberSuffix;
						Utility.Assign(row, text2, text4);
						this.m_numberSuffix++;
					}
					Element element = null;
					foreach (Element element2 in base.Elements)
					{
						if (element == null)
						{
							element = element2;
						}
						Parameter parameter = element2.get_Parameter((BuiltInParameter)(-1002002));
						if (parameter != null && parameter.AsString().Equals(text3))
						{
							element = element2;
							break;
						}
					}
					if (element == null)
					{
						result = null;
					}
					else
					{
						ElementType elementType = element as ElementType;
						Element element3 = elementType.Duplicate(text4);
						base.Elements.Insert(element3);
						row[base.TableInfo[base.TableInfo.PrimaryKeys[0]].Name] = element3.Id.IntegerValue;
						result = element3;
					}
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		private ErrorRow UpdateOneElement(Element elem, DataRow row)
		{
			ErrorRow errorRow = new ErrorRow(row);
			ParameterMap parameterMap = null;
			try
			{
				parameterMap = elem.ParametersMap;
			}
			catch (System.Exception ex)
			{
				Log.WriteLine(string.Concat(new object[]
				{
					"TableName:",
					base.TableInfo.Name,
					" ElementId:",
					elem.Id.IntegerValue
				}));
				Log.WriteLine(ex.ToString());
			}
			foreach (ColumnInfo current in base.TableInfo.Values)
			{
				Parameter param;
				if (current.BuiltInParameter != (BuiltInParameter)(-1))
				{
					param = elem.get_Parameter(current.BuiltInParameter);
				}
				else
				{
					param = APIObjectList.GetParameterByDefinitionName(parameterMap, current.Name);
				}
				object paramValue = row[current.Name];
				UpdateResult state = (current.BuiltInParameter == (BuiltInParameter)(-1002500)) ? UpdateResult.AssemblyCode : this.UpdateParameter(param, paramValue);
				ErrorCell value = new ErrorCell(current.Name, state);
				errorRow.Cells.Add(current.Name, value);
				switch (state)
				{
				case UpdateResult.Unknown:
				case UpdateResult.Success:
				case UpdateResult.Failed:
				case UpdateResult.ReadOnlyFailed:
				case UpdateResult.ParameterNull:
				case UpdateResult.Exception:
					errorRow.HasChange = true;
					break;
				}
			}
			return errorRow;
		}

		private UpdateResult UpdateParameter(Parameter param, object paramValue)
		{
			UpdateResult result;
			try
			{
				bool flag = param == null;
				bool flag2 = this.IsNullOrDBNull(paramValue);
				if (flag)
				{
					if (flag2)
					{
						result = UpdateResult.Equals;
					}
					else
					{
						result = UpdateResult.ParameterNull;
					}
				}
				else
				{
					bool flag3 = this.IsParameterValueEmpty(param);
					bool flag4 = this.IsValueEmpty(param.StorageType, paramValue);
					if (flag3)
					{
						if (flag2 || flag4)
						{
							result = UpdateResult.Equals;
						}
						else if (param.IsReadOnly)
						{
							result = UpdateResult.ReadOnlyFailed;
						}
						else
						{
							result = (this.SetParameter(param, paramValue) ? UpdateResult.Success : UpdateResult.Failed);
						}
					}
					else if (flag2 || flag4)
					{
						if (param.IsReadOnly)
						{
							result = UpdateResult.ReadOnlyFailed;
						}
						else
						{
							result = (this.SetParameterValueToEmpty(param) ? UpdateResult.Success : UpdateResult.Failed);
						}
					}
					else
					{
						bool flag5 = false;
						switch (param.StorageType)
						{
						case 0:
						{
							string text = param.AsValueString();
							if (text == null)
							{
								flag5 = (paramValue == null || paramValue == System.DBNull.Value);
								goto IL_343;
							}
							flag5 = text.Equals(paramValue.ToString());
							goto IL_343;
						}
						case (StorageType) 1:
							flag5 = param.AsInteger().Equals(System.Convert.ToInt32(paramValue));
							goto IL_343;
						case (StorageType)2:
						{
							double num = param.AsDouble();
							double num2 = double.NaN;
							try
							{
								num2 = Unit.CovertFromAPI(param.DisplayUnitType, num);
								double left;
								if (double.TryParse(paramValue.ToString(), out left))
								{
									flag5 = Unit.DoubleEquals(left, num2);
								}
								goto IL_343;
							}
							catch (System.InvalidCastException ex)
							{
								Log.WriteLine(ex);
								Log.WriteLine(string.Concat(new object[]
								{
									"Set (",
									param.Definition.Name,
									") to (",
									paramValue,
									")"
								}));
								Log.WriteLine("\tparamValue = " + paramValue);
								Log.WriteLine("\tparamValue Type = " + paramValue.GetType());
								Log.WriteLine("\tparameter value = " + base.GetParameterDdValue(param));
								Log.WriteLine("\tparameter StorageType = " + param.StorageType);
								Log.WriteLine("\tunitValue = " + num2);
								Log.WriteLine("\tunitValue Type = " + num2.GetType());
								Log.WriteLine("unitValue.Equals((Double)paramValue)");
								Log.WriteLine("{0}.Equals((Double){1})", new object[]
								{
									num2,
									paramValue
								});
								Log.WriteLine("\tInner exeception:" + ex);
								Log.WriteLine("doubleValue.Equals((Double)paramValue");
								Log.WriteLine("{0}.Equals((Double){1})", new object[]
								{
									num,
									paramValue
								});
								throw;
							}
							catch (System.Exception)
							{
								Log.WriteLine(string.Concat(new object[]
								{
									"Set (",
									param.Definition.Name,
									") to (",
									paramValue,
									")"
								}));
								Log.WriteLine("Element Id: " + param.Element.Id);
								throw;
							}
							break;
						}
						case (StorageType)3:
						{
							string text2 = param.AsString();
							if (text2 == null)
							{
								flag5 = (paramValue == null || paramValue == System.DBNull.Value);
								goto IL_343;
							}
							flag5 = text2.Equals(paramValue.ToString());
							goto IL_343;
						}
						case (StorageType)4:
							break;
						default:
							goto IL_343;
						}
						flag5 = param.AsElementId().IntegerValue.Equals(System.Convert.ToInt32(paramValue));
						IL_343:
						if (flag5)
						{
							result = UpdateResult.Equals;
						}
						else if (param.IsReadOnly)
						{
							result = UpdateResult.ReadOnlyFailed;
						}
						else
						{
							bool flag6 = this.SetParameter(param, paramValue);
							if (!flag6)
							{
								Log.WriteWarning(string.Concat(new object[]
								{
									"Update Failed: Set (",
									param.Definition.Name,
									") to (",
									paramValue,
									")"
								}));
							}
							result = (flag6 ? UpdateResult.Success : UpdateResult.Failed);
						}
					}
				}
			}
			catch (System.Exception arg)
			{
				Log.WriteLine("\tOutside Exception: " + arg);
				Log.WriteLine("=======");
				result = UpdateResult.Exception;
			}
			return result;
		}

		private bool IsValueEmpty(StorageType storageType, object paramValue)
		{
			if (paramValue == null || paramValue == System.DBNull.Value)
			{
				return true;
			}
			switch (storageType)
			{
			case (StorageType)0:
				return paramValue.ToString() == null;
			case (StorageType)1:
				return int.Parse(paramValue.ToString()) == 0;
			case (StorageType)2:
				return double.Parse(paramValue.ToString(), RDBResource.Culture) == 0.0;
			case (StorageType)3:
				return string.IsNullOrEmpty(paramValue.ToString());
			case (StorageType)4:
				return int.Parse(paramValue.ToString()) == -1;
			default:
				return false;
			}
		}

		private bool IsParameterValueEmpty(Parameter parameter)
		{
			switch (parameter.StorageType)
			{
			case (StorageType)0:
			{
				string value = parameter.AsValueString();
				return string.IsNullOrEmpty(value);
			}
			case (StorageType)1:
				return parameter.AsInteger() == 0;
			case (StorageType)2:
				return parameter.AsDouble() == 0.0;
			case (StorageType)3:
			{
				string value = parameter.AsString();
				return string.IsNullOrEmpty(value);
			}
			case (StorageType)4:
				return parameter.AsElementId().IntegerValue == -1;
			default:
				return false;
			}
		}

		private bool SetParameterValueToEmpty(Parameter param)
		{
			switch (param.StorageType)
			{
			case (StorageType)0:
				return param.Set((ElementId)null);
			case (StorageType)1:
				return param.Set(0);
			case (StorageType)2:
				return param.Set(0.0);
			case (StorageType)3:
				return param.Set((ElementId)null);
			case (StorageType)4:
				return param.Set(new ElementId(-1));
			default:
				return false;
			}
		}

		private bool SetParameter(Parameter parameter, object value)
		{
			switch (parameter.StorageType)
			{
			case (StorageType)0:
				return parameter.SetValueString(value.ToString());
			case (StorageType)1:
				return parameter.Set((int)value);
			case (StorageType)2:
				try
				{
					double value2;
					bool result;
					if (double.TryParse(value.ToString(), out value2))
					{
						double num = Unit.CovertToAPI(value2, parameter.DisplayUnitType);
						result = parameter.Set(num);
						return result;
					}
					Log.WriteLine("value can't be parsed to double");
					result = false;
					return result;
				}
				catch (System.Exception arg)
				{
					Log.WriteLine("Set Parameter Inner exception: " + arg);
					bool result = parameter.Set((double)value);
					return result;
				}
				break;
			case (StorageType)3:
				return parameter.Set(value.ToString());
			case (StorageType)4:
				break;
			default:
				return false;
			}
			return parameter.Set(new ElementId((int)value));
		}

		private bool IsNullOrDBNull(object paramValue)
		{
			return paramValue == null || paramValue == System.DBNull.Value;
		}
	}
	public class SymbolList<T> : SymbolList where T : ElementType
	{
		public SymbolList(BuiltInCategory builtInCategory) : base(builtInCategory)
		{
		}

		public override void InitializeList()
		{
			base.AddRange(APIObjectList.GetElements(true, new BuiltInCategory[]
			{
				this.m_category
			}).OfType<T>());
		}
	}
}
