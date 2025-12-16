# DailyCost - 长期消费均摊追踪器

## 项目概述

### 产品定位
DailyCost 是一款**消费摊销视角**的个人/家庭财务追踪工具。不同于传统记账软件关注"今天花了多少"，本产品关注"为了维持当前生活方式，我每天实际在消耗多少钱"。

### 核心价值
- 将一次性大额消费（手机、电脑）摊销到每日，让用户感知真实的日均消费
- 将订阅服务（视频会员、云存储）折算成日均成本
- 汇总所有消费项，展示"今日总消耗"，帮助用户建立消费认知

### 目标用户
- 希望了解真实消费水平的个人用户
- 需要共享账本的家庭用户

---

## 核心概念定义

### 消费项 (Expense Item)
用户录入的任何需要追踪的消费，包括：

| 类型 | 示例 | 计算方式 |
|------|------|----------|
| 固定资产 | 手机、电脑、家电 | 总价 ÷ 已使用天数 或 总价 ÷ 预计使用天数 |
| 订阅服务 | 视频会员、云存储、软件订阅 | 订阅费用 ÷ 订阅周期天数 |
| 周期性支出 | 房租、保险、健身卡 | 费用 ÷ 周期天数 |

### 日均成本 (Daily Cost)
单个消费项摊销到每天的金额。

### 今日总消耗 (Today's Total)
所有生效中的消费项的日均成本之和，代表用户今天为维持现状需要消耗的总金额。

### 计算模式
系统支持两种计算模式，用户可全局设置或单项设置：

1. **动态摊销**（默认）：日均成本 = 总价 ÷ 已使用天数
   - 特点：每天日均成本都在变化（逐渐降低）
   - 适用：不确定使用期限的物品

2. **固定摊销**：日均成本 = 总价 ÷ 预计使用天数
   - 特点：日均成本固定不变
   - 适用：有明确使用期限的物品或订阅

---

## 功能需求

### P0 - 核心功能（MVP必须）

#### 1. 用户系统
- 用户注册（邮箱/手机号）
- 用户登录/登出
- 密码找回
- 个人信息管理（头像、昵称）
- 用户偏好设置（默认计算模式、货币单位）

#### 2. 消费项管理
- 新增消费项
  - 必填：名称、金额、开始日期、消费类型
  - 选填：分类、预计使用期限、备注、图片
  - 对于订阅类型：订阅周期（月/季/年）、自动续费标记
- 编辑消费项
- 删除消费项（软删除，保留历史）
- 停用/启用消费项（停止使用某物品但保留记录）
- 消费项列表（筛选、排序、搜索）

#### 3. 分类管理
- 系统预设分类：
  - 电子产品（手机、电脑、平板、耳机等）
  - 订阅服务（视频、音乐、软件、云存储等）
  - 生活家居（家电、家具、厨具等）
  - 出行交通（汽车、电动车、自行车等）
  - 服饰穿搭（衣服、鞋子、包包等）
  - 周期账单（房租、物业、保险等）
  - 其他
- 用户自定义分类（增删改）
- 分类图标选择

#### 4. 首页仪表盘
- **今日总消耗**（核心数字，突出显示）
- 消费项列表（按日均成本排序）
  - 每项显示：名称、分类图标、日均成本、已使用天数
- 快速操作入口（添加消费项）
- 计算模式切换入口

#### 5. 数据统计
- 今日/本周/本月总消耗
- 分类占比饼图
- 消费趋势折线图（近30天）

### P1 - 重要功能（第二期）

#### 6. 家庭/团队功能
- 创建家庭组
- 邀请成员（通过邀请码/链接）
- 成员权限管理（管理员/成员）
- 共享消费项（标记为家庭共有）
- 家庭总消耗视图

#### 7. 提醒通知
- 订阅到期提醒（提前N天）
- 续费日历视图
- 异常消费预警（某项日均成本突然变化）

#### 8. 数据导入导出
- 导出为 Excel/CSV
- 数据备份与恢复
- 批量导入

### P2 - 增强功能（第三期）

#### 9. 高级分析
- 年度消费报告
- 资产折旧曲线
- 二手残值估算
- 消费对比（同比/环比）

#### 10. 扩展能力
- 多币种支持
- 多语言支持
- 深色模式
- 自定义主题色
- API 开放（供第三方接入）

