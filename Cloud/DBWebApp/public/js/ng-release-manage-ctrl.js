angular.module('ng-index-app').controller('ng-release-manage-ctrl', function ($scope, $rootScope, $timeout, $http) {       

    var originalVersion = {
        'LatestIssue': 2016117,
        'History':4,
        'Release':1,
        'Attributes':1,
        'AttributeTemplate':2,
        'LatestLottery':2,
        'Matrix':2,
        'Help':1
    };

    $rootScope.selectedNavIndex = 2;
    $scope.IsVersionChanged = false;
    
    $scope.version = angular.copy(originalVersion);

    $scope.onVersionChanged = function (label, number) {
        $scope.IsVersionChanged = true;
    };

    $http.get('/pre-release/?issue=2016063').success(function (res) {
        $scope.content = res.data; 
    });

    $scope.resetVersion = function () {
        $scope.version = angular.copy(originalVersion);
    };
});