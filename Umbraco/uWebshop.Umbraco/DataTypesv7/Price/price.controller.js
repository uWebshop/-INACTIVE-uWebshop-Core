angular.module("umbraco").controller("uWebshop.Price", function ($scope, assetsService, $routeParams, $http) {

    var decimalseparator = $scope.model.config.separator;

    var storedPrice = "0";

    if ($scope.model.value != '') {
        var storedPrice = (parseInt($scope.model.value) / 100).toFixed(2);

        if (decimalseparator == ",") {
            var storedPrice = storedPrice.replace(".", ",");
        }
    }

    $scope.price = storedPrice;

    $scope.$watch('price', function () {

        var data = $scope.price;

        if (data.indexOf(',') != -1) {
            data = $scope.price.replace(",", ".");
        }

        var currencySymbol = $scope.model.config.symbol;

        $scope.formattedPrice = accounting.formatMoney(data, currencySymbol, 2, ".", decimalseparator);

        var saveValue = parseFloat(data.replace(",", ".")) * 100;

        $scope.model.value = saveValue;

    });

});