---

## 技术架构

### 整体架构

```
┌─────────────────────────────────────────────────────────────────┐
│                         客户端层                                 │
├─────────────────────────────────────────────────────────────────┤
│  Web (Vue 3 + Vite)  │  H5 (同源，响应式)  │  微信小程序 (H5嵌入) │
└─────────────────────────────────────────────────────────────────┘
                                 │
                                 │ HTTPS / RESTful API
                                 ▼
┌─────────────────────────────────────────────────────────────────┐
│                         服务端层                                 │
├─────────────────────────────────────────────────────────────────┤
│                    ASP.NET Core 8 Web API                       │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────────┐    │
│  │ 用户模块  │  │ 消费模块  │  │ 分类模块  │  │ 统计分析模块  │    │
│  └──────────┘  └──────────┘  └──────────┘  └──────────────┘    │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐                      │
│  │ 认证授权  │  │ 文件存储  │  │ 通知服务  │                      │
│  └──────────┘  └──────────┘  └──────────┘                      │
└─────────────────────────────────────────────────────────────────┘
                                 │
                                 │ Entity Framework Core
                                 ▼
┌─────────────────────────────────────────────────────────────────┐
│                         数据层                                   │
├─────────────────────────────────────────────────────────────────┤
│                          MySQL 8.0                              │
└─────────────────────────────────────────────────────────────────┘
```

### 技术栈明细

#### 后端
| 组件 | 技术选型 | 版本 |
|------|----------|------|
| 框架 | ASP.NET Core | 8.0+ |
| ORM | Entity Framework Core | 8.0+ |
| 认证 | ASP.NET Core Identity + JWT | - |
| API文档 | Swagger / OpenAPI | - |
| 日志 | Serilog | - |
| 缓存 | Memory Cache / Redis（可选） | - |
| 对象映射 | AutoMapper | - |
| 验证 | FluentValidation | - |

#### 前端
| 组件 | 技术选型 | 版本 |
|------|----------|------|
| 框架 | Vue | 3.4+ |
| 构建工具 | Vite | 5.0+ |
| 状态管理 | Pinia | 2.0+ |
| 路由 | Vue Router | 4.0+ |
| HTTP客户端 | Axios | - |
| UI框架 | Vant（移动端） / Element Plus（桌面端） | - |
| CSS框架 | TailwindCSS | 3.0+ |
| 图表 | ECharts | - |
| 工具库 | Day.js, Lodash-es | - |

#### 部署
| 组件 | 技术选型 |
|------|----------|
| 容器 | Docker + Docker Compose |
| 反向代理 | Nginx |
| CI/CD | GitHub Actions |
| 云服务 | 阿里云 / 腾讯云 ECS |

---

## 数据库设计

### ER 图

```
┌─────────────┐       ┌─────────────────┐       ┌─────────────┐
│   Users     │       │  ExpenseItems   │       │ Categories  │
├─────────────┤       ├─────────────────┤       ├─────────────┤
│ Id (PK)     │──┐    │ Id (PK)         │    ┌──│ Id (PK)     │
│ Email       │  │    │ UserId (FK)     │◄───┘  │ UserId (FK) │
│ PasswordHash│  └───►│ CategoryId (FK) │◄──────│ Name        │
│ Nickname    │       │ Name            │       │ Icon        │
│ Avatar      │       │ Amount          │       │ IsSystem    │
│ Settings    │       │ ExpenseType     │       │ SortOrder   │
│ CreatedAt   │       │ StartDate       │       │ CreatedAt   │
│ UpdatedAt   │       │ EndDate         │       └─────────────┘
└─────────────┘       │ ExpectedDays    │
       │              │ BillingCycle    │       ┌─────────────┐
       │              │ CalcMode        │       │  Families   │
       │              │ Status          │       ├─────────────┤
       │              │ Note            │       │ Id (PK)     │
       │              │ ImageUrl        │       │ Name        │
       │              │ CreatedAt       │       │ InviteCode  │
       │              │ UpdatedAt       │       │ CreatedAt   │
       │              │ DeletedAt       │       └─────────────┘
       │              └─────────────────┘              │
       │                                               │
       │              ┌─────────────────┐              │
       │              │  FamilyMembers  │              │
       │              ├─────────────────┤              │
       └─────────────►│ UserId (FK)     │◄─────────────┘
                      │ FamilyId (FK)   │
                      │ Role            │
                      │ JoinedAt        │
                      └─────────────────┘
```

