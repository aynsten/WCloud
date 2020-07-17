### 微服务网关

微服务把自己的信息注册到erueka，ocelot网关从erueka中拿到微服务信息，并转发请求

后期可能使用k8s的etcd作为注册发现服务

### 这个站点用来放置vue或者react编译后的前端文件

使用.NET CORE的url rewrite实现友好的访问路径
比如：

xx.com/app-1/xxxx
xx.com/app-2/xxx
参考谷歌：
google.com/maps
google.com/image

为什么所有前端项目放这一个站点？

1.这个站点没有压力，流量大了使用lb挡一层就ok。
2.这么配置访问路径有一个好处，域名统一。浏览器输入xx.com后会自动提示后面的内容