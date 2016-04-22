using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.ComponentModel;
using MoonSharp.Interpreter;

namespace Luavit
{
    public class Helper
    {
        public UIApplication UIApp;
        public MoonSharp.Interpreter.Script MoonScript;

        public List<string> ignoreListMember = new List<string>()
        {
            "IsWorkPlaneFlipped",
            "HostParameter"
        };

        public string Describe(object obj, bool full = false)
        {
            StringBuilder ret = new StringBuilder();
            ret.Append("(" + obj.GetType().Name + ")" + Environment.NewLine);

            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                if (TypeForm.OmittedProperties.Contains(name)) continue;
                
                try
                {
                    object value = descriptor.GetValue(obj);
                    ret.Append(String.Format("{0}={1}", name, value) + Environment.NewLine);
                }
                catch (Exception e)
                {
                    ret.Append(String.Format("{0}=[Luavit: Ignored]", name) + Environment.NewLine);
                }
            }

            if (full)
                foreach (var m in obj.GetType().GetMethods())
                {
                    ret.Append(String.Format("Method: {0}(", m.Name));
                    ret.Append(String.Join(", ", (from p in m.GetParameters() select p.Name).ToArray()) + ")");
                    ret.Append(Environment.NewLine);
                }

            return ret.ToString();
        }

        public MoonSharp.Interpreter.Table GetCLRObjectTable(object obj, int level = 1, bool err = false)
        {
            level--;
            MoonSharp.Interpreter.Table ret = new MoonSharp.Interpreter.Table(MoonScript);
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                if (TypeForm.OmittedProperties.Contains(name)) continue;
                object _value = null;

                try {
                    _value = descriptor.GetValue(obj);
                } catch (Exception e) {
                    ret.Set(name, MoonSharp.Interpreter.DynValue.NewString("[Error]"));
                    continue;
                }

                if (_value is int)
                {
                    ret.Set(name, MoonSharp.Interpreter.DynValue.NewNumber((int)_value));
                }
                else if (_value is double)
                {
                    ret.Set(name, MoonSharp.Interpreter.DynValue.NewNumber((double)_value));
                }
                else if (_value is byte)
                {
                    ret.Set(name, MoonSharp.Interpreter.DynValue.NewNumber((byte)_value));
                }
                else if (_value is string)
                {
                    ret.Set(name, MoonSharp.Interpreter.DynValue.NewString((string)_value));
                }
                else if (_value is bool)
                {
                    ret.Set(name, MoonSharp.Interpreter.DynValue.NewBoolean((bool)_value));
                }
                else if (_value != null && _value.GetType().IsEnum)
                {
                    ret.Set(name, MoonSharp.Interpreter.DynValue.NewNumber(Convert.ToInt32(_value)));
                }
                else if (level > 0)
                {
                    try {
                        ret.Set(name, MoonSharp.Interpreter.DynValue.NewTable(GetCLRObjectTable(_value, level, err)));
                    } catch (Exception e)
                    {
                        if (err) ret.Set(name, MoonSharp.Interpreter.DynValue.NewString("[Cannot get inside]"));
                    }
                }
                else
                {
                    ret.Set(name, MoonSharp.Interpreter.DynValue.NewString(_value?.ToString()));
                }
            }

            return ret;
        }

        public ElementId[] GetSelectedIds()
        {
            return (from e in UIApp.ActiveUIDocument.Selection.GetElementIds()
                    select e).ToArray();
        }

        public Element[] GetSelectedElements()
        {
            return (from e in UIApp.ActiveUIDocument.Selection.GetElementIds()
                    select UIApp.ActiveUIDocument.Document.GetElement(e)).ToArray();
        }

        public Element GetElementById(int id)
        {
            return UIApp.ActiveUIDocument.Document.GetElement(new ElementId(id));
        }

        public Element GetElementById(ElementId id)
        {
            return UIApp.ActiveUIDocument.Document.GetElement(id);
        }

        public Parameter GetElementParameter(Element e, string name)
        {            
            return e.get_Parameter((BuiltInParameter)Enum.Parse(typeof(BuiltInParameter), name));
        }

        //public Autodesk.Revit.DB.XYZ GetLocation(Element e)
        //{
        //    var lp = e.Location as LocationPoint;
        //    if (lp == null)
        //    {
        //        return new Autodesk.Revit.DB.XYZ();
        //    }
        //    else
        //        return lp.Point;
        //}

        public Element[] GetFilteredElements(MoonSharp.Interpreter.Table t)
        {
            var filter = new FilteredElementCollector(UIApp.ActiveUIDocument.Document);

            for (int i = 0; i < t.Keys.Count(); i++)
            {
                switch (t.Keys.ElementAt(i).String)
                {
                    case "full":
                        filter = filter.WherePasses(
                            new LogicalOrFilter(
                              new ElementIsElementTypeFilter(false),
                              new ElementIsElementTypeFilter(true)));
                        break;
                    case "class":
                        filter = filter.OfClass((from x in TypeForm.GlobalTypesList
                                                 where x.Name == t.Values.ElementAt(i).String
                                                 select x).ToArray()[0]);
                        break;
                    case "category":
                        filter = filter.OfCategory((BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), t.Values.ElementAt(i).String));
                        break;
                    case "filter":
                        var func = t.Values.ElementAt(i).Function;
                        List<Element> ret = new List<Element>();

                        foreach (var e in filter)
                        {
                            if (func.Call(e).Boolean) ret.Add(e);
                        }

                        return ret.ToArray();
                }
            }

            if (t.Keys.Count() == 0)
            {
                filter = filter.WherePasses(
                    new LogicalOrFilter(
                      new ElementIsElementTypeFilter(false),
                      new ElementIsElementTypeFilter(true)));
            }

            return filter.ToElements().ToArray();
        }
    }
}
