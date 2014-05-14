angular.module("umbraco").controller("uWebshop.ProductOverview", function ($scope, $routeParams, $location, $http, contentResource, navigationService) {

    $http.get('/Umbraco/uWebshop/PublicApi/GetAllProducts').then(function (res) {

        $scope.ItemArray = res.data;
           
        $scope.getTreeNodeUrl = function (item) {
            contentResource.getById(item.id).then(function (content) {
                navigationService.syncTree({ tree: "content", path: content.path.split(","), forceReload: true, activate: true }).then(function (syncArgs) {
                    $location.path("/content/content/edit/"+ content.id);
                });
            });

        };

    });

});