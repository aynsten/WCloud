const config={
    baseOAuthUrl:"http://wcloudpassport.agilewater.cn",
    //请求授权地址
    userAuthorizationUri:"http://wcloudpassport.agilewater.cn/connect/authorize",
    //accessToken请求地址
    accessTokenUri : "/connect/authorize",
    //用户信息请求地址
    userInfoUri:"http://wcloud.agilewater.cn:5006/user",
    //登出请求地址
    logoutUri:"http://wcloudpassport.agilewater.cn/Account/Logout?returnUrl=",
    //项目地址
    localuri :"http://wcloud.agilewater.cn/RM/",
    redirect_uri : "http://wcloud.agilewater.cn/RM/login",
    //案例资源服务器地址
    resUri:"http://wcloud.agilewater.cn:5007",
    //客户端相关标识，请从认证服务器申请
    clientId: "wx-imp",
    client_secret:"123",
    //申请的权限范围
    scope:"water",
    //可选参数，客户端的当前状态，可以指定任意值，用于校验，此次案例不做相关认证
    state:"bc950c95926d4ad5a032b6325e1f3d25",
    //一些固定的请求参数
    response_type:"token",
    grant_type : "authorization_code",
    code:"",
    baseUrl:"http://wcloud.agilewater.cn:5007/",
    orgUrl:"http://wcloud.agilewater.cn:5006"
  }
  
export default config;
  