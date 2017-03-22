app.controller("DraftController",
[
    "$scope", "backendHubProxy",
    function($scope, backendHubProxy) {
        $scope.draftStarted = false;
        $scope.showNotWantedCards = false;
        $scope.unOpenedBoosters = [];
        $scope.openBoosters = [];
        $scope.currentBooster = [];
        $scope.selectedCards = [];
        $scope.notWantedCards = [];
        $scope.selectedCardsJson = null;
        $scope.connected = false;
        $scope.draftSessionId = null;
        $scope.draftSessionIdCandidate = null;
        $scope.selectedSet = null;
        $scope.sets = [];
        $scope.playerId = null;
        $scope.sets = [];

        var draftHub = backendHubProxy(backendHubProxy.defaultServer, "draftHub", onConnecting);

        function onConnecting() {
            console.log("Connected");
            $scope.connected = true;
            draftHub.invoke("GetAvailableSets", function() {});
        }

        function makeJsonObject() {
            var result = [];
            var cards = $scope.bundleCollection($scope.selectedCards);
            for (var i = 0; i < cards.length; i++) {
                var card = cards[i];
                var simplifiedCard = {
                    name: card.name,
                    amount: card.amount,
                    id: card.multiverseid
                };
                if (card.names !== undefined && card.names !== null && card.names.length > 1) { // find flip id of back side of card
                    simplifiedCard.flipId = "REMEMBER TO ADD GATHERER ID FOR FLIP CARD";
                }
                result.push(simplifiedCard);
            }
            return JSON.stringify(result, null, 2);
        }

        function removeCardFromCurrentBooster(id) {
            for (var i = 0; i < $scope.currentBooster.length; i++) {
                var card = $scope.currentBooster[i];
                if (card.id === id) {
                    var result = $scope.currentBooster.splice(i, 1)[0];
                    $scope.selectedCards.push(result);
                    $scope.selectedCardsJson = makeJsonObject();
                    break;
                }
            }
        };

        $scope.createDraft = function() {
            if ($scope.selectedSet === undefined || $scope.selectedSet === null || $scope.selectedSet === "") return;
            draftHub.invoke("CreateDraft", [$scope.selectedSet], function() {});
        };
        $scope.joinDraft = function() {
            if ($scope.draftSessionIdCandidate === undefined ||
                $scope.draftSessionIdCandidate === null ||
                $scope.draftSessionIdCandidate === "") return;
            draftHub.invoke("JoinDraft", [$scope.draftSessionIdCandidate], function() {});
        };
        $scope.startDraft = function () {
            if ($scope.draftSessionId === undefined ||
                $scope.draftSessionId === null ||
                $scope.draftSessionId === "") return;
            draftHub.invoke("StartDraft", [$scope.draftSessionId], function () { });
        };
        $scope.pickCard = function (event) {
            var cardImg = event.currentTarget.getElementsByTagName("img")[0];
            removeCardFromCurrentBooster(cardImg.id);
            draftHub.invoke("PassOnBooster", [$scope.playerId, $scope.draftSessionId, $scope.currentBooster], function () { });
            $scope.currentBooster = [];
            if ($scope.openBoosters.length > 0) {
                $scope.currentBooster = $scope.openBoosters.splice(0, 1)[0];
            }
        };
        $scope.setCurrentBooster = function () {
            if ($scope.unOpenedBoosters.length > 0 && $scope.openBoosters.length === 0 && $scope.currentBooster.length === 0) {
                $scope.currentBooster = $scope.unOpenedBoosters.pop();
            }
        };
        $scope.addCardToNotWantedCards = function (event) {
            var cardImg = event.currentTarget.getElementsByTagName("img")[0];
            var card = $scope.getCardFrom(cardImg.id, $scope.selectedCards);
            $scope.notWantedCards.push(card);
            $scope.selectedCardsJson = makeJsonObject();
        };
        $scope.removeCardFromNotWantedCards = function (event) {
            var cardImg = event.currentTarget.getElementsByTagName("img")[0];
            var card = $scope.getCardFrom(cardImg.id, $scope.notWantedCards);
            $scope.selectedCards.push(card);
            $scope.selectedCardsJson = makeJsonObject();
            if ($scope.notWantedCards.length === 0) {
                $scope.showNotWantedCards = false;
            }
        };
        $scope.flipShow = function () {
            $scope.showNotWantedCards = !$scope.showNotWantedCards;
        };

        $scope.bundleCollection = function (collection) {
            var tempArray = [];
            for (var j = 0; j < collection.length; j++) {
                var card = collection[j];
                var isCardAlreadyInTempArray = false;
                for (var k = 0; k < tempArray.length; k++) {
                    if (tempArray[k].name === card.name) {
                        isCardAlreadyInTempArray = true;
                        tempArray[k].amount++;
                        break;
                    }
                }
                if (!isCardAlreadyInTempArray) {
                    card.amount = 1;
                    tempArray.push(card);
                }
            }
            return tempArray;
        };
        $scope.getTopCardOf = function (collection) {
            return [collection[collection.length - 1]];
        };
        $scope.getCardFrom = function (id, collection) {
            for (var i = 0; i < collection.length; i++) {
                var card = collection[i];
                if (card.id === id) {
                    return collection.splice(i, 1)[0];
                }
            }
            return null;
        };
        $scope.getPosition = function (index, startValue) {
            var cardsInRow = 30;
            var distanceBetweenCards = 50;
            var bottom = startValue + (Math.floor(index / cardsInRow) * 155);
            var left = startValue + (index * distanceBetweenCards) - (Math.floor(index / cardsInRow) * cardsInRow * distanceBetweenCards);
            return {
                "left": left + "px",
                "bottom": bottom + "px"
            };
        };
        $scope.getLeft = function (index, startValue, incrementer) {
            return { "left": startValue + (index * incrementer) + "px" };
        };
        $scope.getImgSrc = function (card) {
            return "http://gatherer.wizards.com/Handlers/Image.ashx?type=card&multiverseid=" + card.multiverseid;
        };

        draftHub.on("InitializeGame",
            function(data) {
                $scope.draftSessionId = data.draftId;
                $scope.playerId = data.playerId;
                $scope.unOpenedBoosters = data.boosters;
            });

        draftHub.on("SetSets",
            function(data) {
                $scope.sets = data;
            });

        draftHub.on("DraftHasStarted",
            function() {
                $scope.draftStarted = true;
            });

        draftHub.on("ReceiveBooster",
            function(booster) {
                if ($scope.currentBooster.length === 0) {
                    $scope.currentBooster = booster;
                } else {
                    $scope.openBoosters.push(booster);
                }
            });
    }
]);