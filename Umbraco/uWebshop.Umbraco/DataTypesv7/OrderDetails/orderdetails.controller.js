angular.module("umbraco").controller("uWebshop.OrderInfoViewer", function ($scope, assetsService, $routeParams, $http, editorState, contentEditingHelper) {

    var uniqueId = $scope.model.value;

    $http.get('/Umbraco/uWebshop/PublicApi/GetOrderStatusses').then(function (res) {

        $scope.StatusArray = res.data;

        $scope.SelectedOption = $scope.StatusArray[0];
    });

    $http.get('/Umbraco/uWebshop/StoreApi/GetOrder?uniqueOrderId=' + uniqueId).then(function (res) {

        $scope.CurrentOrder = res.data;

        var orderStatus = $scope.CurrentOrder.Status;

        $scope.OrderPaid = $scope.CurrentOrder.Paid;

        for (i = 0; $scope.StatusArray.length > i; i += 1) {
            if ($scope.StatusArray[i] == orderStatus) {
                $scope.SelectedOption = $scope.StatusArray[i];
            }
        }

        $http.get('/Umbraco/uWebshop/PublicApi/GetCountryFromCountryCode?countryCode=' + $scope.CurrentOrder.Customer.CountryCode + '&storeAlias=' + null + '&currencyCode=' + null).then(function (country) {

            $scope.CustomerCountryName = country.data.Name;

        });


        if ($scope.CurrentOrder.Customer.Shipping.CountryCode != "") {
            $http.get('/Umbraco/uWebshop/PublicApi/GetCountryFromCountryCode?countryCode=' + $scope.CurrentOrder.Customer.Shipping.CountryCode + '&storeAlias=' + null + '&currencyCode=' + null).then(function (country) {

                $scope.ShippingCountryName = country.data.Name;

            });
        };

    });

    $scope.$on("formSubmitting", function (e, args) {

        if ($scope.selectedOption != $scope.CurrentOrder.Status) {
            var data = { 'Id': uniqueId, 'status': $scope.SelectedOption, 'emails': $scope.SendEmail };

            $http.post('/Umbraco/uWebshop/StoreApi/PostOrderStatus/', data)
                .success(function (data, status, headers, config) { })
                .error(function (responseData) {
                    alert('error saving status!');
                });
        }

        if ($scope.OrderPaid != $scope.CurrentOrder.Paid) {

            var data = { 'Id': uniqueId, 'paid': $scope.OrderPaid };

            $http.post('/Umbraco/uWebshop/StoreApi/PostPaid/', data)
                .success(function (data, status, headers, config) { })
                .error(function (responseData) {
                    alert('error saving paid!');
                });
        }
    });

});
