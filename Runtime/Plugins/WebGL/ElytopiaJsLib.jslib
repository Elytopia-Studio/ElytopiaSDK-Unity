mergeInto(LibraryManager.library, {
    JS_Log_Dump: function (ptr, type) {
        var msg = UTF8ToString(ptr);
        
        if (typeof dump == 'function')
            dump (msg);
        
        if (typeof window.ElytopiaHubProvider === 'function') {
            window.ElytopiaHubProvider("debuglog", type, msg);
        }
        
        switch (type) {
            case 0: //LogType_Error
            case 1: //LogType_Assert
            case 4: //LogType_Exception
                console.error(msg);
                break;
            case 2: //LogType_Warning
                console.warn(msg);
                break;
            case 3: //LogType_Log
            case 5: //LogType_Debug
                console.log(msg);
                break;
            default:
                console.error("Unknown console message type!");
                console.error(msg);
        }
    },

    ElytopiaSDKExplorer: function (commandPtr, valuesPtr) {
        var command = encodeURIComponent(UTF8ToString(commandPtr));
        var values = encodeURIComponent(UTF8ToString(valuesPtr));

        if (typeof window.ElytopiaHubProvider === 'function') {
            window.ElytopiaHubProvider("message", command, values);
        } else {
            console.log("[ElytopiaSDK] Hub Not Found! Send message: command=" + command + ";values=" + values);
        }
    }
});