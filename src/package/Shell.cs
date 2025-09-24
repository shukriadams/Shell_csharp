namespace MadScience_Shell
{
    public class MadScience_Shell
    {
        public ShellResult Run(string command)
        {
            Process cmd = new Process();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                cmd.StartInfo.FileName = "sh";
                cmd.StartInfo.Arguments = $"-c \"{command}\"";
            }
            else
            {
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.Arguments = $"/k {command}";
            }

            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;

            List<string> stdOut = new List<string>();
            List<string> stdErr = new List<string>();
            
            int timeout = 50000;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    cmd.OutputDataReceived += (sender, e) =>
                    {
                        try
                        {
                            if (e.Data == null)
                                outputWaitHandle.Set();
                            else
                                stdOut.Add(e.Data);
                        }
                        catch (Exception ex)
                        {
                            stdErr.Add(e.ToString());
                        }
                    };

                    cmd.ErrorDataReceived += (sender, e) =>
                    {
                        try
                        {
                            if (e.Data == null)
                                errorWaitHandle.Set();
                            else
                                stdErr.Add(e.Data);
                        }
                        catch (Exception ex)
                        {
                            stdErr.Add(ex.ToString());
                        }
                    };

                    cmd.Start();
                    cmd.BeginOutputReadLine();
                    cmd.BeginErrorReadLine();

                    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || (cmd.WaitForExit(timeout) && outputWaitHandle.WaitOne(timeout) && errorWaitHandle.WaitOne(timeout)))
                        return new ShellResult
                        {
                            StdOut = stdOut,
                            StdErr = stdErr,
                            ExitCode = cmd.ExitCode
                        };
                    else
                        throw new Exception($"Timed out on command : {command} after {timeout} ms");
                }
            }
            else
            {
                cmd.Start();
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();


                while (!cmd.StandardOutput.EndOfStream)
                {
                    string line = cmd.StandardOutput.ReadLine();
                    stdOut.Add(line);
                }

                while (!cmd.StandardError.EndOfStream)
                {
                    string line = cmd.StandardError.ReadLine();
                    stdErr.Add(line);
                }

                return new ShellResult
                {
                    StdOut = stdOut,
                    StdErr = stdErr,
                    ExitCode = cmd.ExitCode
                };
            }
        }
    }
}