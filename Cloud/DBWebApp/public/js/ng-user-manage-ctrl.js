angular.module('ng-index-app').controller('ng-user-manage-ctrl', function ($scope, $rootScope, $timeout, $http) {
    $rootScope.selectedNavIndex = 1;
    $scope.inLoadingData = false;
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

    $scope.selectedPlatform = $scope.platformList[0];
    $scope.selectedScope = $scope.scopeList[0];

    $scope.refreshTable = function() {
        
        if ($scope.selectedPlatform !== undefined && $scope.selectedScope !== undefined) {
            $scope.inLoadingData = true;
            $http.get('/users/?platform=' + $scope.selectedPlatform.index + '&scope=' + $scope.selectedScope.scope).success(function (res) {
               $scope.userList = res.data;
               $scope.countOfResult = $scope.userList.length; 
               $scope.inLoadingData = false;
            });
        }
    }  
});