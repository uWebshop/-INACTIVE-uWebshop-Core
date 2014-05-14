angular.module("umbraco").controller("uWebshop.Ranges", function ($scope, assetsService, $http) {
        
    $scope.SavedData = $scope.model.value;
    
    var splitData = $scope.SavedData.split("#");
      
    var jsonData = [];

    $scope.jsonData = jsonData;

    for (i = 0; splitData.length > i; i += 1) {

        var data = splitData[i];

        var itemdata = data.split("|");
		
        if (itemdata[0]) {
            jsonData.push({ "from": itemdata[0], "to": itemdata[1], "price": itemdata[2] });
        }
    }
	
    $scope.RangeData = jsonData;

    $scope.editItem = function (itemToEdit) {
        event.preventDefault();

        var data = JSON.parse(itemToEdit);

        for (i = 0; $scope.jsonData.length > i; i += 1) {
            if ($scope.jsonData[i].from == data.from && $scope.jsonData[i].to == data.to && $scope.jsonData[i].price == data.price) {

                var matchedItem = $scope.jsonData[i];

                var idx = $scope.jsonData.indexOf(matchedItem);
               // $scope.jsonData.splice(idx, 1);

            }
        }
        $scope.rangefrom = data.from;
        $scope.rangeto = data.to;
        $scope.rangeprice = data.price;
    };

    $scope.saveItem = function () {
        event.preventDefault();

        /*if ($scope.rangefrom && $scope.rangeto && $scope.rangeprice) {
            var data = { "from": $scope.rangefrom, "to": $scope.rangeto, "price": $scope.rangeprice };

            if (jsonData.indexOf(data) == -1) {
                jsonData.push(data);
            }

            $scope.rangefrom = "";
            $scope.rangeto = "";
            $scope.rangeprice = "";
        } */

           if ($scope.rangefrom && $scope.rangeto && $scope.rangeprice) {

				var add = true;
				var idx = 0;
				for (i = 0; $scope.jsonData.length > i; i += 1) {
					if ($scope.jsonData[i].from == $scope.rangefrom) {
						add = false;
						idx = i;
					}
				}

				if (add == false) {
					$scope.jsonData.splice(idx, 1);
				}
				
				var data = { "from": $scope.rangefrom, "to": $scope.rangeto, "price": $scope.rangeprice };

				
				$scope.jsonData.push(data);
			}
    };

    $scope.deleteItem = function (itemToRemove) {
        event.preventDefault();
        jsonData.splice(itemToRemove, 1);
    };

    $scope.$watch('jsonData', function () {

        var savevalue = '';

        $(jsonData).each(function () {

            var string = this.from + "|" + this.to + "|" + this.price;

            if (jsonData.indexOf(this) != jsonData.length - 1) {
                string += "#";
            }

            savevalue += string;

        });

        $scope.model.value = savevalue;

    }, true);

});