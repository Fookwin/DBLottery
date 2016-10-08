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
        
        var issue = urlParams.issue;
        if (issue === undefined) {
            res.status(400).json("{err: 'invalid params!'}");
            return;
        }
        
    },
    getDataFromWeb: function (req, res) {
        self = this;       
        
        // get issue
        var urlParams = url.parse(req.originalUrl, true).query;
        
        var issue = urlParams.issue;
        if (issue === undefined) {
            res.status(400).json("{err: 'invalid params!'}");
            return;
        }   
        
        var options = {
            'url': endPoint + '/GetNextReleaseInfo',
        };
            
        request(options.url, function postResponse(err, response, body) {
        
            if (err) {
                res.status(400).json("{err: err}");
                return;
            }
            
            if (response && response.statusCode === 200) {
                var result = JSON.parse(body);
                
                console.log("called " + result);
                res.status (200).json({error: null, data: body});
            }
        });
    }
};

