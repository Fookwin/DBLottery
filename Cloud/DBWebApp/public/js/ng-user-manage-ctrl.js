angular.module('ng-index-app').controller('ng-user-manage-ctrl', function ($scope, $rootScope, $timeout, $http) {
    $rootScope.selectedNavIndex = 1;
    $scope.platformList = [
        {name: 'Windows Store', index: '1'},
        {name: 'Windows Phone', index: '2'},
        {name: 'Android', index: '3'}
        ];

    $scope.scopeList = [
        {name: 'in one day', scope: '1'},
        {name: 'in one week', scope: '7'},
        {name: 'in one month', scope: '30'}
        ];

    $scope.refreshTable = function() {
        
        if ($scope.selectedPlatform !== undefined && $scope.selectedScope !== undefined) {
            $http.get('/users/?platform=' + $scope.selectedPlatform.index + '&scope=' + $scope.selectedScope.scope).success(function (res) {
               $scope.userList = res.data;
               $scope.countOfResult = $scope.userList.length; 
            });
        }
    }  
});