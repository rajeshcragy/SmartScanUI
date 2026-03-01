using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartScanUI.Scanner.CZUR
{
    /// <summary>
    /// Enumeration for PDF creation status codes from CZUR scanner
    /// </summary>
    public enum PdfCreationStatus
    {
        Success = 0,
        ErrorEnablingPdfComposition = 1,
        EnvironmentNotInitialized = 2,
        PdfFileNameIsNull = 4,
        InvalidJpgCompressionQuality = 6,
        DeviceNotConnected = 17,
        DeviceDoesNotSupportPdfComposition = 18,
        NoImageFileAdded = 20,
        PdfCompositionInProgress = 23
    }
    internal enum CzurInitializationStatus
    {
        Success = 0,
        InitializationError = 1,
        AlreadyInitialized = 3,
        AuthorizationCodeNull = 4,
        OutOfMemory = 7,
        FailedToCreateImageProcessingThread = 14,
        OpenGLOldVersion = 15,
        MicrosoftYaHeiNotInstalled = 30,
        InvalidAuthorizationCode = 32
    }

    internal enum CzurOpenDeviceStatus
    {
        Success = 0,
        Error = 1,
        EnvironmentNotInitialized = 2,
        InvalidCameraSerialNumber = 8,
        InvalidCameraResolution = 10,
        DeviceIsOn = 16,
        DeviceNotConnected = 17,
        DeviceNotSupported = 18
    }

    internal enum CzurSetProcessTypeStatus
    {
        Success = 0,
        EnvironmentNotInitialized = 2,
        DeviceNotSupported = 18,
        MainCameraIsOff = 19,
        MainCameraIsRecording = 23,
        InvalidProcessingMethod = 31
    }
    internal enum CzurDeviceModelStatus
    {
        Error = 0,
        CZUROcxisNotInilialized = 2,
        DeviceDisconnected = 17
    }

    internal enum CzurCloseDeviceStatus
    {
        Success = 0,
        CZUROcxisnotinitialized = 2,
        InvalidserialNoofcamera = 8,
        Thedeviceisoff = 19,
        Recordingorfacerecognitionisinprogress = 23
    }

    internal enum BarcodeType
    {
        EAN8 = 8,
        UPCE = 9,
        UPCA = 12,
        EAN13 = 13,
        ISBN13 = 14,
        Interleaved2Of5 = 25,
        Code39 = 39,
        QRCode = 64,
        Code128 = 128
    }
    
}


