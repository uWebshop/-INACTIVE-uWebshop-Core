angular.module("umbraco").controller("uWebshop.OrderSection", function ($scope, $routeParams, $location, $http, contentResource, navigationService) {

    $http.get('/Umbraco/uWebshop/PublicApi/GetOrderStatusses').then(function (res) {

        $scope.StatusArray = res.data;

        $scope.StatusArray.unshift("All");

        if ($scope.model.value != "") {
            for (i = 0; $scope.StatusArray.length > i; i += 1) {
                if ($scope.StatusArray[i] == $scope.model.value) {
                    $scope.SelectedOption = $scope.StatusArray[i];
                }
            }
        } else {
            $scope.SelectedOption = $scope.StatusArray[0];
        }

        $scope.$watch('SelectedOption', function () {

            $http.get('/Umbraco/uWebshop/StoreApi/GetAllOrders?status=' + $scope.SelectedOption).then(function (res) {

                $scope.ItemArray = res.data;

            });

        });
    });

    $scope.getTreeNodeUrl = function (item) {

        $http.get('/Umbraco/uWebshop/StoreApi/GetOrCreateOrderNode?uniqueOrderId=' + item.UniqueId).then(function (res) {

            contentResource.getById(res.data).then(function (content) {
                navigationService.syncTree({ tree: "content", path: content.path.split(","), forceReload: true, activate: true }).then(function (syncArgs) {
                    $location.path("/content/content/edit/" + content.id);
                });
            });

        });

    };

    $scope.$on("formSubmitting", function (e, args) {

        $scope.model.value = $scope.SelectedOption;

    });

});