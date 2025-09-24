    /// <summary>
    /// Result of shell command.
    /// </summary>
    public class ShellResult
    {
        /// <summary>
        /// Normally 0 when command succeeds and something else when it fails, but Perforce can return warnings or even info as errors.
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// Standard output, per line.
        /// </summary>
        public IEnumerable<string> StdOut { get; set; }

        /// <summary>
        /// Error output, per line.
        /// </summary>
        public IEnumerable<string> StdErr { get; set; }

        public ShellResult()
        {
            this.StdOut = new string[] { };
            this.StdErr = new string[] { };
        }
    }