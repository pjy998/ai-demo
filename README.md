# Blog Backend - ABP Framework 博客后端项目

## 项目概述

这是一个基于 ABP Framework 8.3.1 和 .NET 8.0 构建的博客后端系统。项目采用了领域驱动设计（DDD）架构模式，提供了完整的博客管理功能。

## 技术栈

- **框架**: ABP Framework 8.3.1
- **运行时**: .NET 8.0
- **数据库**: SQLite (开发环境)
- **ORM**: Entity Framework Core 8.0.4
- **身份认证**: OpenIddict
- **API 文档**: Swagger/OpenAPI

## 项目结构

```
aspnet-core/
├── src/
│   ├── BlogBackend.Domain.Shared/           # 共享的领域实体和常量
│   ├── BlogBackend.Domain/                  # 领域实体和业务逻辑
│   ├── BlogBackend.Application.Contracts/   # 应用服务接口
│   ├── BlogBackend.Application/             # 应用服务实现
│   ├── BlogBackend.EntityFrameworkCore/     # 数据访问层
│   ├── BlogBackend.HttpApi/                 # Web API 控制器
│   ├── BlogBackend.HttpApi.Host/            # Web API 主机
│   ├── BlogBackend.HttpApi.Client/          # HTTP 客户端
│   └── BlogBackend.DbMigrator/              # 数据库迁移工具
└── test/
    ├── BlogBackend.Domain.Tests/            # 领域层测试
    ├── BlogBackend.Application.Tests/       # 应用层测试
    ├── BlogBackend.EntityFrameworkCore.Tests/ # 数据访问层测试
    └── BlogBackend.HttpApi.Client.ConsoleTestApp/ # 控制台测试应用
```

## 核心功能

基于需求文档，本系统提供以下核心功能：

### 1. 用户管理
- 用户注册、登录、注销
- 用户资料管理
- 角色权限管理

### 2. 博客管理
- 博客文章的创建、编辑、删除
- 文章分类管理
- 文章标签系统
- 文章发布状态控制

### 3. 评论系统
- 文章评论功能
- 评论审核机制
- 评论回复功能

### 4. 媒体管理
- 图片上传和管理
- 文件存储服务

## 快速开始

### 1. 环境要求

- .NET 8.0 SDK
- Visual Studio 2022 或 VS Code
- SQLite（自动创建）

### 2. 运行项目

1. **克隆项目**
```bash
git clone <repository-url>
cd demo03/aspnet-core
```

2. **构建项目**
```bash
dotnet build
```

3. **运行数据库迁移**
```bash
cd src/BlogBackend.DbMigrator
dotnet run
```

4. **启动 API 服务**
```bash
cd src/BlogBackend.HttpApi.Host
dotnet run
```

5. **访问 API 文档**
打开浏览器访问: https://localhost:44338/swagger

### 3. 默认用户

- **管理员账号**: admin / 1q2w3E*

## 开发文档

项目包含详细的开发文档：

- `docs/requirements.md` - 详细的功能需求文档
- `docs/design.md` - 系统设计和架构文档  
- `docs/tasks.md` - 开发任务分解文档

## 开发历程

### 版本兼容性调整

在开发过程中，我们遇到了版本兼容性问题：

1. **初始问题**: ABP Framework 9.3.1 需要 .NET 9.0，但开发环境是 .NET 8.0
2. **解决方案**: 
   - 将 ABP Framework 版本从 9.3.1 降级到 8.3.1
   - 将目标框架从 net9.0 调整为 net8.0
   - 修复 OpenIddict API 兼容性问题
   - 调整静态资源配置方法

### 主要修复

- **OpenIddict API 兼容性**: 
  - `EndSession` → `Logout`
  - `DeviceAuthorization` → `Device`
- **静态资源配置**: `MapAbpStaticAssets()` → `UseStaticFiles()`
- **Entity Framework Core**: 版本统一为 8.0.4

## API 接口

项目提供 RESTful API 接口，主要包括：

- **Account**: 账户管理 (`/api/account/`)
- **Identity**: 身份管理 (`/api/identity/`)
- **Feature Management**: 功能管理 (`/api/feature-management/`)
- **Permission Management**: 权限管理 (`/api/permission-management/`)
- **Setting Management**: 设置管理 (`/api/setting-management/`)
- **Tenant Management**: 租户管理 (`/api/tenant-management/`)

详细的 API 文档可通过 Swagger UI 查看。

## 测试

运行所有测试：
```bash
dotnet test
```

## 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 打开 Pull Request

## 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详细信息

## 联系方式

如有问题，请通过 Issues 提出或联系项目维护者。

---

*本项目基于 ABP Framework 构建，遵循最佳实践和现代软件开发原则。*
