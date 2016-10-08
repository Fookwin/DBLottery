angular.module('ng-index-app').controller('ng-user-manage-ctrl', function ($scope, $timeout, $http) {
    $scope.platformList = [
        {name: 'Windows Store', index: '1'},
        {name: 'Windows Phone', index: '2'},
        {name: 'Android', index: '3'}
        ];
    
    $scope.refreshTable = function() {
        
        if ($scope.selectedPlatform !== undefined) {
            $http.get('/users/?platform=' + $scope.selectedPlatform.index).success(function (res) {
               $scope.userList = res.data; 
            });
        }
    }  
});