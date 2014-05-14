angular.module("umbraco").controller("uWebshop.EmailTemplatePicker", function ($scope, assetsService, editorState, $routeParams, $http) {

    $http.get('/Umbraco/uWebshop/StoreApi/GetEmailTemplates').then(function (res) {

        $scope.ItemArray = res.data;

        $scope.SelectedOption = $scope.ItemArray[0];

        for (i = 0; $scope.ItemArray.length > i; i += 1) {
            if ($scope.ItemArray[i] == $scope.model.value) {
                $scope.SelectedOption = $scope.ItemArray[i];
            }
        }

        $scope.update = function () {
            $scope.model.value = $scope.SelectedOption;
        };
    });
});