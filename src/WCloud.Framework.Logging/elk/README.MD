# logstash的redis input

> 暂时不启用
> 如果logstash挂了会导致redis list消息堆积

> 现在还是写文件日志
> 然后以`sidecar`模式运行`filebeat`来收集app的文件日志