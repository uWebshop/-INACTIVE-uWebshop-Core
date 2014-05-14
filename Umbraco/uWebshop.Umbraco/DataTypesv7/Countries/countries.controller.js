angular.module("umbraco").controller("uWebshop.Countries", function ($scope, assetsService, $routeParams, $http, editorState) {

    $http.get('/Umbraco/uWebshop/PublicApi/GetCountrySelector').then(function (res) {

        $scope.ItemArray = res.data;

        $scope.combined = function (country) {
            return country.Name + " (" + country.Code + ")";
        };

        if ($scope.model.value != '') {
            for (i = 0; $scope.ItemArray.length > i; i += 1) {
                if ($scope.ItemArray[i].Code == $scope.model.value) {
                    $scope.selectedOption = $scope.ItemArray[i];
                }
            }
        }
        else {
            $scope.selectedOption = $scope.ItemArray[0];
        }

        for (i = 0; $scope.ItemArray.length > i; i += 1) {
            if ($scope.ItemArray[i].LanguageId == $scope.model.value) {
                $scope.selectedOption = $scope.ItemArray[i];
            }
        }

        $scope.update = function () {
            $scope.model.value = $scope.selectedOption.Code;
        };

    });
});