angular.module("umbraco").controller("uWebshop.CouponCodes", function ($scope, assetsService, $routeParams, $http) {

    var nodeId = $routeParams.id;

    $http.get('/Umbraco/uWebshop/StoreApi/GetCouponCodes?discountId=' + nodeId).then(function (res) {

        $scope.Coupons = res.data;

        $scope.editItem = function(itemToEdit) {
            event.preventDefault();

            var data = JSON.parse(itemToEdit);

            for (i = 0; $scope.Coupons.length > i; i += 1) {
                if ($scope.Coupons[i].CouponCode == data.CouponCode && $scope.Coupons[i].NumberAvailable == data.NumberAvailable) {

                    var matchedItem = $scope.Coupons[i];

                    var idx = $scope.Coupons.indexOf(matchedItem);
                    // $scope.jsonData.splice(idx, 1);

                }
            }
            $scope.CouponCode = data.CouponCode;
            $scope.NumberAvailable = data.NumberAvailable;
        };

        $scope.saveItem = function() {
            event.preventDefault();

            if ($scope.CouponCode && $scope.NumberAvailable) {
                console.log("Save!");

                var add = true;
                var idx = 0;
                for (var i = 0; $scope.Coupons.length > i; i += 1) {
                    if ($scope.Coupons[i].CouponCode == $scope.CouponCode) {
                        add = false;
                        idx = i;
                        break;
                    }
                }

                if (add == false) {
                    $scope.Coupons.splice(idx, 1);
                }

                var data = { "CouponCode": $scope.CouponCode, "NumberAvailable": $scope.NumberAvailable };

                $scope.Coupons.push(data);
            }
        };

        $scope.deleteItem = function(itemToRemove) {
            event.preventDefault();
            $scope.Coupons.splice(itemToRemove, 1);
        };

        $scope.$on("formSubmitting", function (e, args) {

            $http.post('/Umbraco/uWebshop/StoreApi/PostCouponCodes', { "Coupons": $scope.Coupons, "DiscountId": nodeId })
            .success(function (data, status, headers, config) { })
            .error(function (responseData) {
                alert('error saving couponcodes!');
            });
        });
    });
});