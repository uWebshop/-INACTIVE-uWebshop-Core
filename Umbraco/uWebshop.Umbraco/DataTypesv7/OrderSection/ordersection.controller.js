function listViewController($rootScope, $scope, $routeParams, $injector, notificationsService, iconHelper, dialogService, navigationService, $location, $http) {

    //this is a quick check to see if we're in create mode, if so just exit - we cannot show children for content 
    // that isn't created yet, if we continue this will use the parent id in the route params which isn't what
    // we want. NOTE: This is just a safety check since when we scaffold an empty model on the server we remove
    // the list view tab entirely when it's new.
    if ($routeParams.create) {
        $scope.isNew = true;
        return;
    }

    var contentResource, contentTypeResource;

    contentResource = $injector.get('contentResource');
    contentTypeResource = $injector.get('contentTypeResource');
    $scope.entityType = "content";


    $scope.isNew = false;
    $scope.actionInProgress = false;
    $scope.listViewResultSet = {
        totalPages: 0,
        items: []
    };

    $scope.options = {
        pageSize: 10,
        pageNumber: 1,
        filter: '',
        orderBy: 'UpdateDate',
        orderDirection: "desc"
    };

    $scope.StatusArray = ["All"];

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

    /*Loads the search results, based on parameters set in prev,next,sort and so on*/
    /*Pagination is done by an array of objects, due angularJS's funky way of monitoring state
    with simple values */

    $scope.reloadView = function (id) {
        $scope.pagination = [];

        $http.get('/Umbraco/uWebshop/StoreApi/GetAllOrders').then(function (res) {
            $scope.listViewResultSet.items = res.data;
            $scope.listViewResultSet.totalItems = res.data.length;

            $scope.searchText = $scope.options.filter;

        });


        for (var i = $scope.listViewResultSet.totalPages - 1; i >= 0; i--) {
            $scope.pagination[i] = { index: i, name: i + 1 };
        }

        if ($scope.options.pageNumber > $scope.listViewResultSet.totalPages) {
            $scope.options.pageNumber = $scope.listViewResultSet.totalPages;
        }
    };

    //assign debounce method to the search to limit the queries
    $scope.search = _.debounce(function () {
        $scope.options.pageNumber = 1;
        $scope.reloadView($scope.contentId);
    }, 100);

    $scope.selectAll = function ($event) {
        var checkbox = $event.target;
        if (!angular.isArray($scope.listViewResultSet.items)) {
            return;
        }
        for (var i = 0; i < $scope.listViewResultSet.items.length; i++) {
            var entity = $scope.listViewResultSet.items[i];
            entity.selected = checkbox.checked;
        }
    };

    $scope.isSelectedAll = function () {
        if (!angular.isArray($scope.listViewResultSet.items)) {
            return false;
        }
        return _.every($scope.listViewResultSet.items, function (item) {
            return item.selected;
        });
    };

    $scope.isAnythingSelected = function () {
        if (!angular.isArray($scope.listViewResultSet.items)) {
            return false;
        }
        return _.some($scope.listViewResultSet.items, function (item) {
            return item.selected;
        });
    };


    $scope.publish = function () {
        var selected = _.filter($scope.listViewResultSet.items, function (item) {
            return item.selected;
        });
        var total = selected.length;
        if (total === 0) {
            return;
        }

        $scope.actionInProgress = true;
        $scope.bulkStatus = "Starting with updating";
        var current = 1;

        for (var i = 0; i < selected.length; i++) {
            $scope.bulkStatus = "Updating " + current + " out of " + total + " documents";

            var isPaid = false;

            if (selected[i].ispaidselected) {
                isPaid = true;
            }

            var IsFulfilled = false;

            if (selected[i].isfulfilledselected) {
                IsFulfilled = true;
            }

            var data = { 'Id': selected[i].UniqueId, 'status': selected[i].Status, 'paid': isPaid, 'fulfilled': IsFulfilled, 'emails': 'false' };

            console.log(data);

            $http.post('/Umbraco/uWebshop/StoreApi/PostOrder/', data)
                .success(function (data, status, headers, config) {

                    $scope.bulkStatus = "";
                    $scope.reloadView($scope.contentId);
                    $scope.actionInProgress = false;

                    //if there are validation errors for publishing then we need to show them
                    if (err.status === 400 && err.data && err.data.Message) {
                        notificationsService.error("Publish error", err.data.Message);
                    }
                    else {
                        dialogService.ysodDialog(err);
                    }

                })
                .error(function (responseData) {

                });
        }
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

    $scope.selectAllPaid = function ($event) {
        var checkbox = $event.target;

        if (!angular.isArray($scope.listViewResultSet.items)) {
            return;
        }
        for (var i = 0; i < $scope.listViewResultSet.items.length; i++) {
            var entity = $scope.listViewResultSet.items[i];
            if (!entity.IsPaid) {
                entity.ispaidselected = checkbox.checked;
                entity.selected = true;
            }
        }
    };

    $scope.selectAllFulfilled = function ($event) {
        var checkbox = $event.target;

        if (!angular.isArray($scope.listViewResultSet.items)) {
            return;
        }
        for (var i = 0; i < $scope.listViewResultSet.items.length; i++) {
            var entity = $scope.listViewResultSet.items[i];
            if (!entity.IsFulfilled) {
                entity.isfulfilledselected = checkbox.checked;
                entity.selected = true;
            }
        }
    };

    $scope.selectrow = function (item) {
        item.selected = true;
    };

    if ($routeParams.id) {
        $scope.pagination = new Array(10);
        $scope.listViewAllowedTypes = contentTypeResource.getAllowedTypes($routeParams.id);
        $scope.reloadView($routeParams.id);

        $scope.contentId = $routeParams.id;
        $scope.isTrashed = $routeParams.id === "-20" || $routeParams.id === "-21";

    }

}

angular.module("umbraco").controller("uWebshop.OrderSection", listViewController);