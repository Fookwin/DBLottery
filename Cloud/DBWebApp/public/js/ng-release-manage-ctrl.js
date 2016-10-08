angular.module('ng-index-app').controller('ng-release-manage-ctrl', function ($scope, $timeout, $http) {       
        
        $http.get('/pre-release/?issue=2016063').success(function (res) {
            $scope.content = res.data; 
        });
});