angular.module("umbraco").controller("uWebshop.Currencies", function ($scope, assetsService, $http) {

    $http.get('/Umbraco/uWebshop/PublicApi/GetCurrencyDataEditor').then(function (res) {

		$scope.ItemArray = res.data;
		
		$scope.combined = function (culture) {
			return culture.ISOCurrencySymbol + " (" + culture.CurrencySymbol + " - " + culture.CurrencyEnglishName + ")";
		};

		$scope.simplecombined = function (item) {
			return item.currency + " : " + item.index;
		};
		
		var splitData = $scope.model.value.split("#");

		
		$scope.SelectedArray = [];

		for (i = 0; splitData.length > i; i += 1) {

			var data = splitData[i];

			var itemdata = data.split("|");

			if (itemdata[0]) {
				$scope.SelectedArray.push({ "currency": itemdata[0], "index": itemdata[1] });
			}
		}

		$scope.selected = $scope.SelectedArray[0];

		$scope.editItem = function (itemToEdit) {
			event.preventDefault();

			var data = itemToEdit;

			for (i = 0; $scope.ItemArray.length > i; i += 1) {
				if ($scope.ItemArray[i].ISOCurrencySymbol == data.currency) {

					var matchedItem = $scope.ItemArray[i];

					var idx = $scope.ItemArray.indexOf(matchedItem);

					$scope.currency = $scope.ItemArray[idx];
				}
			}

			$scope.index = data.index;
		};

		$scope.saveItem = function () {
			event.preventDefault();

			if ($scope.currency.ISOCurrencySymbol && $scope.index) {

				var add = true;
				var idx = 0;
				for (i = 0; $scope.SelectedArray.length > i; i += 1) {
					if ($scope.SelectedArray[i].currency == $scope.currency.ISOCurrencySymbol) {
						add = false;
						idx = i;
					}
				}

				if (add == false) {
					$scope.SelectedArray.splice(idx, 1);
				}
				
				var data = { "currency": $scope.currency.ISOCurrencySymbol, "index": $scope.index };

				
				$scope.SelectedArray.push(data);

				var index = $scope.SelectedArray.indexOf(data);

				$scope.selected = $scope.SelectedArray[index];
			}
		};

		$scope.deleteItem = function (itemToRemove) {
			event.preventDefault();

			var idx = $scope.ItemArray.indexOf(itemToRemove);

			$scope.SelectedArray.splice(idx, 1);
		};

		$scope.$watch('SelectedArray', function () {

			var savevalue = '';

			$($scope.SelectedArray).each(function () {

				var string = this.currency + "|" + this.index;

				if ($scope.SelectedArray.indexOf(this) != $scope.SelectedArray.length - 1) {
					string += "#";
				}

				savevalue += string;

			});

			$scope.model.value = savevalue;

		}, true);

	});
});