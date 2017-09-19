namespace SimpleApduSender
{
    public enum SCardShareModes
    {
        // This application will NOT allow others to share the reader. (SCARD_SHARE_EXCLUSIVE)
        Exclusive = 0x0001,

        // This application will allow others to share the reader. (SCARD_SHARE_SHARED)
        Shared = 0x0002,

        // Direct control of the reader, even without a card. (SCARD_SHARE_DIRECT)
        Direct = 0x0003
    }
}