### 表结构详细定义

#### Users（用户表）
```sql
CREATE TABLE Users (
    Id CHAR(36) PRIMARY KEY,                    -- GUID
    Email VARCHAR(255) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    Nickname VARCHAR(50),
    Avatar VARCHAR(500),                        -- 头像URL
    DefaultCalcMode TINYINT DEFAULT 0,          -- 0:动态摊销 1:固定摊销
    Currency VARCHAR(10) DEFAULT 'CNY',
    Timezone VARCHAR(50) DEFAULT 'Asia/Shanghai',
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    LastLoginAt DATETIME,
    IsDeleted BOOLEAN DEFAULT FALSE
);
```

#### Categories（分类表）
```sql
CREATE TABLE Categories (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36),                            -- NULL表示系统预设
    Name VARCHAR(50) NOT NULL,
    Icon VARCHAR(100),                          -- 图标标识
    Color VARCHAR(20),                          -- 主题色
    IsSystem BOOLEAN DEFAULT FALSE,             -- 是否系统预设
    SortOrder INT DEFAULT 0,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    IsDeleted BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
```

#### ExpenseItems（消费项表）
```sql
CREATE TABLE ExpenseItems (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    CategoryId CHAR(36),
    FamilyId CHAR(36),                          -- 关联家庭（可选）
    
    -- 基本信息
    Name VARCHAR(100) NOT NULL,
    Amount DECIMAL(12,2) NOT NULL,              -- 金额
    ExpenseType TINYINT NOT NULL,               -- 0:固定资产 1:订阅服务 2:周期支出
    
    -- 时间相关
    StartDate DATE NOT NULL,                    -- 开始日期/购买日期
    EndDate DATE,                               -- 结束日期（停用时设置）
    ExpectedDays INT,                           -- 预计使用天数（固定摊销模式用）
    
    -- 订阅相关
    BillingCycle TINYINT,                       -- 订阅周期: 0:月 1:季 2:年
    AutoRenew BOOLEAN DEFAULT TRUE,             -- 是否自动续费
    NextRenewalDate DATE,                       -- 下次续费日期
    
    -- 计算与状态
    CalcMode TINYINT,                           -- 计算模式 NULL:跟随用户设置 0:动态 1:固定
    Status TINYINT DEFAULT 0,                   -- 0:使用中 1:已停用
    
    -- 附加信息
    Note TEXT,
    ImageUrl VARCHAR(500),
    Tags VARCHAR(500),                          -- JSON数组存储标签
    
    -- 审计字段
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    DeletedAt DATETIME,                         -- 软删除
    
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    INDEX idx_user_status (UserId, Status),
    INDEX idx_user_category (UserId, CategoryId)
);
```

#### Families（家庭组表）
```sql
CREATE TABLE Families (
    Id CHAR(36) PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    InviteCode VARCHAR(20) UNIQUE,              -- 邀请码
    CreatedBy CHAR(36) NOT NULL,
    CreatedAt DATETIME NOT NULL,
    UpdatedAt DATETIME NOT NULL,
    IsDeleted BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (CreatedBy) REFERENCES Users(Id)
);
```

#### FamilyMembers（家庭成员表）
```sql
CREATE TABLE FamilyMembers (
    Id CHAR(36) PRIMARY KEY,
    FamilyId CHAR(36) NOT NULL,
    UserId CHAR(36) NOT NULL,
    Role TINYINT DEFAULT 0,                     -- 0:成员 1:管理员
    JoinedAt DATETIME NOT NULL,
    FOREIGN KEY (FamilyId) REFERENCES Families(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    UNIQUE KEY uk_family_user (FamilyId, UserId)
);
```

#### RefreshTokens（刷新令牌表）
```sql
CREATE TABLE RefreshTokens (
    Id CHAR(36) PRIMARY KEY,
    UserId CHAR(36) NOT NULL,
    Token VARCHAR(500) NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL,
    RevokedAt DATETIME,
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    INDEX idx_token (Token(255))
);
```

---

## API 设计

### 基础约定

- Base URL: `/api/v1`
- 认证方式: Bearer Token (JWT)
- 响应格式: JSON
- 时间格式: ISO 8601

