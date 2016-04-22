using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Reflection;

namespace Luavit
{
    public class Maker
    {
        public IEnumerable<Type> TypeList;

        public Autodesk.Revit.DB.XYZ NewPoint(double X, double Y, double Z)
        {
            return new Autodesk.Revit.DB.XYZ(X, Y, Z);
        }

        public Autodesk.Revit.DB.XYZ NewPoint(Autodesk.Revit.DB.XYZ p)
        {
            return new Autodesk.Revit.DB.XYZ(p.X, p.Y, p.Z);
        }

        public Autodesk.Revit.DB.Architecture.Room NewRoom(Autodesk.Revit.DB.Element e)
        {
            //(e as Autodesk.Revit.DB.Architecture.Room).get_Parameter(BuiltInParameter.ROOM_AREA)
            return e as Autodesk.Revit.DB.Architecture.Room;
        }

        public object CLRObject(string name, MoonSharp.Interpreter.Table t, string ns = "")
        {
            var tmp = (from x in TypeForm.GlobalTypesList
                       where x.Name == name && (ns == "" || x.Namespace == ns)
                       select x);
            if (tmp.Count() == 0)
                throw new System.InvalidOperationException("This type is not imported: " + name);

            var clsType = tmp.ToArray()[0];

            List<object> _params = new List<object>();

            foreach (var v in t.Values)
            {
                _params.Add(v.ToObject());
            }

            return Activator.CreateInstance(clsType, _params.ToArray());
        }

        public object CLRExplicitObject(string name, List<MoonSharp.Interpreter.Table> t, string ns = "") //List<object> t)
        {
            var tmp = (from x in TypeForm.GlobalTypesList
                       where x.Name == name && (ns == "" || x.Namespace == ns)
                       select x);
            if (tmp.Count() == 0)
                throw new System.InvalidOperationException("This type is not imported: " + name);

            var clsType = tmp.ToArray()[0];

            List<object> _params = new List<object>();
            List<Type> _types = new List<Type>();

            foreach (var v in t)
            {
                switch (v.Keys.First().String)
                {
                    case "byte":
                        _params.Add((byte)v.Values.First().Number);
                        _types.Add(typeof(byte));
                        break;
                    case "int":
                        _params.Add((int)v.Values.First().Number);
                        _types.Add(typeof(int));
                        break;
                    case "double":
                        _params.Add((double)v.Values.First().Number);
                        _types.Add(typeof(double));
                        break;
                    case "bool":
                        _params.Add((bool)v.Values.First().Boolean);
                        _types.Add(typeof(bool));
                        break;
                    case "string":
                        _params.Add((string)v.Values.First().String);
                        _types.Add(typeof(string));
                        break;
                    default:
                        var ty = Type.GetType(v.Keys.First().String);
                        if (ty == null)
                        {
                            ty = (from x in TypeForm.GlobalTypesList
                                  where x.Name == v.Keys.First().String
                                  select x).ToArray()[0];
                        }
                        if (ty == null)
                            throw new System.InvalidOperationException("This type is not imported: " + v.Keys.First().String);
                        _types.Add(ty);

                        _params.Add(v.Values.First().ToObject());
                        break;
                }
            }

            ConstructorInfo ctor = clsType.GetConstructor(_types.ToArray());
            object instance = ctor.Invoke(_params.ToArray());

            return ctor.Invoke(_params.ToArray());  //Activator.CreateInstance(clsType, _params.ToArray());
        }
    }
}
