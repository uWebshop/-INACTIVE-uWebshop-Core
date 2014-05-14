angular.module("umbraco").controller("uWebshop.ShippingProviderType", function ($scope, assetsService, $routeParams, $http, editorState) {

    $scope.ItemArray = [];

    $http.get('/Umbraco/uWebshop/StoreApi/GetShippingProviderTypes').then(function (res) {

        $scope.ItemArray = res.data;

        $scope.selectedOption = $scope.ItemArray[0];

        for (i = 0; $scope.ItemArray.length > i; i += 1) {
            if ($scope.ItemArray[i] == $scope.model.value) {
                $scope.selectedOption = $scope.ItemArray[i];
            }
        }

        $scope.update = function () {
            $scope.model.value = $scope.selectedOption;
        };

    });
});