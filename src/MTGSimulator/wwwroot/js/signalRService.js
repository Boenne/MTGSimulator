"use strict";

app.factory("backendHubProxy",
[
    "$rootScope", "backendServerUrl",
    function($rootScope, backendServerUrl) {

        function backendFactory(serverUrl, hubName, onConnecting) {
            var connection = $.hubConnection(backendServerUrl);
            var proxy = connection.createHubProxy(hubName);

            connection.start().done(onConnecting);

            return {
                on: function(eventName, callback) {
                    proxy.on(eventName,
                        function(result) {
                            $rootScope.$apply(function() {
                                if (callback) {
                                    callback(result);
                                }
                            });
                        });
                },
                invoke: function(methodName, args, callback) {
                    proxy.invoke.apply(proxy, $.merge([methodName], args))
                        .done(function(result) {
                            $rootScope.$apply(function() {
                                if (callback) {
                                    callback(result);
                                }
                            });
                        })
                        .fail(function(error) {
                            console.log("Error invoking " + methodName + " on server: " + error);
                        });
                }
            };
        };

        return backendFactory;
    }
]);