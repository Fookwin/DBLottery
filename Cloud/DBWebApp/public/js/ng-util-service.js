angular.module('ng-index-app').service('util', function ($rootScope, $http) {

    this.getRandomNumber = function random(max) {
        return Math.ceil(Math.ceil(Math.random() * max * 100) / 100);
    }

    this.getRandomNumbers = function(count, max) {
        var nums = [];
        while (nums.length < count) {
            var rN = this.getRandomNumber(max);
            if (!nums.find(function (num) { return num === rN } )) {
                nums.push(rN);
            }
        }

        return nums;
    }

    this.getMoneyFormat = function fomatMoney(money) {
        var output = "";
        var yi = Math.floor(money / 100000000);
        if (yi > 0)
            output += yi + "亿";

        var wan = Math.floor((money % 100000000) / 10000);
        if (wan > 0)
            output += wan + "万";

        var yuan = money % 10000;
        if (yuan > 0)
            output += yuan;

        output += "元";

        return output;
    }

    this.syncReleaseDateToCloud = function(callback) {
        var self = this;
        $http.get('/last').success(function (res) {
            $rootScope.originalReleaseContent = res.data;
            
            // convert string to date
            $rootScope.originalReleaseContent.next.date = new Date($rootScope.originalReleaseContent.next.date);
            $rootScope.originalReleaseContent.next.cutOffTime = new Date($rootScope.originalReleaseContent.next.cutOffTime);
            $rootScope.originalReleaseContent.lottery.date = new Date($rootScope.originalReleaseContent.lottery.date);

            $rootScope.releaseContent = angular.copy($rootScope.originalReleaseContent);
            
            callback();
        });
    }
});