### 统一响应结构

```json
// 成功响应
{
    "success": true,
    "data": { ... },
    "message": null
}

// 错误响应
{
    "success": false,
    "data": null,
    "message": "错误描述",
    "errors": {
        "field": ["错误信息"]
    }
}

// 分页响应
{
    "success": true,
    "data": {
        "items": [...],
        "total": 100,
        "page": 1,
        "pageSize": 20,
        "totalPages": 5
    }
}
```

### API 端点列表

#### 认证模块 `/api/v1/auth`

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| POST | /register | 用户注册 | 否 |
| POST | /login | 用户登录 | 否 |
| POST | /refresh-token | 刷新令牌 | 否 |
| POST | /logout | 登出 | 是 |
| POST | /forgot-password | 忘记密码 | 否 |
| POST | /reset-password | 重置密码 | 否 |

#### 用户模块 `/api/v1/users`

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | /me | 获取当前用户信息 | 是 |
| PUT | /me | 更新用户信息 | 是 |
| PUT | /me/password | 修改密码 | 是 |
| PUT | /me/settings | 更新用户设置 | 是 |
| POST | /me/avatar | 上传头像 | 是 |

#### 分类模块 `/api/v1/categories`

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | / | 获取分类列表（含系统预设） | 是 |
| POST | / | 创建自定义分类 | 是 |
| PUT | /{id} | 更新分类 | 是 |
| DELETE | /{id} | 删除分类 | 是 |
| PUT | /sort | 调整分类排序 | 是 |

#### 消费项模块 `/api/v1/expenses`

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | / | 获取消费项列表 | 是 |
| GET | /{id} | 获取消费项详情 | 是 |
| POST | / | 创建消费项 | 是 |
| PUT | /{id} | 更新消费项 | 是 |
| DELETE | /{id} | 删除消费项 | 是 |
| PUT | /{id}/status | 更新状态（启用/停用） | 是 |
| GET | /export | 导出数据 | 是 |

**消费项列表查询参数**：
```
GET /api/v1/expenses?status=0&categoryId=xxx&keyword=手机&page=1&pageSize=20&sortBy=dailyCost&sortOrder=desc
```

#### 统计模块 `/api/v1/statistics`

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | /today | 今日消耗汇总 | 是 |
| GET | /overview | 总览数据 | 是 |
| GET | /trend | 消费趋势（近N天） | 是 |
| GET | /by-category | 分类统计 | 是 |

#### 家庭模块 `/api/v1/families`（P1）

| 方法 | 端点 | 描述 | 认证 |
|------|------|------|------|
| GET | / | 获取我的家庭列表 | 是 |
| POST | / | 创建家庭 | 是 |
| GET | /{id} | 获取家庭详情 | 是 |
| PUT | /{id} | 更新家庭信息 | 是 |
| DELETE | /{id} | 解散家庭 | 是 |
| POST | /{id}/invite | 生成邀请码 | 是 |
| POST | /join | 通过邀请码加入 | 是 |
| DELETE | /{id}/members/{userId} | 移除成员 | 是 |
| PUT | /{id}/members/{userId}/role | 更新成员角色 | 是 |

### 关键 API 详细设计

#### 创建消费项
```
POST /api/v1/expenses

Request Body:
{
    "name": "iPhone 15 Pro",
    "amount": 8999.00,
    "expenseType": 0,           // 0:固定资产
    "categoryId": "uuid",
    "startDate": "2024-01-15",
    "expectedDays": 1095,       // 可选，预计3年
    "calcMode": null,           // null:跟随用户设置
    "note": "256G 白色",
    "imageUrl": null
}

Response:
{
    "success": true,
    "data": {
        "id": "uuid",
        "name": "iPhone 15 Pro",
        "amount": 8999.00,
        "expenseType": 0,
        "expenseTypeName": "固定资产",
        "category": {
            "id": "uuid",
            "name": "电子产品",
            "icon": "smartphone"
        },
        "startDate": "2024-01-15",
        "usedDays": 45,
        "expectedDays": 1095,
        "calcMode": 0,
        "dailyCost": 199.98,    // 动态: 8999/45
        "status": 0,
        "note": "256G 白色",
        "createdAt": "2024-01-15T10:30:00Z"
    }
}
```

