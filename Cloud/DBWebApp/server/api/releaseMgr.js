var BasicHttpBinding = require('wcf.js').BasicHttpBinding,
    Proxy = require('wcf.js').Proxy, 
    binding = new BasicHttpBinding(), 
    proxy = new Proxy(binding, "http://dbdataquery.cloudapp.net/DBSqlService.svc"),
    url = require('url'),
    request = require('request');

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
        
        var message = '<Envelope xmlns="http://schemas.xmlsoap.org/soap/envelope/">' +
                        '<Header />' +
                        '<Body>' +
                            '<GetLotteryData xmlns="http://tempuri.org/">' +
                                '<issue>' + issue + '</issue>' +
                            '</GetLotteryData>' +
                        '</Body>' +
                      '</Envelope>';

        proxy.send(message, "http://tempuri.org/ISqlService/GetLotteryData", function(response, ctx) {
            console.log("called " + message);
            res.status (200).json({error: null, data: response});
        });
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
            'url': 'http://localhost:53998/RFxDBManageService.svc/ReadLotteryDataFromWeb/?issue=' + issue,
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

