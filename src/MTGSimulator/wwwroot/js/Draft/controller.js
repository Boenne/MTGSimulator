app.controller("DraftController",
[
    "$scope", "backendHubProxy",
    function($scope, backendHubProxy) {
        var draftHub = backendHubProxy(backendHubProxy.defaultServer, "draftHub", onConnecting);

        function onConnecting() {
            console.log("Connected");
            draftHub.invoke("CreateDraft", ["LEA"], function() {});
        }

        draftHub.on("InitializeGame",
            function(response) {
                console.log(response.draftId);
                console.log(response.playerId);
                console.log(response.boosters);
            });
    }
]);