(function () {
    "use strict";

    var dataArray = [
    { type: "item", title: "InMobi, Bangalore", picture: "images/inmobi_booth.png" },
    { type: "item", title: "Inmobi at Work", picture: "images/inmobiAtWork.jpg" },
    { type: "item", title: "Awards received", picture: "images/inmobiAtAwards.jpg" },
    { type: "item", title: "Logo", picture: "images/inMobiLogo.jpg" }
    ];

    var dataList = new WinJS.Binding.List(dataArray);

    // Create a namespace to make the data publicly
    // accessible. 
    var publicMembers =
        {
            itemList: dataList
        };
    WinJS.Namespace.define("DataExample", publicMembers);

})();