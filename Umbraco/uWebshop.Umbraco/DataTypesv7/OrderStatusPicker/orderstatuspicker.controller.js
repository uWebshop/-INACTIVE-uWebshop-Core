angular.module("umbraco").controller("uWebshop.OrderStatusPicker", function ($scope, assetsService, $routeParams, $http, editorState, contentEditingHelper) {


    $http.get('/Umbraco/uWebshop/PublicApi/GetOrderStatusses').then(function (res) {

        $scope.ItemArray = res.data;

        $scope.selectedOption = $scope.ItemArray[0];

        var properties = contentEditingHelper.getAllProps(editorState.current);

        var uniqueId = "";

        for (i = 0; properties.length > i; i += 1) {

            var property = properties[i];

            if (property.alias == "orderGuid") {
                uniqueId = property.value;
            }
        }

        $http.get('/Umbraco/uWebshop/StoreApi/GetOrder?uniqueOrderId=' + uniqueId).then(function (res) {
            $scope.CurrentOrder = res.data;

            var orderStatus = $scope.CurrentOrder.Status;

            for (i = 0; $scope.ItemArray.length > i; i += 1) {
                if ($scope.ItemArray[i] == orderStatus) {
                    $scope.selectedOption = $scope.ItemArray[i];
                }
            }
        });

        $scope.update = function () {
            $scope.model.value = $scope.selectedOption.value;
        };
    });
});