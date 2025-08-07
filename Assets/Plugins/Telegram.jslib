mergeInto(LibraryManager.library, {
    JsSetTimeout: function (message) {
        // Create copy of message because it might be deleted before callback is run
        var newMess = "you said "+ UTF8ToString(message);
        SendMessage('JSCallBack', 'OnDataReceived', newMess);       
    },

    InitTeleWebApp: function(){
        Telegram.WebApp.ready();        
    },

    GetUserInfo: function(){
        const user = Telegram.WebApp.initDataUnsafe.user;
        if (user) {
            console.log("User data:", user);
            SendMessage('GetUser', 'CallBackUserData', JSON.stringify(user));
          } else {
            console.warn("No user data available");
            SendMessage('GetUser', 'CallBackUserData', "nodata");
          }   
    },

    OpenInvoice: function(message){  
  
        var newMess = UTF8ToString(message);   
        //Telegram.WebApp.openLink(newMess);
        //SendMessage('Purchase', 'ShowPurchaseLog', "buy success ");    
        Telegram.WebApp.openInvoice(newMess, function(response) {
            if (response.success) {
                //console.log("Invoice opened successfully");
                checkPaymentStatus();
            } else {
                //console.error("Failed to open invoice", response.error);
                //SendMessage('Purchase', 'ShowPurchaseLog', "Failed");
            }
        });
    },

});