using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleApduSender
{
    public class SCardWrapper
    {
        private IntPtr _context;
        private IntPtr _cardHandle;

        private uint _scope;
        private uint _shareMode;
        private uint _protocol;
        IntPtr activeProtocol = IntPtr.Zero;

        public int LastError { get; private set; }
        public string LastErrorString { get; private set; }

        public SCardWrapper(
            SCardScopes scope,
            SCardShareModes shareMode,
            SCardProtocol protocol)
        {
            _scope = (uint)scope;
            _shareMode = (uint)shareMode;
            _protocol = (uint)protocol;
        }

        private bool IsSuccess(int retval)
        {
            LastErrorString = ReturnValueInString((SCardReturnValues)retval);

            if ((SCardReturnValues)LastError == SCardReturnValues.SCARD_S_SUCCESS)
                return true;
            else
                return false;
        }

        private string ReturnValueInString(SCardReturnValues ReturnValue)
        {
            string sRet = "";

            switch (ReturnValue)
            {
                case SCardReturnValues.ERROR_BROKEN_PIPE:
                    sRet = "The client attempted a smart card operation in a remote session, such as a client session running on a terminal server, and the operating system in use does not support smart card redirection.";
                    break;
                case SCardReturnValues.SCARD_E_BAD_SEEK:
                    sRet = "An error occurred in setting the smart card file object pointer.";
                    break;
                case SCardReturnValues.SCARD_E_CANCELLED:
                    sRet = "The action was canceled by an SCardCancel request.";
                    break;
                case SCardReturnValues.SCARD_E_CANT_DISPOSE:
                    sRet = "The system could not dispose of the media in the requested manner.";
                    break;
                case SCardReturnValues.SCARD_E_CARD_UNSUPPORTED:
                    sRet = "The smart card does not meet minimal requirements for support.";
                    break;
                case SCardReturnValues.SCARD_E_CERTIFICATE_UNAVAILABLE:
                    sRet = "The requested certificate could not be obtained.";
                    break;
                case SCardReturnValues.SCARD_E_COMM_DATA_LOST:
                    sRet = "A communications error with the smart card has been detected.";
                    break;
                case SCardReturnValues.SCARD_E_DIR_NOT_FOUND:
                    sRet = "The specified directory does not exist in the smart card.";
                    break;
                case SCardReturnValues.SCARD_E_DUPLICATE_READER:
                    sRet = "The reader driver did not produce a unique reader name.";
                    break;
                case SCardReturnValues.SCARD_E_FILE_NOT_FOUND:
                    sRet = "The specified file does not exist in the smart card.";
                    break;
                case SCardReturnValues.SCARD_E_ICC_CREATEORDER:
                    sRet = "The requested order of object creation is not supported.";
                    break;
                case SCardReturnValues.SCARD_E_ICC_INSTALLATION:
                    sRet = "No primary provider can be found for the smart card.";
                    break;
                case SCardReturnValues.SCARD_E_INSUFFICIENT_BUFFER:
                    sRet = "The data buffer for returned data is too small for the returned data.";
                    break;
                case SCardReturnValues.SCARD_E_INVALID_ATR:
                    sRet = "An ATR string obtained from the registry is not a valid ATR string.";
                    break;
                case SCardReturnValues.SCARD_E_INVALID_CHV:
                    sRet = "The supplied PIN is incorrect.";
                    break;
                case SCardReturnValues.SCARD_E_INVALID_HANDLE:
                    sRet = "The supplied handle was not valid.";
                    break;
                case SCardReturnValues.SCARD_E_INVALID_PARAMETER:
                    sRet = "One or more of the supplied parameters could not be properly interpreted.";
                    break;
                case SCardReturnValues.SCARD_E_INVALID_TARGET:
                    sRet = "Registry startup information is missing or not valid.";
                    break;
                case SCardReturnValues.SCARD_E_INVALID_VALUE:
                    sRet = "One or more of the supplied parameter values could not be properly interpreted.";
                    break;
                case SCardReturnValues.SCARD_E_NO_ACCESS:
                    sRet = "Access is denied to the file.";
                    break;
                case SCardReturnValues.SCARD_E_NO_DIR:
                    sRet = "The supplied path does not represent a smart card directory.";
                    break;
                case SCardReturnValues.SCARD_E_NO_FILE:
                    sRet = "The supplied path does not represent a smart card file.";
                    break;
                case SCardReturnValues.SCARD_E_NO_KEY_CONTAINER:
                    sRet = "The requested key container does not exist on the smart card.";
                    break;
                case SCardReturnValues.SCARD_E_NO_MEMORY:
                    sRet = "Not enough memory available to complete this command.";
                    break;
                case SCardReturnValues.SCARD_E_NO_PIN_CACHE:
                    sRet = "The smart card PIN cannot be cached.";
                    break;
                case SCardReturnValues.SCARD_E_NO_READERS_AVAILABLE:
                    sRet = "No smart card reader is available.";
                    break;
                case SCardReturnValues.SCARD_E_NO_SERVICE:
                    sRet = "The smart card resource manager is not running.";
                    break;
                case SCardReturnValues.SCARD_E_NO_SMARTCARD:
                    sRet = "The operation requires a smart card, but no smart card is currently in the device.";
                    break;
                case SCardReturnValues.SCARD_E_NO_SUCH_CERTIFICATE:
                    sRet = "The requested certificate does not exist.";
                    break;
                case SCardReturnValues.SCARD_E_NOT_READY:
                    sRet = "The reader or card is not ready to accept commands.";
                    break;
                case SCardReturnValues.SCARD_E_NOT_TRANSACTED:
                    sRet = "An attempt was made to end a nonexistent transaction.";
                    break;
                case SCardReturnValues.SCARD_E_PCI_TOO_SMALL:
                    sRet = "The PCI receive buffer was too small.";
                    break;
                case SCardReturnValues.SCARD_E_PIN_CACHE_EXPIRED:
                    sRet = "The smart card PIN cache has expired.";
                    break;
                case SCardReturnValues.SCARD_E_PROTO_MISMATCH:
                    sRet = "The requested protocols are incompatible with the protocol currently in use with the card.";
                    break;
                case SCardReturnValues.SCARD_E_READ_ONLY_CARD:
                    sRet = "The smart card is read-only and cannot be written to.";
                    break;
                case SCardReturnValues.SCARD_E_READER_UNAVAILABLE:
                    sRet = "The specified reader is not currently available for use.";
                    break;
                case SCardReturnValues.SCARD_E_READER_UNSUPPORTED:
                    sRet = "The reader driver does not meet minimal requirements for support.";
                    break;
                case SCardReturnValues.SCARD_E_SERVER_TOO_BUSY:
                    sRet = "The smart card resource manager is too busy to complete this operation.";
                    break;
                case SCardReturnValues.SCARD_E_SERVICE_STOPPED:
                    sRet = "The smart card resource manager has shut down.";
                    break;
                case SCardReturnValues.SCARD_E_SHARING_VIOLATION:
                    sRet = "The smart card cannot be accessed because of other outstanding connections.";
                    break;
                case SCardReturnValues.SCARD_E_SYSTEM_CANCELLED:
                    sRet = "The action was canceled by the system, presumably to log off or shut down.";
                    break;
                case SCardReturnValues.SCARD_E_TIMEOUT:
                    sRet = "The user-specified time-out value has expired.";
                    break;
                case SCardReturnValues.SCARD_E_UNEXPECTED:
                    sRet = "An unexpected card error has occurred.";
                    break;
                case SCardReturnValues.SCARD_E_UNKNOWN_CARD:
                    sRet = "The specified smart card name is not recognized.";
                    break;
                case SCardReturnValues.SCARD_E_UNKNOWN_READER:
                    sRet = "The specified reader name is not recognized.";
                    break;
                case SCardReturnValues.SCARD_E_UNKNOWN_RES_MNG:
                    sRet = "An unrecognized error code was returned.";
                    break;
                case SCardReturnValues.SCARD_E_UNSUPPORTED_FEATURE:
                    sRet = "This smart card does not support the requested feature.";
                    break;
                case SCardReturnValues.SCARD_E_WRITE_TOO_MANY:
                    sRet = "An attempt was made to write more data than would fit in the target object.";
                    break;
                case SCardReturnValues.SCARD_F_COMM_ERROR:
                    sRet = "An internal communications error has been detected.";
                    break;
                case SCardReturnValues.SCARD_F_INTERNAL_ERROR:
                    sRet = "An internal consistency check failed.";
                    break;
                case SCardReturnValues.SCARD_F_UNKNOWN_ERROR:
                    sRet = "An internal error has been detected, but the source is unknown.";
                    break;
                case SCardReturnValues.SCARD_F_WAITED_TOO_LONG:
                    sRet = "An internal consistency timer has expired.";
                    break;
                case SCardReturnValues.SCARD_P_SHUTDOWN:
                    sRet = "The operation has been aborted to allow the server application to exit.";
                    break;
                case SCardReturnValues.SCARD_S_SUCCESS:
                    sRet = "No error was encountered.";
                    break;
                case SCardReturnValues.SCARD_W_CANCELLED_BY_USER:
                    sRet = "The action was canceled by the user.";
                    break;
                case SCardReturnValues.SCARD_W_CACHE_ITEM_NOT_FOUND:
                    sRet = "The requested item could not be found in the cache.";
                    break;
                case SCardReturnValues.SCARD_W_CACHE_ITEM_STALE:
                    sRet = "The requested cache item is too old and was deleted from the cache.";
                    break;
                case SCardReturnValues.SCARD_W_CACHE_ITEM_TOO_BIG:
                    sRet = "The new cache item exceeds the maximum per-item size defined for the cache.";
                    break;
                case SCardReturnValues.SCARD_W_CARD_NOT_AUTHENTICATED:
                    sRet = "No PIN was presented to the smart card.";
                    break;
                case SCardReturnValues.SCARD_W_CHV_BLOCKED:
                    sRet = "The card cannot be accessed because the maximum number of PIN entry attempts has been reached.";
                    break;
                case SCardReturnValues.SCARD_W_EOF:
                    sRet = "The end of the smart card file has been reached.";
                    break;
                case SCardReturnValues.SCARD_W_REMOVED_CARD:
                    sRet = "The smart card has been removed, so further communication is not possible.";
                    break;
                case SCardReturnValues.SCARD_W_RESET_CARD:
                    sRet = "The smart card was reset.";
                    break;
                case SCardReturnValues.SCARD_W_SECURITY_VIOLATION:
                    sRet = "Access was denied because of a security violation.";
                    break;
                case SCardReturnValues.SCARD_W_UNPOWERED_CARD:
                    sRet = "Power has been removed from the smart card, so that further communication is not possible.";
                    break;
                case SCardReturnValues.SCARD_W_UNRESPONSIVE_CARD:
                    sRet = "The smart card is not responding to a reset.";
                    break;
                case SCardReturnValues.SCARD_W_UNSUPPORTED_CARD:
                    sRet = "The reader cannot communicate with the card, due to ATR string configuration conflicts.";
                    break;
                case SCardReturnValues.SCARD_W_WRONG_CHV:
                    sRet = "The card cannot be accessed because the wrong PIN was presented.";
                    break;
                default:
                    break;
            }

            return sRet;
        }

        public List<string> GetReaderList()
        {
            List<string> retval = new List<string>();
            uint pcchReaders = 0;
            int nullindex = -1;
            char nullchar = (char)0;

            try
            {
                // Establish context
                if (!EstablishContext())
                    return null;

                // Find out length of the available readers character
                LastError = WinSCard.SCardListReaders(
                    _context, 
                    null, 
                    null, 
                    ref pcchReaders);

                if (!IsSuccess(LastError))
                    return null;

                // Create a buffer and fill it
                byte[] szBuffer = new byte[pcchReaders];
                LastError = WinSCard.SCardListReaders(
                    _context,
                    null,
                    szBuffer,
                    ref pcchReaders);

                if (!IsSuccess(LastError))
                    return null;

                // Convert szReaders to string
                string sBuffer = Encoding.ASCII.GetString(szBuffer);

                // Convert length of the available readers character to int
                int len = (int)pcchReaders;

                while (sBuffer[0] != (char)0)
                {
                    nullindex = sBuffer.IndexOf(nullchar);
                    string reader = sBuffer.Substring(0, nullindex);

                    retval.Add(reader);

                    len = len - (reader.Length + 1);
                    sBuffer = sBuffer.Substring(nullindex + 1, len);
                }

                // Release context
                ReleaseContext();

                return retval;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool EstablishContext()
        {
            try
            {
                LastError = WinSCard.SCardEstablishContext(
                    _scope,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    out _context);

                if (!IsSuccess(LastError))
                    return false;

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ReleaseContext()
        {
            try
            {
                LastError = WinSCard.SCardReleaseContext(
                    _context);

                if (!IsSuccess(LastError))
                    return false;

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ConnectReader(string SelectedReader)
        {
            try
            {
                // Connect to selected reader
                LastError = WinSCard.SCardConnect(
                    _context,
                    SelectedReader,
                    _shareMode,
                    _protocol,
                    ref _cardHandle,
                    ref activeProtocol);

                if (!IsSuccess(LastError))
                    return false;

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DisconnectReader()
        {
            try
            {
                // Disconnect to selected Card Handle
                LastError = WinSCard.SCardDisconnect(
                    _cardHandle,
                    (int)SCardReaderDisposition.Unpower);

                if (!IsSuccess(LastError))
                    return false;

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string SendAPDU(string sAPDU)
        {
            SCARD_IO_REQUEST sendRequest;
            sendRequest.dwProtocol = (int)activeProtocol;
            sendRequest.cbPciLength = 8;

            SCARD_IO_REQUEST receiveRequest;
            receiveRequest.cbPciLength = 8;
            receiveRequest.dwProtocol = (int)activeProtocol;

            byte[] abAPDU = new byte[300];
            byte[] abResp = new byte[300];
            UInt32 wLenSend = 0;
            Int32 wLenRecv = 260;

            sAPDU = Utility.RemoveNonHexa(sAPDU);
            wLenSend = Utility.StrByteArrayToByteArray(sAPDU, ref abAPDU);

            LastError = WinSCard.SCardTransmit(
                _cardHandle, 
                ref sendRequest, 
                abAPDU, 
                (int)wLenSend, 
                ref receiveRequest, 
                abResp, 
                ref wLenRecv);

            if (!IsSuccess(LastError))
                return String.Empty;

            return Utility.ByteArrayToStrByteArray(
                abResp, 
                (UInt16)wLenRecv);
        }

        public bool ResetReader(
            string SelectedReader,
            out string ATR)
        {
            String atr_temp = "";
            String s = "";
            bool boRet = true;

            ATR = atr_temp;

            // Disconnect to with reset disposition
            LastError = WinSCard.SCardDisconnect(
                _cardHandle,
                (int)SCardReaderDisposition.Reset);

            if (!IsSuccess(LastError))
                return false;

            // Connect to selected reader
            if (!ConnectReader(SelectedReader))
                return false;

            // Init readerState
            SCARD_READERSTATE readerState = new SCARD_READERSTATE
            {
                RdrName = SelectedReader,
                RdrCurrState = 0,
                RdrEventState = 0,
                UserData = "Card",
            };

            // Get Status Change
            LastError = WinSCard.SCardGetStatusChange(
                _context,
                0,
                ref readerState,
                1);

            if (!IsSuccess(LastError))
                return false;

            StringBuilder hex = new StringBuilder(readerState.ATRValue.Length * 2);
            foreach (byte b in readerState.ATRValue)
                hex.AppendFormat("{0:X2}", b);
            atr_temp = hex.ToString();

            ATR = atr_temp;
            return boRet;
        }
    }
}
