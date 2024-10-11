using DevTools.MVVM.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DevTools.Services.Solution
{
    public class SolutionBuilder : ISolutionBuilder
    {
        private readonly SettingsModel _settings;
        private string commandPrefix = @"dotnet new sln --name ";

        public SolutionBuilder(
            //Settings
            IOptions<SettingsModel> settings
            ) 
        {
            _settings = settings.Value;
        }
        /// <summary>
        /// Permette di creare la soluzione con percorso C:\DevSln, creato automaticamente, con nome DevSln$.sln ($ = progressivo numerico)
        /// </summary>
        /// <param name="devToolModel">Modello con i parametri da usare per la creazione della soluzione</param>
        public void BuildSolution(DevToolModel devToolModel)
        {
            var cmdOut = string.Empty;
            string? createSlnCmd;
            string? busStartName;
            try
            {
                CreateDirAndFileName(devToolModel);
                createSlnCmd = commandPrefix + devToolModel.SolutionName;
                if (devToolModel.isExpirience())
                {
                    busStartName = "BUSINESS";
                } else
                {
                    busStartName = "BUSCUBE";
                }
                var oInfo = new ProcessStartInfo("cmd")
                {
                    WorkingDirectory = _settings.SolutionDir,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true
                };

                var p = new Process { StartInfo = oInfo };
                p.Start();

                using (StreamWriter sw = p.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        // Issue the commands
                        sw.WriteLine(createSlnCmd);
                        var vbProj = $@"C:\biz2017{devToolModel.BusinessCube!.Version.ToString()}\{busStartName}\{busStartName}.vbproj";
                        sw.WriteLine($@"dotnet sln {devToolModel.SolutionName}.sln add --in-root {vbProj}");
                        if (devToolModel.SetCfgDebugger && File.Exists($"{vbProj}.user"))
                        {
                            string text = File.ReadAllText($"{vbProj}.user");
                            text = Regex.Replace(text, @"<StartArguments>(.*)+", $"<StartArguments>. . . {devToolModel.CfgConfiguration!.CfgName}</StartArguments>");
                            File.WriteAllText($"{vbProj}.user", text);
                        }

                        foreach (string line in devToolModel.SolutionObject!.ProjectList!)
                        {
                            sw.WriteLine($@"dotnet sln {devToolModel.SolutionName}.sln add --in-root C:\biz2017{devToolModel.BusinessCube!.Version.ToString()}\{line}\{line}.vbproj");
                        }

                        // Keep console open for debugging
                        sw.WriteLine("pause");
                    }
                }

                p.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        cmdOut += $"{args.Data}\n";
                };

                p.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                        cmdOut += $"ERROR: {args.Data}\n";
                };

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    MessageBox.Show($"Process failed with exit code: {p.ExitCode}\n{cmdOut}");
                }

                if (!string.IsNullOrEmpty(_settings.VsPath))
                {
                    Process.Start(_settings.VsPath, _settings.SolutionDir + @"\" + devToolModel.SolutionName + ".sln");
                }
                else
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Path.Combine(_settings.SolutionDir, devToolModel.SolutionName + ".sln"),
                        UseShellExecute = true // used to run the default application
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CreateDirAndFileName(DevToolModel devToolModel)
        {
            //Se non c'è il percorso file lo creo, determino anche il nome
            Directory.CreateDirectory(_settings.SolutionDir);
            if (File.Exists(_settings.SolutionDir + @"\DevSln.sln"))
            {
                var i = 1;
                var bLoop = false;
                do
                {
                    if (File.Exists(_settings.SolutionDir + $@"\DevSln{i.ToString()}.sln"))
                    {
                        i++;
                    }
                    else
                    {
                        devToolModel.SolutionName = $"DevSln{i.ToString()}";
                        bLoop = true;
                    }
                }
                while (bLoop == false);
            }
            else { devToolModel.SolutionName = "DevSln"; }
        }
    }
}
