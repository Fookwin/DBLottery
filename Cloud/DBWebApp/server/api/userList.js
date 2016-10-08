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
        
        var query;
        
        var urlParams = url.parse(req.originalUrl, true).query;
        
        var devid = urlParams.devid;
        if (devid !== undefined) {
            query = new azure.TableQuery().where('DevId eq ?', devid);
        } else {
            var platform = urlParams.platform;
            if (platform === undefined) {
                res.status(400).json("{err: 'invalid params!'}");
                return;
            }
            
            query = new azure.TableQuery().where('PartitionKey eq ?', platform).top(10);            
        }
            
        self.table.find(query, function itemsFound(error, items) {
           var userList = [];
           items.forEach(function(item) {
              userList.push({
                  Platform: item.PlatForm._,
                  DeviceId: item.DeviceId._,
                  LastLogin: item.LastLogin._,
                  ClientVersion: item.ClientVersion._
               });      
           }, this);
           
           res.status (200).json({error: error, data: userList}); 
        });
    }
};