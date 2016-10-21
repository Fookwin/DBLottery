var url = require('url'),
    request = require('request'),
    global = require('../config/global.js'),
    endPoint = require('../config/config.js')[global.env].endPoint;

module.exports = releaseMgr;

function releaseMgr() {
    
}

releaseMgr.prototype = {
    getReleaseData: function (req, res) {
        self = this;

        // get issue
        var urlParams = url.parse(req.originalUrl, true).query; 

        var options = {
            'url': endPoint + '/GetLatestRelease',
        };

        var issue = urlParams.issue;
        if (issue === undefined) {

        }
            
        request(options.url, function postResponse(err, response, body) {
        
            if (err) {
                return res.status(400).json({error: err});
            }
            
            if (response && response.statusCode === 200) {
                var result = JSON.parse(body);
                
                console.log("SUCCESS: get latet release data" + body);
                res.status (200).json({error: null, data: result});
            } else {
                res.status(400).json({error: 'failed to get the last release.'});
            }
        });
    },
    getOfficalLotteryData: function (req, res) {
        self = this;       
        
        // get issue
        var urlParams = url.parse(req.originalUrl, true).query;
        
        var issue = urlParams.issue;
        if (issue === undefined) {
            res.status(400).json({err: 'invalid params!'});
            return;
        }   
        
        var options = {
            'url': endPoint + '/SyncLotteryToOffical/?issue=' + issue,
        };
            
        request(options.url, function postResponse(err, response, body) {
        
            if (err) {
                res.status(400).json({error: err});
                return;
            }
            
            if (response && response.statusCode === 200) {
                var result = body ? JSON.parse(body) : undefined;
                
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

