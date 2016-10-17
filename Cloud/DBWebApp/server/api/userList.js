var azure = require('azure-storage'),
    async = require('async'),
    url = require('url');
    
module.exports = UserList;

function UserList(tb) {
    this.table = tb;
}

UserList.prototype = {
    getUsers: function(req, res) {
        self = this;
        
        var query,
            urlParams = url.parse(req.originalUrl, true).query,
            platform = urlParams.platform,
            scope = urlParams.scope || 7;

        if (platform === undefined) {            
            return res.status(400).json("{err: 'invalid params!'}");
        }

        var laterThan = new Date();
        laterThan.setDate(laterThan.getDate() - scope);
        
        query = new azure.TableQuery()
                .where('PartitionKey eq ? && LastLogin >= ?', platform, laterThan)
                .select('DeviceId', 'LastLogin', 'ClientVersion');
            
        self.table.find(query, function itemsFound(error, items) {
            if (error) {
                console.error('ERR ' + 'Fail to get users registered on platform ' + platform + '. Error: ' + error);
                return res.status (200).json({error: error, data: []}); 
            }

            var userList = [];
            items.forEach(function(item) {
                userList.push({
                    DeviceId: item.DeviceId._,
                    LastLogin: item.LastLogin._,
                    ClientVersion: item.ClientVersion._
                });      
            }, this);

            console.log('LOG ' + userList.length + ' users registered on platform ' + platform);
            
            res.status (200).json({error: error, data: userList}); 
        });
    }
};