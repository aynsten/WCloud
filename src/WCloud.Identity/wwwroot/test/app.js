
var config = {
    authority: "http://localhost:5001",
    client_id: "wx-code",
    redirect_uri: "http://localhost:5001/test/oidc-callback.html",
    response_type: "code",
    scope: "openid profile water offline_access",
    post_logout_redirect_uri: "http://localhost:5001/account/logout",
};

var mgr = new Oidc.UserManager(config);