angular.module('ng-index-app').service('util', function ($rootScope, $http) {

    // Get a formated date string from "\/Date(1476720000000+0800)\/".
    this.correctMSDateString = function(jsondate) {     
        jsondate = jsondate.replace("/Date(", "").replace(")/", "");     
        if (jsondate.indexOf("+") > 0) {    
            jsondate = jsondate.substring(0, jsondate.indexOf("+"));     
        }     
        else if (jsondate.indexOf("-") > 0) {    
            jsondate = jsondate.substring(0, jsondate.indexOf("-"));     
        }     
        
        var date = new Date(parseInt(jsondate, 10));   
        var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;    
        var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();    
        return date.getFullYear() + "-" + month + "-" + currentDate;    
    } 

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
        $http.get('/release').success(function (res) {
            $rootScope.originalReleaseContent = res.data;

            // correct the data format
            $rootScope.originalReleaseContent.next.date = new Date(self.correctMSDateString(res.data.next.cutOffTime));
            $rootScope.originalReleaseContent.next.cutOffTime = new Date(self.correctMSDateString(res.data.next.cutOffTime));
            $rootScope.originalReleaseContent.next.lotteryTime = new Date(self.correctMSDateString(res.data.next.lotteryTime));
            $rootScope.originalReleaseContent.lottery.date = new Date(self.correctMSDateString(res.data.lottery.date));

            $rootScope.releaseContent = angular.copy($rootScope.originalReleaseContent);
            
            callback();
        });
    }
});