#### 今日消耗汇总
```
GET /api/v1/statistics/today

Response:
{
    "success": true,
    "data": {
        "totalDailyCost": 156.78,           // 今日总消耗
        "activeItemCount": 23,               // 生效中的消费项数量
        "breakdown": {                       // 分类细分
            "电子产品": 45.60,
            "订阅服务": 12.30,
            "生活家居": 35.88,
            "周期账单": 63.00
        },
        "topItems": [                        // 日均成本Top5
            {
                "id": "uuid",
                "name": "MacBook Pro",
                "dailyCost": 25.30,
                "category": "电子产品"
            },
            ...
        ],
        "calculatedAt": "2024-03-01T00:00:00Z"
    }
}
```

---

## 前端页面设计

### 页面结构

```
/                       # 首页（仪表盘）
/login                  # 登录
/register               # 注册
/forgot-password        # 忘记密码

/expenses               # 消费项列表
/expenses/add           # 添加消费项
/expenses/:id           # 消费项详情
/expenses/:id/edit      # 编辑消费项

/categories             # 分类管理
/statistics             # 数据统计

/family                 # 家庭管理（P1）
/family/:id             # 家庭详情

/settings               # 设置
/settings/profile       # 个人信息
/settings/preferences   # 偏好设置
/settings/export        # 数据导出
```

### 首页（仪表盘）设计

```
┌─────────────────────────────────────────────┐
│  DailyCost                         ⚙️ 👤    │
├─────────────────────────────────────────────┤
│                                             │
│         今日消耗                             │
│         ¥ 156.78                            │
│         ──────────                          │
│         共 23 项生效中                       │
│                                             │
│  ┌─────────────────────────────────────┐   │
│  │ 📊 分类占比                    查看 > │   │
│  │ ┌─────┐                              │   │
│  │ │ 饼图 │  电子产品 29%              │   │
│  │ │     │  订阅服务  8%               │   │
│  │ └─────┘  生活家居 23%               │   │
│  │          周期账单 40%               │   │
│  └─────────────────────────────────────┘   │
│                                             │
│  ┌─────────────────────────────────────┐   │
│  │ 📱 MacBook Pro 14寸                  │   │
│  │    电子产品 · 已用 320 天             │   │
│  │                         ¥25.30/天    │   │
│  ├─────────────────────────────────────┤   │
│  │ 🏠 房租                              │   │
│  │    周期账单 · 月付                    │   │
│  │                         ¥63.00/天    │   │
│  ├─────────────────────────────────────┤   │
│  │ 📺 Netflix                           │   │
│  │    订阅服务 · 月付                    │   │
│  │                          ¥3.20/天    │   │
│  ├─────────────────────────────────────┤   │
│  │           查看全部消费项 >            │   │
│  └─────────────────────────────────────┘   │
│                                             │
├─────────────────────────────────────────────┤
│   🏠        📝        📊        👤          │
│   首页      消费       统计      我的        │
└─────────────────────────────────────────────┘
```

### 添加消费项页面

```
┌─────────────────────────────────────────────┐
│  ← 添加消费项                               │
├─────────────────────────────────────────────┤
│                                             │
│  消费类型                                    │
│  ┌─────────┬─────────┬─────────┐           │
│  │ 固定资产 │ 订阅服务 │ 周期支出 │           │
│  │   ✓    │         │         │           │
│  └─────────┴─────────┴─────────┘           │
│                                             │
│  名称 *                                     │
│  ┌─────────────────────────────────────┐   │
│  │ iPhone 15 Pro                       │   │
│  └─────────────────────────────────────┘   │
│                                             │
│  金额 *                                     │
│  ┌─────────────────────────────────────┐   │
│  │ ¥ 8999.00                           │   │
│  └─────────────────────────────────────┘   │
│                                             │
│  分类                                       │
│  ┌─────────────────────────────────────┐   │
│  │ 📱 电子产品                      >   │   │
│  └─────────────────────────────────────┘   │
│                                             │
│  开始日期 *                                 │
│  ┌─────────────────────────────────────┐   │
│  │ 2024-01-15                      📅  │   │
│  └─────────────────────────────────────┘   │
│                                             │
│  预计使用期限                                │
│  ┌─────────────────────────────────────┐   │
│  │ 3 年 (1095天)                    >   │   │
│  └─────────────────────────────────────┘   │
│  💡 不填则按已使用天数计算日均成本            │
│                                             │
│  计算模式                                    │
│  ┌─────────────────────────────────────┐   │
│  │ ○ 跟随全局设置                       │   │
│  │ ○ 动态摊销（按已使用天数）            │   │
│  │ ○ 固定摊销（按预计使用期限）          │   │
│  └─────────────────────────────────────┘   │
│                                             │
│  备注                                       │
│  ┌─────────────────────────────────────┐   │
│  │ 256G 原色钛金属                      │   │
│  └─────────────────────────────────────┘   │
│                                             │
│  ┌─────────────────────────────────────┐   │
│  │             保  存                   │   │
│  └─────────────────────────────────────┘   │
│                                             │
└─────────────────────────────────────────────┘
```

