angular.module('ng-index-app').controller('ng-publish-ctrl', function ($scope, $rootScope, $timeout, $http, $location) {       

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

    var testData = {
        issue: 2016117,
        date: new Date('2016/10/13'),
        scheme:'01 02 03 04 05 06+07',
        pool:5555555555,
        sell:6666666666,
        bonusCounts: [
            2, 14, 566, 4886, 52148, 121212
        ],
        bonusPrizes: [
            5000000, 140000, 3000, 200, 10, 5
        ],
        details: '一等奖中降地：北京1注 内蒙古2注 上海2注 安徽1注 湖南1注 广东2注 重庆1注 贵州1注 新疆1注 。开奖顺序： 05 06 04 02 01 03',
        next: {
            issue : 2006118,
            date: new Date('2016/10/16')
        },
        forcast: {
            includedReds: '02 03 05',
            excludedReds: '08 09',
            includedBlues: '02 03 05',
            excludedBlues: '08 09'
        }
    }

    $rootScope.selectedNavIndex = 2;
    
    $scope.version = angular.copy(originalVersion);

    $scope.onVersionChanged = function (label, number) {
        $scope.isVersionChanged = true;
    };

    $scope.onReleaseDataChanged = function () {
        $scope.isReleaseDataChanged = true;
    }

    $scope.releaseContent = angular.copy(testData);
    $http.get('/pre-release/?issue=2016063').success(function (res) {
        $scope.releaseContent = res.data; 
    });

    $scope.resetVersion = function () {
        $scope.version = angular.copy(originalVersion);
        $scope.isVersionChanged = false;
    };

    $scope.resetReleaseData = function () {
        $scope.releaseContent = angular.copy(testData);
        $scope.isReleaseDataChanged = false;
    };

    $scope.leaveVersion = function () {
        if ($scope.isVersionChanged){
            $('#submitVersionModal').modal('show') 
        } else {
            $location.url('/publish/notification');
        }
    }

    $scope.saveVersion = function () {
        alert('done');
        $scope.isVersionChanged = false;
        $('#submitVersionModal').modal('hide');
        $('#submitVersionModal').on('hidden.bs.modal', function (e) {
            $location.url('/publish/notification');
        });
    };

    $scope.leaveReleaseData = function () {
        if ($scope.isReleaseDataChanged){
            $('#submitReleaseDataModal').modal('show') 
        } else {
            $location.url('/publish/version');
        }
    }

    $scope.saveReleaseData = function () {
        alert('done');
        $scope.isReleaseDataChanged = false;
        $('#submitReleaseDataModal').modal('hide');
        $('#submitReleaseDataModal').on('hidden.bs.modal', function (e) {
            $location.url('/publish/version');
        });
    };
});