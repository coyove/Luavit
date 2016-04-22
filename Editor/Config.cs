using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using System.Windows.Forms;

namespace Luavit
{
    class Config
    {
        public static void LoadConfig(string configScript, Form main )
        {
            Script script = new Script();
            script.Globals.Set("form", UserData.Create(main));
            // script.DoFile(configScript);
            script.DoString(System.IO.File.ReadAllText(configScript));
        }
    }
}