### 设计规范

#### 颜色系统
```css
/* 主色调 - 沉稳专业 */
--primary: #2563EB;          /* 主色：靛蓝 */
--primary-light: #3B82F6;
--primary-dark: #1D4ED8;

/* 语义色 */
--success: #10B981;          /* 绿色 */
--warning: #F59E0B;          /* 琥珀 */
--danger: #EF4444;           /* 红色 */
--info: #6366F1;             /* 紫色 */

/* 中性色 */
--gray-50: #F9FAFB;
--gray-100: #F3F4F6;
--gray-200: #E5E7EB;
--gray-300: #D1D5DB;
--gray-400: #9CA3AF;
--gray-500: #6B7280;
--gray-600: #4B5563;
--gray-700: #374151;
--gray-800: #1F2937;
--gray-900: #111827;

/* 背景色 */
--bg-primary: #FFFFFF;
--bg-secondary: #F9FAFB;
--bg-tertiary: #F3F4F6;
```

#### 字体规范
```css
/* 字体家族 */
font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 
             'Helvetica Neue', Arial, 'PingFang SC', 'Microsoft YaHei', sans-serif;

/* 字号 */
--text-xs: 12px;
--text-sm: 14px;
--text-base: 16px;
--text-lg: 18px;
--text-xl: 20px;
--text-2xl: 24px;
--text-3xl: 30px;
--text-4xl: 36px;

/* 关键数字（今日消耗）使用 */
--text-display: 48px;
font-weight: 600;
```

#### 间距系统
```css
--spacing-1: 4px;
--spacing-2: 8px;
--spacing-3: 12px;
--spacing-4: 16px;
--spacing-5: 20px;
--spacing-6: 24px;
--spacing-8: 32px;
--spacing-10: 40px;
--spacing-12: 48px;
```

#### 圆角
```css
--radius-sm: 4px;
--radius-md: 8px;
--radius-lg: 12px;
--radius-xl: 16px;
--radius-full: 9999px;
```

#### 阴影
```css
--shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
--shadow-md: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
--shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
```

---

## 项目结构

### 后端项目结构

```
DailyCost/
├── src/
│   ├── DailyCost.Api/                    # Web API 项目
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── UsersController.cs
│   │   │   ├── CategoriesController.cs
│   │   │   ├── ExpensesController.cs
│   │   │   └── StatisticsController.cs
│   │   ├── Middlewares/
│   │   ├── Filters/
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Program.cs
│   │
│   ├── DailyCost.Application/            # 应用层
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   ├── User/
│   │   │   ├── Category/
│   │   │   ├── Expense/
│   │   │   └── Statistics/
│   │   ├── Services/
│   │   │   ├── IAuthService.cs
│   │   │   ├── AuthService.cs
│   │   │   ├── IUserService.cs
│   │   │   ├── UserService.cs
│   │   │   ├── ICategoryService.cs
│   │   │   ├── CategoryService.cs
│   │   │   ├── IExpenseService.cs
│   │   │   ├── ExpenseService.cs
│   │   │   ├── IStatisticsService.cs
│   │   │   └── StatisticsService.cs
│   │   ├── Validators/
│   │   └── Mappings/
│   │       └── AutoMapperProfile.cs
│   │
│   ├── DailyCost.Domain/                 # 领域层
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Category.cs
│   │   │   ├── ExpenseItem.cs
│   │   │   ├── Family.cs
│   │   │   ├── FamilyMember.cs
│   │   │   └── RefreshToken.cs
│   │   ├── Enums/
│   │   │   ├── ExpenseType.cs
│   │   │   ├── CalcMode.cs
│   │   │   ├── BillingCycle.cs
│   │   │   └── ExpenseStatus.cs
│   │   └── Interfaces/
│   │       └── IRepository.cs
│   │
│   └── DailyCost.Infrastructure/         # 基础设施层
│       ├── Data/
│       │   ├── AppDbContext.cs
│       │   ├── Configurations/           # EF Core 配置
│       │   └── Migrations/
│       ├── Repositories/
│       └── Services/
│           └── JwtService.cs
│
├── tests/
│   ├── DailyCost.UnitTests/
│   └── DailyCost.IntegrationTests/
│
├── docker/
│   ├── Dockerfile
│   └── docker-compose.yml
│
├── DailyCost.sln
└── README.md
```

