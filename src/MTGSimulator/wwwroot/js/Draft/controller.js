app.controller("DraftController",
[
    "$scope", "backendHubProxy",
    function($scope, backendHubProxy) {
        var draftHub = backendHubProxy(backendHubProxy.defaultServer, "draftHub", onConnecting);

        function onConnecting() {
            console.log("Connected");
        }
    }
]);