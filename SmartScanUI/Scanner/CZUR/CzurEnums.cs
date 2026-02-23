using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartScanUI.Scanner.CZUR
{
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
    
}


