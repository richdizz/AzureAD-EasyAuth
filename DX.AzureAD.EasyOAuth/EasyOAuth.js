var AzureADAuth;
(function (AzureADAuth) {
    AzureADAuth._clientId = null;
    AzureADAuth._clientSupported = false;
    AzureADAuth._socketId = null;
    AzureADAuth._socketReady = false;

    if (window.WebSocket) {
        //mark browser safe for web sockets
        AzureADAuth._clientSupported = true;
        window.addEventListener("load", function () {
            //determine protocol
            var protocol = "ws";
            if (window.location.href.toLowerCase().indexOf("https") != -1)
                protocol = "wss";

            var wsUri = protocol + "://easyauth.azurewebsites.net/api/Token";
            websocket = new WebSocket(wsUri);
            websocket.onopen = function (evt) {
                AzureADAuth._socketReady = true;
            };
            websocket.onclose = function (evt) {
                var x = "TODO: Ignore???";
            };
            websocket.onmessage = function (evt) {
                //first message should be the socketId
                if (AzureADAuth._socketId === null)
                    AzureADAuth._socketId = evt.data;
                else if (evt.data !== null && evt.data !== undefined && evt.data !== "")
                    AzureADAuth.Deferred._thenCallback(evt.data);
                else
                    AzureADAuth.Deferred._errorCallback("Error occurred");
            };
            websocket.onerror = function (evt) {
                var x = "TODO: Alert user???";
            };
        }, false);
    }
    else {
        //alert the user that their browser is not supported
        alert("Your browser does not support Azure AD Easy Auth");
    }

    //configure the getAccessToken function
    AzureADAuth.getAccessToken = function () {
        this.Deferred = new Deferred();

        if (AzureADAuth._clientSupported) {

            //get all the script elements and look for the proxy script
            var scripts = document.getElementsByTagName("script");
            for (var i = 0; i < scripts.length; i++) {
                if (scripts[i].hasAttribute("data-clientid")) {
                    AzureADAuth._clientId = scripts[i].getAttribute("data-clientid");
                    break;
                }
            }

            //wait for response from window open
            function listener(event) {
                if (event.origin !== "https://easyauth.azurewebsites.net")
                    return

                //parse the data and then perform the appropriate promise callback
                var data = JSON.parse(event.data);
                if (data.action === "success")
                    AzureADAuth.Deferred._thenCallback(data.token);
                else if (data.action === "error")
                    AzureADAuth.Deferred._errorCallback(data.error);

                //unwire events
                if (window.removeEventListener) {
                    removeEventListener("message", listener, false);
                }
                else {
                    detachEvent("onmessage", listener);
                }
            }
            //if (window.addEventListener) {
            //    addEventListener("message", listener, false);
            //}
            //else {
            //    attachEvent("onmessage", listener);
            //}

            //window open launch the auth flow
            var resource = "https://graph.microsoft.com"
            var path = "https://login.microsoftonline.com/common/oauth2/authorize?client_id={0}&resource={1}&response_type=code&state={2}&redirect_uri=https://easyauth.azurewebsites.net/OAuth/AuthCode/{0}/{3}/";
            path = path.replace("{0}", AzureADAuth._clientId).replace("{0}", AzureADAuth._clientId);
            path = path.replace("{1}", resource);
            path = path.replace("{2}", window.location.origin + "|" + resource);
            path = path.replace("{3}", AzureADAuth._socketId);
            window.open(path, "", "left = 0, top = 0, height = 600, width = 945, status = yes, toolbar = no, menubar = no, location = yes, resizable = yes");
        }

        return this.Deferred;
    };

    var Deferred = (function () {
        function Deferred() {
            this._thenCallback = null;
            this._errorCallback = null;
        }
        Object.defineProperty(Deferred.prototype, "thenCallback", {
            get: function () {
                return this._thenCallback;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(Deferred.prototype, "errorCallback", {
            get: function () {
                return this._errorCallback;
            },
            enumerable: true,
            configurable: true
        });
        Deferred.prototype.then = function (callback) {
            this._thenCallback = callback;
            return this;
        };
        Deferred.prototype.error = function (callback) {
            this._errorCallback = callback;
            return this;
        };
        return Deferred;
    })();
    AzureADAuth.Deferred = Deferred;

    return AzureADAuth.Deferred;
})(AzureADAuth || (AzureADAuth = {}));