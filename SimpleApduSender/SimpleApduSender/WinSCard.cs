using System;
using System.Runtime.InteropServices;

namespace SimpleApduSender
{
    //IO Request Control
    public struct SCARD_IO_REQUEST
    {
        public int dwProtocol;
        public int cbPciLength;
    }

    //Reader State
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SCARD_READERSTATE
    {
        public string RdrName;
        public string UserData;
        public uint RdrCurrState;
        public uint RdrEventState;
        public uint ATRLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x24, ArraySubType = UnmanagedType.U1)]
        public byte[] ATRValue;
    }

    public class WinSCard
    {
        [DllImport("winscard.dll")]
        public static extern int SCardEstablishContext(
            uint dwScope,
            IntPtr notUsed1,
            IntPtr notUsed2,
            out IntPtr phContext);

        [DllImport("winscard.dll")]
        public static extern int SCardReleaseContext(
            IntPtr phContext);

        [DllImport("winscard.dll")]
        public static extern int SCardConnect(
            IntPtr hContext,
            string cReaderName,
            uint dwShareMode,
            uint dwPrefProtocol,
            ref IntPtr hCard,
            ref IntPtr ActiveProtocol);

        [DllImport("winscard.dll")]
        public static extern int SCardDisconnect(
            IntPtr hCard, int Disposition);

        [DllImport("winscard.dll", EntryPoint = "SCardListReadersA", CharSet = CharSet.Ansi)]
        public static extern int SCardListReaders(
            IntPtr hContext,
            byte[] mszGroups,
            byte[] mszReaders,
            ref UInt32 pcchReaders);

        [DllImport("winscard.dll")]
        public static extern int SCardState(
            IntPtr hCard, 
            ref IntPtr state, 
            ref IntPtr protocol, 
            ref Byte[] ATR, 
            ref int ATRLen);

        [DllImport("winscard.dll")]
        public static extern int SCardTransmit(
            IntPtr hCard, 
            ref SCARD_IO_REQUEST pioSendRequest,
            Byte[] SendBuff,
            int SendBuffLen,
            ref SCARD_IO_REQUEST pioRecvRequest,
            Byte[] RecvBuff, ref int RecvBuffLen);

        [DllImport("winscard.dll", CharSet = CharSet.Unicode)]
        public static extern int SCardGetStatusChange(
            IntPtr hContext,
            int value_Timeout,
            ref SCARD_READERSTATE ReaderState,
            uint ReaderCount);
    }
}
