using System;
using System.Collections.Generic;
using System.Text;
using Imagine.Library;
using IronPython.Hosting;
using System.IO;
using System.Windows.Forms;
using Imagine.GUI;
using Standard_Machine_GUIs;

namespace Imagine.StandardMachines
{
    [UniqueName("Imagine.PythonScript")]
    [InputNames("input 1", "input 2", "input 3", "input 4")]
    [InputCodes('1', '2', '3', '4')]
    [OutputNames("output")]
    [OutputCodes(' ')]
    [Description("Write your own machine in Python (slow as hell).")]
    public class PythonMachine : Machine
    {
        private PythonEngine engine = new PythonEngine();

        private string pythonlib = Resources.pythonlib;

        private string script = "";

        public string Script
        {
            get { return script; }
            set { script = value; OnMachineChanged(); }
        }

        private string errors = "";

        public string Errors
        {
            get { return errors; }
        }
        
        public PythonMachine()
        {
            engine.AddToPath(Path.GetDirectoryName(Application.ExecutablePath));

            script =
                "# This is a sample script that just copies everything\r\n" +
                "# from input 1 to a new full image on the output\r\n\r\n" +
                "input = inputs[0]\r\n" +
                "outputs[0] = FullImage(input.Width, input.Height)\r\n" +
                "for x in range(input.Width):\r\n" +
                "\tfor y in range(input.Height):\r\n" +
                "\t\ta, r, g, b = get(input, x, y)\r\n" +
                "\t\tset(outputs[0], x, y, a, r, g, b)\r\n";
        }

        public override string Caption
        {
            get { return "Python"; }
        }

        protected override ImagineImage[] DoProcess(ImagineImage[] inputs, ProgressCallback callback)
        {
            ImagineImage[] outputs = new ImagineImage[outputCodes.Length];

            Dictionary<string, object> locals = new Dictionary<string,object>();
            locals.Add("inputs", inputs);
            locals.Add("outputs", outputs);
            locals.Add("_callback", callback);

            try
            {
                engine.Execute(pythonlib + "\r\n" + script, engine.DefaultModule, locals);
                errors = "";
            }
            catch (Exception e)
            {
                errors = e.Message;
            }
            
            return outputs;
        }

        public override void LoadSettings(string settings)
        {
            Dictionary<string, string> settingsMap = ParseSettings(settings);
            if (!settingsMap.ContainsKey("script"))
                return;
          
            script = DecodeEscapeSequences(settingsMap["script"]);
        }

        public override string SaveSettings()
        {
            List<char> lettersToEncode = new List<char>(new char[] { '{', '}', '"', '\'', '\t', '\n', '\r' });
            Dictionary<string, string> settingsMap = new Dictionary<string, string>();
            settingsMap["script"] = ReplaceWithEscapeSequence(script, lettersToEncode);
            return CompileSettings(settingsMap);
        }

        private string ReplaceWithEscapeSequence(string str, List<char> letters)
        {
            StringBuilder outstr = new StringBuilder();
            foreach(Char c in str)
            {
                if (c == '\\' || letters.Contains(c))
                    outstr.Append("\\").Append(((Int16)c).ToString("X4"));
                else
                    outstr.Append(c);
            }
            return outstr.ToString();
        }

        private string DecodeEscapeSequences(string str)
        {
            StringBuilder outstr = new StringBuilder();
            int pos = 0;
            while (pos < str.Length)
            {
                if (str[pos] == '\\')
                {
                    string code = str.Substring(pos + 1, 4);
                    Int16 unicode = Int16.Parse(code, System.Globalization.NumberStyles.HexNumber);
                    outstr.Append((char)unicode);
                    pos += 4;
                }
                else
                    outstr.Append(str[pos]);

                pos++;
            }

            return outstr.ToString();
        }
    }

    [GUIForMachine("Imagine.PythonScript")]
    public class PythonGUI : MachineGUI
    {
        public PythonGUI()
        {
            SetBitmap(Resources.Imagine_PythonScript);
        }

        public PythonMachine MyMachine
        {
            get { return (PythonMachine)Node.Machine; }
        }

        public override void LaunchSettings(GraphArea graphArea)
        {
            ScriptInput scriptInput = new ScriptInput();
            scriptInput.Value = MyMachine.Script;
            scriptInput.Errors = MyMachine.Errors;
            if (scriptInput.ShowDialog() == DialogResult.OK)
            {
                MyMachine.Script = scriptInput.Value;
            }
            scriptInput.Dispose();
        }
    }
}
