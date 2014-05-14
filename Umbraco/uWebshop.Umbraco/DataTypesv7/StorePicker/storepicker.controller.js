angular.module("umbraco").controller("uWebshop.StorePicker", function ($scope, assetsService, $http) {

    $http.get('/Umbraco/uWebshop/StoreApi/GetAllStoreAliasses').then(function (res) {

        $scope.ItemArray = res.data;

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
});