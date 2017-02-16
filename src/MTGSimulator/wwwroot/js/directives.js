app
    .directive("card", function () {
        return {
            restrict: "E",
            scope: false,
            templateUrl: "../HtmlPartials/CardWithEnchantmentsPartial.html"
        };
    })
    .directive("previewCard", function () {
        return {
            restrict: "E",
            scope: false,
            templateUrl: "../HtmlPartials/PreviewCard.html"
        };
    })
    .directive("draftCard", function () {
        return {
            restrict: "E",
            scope: false,
            templateUrl: "../HtmlPartials/DraftCard.html"
        };
    })