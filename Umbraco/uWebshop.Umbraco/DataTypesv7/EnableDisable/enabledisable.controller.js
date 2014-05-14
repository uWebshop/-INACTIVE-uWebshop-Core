angular.module("umbraco").controller("uWebshop.EnableDisable", function ($scope, assetsService, $routeParams, $http, editorState) {

    $scope.ItemArray = [];

    if (editorState.current.contentTypeAlias != "uwbsStore") {
        $scope.ItemArray.push({ name: "Store Default", value: "default" });
    }

    $scope.ItemArray.push({ name: "Enable", value: "enable" }, { name: "Disable", value: "disable" });

    $scope.selectedOption = $scope.ItemArray[0];

    if ($scope.model.value != '') {
        for (i = 0; $scope.ItemArray.length > i; i += 1) {
            if ($scope.ItemArray[i].value == $scope.model.value) {
                $scope.selectedOption = $scope.ItemArray[i];
            }
        }
    }

    $scope.update = function() {
        $scope.model.value = $scope.selectedOption.value;
    };
});