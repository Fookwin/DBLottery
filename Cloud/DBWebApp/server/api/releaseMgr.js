var url = require('url'),
    request = require('request'),
    global = require('../config/global.js'),
    endPoint = require('../config/config.js')[global.env].endPoint;

module.exports = releaseMgr;

function releaseMgr() {
    this.getEmptyLottery = function(issue, date) {
        return {
            issue: issue,
            date:  date,
            scheme:'',
            pool:0,
            bet:0,
            bonus: [
                0, 0, 0, 0, 0, 3000, 0, 200, 0, 10, 0, 5
            ],
            details: '',
        };
    }

    this.getRandomNumber = function(max) {
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

    // Get a formated date string from "\/Date(1476720000000+0800)\/".
    this.getDataFromMSTime = function(jsondate) {     
        jsondate = jsondate.replace("/Date(", "").replace(")/", "");     
        if (jsondate.indexOf("+") > 0) {    
            jsondate = jsondate.substring(0, jsondate.indexOf("+"));     
        }     
        else if (jsondate.indexOf("-") > 0) {    
            jsondate = jsondate.substring(0, jsondate.indexOf("-"));     
        }     
        
        return new Date(parseInt(jsondate, 10));
    } 
}

releaseMgr.prototype = {
    getLastReleaseData: function (req, res) {
        self = this;

        // get issue
        var urlParams = url.parse(req.originalUrl, true).query; 

        var options = {
            'url': endPoint + '/GetLatestRelease',
        };
            
        request(options.url, function postResponse(err, response, body) {
        
            if (err) {
                return res.status(400).json({error: err});
            }
            
            if (response && response.statusCode === 200) {
                var result = JSON.parse(body);

                // correct the MS date string        
                result.next.date = self.getDataFromMSTime(result.next.date);
                result.next.cutOffTime = self.getDataFromMSTime(result.next.cutOffTime);
                result.lottery.date = self.getDataFromMSTime(result.lottery.date);
                
                console.log("SUCCESS: get latet release data" + body);
                res.status (200).json({error: null, data: result});
            } else {
                res.status(400).json({error: 'failed to get the last release.'});
            }
        });
    },    
    buildNewReleaseData: function (req, res) {
        self = this;

        if (req.body === undefined) {
            return res.status(400).json({err: 'invalid params!'});
        }
        
        var options = {
            url: endPoint + '/CalcualateNextReleaseInfo/?currentIssue=' + req.body.issue + '&currentDate=' + req.body.date
        };
            
        request(options.url, function postResponse(err, response, body) {
        
            if (err) {
                return res.status(400).json({error: err});
            }
            
            if (response && response.statusCode === 200) {
                var nextReleaseInfo = JSON.parse(body);

                // get random recommendation
                var redNums = self.getRandomNumbers(8, 33);
                var blueNums = self.getRandomNumbers(4, 16);

                var newReleaseData = {
                    lottery: self.getEmptyLottery(req.body.issue, req.body.date),
                    next: nextReleaseInfo,
                    recommendation: {
                        redExcludes: redNums.slice(2).sort(function (a, b) { return a > b; }),
                        redIncludes: redNums.slice(0, 2).sort(function (a, b) { return a > b; }),
                        blueIncludes: blueNums.slice(0, 1).sort(function (a, b) { return a > b; }),
                        blueExcludes: blueNums.slice(1).sort(function (a, b) { return a > b; })
                    },
                    dataVersion: undefined // leave emtpy for input.
                };

                // correct the MS date string        
                newReleaseData.next.date = self.getDataFromMSTime(newReleaseData.next.date);
                newReleaseData.next.cutOffTime = self.getDataFromMSTime(newReleaseData.next.cutOffTime);
            
                console.log("SUCCESS: get next release data" + newReleaseData);
                res.status (200).json({error: null, data: newReleaseData});
            } else {
                res.status(400).json({error: 'failed to get next release data.'});
            }
        });
    },
    getOfficalLotteryData: function (req, res) {
        self = this;       
        
        // get issue
        var urlParams = url.parse(req.originalUrl, true).query;
        
        var issue = urlParams.issue;
        if (issue === undefined) {
            return res.status(400).json({err: 'invalid params!'});
        }   
        
        var options = {
            'url': endPoint + '/SyncLotteryToOffical/?issue=' + issue,
        };
            
        request(options.url, function postResponse(err, response, body) {
        
            if (err) {
                return res.status(400).json({error: err});
            }
            
            if (response && response.statusCode === 200) {
                var result = body ? JSON.parse(body) : undefined;
                
                // correct the MS date string
                result.date = self.getDataFromMSTime(result.date);

                console.log("SUCCESS: get offical lottery data" + body);
                res.status (200).json({error: null, data: result});
            } else {
                res.status(400).json({error: 'failed to get the offical lottery data.'});
            }
        });
    },
    postNotification: function (req, res) {
        self = this;     

        // get platform id
        var urlParams = url.parse(req.originalUrl, true).query;        

        var body = {
            Platforms: req.body.platforms,
            Message: req.body.msg
        };

        var options = {
            url: endPoint + '/PushNotification',
            method: 'POST',
            json: body
        };
            
        request(options, function postResponse(err, response, body) {
        
            if (err) {
                console.error("ERROR:" + 'Failed call on postNotification for: ' + err);
                res.status(400).json({error: err});
                return;
            }
            
            if (response && response.statusCode === 200) {                
                console.log("SUCCESS: " + 'Successful call on postNotification with response: ' + body);
                res.status (200).json({error: null, data: body});
            } else {
                console.error("ERROR:" + 'Failed call on postNotification for code ' + response.statusCode);
                res.status(response.statusCode).json({error: body});
            }
        });
    }
};

