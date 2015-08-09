using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Arkansalt.DevConsole
{
    public class ProgramHelper
    {
        public static void MainHelper()
        {
            // setup console
            Console.WindowHeight = 40;
            Console.WindowWidth = 100;


            // setup screen
            ProgramHelper.DrawHeader();

            // setup menu
            ConsoleMenu menu = new ConsoleMenu(
                "Tests Menu"
                , 3, 4
                , ConsoleColor.Yellow, Console.BackgroundColor
                , ConsoleColor.Black, ConsoleColor.White
                )
            {
                ItemActionInvoker = ProgramHelper.RunMenuItem
            };


            menu.AddMenuItem("Read System Up Time", ProgramHelper.ReadSystemUpTime);
            
            menu.AddSeperatorItem();
            
            ConsoleMenu.MenuFolder driveFolder = menu.AddFolder("Google Drive");
            menu.AddSeperatorItem().Folder = driveFolder;
            menu.AddMenuItem("Connect to Google Drive",
                () => ProgramHelper.RunTest("Google Drive Connect", (new GoogleDriveTests()).ConnectToDrive)).Folder = driveFolder;
            menu.AddMenuItem("List Service Act. File Titles",
                () => ProgramHelper.RunTest("List Service Account File Titles", (new GoogleDriveTests()).ListServiceAccountFiles)).Folder = driveFolder;
            menu.AddMenuItem("List User Act. File Titles",
                () => ProgramHelper.RunTest("List User Account File Titles", (new GoogleDriveTests()).ListUserAccountFiles)).Folder = driveFolder;
            menu.AddSeperatorItem().Folder = driveFolder;
            menu.AddMenuItem("Create Service Act. Public Folder",
                () => ProgramHelper.RunTest("Create Public Folder - Service Account", (new GoogleDriveTests()).CreateServiceAccountPublicFolder)).Folder = driveFolder;
            menu.AddMenuItem("Create User Act. Public Folder",
                () => ProgramHelper.RunTest("Create Public Folder - User Account", (new GoogleDriveTests()).CreateUserAccountPublicFolder)).Folder = driveFolder;
            
            menu.AddSeperatorItem();

            ConsoleMenu.MenuFolder gmailFolder = menu.AddFolder("Gmail");
            menu.AddSeperatorItem().Folder = gmailFolder;
            menu.AddMenuItem("List Email Subjects",
                () => ProgramHelper.RunTest("List Gmail Email Subjects", (new GoogleGmailTests()).ListUserAccountSubjects)).Folder = gmailFolder;
            menu.AddMenuItem("List Email Subjects (SA)",
                            () => ProgramHelper.RunTest("List Gmail Service Account Subjects", (new GoogleGmailTests()).ListServiceAccountSubjects)).Folder = gmailFolder;

            menu.AddSeperatorItem();
            menu.AddMenuItem("Restart", () =>
            {
                ProgramHelper.MainHelper();
                menu.ExitMenu();
            });
            menu.AddSeperatorItem();
            menu.AddMenuItem("Exit", menu.ExitMenu);

            menu.RedrawScreen = () =>
            {
                Console.Clear();
                ProgramHelper.DrawHeader();
            };

            menu.RunMenu();


            // exit
            ProgramHelper.DrawHeader();
            Console.SetCursorPosition(2, Console.WindowHeight - 3);
            Console.Write("Press a key to exit.");
            Console.ReadKey(true);
        }


        private static void DrawHeader()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            string title = string.Format("{0} - Dev Console", Core.AppInfo.Name);
            string underline = string.Empty;
            title.ToList().ForEach(c => underline += "~");

            Console.SetCursorPosition(1, 1);
            Console.Write(title);
            Console.SetCursorPosition(1, 2);
            Console.Write(underline);
        }

        private static void RunMenuItem(ConsoleMenu.MenuItemAction action, ConsoleMenu menu)
        {
            ProgramHelper.DrawHeader();
            action.Invoke();
            ProgramHelper.DrawHeader();
            menu.DrawMenu();
        }


        #region test handling

        private static void RunTest(string testName, Action<ConsoleFunctionOutput> action)
        {
            int cursorRow = 4;
            int cursorCol = 3;

            // write header
            Console.SetCursorPosition(cursorCol, cursorRow);
            Console.Write(testName);
            cursorRow++;
            Console.SetCursorPosition(cursorCol, cursorRow);
            for (int i = 0; i < testName.Length; i++)
                Console.Write("~");

            // wait message...
            cursorRow += 2;
            Console.SetCursorPosition(cursorCol, cursorRow);
            Console.Write("Executing...");

            // output prep
            ConsoleFunctionOutput output = new ConsoleFunctionOutput(OutputReadyEventHandler, GetInputEventHandler);
            ProgramHelper.CurrentCursorCol = cursorCol;
            ProgramHelper.CurrentCursorRow = cursorRow;

            // invoke action
            try
            {
                action(output);
            }
            catch (Exception ex)
            {
                cursorRow = ProgramHelper.CurrentCursorRow;

                string msg = string.Format("Error: {0}", ex.Message);
                
                cursorRow += 2;
                Console.SetCursorPosition(cursorCol, cursorRow);
                Console.Write(msg);

                // count rows
                int newLineCount = msg.Split('\n').Length;
                cursorRow += newLineCount;

                System.Windows.Forms.Clipboard.SetText(ex.Message);
                cursorRow += 2;
                Console.SetCursorPosition(cursorCol, cursorRow);
                Console.Write("(Error message copied to clipboard.)");

                ProgramHelper.CurrentCursorCol = cursorCol;
                ProgramHelper.CurrentCursorRow = cursorRow;
            }

            // post output
            cursorCol = ProgramHelper.CurrentCursorCol;
            cursorRow = ProgramHelper.CurrentCursorRow;

            // exit
            cursorRow += 3;
            Console.SetCursorPosition(cursorCol, cursorRow);
            Console.Write("Press a key to exit.");
            Console.ReadKey(true);
        }

        private static int CurrentCursorCol { get; set; }
        private static int CurrentCursorRow { get; set; }

        private static void OutputReadyEventHandler(object sender, ConsoleFunctionOutputEventArgs args)
        {
            ProgramHelper.CurrentCursorRow++;

            int cursorCol = ProgramHelper.CurrentCursorCol;
            
            int cursorMax = Console.WindowWidth;
            int textMax = cursorMax - cursorCol - 2;
            
            if (args.IsDataRow)
            {
                // row = spaces + string.Format("{0,2}", rowNum) + ".  " + row;
                int rowNum = args.DataRowNum;
                string rowNumText = string.Format("{0,2}.  ", rowNum);

                Console.SetCursorPosition(cursorCol, ProgramHelper.CurrentCursorRow);
                Console.Write(rowNumText);

                cursorCol += rowNumText.Length;

                textMax -= rowNumText.Length;
            }

            string writeText = args.Output;

            if (writeText.Length > textMax)
            {
                writeText = writeText.Substring(0, textMax - 3);
                writeText += "...";
            }

            Console.SetCursorPosition(cursorCol, ProgramHelper.CurrentCursorRow);
            Console.Write(writeText);
        }

        private static void GetInputEventHandler(object sender, ConsoleFunctionOutputGetInputEventArgs args)
        {
            if (args.PrePromptText != null)
            {
                ProgramHelper.CurrentCursorRow++;
                Console.SetCursorPosition(ProgramHelper.CurrentCursorCol, ProgramHelper.CurrentCursorRow);
                Console.Write(args.PrePromptText);
            }
            
            ProgramHelper.CurrentCursorRow++;
            Console.SetCursorPosition(ProgramHelper.CurrentCursorCol, ProgramHelper.CurrentCursorRow);
            Console.Write(args.PromptText);
            Console.CursorVisible = true;
            string userInput = Console.ReadLine();
            Console.CursorVisible = false;
            args.UserInput = userInput;
        }

        #endregion
        
        #region local tests

        private static void ReadSystemUpTime()
        {
            int cursorRow = 4;
            int cursorCol = 3;

            Console.SetCursorPosition(cursorCol, cursorRow);
            Console.Write("System Up Time");

            cursorRow++;
            Console.SetCursorPosition(cursorCol, cursorRow);
            Console.Write("~~~~~~~~~~~~~~");

            string cmdFilename = @"C:\Public\PsTools\PsInfo.exe";
            string cmdDir = @"C:\Public\PsTools\";
            string cmdArgs = @"-c uptime";
            //string cmdArgs = @"";

            string cmdOutput = null;
            string cmdErrorOutput = null;
            bool isCmdError = false;

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = cmdFilename,
                WorkingDirectory = cmdDir,
                Arguments = cmdArgs,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using (Process p = new Process())
            {
                p.StartInfo = startInfo;

                p.OutputDataReceived += delegate(object sender, DataReceivedEventArgs args)
                {
                    cmdOutput = args.Data;
                };

                p.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs args)
                {
                    cmdErrorOutput = args.Data;
                    isCmdError = false;
                };

                cursorRow += 2;
                Console.SetCursorPosition(cursorCol, cursorRow);
                Console.Write("Executing...");

                // run!
                p.Start();

                // get output
                cmdOutput = p.StandardOutput.ReadToEnd();

                // do closexit
                p.WaitForExit();
                p.Close();
            }

            if (isCmdError)
            {
                cursorRow++;
                Console.SetCursorPosition(cursorCol, cursorRow);
                Console.Write("Execution errors: ");

                cursorRow++;
                Console.SetCursorPosition(cursorCol, cursorRow);
                Console.Write(cmdErrorOutput);
            }

            cursorRow += 2;
            Console.SetCursorPosition(cursorCol, cursorRow);
            Console.Write("Output:");

            string spaces = string.Empty;
            for (int i = 0; i < cursorCol; i++)
            {
                spaces += " ";
            }

            cmdOutput = cmdOutput.Replace("\n", "\n" + spaces);

            cursorRow += 2;
            Console.SetCursorPosition(cursorCol, cursorRow);
            Console.Write(cmdOutput);

            // factor output in cursor row
            if (!string.IsNullOrWhiteSpace(cmdOutput))
            {
                int newLineCount = cmdOutput.Split('\n').Length;
                cursorRow += newLineCount;
            }


            string[] outputData = cmdOutput.Split(',');
            if (outputData.Length > 1)
            {
                // remove computer name
                string[] uptimeData = outputData[1].Split(' ');

                // "0 days 1 hour 50 minutes 6 seconds"
                if (uptimeData.Length == 8)
                {
                    List<string> uptimesList = new List<string>();
                    uptimesList.Add(uptimeData[0] + " " + uptimeData[1]);
                    uptimesList.Add(uptimeData[2] + " " + uptimeData[3]);
                    uptimesList.Add(uptimeData[4] + " " + uptimeData[5]);
                    uptimesList.Add(uptimeData[6] + " " + uptimeData[7]);

                    string[] uptimes = uptimesList.ToArray();


                    // begin-debug
                    cursorRow += 2;
                    Console.SetCursorPosition(cursorCol, cursorRow);
                    Console.Write("Parsed: ");
                    cursorRow++;
                    //
                    foreach (string ut in uptimes)
                    {
                        cursorRow++;
                        Console.SetCursorPosition(cursorCol, cursorRow);
                        Console.Write(ut);
                    }
                    // end-debug


                    int days = int.Parse(uptimeData[0]);
                    int hours = int.Parse(uptimeData[2]);
                    int minutes = int.Parse(uptimeData[4]);
                    int seconds = int.Parse(uptimeData[6]);

                    TimeSpan uptime = new TimeSpan(days, hours, minutes, seconds);



                    // begin-debug
                    cursorRow += 2;
                    Console.SetCursorPosition(cursorCol, cursorRow);
                    Console.Write("Timespan: {0}", uptime.ToString(@"dd\ hh\ mm\ ss"));
                    cursorRow++;
                    // end-debug
                }
            }

            // exit
            cursorRow += 3;
            Console.SetCursorPosition(cursorCol, cursorRow);
            Console.Write("Press a key to exit.");
            Console.ReadKey(true);
        }

        #endregion

        #region ouput

        private static ConsoleOutput PrepareOutput(string output, int cursorCol, int cursorRow
            , bool addRowNumbers = false, int dataStartRow = 0, int dataEndRow = 0)
        {
            if (!string.IsNullOrWhiteSpace(output))
            {
                // move each row beginning to cursor col
                string spaces = string.Empty;
                for (int i = 0; i < cursorCol; i++)
                {
                    spaces += " ";
                }
                output = output.Replace("\n", "\n" + spaces);

                // count rows
                int newLineCount = output.Split('\n').Length;
                cursorRow += newLineCount;

                // add row numbers
                if (addRowNumbers && dataStartRow != 0)
                {
                    StringBuilder newOutput = new StringBuilder();
                    using (StringReader reader = new StringReader(output))
                    {
                        int rowCount = 0;
                        int rowNum = 0;

                        string row = reader.ReadLine();
                        while (row != null)
                        {
                            rowCount++;
                            if (rowCount >= dataStartRow)
                            {
                                if (dataEndRow == 0 || dataEndRow > rowCount)
                                {
                                    rowNum++;
                                    row = spaces + string.Format("{0,2}", rowNum) + ".  " + row;
                                }
                            }
                            newOutput.AppendLine(row);

                            row = reader.ReadLine();
                        }

                        reader.Close();
                    }
                    output = newOutput.ToString();
                }
            }

            ConsoleOutput rv = new ConsoleOutput(cursorRow, cursorCol, output);
            return rv;
        }

        private class ConsoleOutput
        {
            public ConsoleOutput(int cursorRow, int cursorCol, string output)
            {
                this.CursorRow = cursorRow;
                this.CursorCol = cursorCol;
                this.Output = output;
            }


            public int CursorRow { get; private set; }

            public int CursorCol { get; private set; }

            public string Output { get; private set; }

        }

        #endregion

    }
}