### 前端项目结构

```
dailycost-web/
├── public/
│   ├── favicon.ico
│   └── manifest.json              # PWA 配置
│
├── src/
│   ├── api/                       # API 请求
│   │   ├── index.ts               # Axios 实例
│   │   ├── auth.ts
│   │   ├── user.ts
│   │   ├── category.ts
│   │   ├── expense.ts
│   │   └── statistics.ts
│   │
│   ├── assets/                    # 静态资源
│   │   ├── icons/
│   │   ├── images/
│   │   └── styles/
│   │       ├── variables.css      # CSS 变量
│   │       └── global.css
│   │
│   ├── components/                # 公共组件
│   │   ├── common/
│   │   │   ├── AppHeader.vue
│   │   │   ├── AppTabBar.vue
│   │   │   ├── LoadingSpinner.vue
│   │   │   └── EmptyState.vue
│   │   ├── expense/
│   │   │   ├── ExpenseCard.vue
│   │   │   ├── ExpenseForm.vue
│   │   │   └── ExpenseList.vue
│   │   └── charts/
│   │       ├── PieChart.vue
│   │       └── TrendChart.vue
│   │
│   ├── composables/               # 组合式函数
│   │   ├── useAuth.ts
│   │   ├── useExpense.ts
│   │   └── useStatistics.ts
│   │
│   ├── layouts/                   # 布局组件
│   │   ├── DefaultLayout.vue
│   │   └── AuthLayout.vue
│   │
│   ├── router/                    # 路由配置
│   │   └── index.ts
│   │
│   ├── stores/                    # Pinia 状态管理
│   │   ├── auth.ts
│   │   ├── user.ts
│   │   ├── category.ts
│   │   └── expense.ts
│   │
│   ├── types/                     # TypeScript 类型
│   │   ├── api.ts
│   │   ├── expense.ts
│   │   └── user.ts
│   │
│   ├── utils/                     # 工具函数
│   │   ├── format.ts              # 格式化
│   │   ├── calculate.ts           # 计算逻辑
│   │   └── storage.ts             # 本地存储
│   │
│   ├── views/                     # 页面组件
│   │   ├── auth/
│   │   │   ├── LoginView.vue
│   │   │   └── RegisterView.vue
│   │   ├── home/
│   │   │   └── HomeView.vue
│   │   ├── expense/
│   │   │   ├── ExpenseListView.vue
│   │   │   ├── ExpenseDetailView.vue
│   │   │   └── ExpenseFormView.vue
│   │   ├── statistics/
│   │   │   └── StatisticsView.vue
│   │   └── settings/
│   │       └── SettingsView.vue
│   │
│   ├── App.vue
│   └── main.ts
│
├── index.html
├── vite.config.ts
├── tailwind.config.js
├── tsconfig.json
├── package.json
└── README.md
```

---

## 部署方案

### Docker Compose 配置

```yaml
# docker-compose.yml
version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: docker/Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=mysql;Database=dailycost;User=root;Password=${MYSQL_ROOT_PASSWORD};
      - Jwt__Secret=${JWT_SECRET}
      - Jwt__Issuer=${JWT_ISSUER}
      - Jwt__Audience=${JWT_AUDIENCE}
    depends_on:
      - mysql
    restart: unless-stopped

  web:
    build:
      context: ./dailycost-web
      dockerfile: Dockerfile
    ports:
      - "80:80"
    depends_on:
      - api
    restart: unless-stopped

  mysql:
    image: mysql:8.0
    ports:
      - "3306:3306"
    environment:
      - MYSQL_ROOT_PASSWORD=${MYSQL_ROOT_PASSWORD}
      - MYSQL_DATABASE=dailycost
    volumes:
      - mysql_data:/var/lib/mysql
    restart: unless-stopped

volumes:
  mysql_data:
```

