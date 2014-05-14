angular.module("umbraco").controller("uWebshop.VatPicker", function ($scope, assetsService,editorState, $routeParams, $http) {

    //setup the default config
    var config = {
        items: []
    };


    //map the user config
    angular.extend(config, $scope.model.config);
    //map back to the model
    $scope.model.config = config;

    $scope.VatArray = [];
	
    if(editorState.current.contentTypeAlias != "uwbsStore") {
	$scope.VatArray.push({ name: "Store Default", value: "default" });
	}


    if (angular.isArray($scope.model.config.items)) {

        
        $scope.model.config.items.sort(function (a, b) {
            return (parseInt(a['value']) > parseInt(b['value']));
        });

        //now we need to format the items in the array because we always want to have a dictionary
        for (var i = 0; i < $scope.model.config.items.length; i++) {

            $scope.VatArray.push({
                name: $scope.model.config.items[i].value + '%',
                value: $scope.model.config.items[i].value
            });
        }
    }
    else if (!angular.isObject($scope.model.config.items)) {
        throw "The items property must be either an array or a dictionary";
    }

    $scope.selectedOption = $scope.VatArray[0];

    for (i = 0; $scope.VatArray.length > i; i += 1) {
        if ($scope.VatArray[i].value == $scope.model.value) {
            $scope.selectedOption = $scope.VatArray[i];
        }
    }

    $scope.update = function() {
        $scope.model.value = $scope.selectedOption.value;
    };
});