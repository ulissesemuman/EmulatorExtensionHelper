using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EmulatorExtensionHelper.ContextActions;

namespace EmulatorExtensionHelper
{
    internal static class ContextActions
    {
        private static LanguageManager lang = new LanguageManager();

        private class ParsedArgs
        {
            public ExecutionMode Mode { get; set; } = ExecutionMode.Unknown;
            public string FileName { get; set; } = "";
            public string Extension { get; set; } = "";
        }

        public enum ExecutionMode
        {
            ExecuteEmulator,
            AssociateFileName,
            AssociateExtension,
            DisassociateFileName,
            DisassociateExtension,
            Unknown
        }

        public static void Execute(string[] args)
        {
            var parsed = ParseArguments(args);
            switch (parsed.Mode)
            {
                case ExecutionMode.ExecuteEmulator:
                    HandleExecute(parsed.FileName);
                    break;

                case ExecutionMode.AssociateFileName:
                    HandleAssociateFileName(parsed.FileName);
                    break;

                case ExecutionMode.AssociateExtension:
                    HandleAssociateExtension(parsed.FileName);
                    break;

                case ExecutionMode.DisassociateFileName:
                    HandleDisassociateFileName(parsed.FileName);
                    break;

                case ExecutionMode.DisassociateExtension:
                    HandleDisassociateExtension(parsed.FileName);
                    break;

                case ExecutionMode.Unknown:
                default:
                    MessageBox.Show(lang.T("ContextActions.InvalidExecutionMode"), lang.T("Common.Error"));
                    break;
            }
        }

        private static void HandleExecute(string fileName)
        {
            Application.Run(new frmEmulatorSelector(fileName, frmEmulatorSelector.ActionType.ExecuteEmulator));
        }

        private static void HandleAssociateFileName(string fileName)
        {
             Application.Run(new frmEmulatorSelector(fileName, frmEmulatorSelector.ActionType.AssociateFileName));
        }

        private static void HandleAssociateExtension(string fileName)
        {
            Application.Run(new frmEmulatorSelector(fileName, frmEmulatorSelector.ActionType.AssociateExtension));
        }

        private static void HandleDisassociateFileName(string fileName)
        {
            Application.Run(new frmEmulatorSelector(fileName, frmEmulatorSelector.ActionType.DisassociateFileName));
        }

        private static void HandleDisassociateExtension(string fileName)
        {
            Application.Run(new frmEmulatorSelector(fileName, frmEmulatorSelector.ActionType.DisassociateExtension));
        }

        private static ParsedArgs ParseArguments(string[] args)
        {
            var parsed = new ParsedArgs();

            foreach (var arg in args)
            {
                if (arg.StartsWith("--action=", StringComparison.OrdinalIgnoreCase))
                {
                    string mode = arg.Substring("--action=".Length).ToLower();
                    parsed.Mode = mode switch
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
    }
}

