module.exports = function(app) {
    
    var azure = require('azure-storage');
    var nconf = require('nconf');
    
    // read configrtion...
    nconf.env().file({file: 'config.json', search : true});
    
    var userTableName = nconf.get('USER_TABLE'),
        partitionKey = nconf.get('STORAGE_PK'),
        accountName = nconf.get('STORAGE_AN'),
        accountKey = nconf.get('STORAGE_AK');
    
    
    // initialize data modules
    var UserList = require('./api/userList.js');
    var UserTable = require('./modules/user-table.js');
    var ReleaseManager = require('./api/releaseMgr.js');
    var _userTable = new UserTable(azure.createTableService(accountName, accountKey), userTableName);
    var _list = new UserList(_userTable);
    
    var _releaseMgr = new ReleaseManager();
    
    /** HTTP GET */
    app.get('/users', _list.getUsers.bind(_list));
    app.get('/release', _releaseMgr.getReleaseData.bind(_releaseMgr));
    app.get('/pre-release/:issue?', _releaseMgr.getDataFromWeb.bind(_releaseMgr));
    app.post('/notify', _releaseMgr.postNotification.bind(_releaseMgr));
};