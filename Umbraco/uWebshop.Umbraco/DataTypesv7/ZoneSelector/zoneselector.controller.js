angular.module("umbraco").controller("uWebshop.ZoneSelector", function ($scope, assetsService, $http) {

    //var prevalueField = $scope.model.config.prevaluekeyname;

    $http.get('/Umbraco/uWebshop/PublicApi/GetCountrySelector').then(function (res) {

        // $scope.selectedCountries = $scope.model.value.split(",");

        $scope.Countries = res.data;

        $scope.availableItems = $scope.Countries;

        $scope.selectedItems = [];

        $scope.selectedCountries = [];
        $scope.CurrentCountries = $scope.model.value.split(",");

        $($scope.CurrentCountries).each(function () {
            for (i = 0; $scope.Countries.length > i; i += 1) {
                if ($scope.Countries[i].Code == this) {

                    var matchedItem = $scope.Countries[i];

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
                    if (sourceList[i].Code == this) {

                        targetList.push(sourceList[i]);

                        sourceList.splice(i, 1);
                    }
                };

            });

            itemsToMove.length = 0;
        };

        $scope.$watchCollection('selectedItems', function (newValues, oldValues) {

            $scope.countriesToSave = [];

            $(newValues).each(function () {
                if ($scope.countriesToSave.indexOf(this.Code) == -1) {
                    $scope.countriesToSave.push(this.Code);
                }
            });
                        
            $scope.model.value = $scope.countriesToSave.join();
        });


        $scope.$apply();

    });



});