### Nginx 配置

```nginx
# nginx.conf
server {
    listen 80;
    server_name your-domain.com;
    
    # 前端静态文件
    location / {
        root /usr/share/nginx/html;
        index index.html;
        try_files $uri $uri/ /index.html;
    }
    
    # API 代理
    location /api {
        proxy_pass http://api:80;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

---

## 开发阶段规划

### 第一阶段：MVP（4-6周）

**Week 1-2: 后端基础**
- [ ] 项目初始化，搭建项目结构
- [ ] 数据库设计，EF Core 配置
- [ ] 用户认证模块（注册/登录/JWT）
- [ ] 分类模块 CRUD

**Week 3-4: 后端核心功能**
- [ ] 消费项 CRUD
- [ ] 日均成本计算逻辑
- [ ] 统计 API
- [ ] 单元测试

**Week 5-6: 前端开发**
- [ ] 项目初始化，UI 框架搭建
- [ ] 登录注册页面
- [ ] 首页仪表盘
- [ ] 消费项列表/添加/编辑页面
- [ ] 响应式适配（H5）

### 第二阶段：完善（2-3周）

- [ ] 数据统计页面（图表）
- [ ] 分类管理页面
- [ ] 设置页面
- [ ] PWA 配置
- [ ] 微信小程序 H5 嵌入适配

### 第三阶段：扩展（按需）

- [ ] 家庭功能
- [ ] 提醒通知
- [ ] 数据导入导出
- [ ] 深色模式
- [ ] iOS 原生应用

---

## 核心算法说明

### 日均成本计算

```csharp
public decimal CalculateDailyCost(ExpenseItem item, DateTime today)
{
    // 确定计算模式
    var calcMode = item.CalcMode ?? item.User.DefaultCalcMode;
    
    switch (item.ExpenseType)
    {
        case ExpenseType.FixedAsset:  // 固定资产
            return calcMode == CalcMode.Dynamic
                ? CalculateDynamic(item, today)
                : CalculateFixed(item);
                
        case ExpenseType.Subscription:  // 订阅服务
            return CalculateSubscription(item);
            
        case ExpenseType.Periodic:  // 周期支出
            return CalculatePeriodic(item);
            
        default:
            return 0;
    }
}

// 动态摊销：总价 / 已使用天数
private decimal CalculateDynamic(ExpenseItem item, DateTime today)
{
    var usedDays = (today - item.StartDate).Days + 1;
    return usedDays > 0 ? item.Amount / usedDays : item.Amount;
}

// 固定摊销：总价 / 预计使用天数
private decimal CalculateFixed(ExpenseItem item)
{
    var expectedDays = item.ExpectedDays ?? 365; // 默认1年
    return item.Amount / expectedDays;
}

// 订阅服务：订阅费 / 周期天数
private decimal CalculateSubscription(ExpenseItem item)
{
    var cycleDays = item.BillingCycle switch
    {
        BillingCycle.Monthly => 30,
        BillingCycle.Quarterly => 90,
        BillingCycle.Yearly => 365,
        _ => 30
    };
    return item.Amount / cycleDays;
}

// 周期支出：同订阅服务
private decimal CalculatePeriodic(ExpenseItem item)
{
    return CalculateSubscription(item);
}
```

---

## 给 Claude Code 的开发指引

### 开发优先级
1. 先后端后前端
2. 先核心功能后扩展功能
3. 先单用户后多用户/家庭

### 代码规范
- 后端遵循 .NET 命名规范
- 前端使用 ESLint + Prettier
- 提交信息遵循 Conventional Commits

### 关键注意事项
1. 日均成本计算是核心，务必保证准确性
2. 所有金额使用 `decimal` 类型，保留2位小数
3. 软删除优先，保留用户数据历史
4. API 响应格式统一
5. 做好异常处理和日志记录

### 测试要求
- 核心计算逻辑必须有单元测试
- API 层需要集成测试
- 前端关键流程需要 E2E 测试

---

*文档版本: 1.0*
*创建时间: 2024*
*最后更新: 待定*
