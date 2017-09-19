using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleApduSender
{
    public enum SCardReaderDisposition
    {

        // Do nothing. (SCARD_LEAVE_CARD)
        Leave = 0x0000,

        // Reset the card. (SCARD_RESET_CARD)
        Reset = 0x0001,

        // Unpower the card. (SCARD_UNPOWER_CARD)
        Unpower = 0x0002,

        // Eject the card. (SCARD_EJECT_CARD)
        Eject = 0x0003
    }
}
