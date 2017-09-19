namespace SimpleApduSender
{
    public class ApduScript
    {
        public string Input { get; private set; }
        public string ExpectedOutput { get; private set; }
        public bool IsReset { get; private set; }

        public ApduScript(
            bool isReset)
        {
            IsReset = isReset;
        }

        public ApduScript(
            string input,
            string output) : this(false)
        {
            Input = input;
            ExpectedOutput = output;
        }
    }
}
