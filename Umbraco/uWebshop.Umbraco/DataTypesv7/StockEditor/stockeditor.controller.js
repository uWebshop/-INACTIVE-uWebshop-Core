angular.module("umbraco").controller("uWebshop.Stock", function ($scope, assetsService, contentEditingHelper, $routeParams, editorState, $http) {

    var nodeId = $routeParams.id;

    $scope.ItemArray = [];

    $http.get('/Umbraco/uWebshop/PublicApi/GetStock?id=' + nodeId).then(function (res) {

        $scope.stock = { value: res.data };

        $http.get('/Umbraco/uWebshop/StoreApi/GetStoreSpecificStockStoreAliasses').then(function (res) {

            $scope.ItemArray = res.data;

            $scope.ItemArray.splice(0, 0, "All Stores");

            $scope.selectedOption = $scope.ItemArray[0];

            if ($scope.model.value != '') {
                for (i = 0; $scope.ItemArray.length > i; i += 1) {
                    if ($scope.ItemArray[i] == $scope.model.value) {
                        $scope.selectedOption = $scope.ItemArray[i];
                    }
                }
            }

            $scope.update = function () {
                $scope.model.value = $scope.selectedOption;
            }

        });

        $scope.$on("formSubmitting", function (e, args) {

            var data = { 'id': nodeId, 'stock': $scope.stock.value, 'storealias': $scope.selectedOption };

            $http.post('/Umbraco/uWebshop/StoreApi/PostStock/', data)
            .success(function (data, status, headers, config) { })
            .error(function (responseData) {
                alert('error saving stock!');
            });
        });
    });

    $scope.$watch('selectedOption', function () {

        var storeAlias = $scope.selectedOption;

        $http.get('/Umbraco/uWebshop/PublicApi/GetStock?id=' + nodeId + "&storeAlias=" + storeAlias).then(function (res) {

            $scope.stock = { value: res.data };

        });
    });
});
