angular.module("umbraco").controller("uWebshop.OrderedCount", function ($scope, assetsService, $routeParams, $http) {

    var nodeId = $routeParams.id;

    $http.get('/Umbraco/uWebshop/PublicApi/GetOrderCount?id=' + nodeId).then(function (res) {
        
        $scope.ordercount = {value: res.data};
                
    });
});
