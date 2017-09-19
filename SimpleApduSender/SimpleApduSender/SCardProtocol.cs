namespace SimpleApduSender
{
    public enum SCardProtocol
    {
        // Protocol not defined.
        Unset = 0x0000,

        // T=0 active protocol
        T0 = 0x0001,

        // <summary>T=1 active protocol.
        T1 = 0x0002,

        // Raw active protocol. Use with memory type cards.
        Raw = 0x0004,

        // T=15 protocol.
        T15 = 0x0008,

        // IFD (Interface device) determines protocol.
        Any = (T0 | T1)
    }
}
