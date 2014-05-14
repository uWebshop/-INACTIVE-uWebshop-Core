angular.module("umbraco").controller("uWebshop.Cultures", function ($scope, assetsService, $routeParams, $http, editorState) {

    $http.get('/Umbraco/uWebshop/PublicApi/GetLanguagePicker').then(function (res) {

        $scope.ItemArray = res.data;

        $scope.combined = function (culture) {
            return culture.FriendlyName + " " + culture.ISOCurrencySymbol + " (" + culture.CurrencySymbol + " - " + culture.CurrencyEnglishName + ")";
        };

        if ($scope.model.value != '') {
            for (i = 0; $scope.ItemArray.length > i; i += 1) {
                if ($scope.ItemArray[i].LanguageId == $scope.model.value) {
                    $scope.selectedOption = $scope.ItemArray[i];
                }
            }
        }
        else {
             $scope.selectedOption = $scope.ItemArray[0];
        }

        $scope.update = function () {
            $scope.model.value = $scope.selectedOption.LanguageId;
        };

    });
});