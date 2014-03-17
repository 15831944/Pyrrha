﻿#region Referencing

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Exception = System.Exception;

#endregion

namespace Pyrrha.Scripting.Runtime
{
    public class PythonSession : IDisposable
    {
        internal string TempFilePath = string.Format(@"{0}\local\temp\PyrrhaScriptsTempFolder\{1}_{2}.py", 
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            Environment.UserName, 
            DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace('\\', ('_')));

        private Queue<string> _sessionCodeRepo;
        private Queue<string> SessionCodeRepo
        {
            get { return this._sessionCodeRepo ?? (this._sessionCodeRepo = new Queue<string>()); }
        }

        private bool _userCanceled { get; set; }

        public bool EncounterdErrors 
        {
            get { return this.SessionEngine.ErrorListener.FoundError; }
        }

        internal PyrrhaScriptEngine SessionEngine;

        public PythonSession()
        {
            this.SessionEngine = new PyrrhaScriptEngine();
        }

        [CommandMethod("pystart")]
        public void StartPythonScripting()
        {
            try
            {
                this.SourceAccumulate();
                if (!this.EncounterdErrors && !this._userCanceled)
                    this.CopyCodeToFile_RequestSave();

            }
            catch (Exception e)
            {
                this.SessionEngine.LinkedDocument.Editor.WriteMessage(
                        string.Format("\nMessage: {0}\nSource:{1}", e.Message, e.Source));
            }
            this.SessionEngine.Dispose();
        }

        private void SourceAccumulate()
        {
            for (;;)
            {
                var promptOptions = new PromptStringOptions(">>> ") { AllowSpaces = true };

                var response = this.SessionEngine.LinkedDocument.Editor.GetString(promptOptions);

                if ( response.Status != PromptStatus.OK )
                {
                    this.SessionEngine.Dispose();
                    this._userCanceled = true;
                    return;
                }

                if (response.StringResult.Equals("end", StringComparison.InvariantCultureIgnoreCase) || !this._execute(response.StringResult))
                    return;
            }
        }

        private bool _execute(string code)
        {
            var compiledcode = this.SessionEngine.Compile(code);

            if (this.SessionEngine.ErrorListener.FoundError)
            {
                this.ReportErrors();
                return false;
            }

            string scopeKey = null;
            if (code.Contains("="))
                scopeKey = code.Split('=')[0].Replace(" ", string.Empty);

            var scopeObj = this.SessionEngine.Execute(compiledcode);
            this.SessionCodeRepo.Enqueue(code);

            if (scopeKey != null && scopeObj != null)
                this.SessionEngine.CurrentScope.SetVariable(scopeKey, scopeObj);
            return true;
        }

        

        private void CopyCodeToFile_RequestSave()
        {
            var sfd = new SaveFileDialog()
            {
                SupportMultiDottedExtensions = true,
                CreatePrompt = false,
                AddExtension = true,
                DefaultExt = Path.GetFileNameWithoutExtension(this.TempFilePath) + ".py",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var willSave = false;

            if (MessageBox.Show("Would you like to save this script source?", "Save File", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                willSave = true;
                if (sfd.ShowDialog() != DialogResult.OK)
                    willSave = false;
            }

            if (willSave)
                this.TempFilePath = sfd.FileName;

            using (var stream = new FileStream(this.TempFilePath, FileMode.Create))
            using (var writer = new StreamWriter(stream))
                foreach (var line in this.SessionCodeRepo)
                    writer.WriteLine(line);
        }

        //private bool LoadedFromIDE(params string[] code)
        //{
        //    throw new NotImplementedException();
        //}

        //private bool proccessPythonScript(ScriptSource source)
        //{
        //    // TODO Implement checking and loading the default imports statments
        //    return complieAndRunScriptSource(source);
        //}

        public void ExecuteScriptFile(string path)
        {
            var scriptSource = this.SessionEngine.CreateScriptSourceFromFile( path );
            var compiledCode = this.SessionEngine.Compile( scriptSource );
            if ( this.EncounterdErrors )
            {
                this.ReportErrors();
                return;
            }

            this.SessionEngine.Execute( compiledCode );
        }

        private void ReportErrors()
        {
            this.SessionEngine.LinkedDocument.Editor.WriteMessage("****___________  Errors thrown  ___________****\n");
            foreach (var error in this.SessionEngine.ErrorListener.ErrorData)
                this.SessionEngine.LinkedDocument.Editor.WriteMessage(
                    string.Format("{1} Error: {0}\n", error.Message, error.Severity)
                    );

            this.SessionEngine.LinkedDocument.Editor.WriteMessage("****___________  End Errors  ___________****\n");
        }

        #region IDisposable Implementaiton

        private bool _isDisposed;

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || this._isDisposed)
                return;

            this.SessionEngine.Dispose();
            this._isDisposed = true;
        }

        #endregion
    }
}
