angular.module("umbraco").controller("uWebshop.OrderSection", function ($rootScope, $scope, $routeParams, $injector, $location, $http, contentResource, navigationService, notificationsService, iconHelper, dialogService) {

    $scope.listViewResultSet = {
        totalPages: 0,
        items: []
    };

    $scope.options = {
        pageSize: 1,
        pageNumber: 1,
        filter: '',
        orderBy: 'SortOrder',
        orderDirection: "asc"
    };

    $scope.next = function () {
        if ($scope.options.pageNumber < $scope.listViewResultSet.totalPages) {
            $scope.options.pageNumber++;
            $scope.reloadView($scope.contentId);
        }
    };

    $scope.goToPage = function (pageNumber) {
        $scope.options.pageNumber = pageNumber + 1;
        $scope.reloadView($scope.contentId);
    };

    $scope.sort = function (field) {

        $scope.options.orderBy = field;

        if ($scope.options.orderDirection === "desc") {
            $scope.options.orderDirection = "asc";
        } else {
            $scope.options.orderDirection = "desc";
        }


        $scope.reloadView($scope.contentId);
    };


    $scope.prev = function () {
        if ($scope.options.pageNumber > 1) {
            $scope.options.pageNumber--;
            $scope.reloadView($scope.contentId);
        }
    };

    $scope.StatusArray = ["All"];
    $scope.ItemStatusArray = [];
    $scope.pagination = [];

    $http.get('/Umbraco/uWebshop/PublicApi/GetOrderStatusses').then(function (res) {

        $scope.ItemStatusArray = res.data;
        $scope.SelectedItemStatus = $scope.ItemStatusArray[0];

        angular.forEach(res.data, function (item) {
            $scope.StatusArray.push(item);
        });

        $scope.SelectedOption = $scope.StatusArray[0];

        if ($scope.model.value != "") {
            for (i = 0; $scope.StatusArray.length > i; i += 1) {
                if ($scope.StatusArray[i] == $scope.model.value) {
                    $scope.SelectedOption = $scope.StatusArray[i];
                }
            }
        }
    });

    $scope.reloadView = function (id) {

        $scope.predicate = '-OrderReference';

        $scope.$watch('SelectedOption', function () {

            $http.get('/Umbraco/uWebshop/StoreApi/GetAllOrders?status=' + $scope.SelectedOption).then(function (res) {

                $scope.listViewResultSet = res.data;

                $scope.currentSorting = $scope.options.orderBy;

                $scope.searchText = $scope.options.filter;

                for (var i = $scope.listViewResultSet.totalPages - 1; i >= 0; i--) {
                    $scope.pagination[i] = { index: i, name: i + 1 };
                }

                if ($scope.options.pageNumber > $scope.listViewResultSet.totalPages) {
                    $scope.options.pageNumber = $scope.listViewResultSet.totalPages;
                }

            });
        });

    };

    $scope.search = _.debounce(function () {
        $scope.reloadView($scope.contentId);
    }, 100);


    $scope.selectAll = function ($event) {
        var checkbox = $event.target;

        if (!angular.isArray($scope.listViewResultSet)) {
            return;
        }
        for (var i = 0; i < $scope.listViewResultSet.length; i++) {
            var entity = $scope.listViewResultSet[i];
            entity.selected = checkbox.checked;
        }
    };

    $scope.selectAllPaid = function ($event) {
        var checkbox = $event.target;

        if (!angular.isArray($scope.listViewResultSet)) {
            return;
        }
        for (var i = 0; i < $scope.listViewResultSet.length; i++) {
            var entity = $scope.listViewResultSet[i];
            if (!entity.IsPaid) {
                entity.ispaidselected = checkbox.checked;
            }
        }
    };

    $scope.selectAllFulfilled = function ($event) {
        var checkbox = $event.target;

        if (!angular.isArray($scope.listViewResultSet)) {
            return;
        }
        for (var i = 0; i < $scope.listViewResultSet.length; i++) {
            var entity = $scope.listViewResultSet[i];
            if (!entity.IsFulfilled) {
                entity.isfulfilledselected = checkbox.checked;
            }
        }
    };


    $scope.isSelectedAll = function () {
        if (!angular.isArray($scope.listViewResultSet)) {
            return false;
        }
        return _.every($scope.listViewResultSet, function (item) {
            return item.selected;
        });
    };

    $scope.isAnythingSelected = function () {
        if (!angular.isArray($scope.listViewResultSet)) {
            return false;
        }
        return _.some($scope.listViewResultSet, function (item) {
            return item.selected;
        });
    };

    $scope.getTreeNodeUrl = function (item) {

        $http.get('/Umbraco/uWebshop/StoreApi/GetOrCreateOrderNode?uniqueOrderId=' + item.UniqueId).then(function (res) {

            contentResource.getById(res.data).then(function (content) {
                navigationService.syncTree({ tree: "content", path: content.path.split(","), forceReload: true, activate: true }).then(function (syncArgs) {
                    $location.path("/content/content/edit/" + content.id);
                });
            });

        });

    };

    $scope.$on("formSubmitting", function (e, args) {

        $scope.model.value = $scope.SelectedOption;

    });

    if ($routeParams.id) {

        $scope.pagination = new Array(10);
        $scope.reloadView($routeParams.id);

        $scope.contentId = $routeParams.id;
        $scope.isTrashed = $routeParams.id === "-20" || $routeParams.id === "-21";

    }


});