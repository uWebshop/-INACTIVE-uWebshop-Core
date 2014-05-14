angular.module("umbraco").controller("uWebshop.StoreTemplatePicker", function ($scope, assetsService, editorState, $routeParams, $http) {

	var nodeId = $routeParams.id;

	$http.get('/Umbraco/uWebshop/PublicApi/GetTemplates?id=' + nodeId).then(function (res) {
		
		console.log($scope.model.value);

		$scope.ItemArray = [{ id: 0, name: "Default", alias: "default" }];
		
		angular.forEach(res.data, function (item) {
			$scope.ItemArray.push(item);
		});
		
		$scope.selectedOption = $scope.ItemArray[0];

		for (i = 0; $scope.ItemArray.length > i; i += 1) {
			if ($scope.ItemArray[i].id == $scope.model.value) {
				$scope.selectedOption = $scope.ItemArray[i];
			}
		}

		$scope.update = function () {
			$scope.model.value = $scope.selectedOption.id;
		};
	});
});