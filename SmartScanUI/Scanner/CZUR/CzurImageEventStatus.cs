using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartScanUI.Scanner.CZUR
{
    internal enum CzurImageEventStatus
    {
        Success = 0,
        ImageProcessingError = 1,
        InsufficientSystemMemory = 7,
        BlankImageDetected = 25,
        FailedToSaveImageFile = 29,
        PDFPasswordProtected = 55
    }
}
