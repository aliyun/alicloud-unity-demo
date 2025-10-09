## 移动研发平台unity Demo

------

阿里云EMAS（移动研发平台）是阿里巴巴移动技术的开放平台，沉淀了阿里巴巴多年移动互联网系统架构积累，汇聚和开放了阿里巴巴移动中台核心技术能力，期望为广大开发者提供稳定、弹性、安全、快速的移动应用基础设施，帮助开发者构建工程化、系统化、智能化的企业级移动互联网研发体系。

移动云产品管理地址：[移动云产品](https://emas.console.aliyun.com/)

SDK下载：请参考EMAS快速入门中 ->下载SDK [地址](https://help.aliyun.com/document_detail/169962.html#title-fvd-ozh-524) 

<!--  > 注：demo中的账密信息配置只为方便demo例程的运行，真实产品中，建议您使用安全黑匣子或其他方式保障密钥的安全性。 -->

### 一、HTTPDNS

------

<div align="center">
<img src="./assets/httpdns_logo.png">
</div>

HTTPDNS使用HTTP协议进行域名解析，代替现有基于UDP的DNS协议，域名解析请求直接发送到阿里云的HTTPDNS服务器，从而绕过运营商的Local DNS，能够避免Local DNS造成的域名劫持问题和调度不精准问题。

- Demo对应目录：[httpdns_unity_demo](https://github.com/aliyun/alicloud-unity-demo)
- [产品官网](https://www.aliyun.com/product/httpdns)

### 二、APM

------

阿里云应用性能监控（APM）为移动端应用提供稳定性观测能力，覆盖崩溃分析、异常监控等关键场景。`apm-unity-demo` 展示了在 Unity 项目中接入 APM SDK 的基础流程，包含初始化、配置与数据上报验证示例，帮助您验证监控链路是否工作正常。

- Demo对应目录：[apm-unity-demo](./apm-unity-demo)
- [产品官网](https://www.aliyun.com/product/emascrash/apm)
