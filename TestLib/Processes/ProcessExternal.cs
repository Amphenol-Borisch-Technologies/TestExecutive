using ABT.Test.TestExecutive.TestLib.Configuration;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABT.Test.TestExecutive.TestLib.Processes {
    public enum PROCESS_METHOD { ExitCode, Redirect }

    public static class ProcessExternal {
        [DllImport("kernel32.dll")] private static extern Boolean GetConsoleMode(IntPtr hConsoleHandle, out UInt32 lpMode);
        [DllImport("kernel32.dll")] private static extern Boolean SetConsoleMode(IntPtr hConsoleHandle, UInt32 dwMode);
        [DllImport("kernel32.dll")] private static extern IntPtr GetStdHandle(Int32 nStdHandle);
        private const Int32 STD_INPUT_HANDLE = -10;

        public static void Connect(String Description, String Connector, Action PreConnect, Action PostConnect, Boolean AutoContinue = false) {
            PreConnect?.Invoke();
            String message = $"UUT unpowered.{Environment.NewLine}{Environment.NewLine}" +
                             $"Connect '{Description}' to UUT '{Connector}'.{Environment.NewLine}{Environment.NewLine}" +
                             $"AFTER connecting, click OK to continue.";
            if (AutoContinue) _ = MessageBox.Show(FormInterconnectGet(), message, $"Connect '{Connector}'", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            else _ = MessageBox.Show(message, $"Connect '{Connector}'", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            PostConnect?.Invoke();
        }

        public static void DisConnect(String Description, String Connector, Action PreDisconnect, Action PostDisconnect, Boolean AutoContinue = false) {
            PreDisconnect?.Invoke();
            String message = $"UUT unpowered.{Environment.NewLine}{Environment.NewLine}" +
                             $"Disconnect '{Description}' from UUT '{Connector}'.{Environment.NewLine}{Environment.NewLine}" +
                             $"AFTER disconnecting, click OK to continue.";
            if (AutoContinue) _ = MessageBox.Show(FormInterconnectGet(), message, $"Disconnect '{Connector}'", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            else _ = MessageBox.Show(message, $"Disconnect '{Connector}'", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            if (!AutoContinue) _ = MessageBox.Show(message, $"Disconnect '{Connector}'", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            PostDisconnect?.Invoke();
        }

        public static String ExitCode(MethodProcess methodProcess) { return ProcessExitCode(methodProcess.Parameters, methodProcess.File, methodProcess.Folder); }

        private static Form FormInterconnectGet() {
            Form form = new Form() { Size = new Size(0, 0) };
            Task.Delay(TimeSpan.FromSeconds(1.0)).ContinueWith((t) => form.Close(), TaskScheduler.FromCurrentSynchronizationContext());
            return form;
        }

        public static String ProcessExitCode(String arguments, String fileName, String workingDirectory) {
            Int32 exitCode = -1;
            using (Process process = new Process()) {
                ProcessStartInfo psi = new ProcessStartInfo {
                    Arguments = arguments,
                    FileName = workingDirectory + @"\" + fileName,
                    WorkingDirectory = workingDirectory,
                    CreateNoWindow = false,
                    WindowStyle = ProcessWindowStyle.Maximized,
                    UseShellExecute = false,
                    RedirectStandardError = false,
                    RedirectStandardOutput = false
                };
                process.StartInfo = psi;
                process.Start();
                DisableQuickEdit(GetStdHandle(STD_INPUT_HANDLE));
                process.WaitForExit();
                exitCode = process.ExitCode;
            }
            return exitCode.ToString();
        }

        public static (String StandardError, String StandardOutput, Int32 ExitCode) ProcessTee(String arguments, String fileName, String workingDirectory) {
            StringBuilder standardOutput = new StringBuilder();
            StringBuilder standardError = new StringBuilder();
            Int32 exitCode = -1;

            using (Process process = new Process()) {
                ProcessStartInfo processStartInfo = new ProcessStartInfo {
                    FileName = $"\"{workingDirectory}\\{fileName}\"",
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                };

                process.StartInfo = processStartInfo;
                process.EnableRaisingEvents = true;
                DisableQuickEdit(GetStdHandle(STD_INPUT_HANDLE));
                process.OutputDataReceived += (_, e) => {
                    if (e.Data is null) return;
                    Console.Out.WriteLine(e.Data);
                    standardOutput.AppendLine(e.Data);
                };
                process.ErrorDataReceived += (_, e) => {
                    if (e.Data is null) return;
                    Console.Error.WriteLine(e.Data);
                    standardError.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                exitCode = process.ExitCode;
            }
            return (standardError.ToString(), standardOutput.ToString(), exitCode);
        }

        public static (String StandardError, String StandardOutput, Int32 ExitCode) ProcessRedirect(String arguments, String fileName, String workingDirectory) {
            String standardError, standardOutput;
            Int32 exitCode = -1;
            using (Process process = new Process()) {
                ProcessStartInfo psi = new ProcessStartInfo {
                    Arguments = arguments,
                    FileName = workingDirectory + @"\" + fileName,
                    WorkingDirectory = workingDirectory,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };
                process.StartInfo = psi;
                process.Start();
                DisableQuickEdit(GetStdHandle(STD_INPUT_HANDLE));
                process.WaitForExit();
                StreamReader se = process.StandardError;
                standardError = se.ReadToEnd();
                StreamReader so = process.StandardOutput;
                standardOutput = so.ReadToEnd();
                exitCode = process.ExitCode;
            }
            return (standardError, standardOutput, exitCode);
        }

        public static (String StandardError, String StandardOutput, Int32 ExitCode) Redirect(MethodProcess methodProcess) { return ProcessRedirect(methodProcess.Parameters, methodProcess.File, methodProcess.Folder); }

        private static void DisableQuickEdit(IntPtr processHandle) {
            // https://stackoverflow.com/questions/13656846/how-to-programmatic-disable-c-sharp-console-applications-quick-edit-mode
            GetConsoleMode(processHandle, out UInt32 consoleMode);
            consoleMode &= ~0x0040U; // Clear the ENABLE_QUICK_EDIT_MODE bit.
            consoleMode |= 0x0080U;  // Set the ENABLE_EXTENDED_FLAGS bit.
            SetConsoleMode(processHandle, consoleMode);
        }
    }
}
