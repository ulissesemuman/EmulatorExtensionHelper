using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EmulatorExtensionHelper.ContextActions;
using static EmulatorExtensionHelper.frmEmulatorSelector;

namespace EmulatorExtensionHelper
{
    internal static class ContextActions
    {
        private static LanguageManager lang = new LanguageManager();
        private static FormExecutionModes formExecutionMode = FormExecutionModes.Run;
        private static string fileName = string.Empty;

        public static FormExecutionModes FormExecutionMode
        {
            get => formExecutionMode;
            set => formExecutionMode = value;
        }

        private class ParsedArgs
        {
            public ExecutionMode ActionType { get; set; } = ExecutionMode.Unknown;
            public string FileName { get; set; } = "";
            public string Extension { get; set; } = "";
        }

        public enum FormExecutionModes
        {
            Run,
            ShowDialog
        }

        public static void Execute(string[] args)
        {
            var parsed = ParseArguments(args);

            fileName = parsed.FileName;

            var actionType = parsed.ActionType; 

            if (actionType == ExecutionMode.Unknown)
            {
                MessageBox.Show(lang.T("ContextActions.InvalidExecutionMode"), lang.T("Common.Error"));
                return;
            }

            ShowForm(parsed.ActionType);
        }

        private static void HandleExecute()
        {
            ShowForm(ExecutionMode.ExecuteEmulator);
        }

        private static void HandleAssociateFileName()
        {
            ShowForm(ExecutionMode.AssociateFileName);
        }

        private static void HandleAssociateExtension()
        {
            ShowForm(ExecutionMode.AssociateExtension);
        }

        private static void HandleDisassociateFileName()
        {
            ShowForm(ExecutionMode.DisassociateFileName);
        }

        private static void HandleDisassociateExtension()
        {
            ShowForm(ExecutionMode.DisassociateExtension);
        }

        private static ParsedArgs ParseArguments(string[] args)
        {
            var parsed = new ParsedArgs();

            foreach (var arg in args)
            {
                if (arg.StartsWith("--action=", StringComparison.OrdinalIgnoreCase))
                {
                    string mode = arg.Substring("--action=".Length).ToLower();
                    parsed.ActionType = mode switch
                    {
                        "execute" => ExecutionMode.ExecuteEmulator,
                        "associatefilename" => ExecutionMode.AssociateFileName,
                        "associateextension" => ExecutionMode.AssociateExtension,
                        "disassociatefilename" => ExecutionMode.DisassociateFileName,
                        "disassociateextension" => ExecutionMode.DisassociateExtension,
                        _ => ExecutionMode.Unknown
                    };
                }
                else if (arg.StartsWith("--file=", StringComparison.OrdinalIgnoreCase))
                {
                    parsed.FileName = arg.Substring("--file=".Length).Trim().ToLower();
                }

            }

            return parsed;
        }

        private static void ShowForm(ExecutionMode actionType)
        {
            switch (FormExecutionMode)
            {
                case FormExecutionModes.Run:
                    Application.Run(new frmEmulatorSelector(fileName, actionType));
                    break;
                case FormExecutionModes.ShowDialog:
                    using (var form = new frmEmulatorSelector(fileName, actionType))
                    {
                        form.ShowDialog();
                    }
                    break;
                default:
                    MessageBox.Show(lang.T("ContextActions.InvalidExecutionMode"), lang.T("Common.Error"));
                    break;
            }
        }
    }
}

