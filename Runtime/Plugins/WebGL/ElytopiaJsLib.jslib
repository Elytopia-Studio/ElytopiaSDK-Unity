mergeInto(LibraryManager.library, {
    JS_Log_Dump: function (ptr, type) {
        var msg = UTF8ToString(ptr);
        if (type === 0 || type === 1 || type === 4) {
            if (typeof SendMessage === "function") {
                SendMessage("ElytopiaSDK", "OnReceiveFromConsole", encodeURIComponent(msg));
            }
        }

        switch (type) {
            case 0:
            case 1:
            case 4:
                console.error(msg);
                break;
            case 2:
                console.warn(msg);
                break;
            case 3:
            case 5:
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