angular.module("umbraco").controller("uWebshop.MemberGroups", function ($scope, assetsService, $http) {

    $http.get('/Umbraco/uWebshop/StoreAPI/GetAllMemberGroups').then(function (res) {

        $scope.Items = res.data;

        $scope.availableItems = $scope.Items;

        $scope.selectedItems = [];

        $scope.CurrentItems = $scope.model.value.split(",");

        $($scope.CurrentItems).each(function () {
            for (i = 0; $scope.Items.length > i; i += 1) {
                if ($scope.Items[i].Alias == this) {

                    var matchedItem = $scope.Items[i];

                    var idx = $scope.availableItems.indexOf(matchedItem);
                    $scope.availableItems.splice(idx, 1);

                    $scope.selectedItems.push(matchedItem);
                }
            }
        });

        $scope.moveItem = function (itemsToMove, sourceList, targetList) {
            event.preventDefault();

            $(itemsToMove).each(function () {

                for (i = 0; sourceList.length > i; i += 1) {
                    if (sourceList[i].Alias == this) {

                        targetList.push(sourceList[i]);

                        sourceList.splice(i, 1);
                    }
                };

            });

            itemsToMove.length = 0;
        };

        $scope.$watchCollection('selectedItems', function (newValues, oldValues) {

            $scope.ItemsToSave = [];

            $(newValues).each(function () {
                if ($scope.ItemsToSave.indexOf(this.Alias) == -1) {
                    $scope.ItemsToSave.push(this.Alias);
                }
            });

            $scope.model.value = $scope.ItemsToSave.join();
        });


        $scope.$apply();

    });


});