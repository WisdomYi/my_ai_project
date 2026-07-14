# GitLab & GitLab Duo 精通学习指南

> 🔖 本文档为 MYO-67 任务的交付成果，提供从零基础到精通的 GitLab 及 GitLab Duo 系统化学习路径。

---

## 📋 目录

1. [学习路线图](#-学习路线图)
2. [第一阶段：Git 基础（1-2 周）](#第一阶段git-基础12-周)
3. [第二阶段：GitLab 平台入门（1-2 周）](#第二阶段gitlab-平台入门12-周)
4. [第三阶段：GitLab CI/CD 精通（2-3 周）](#第三阶段gitlab-cicd-精通23-周)
5. [第四阶段：GitLab 高级功能（1-2 周）](#第四阶段gitlab-高级功能12-周)
6. [第五阶段：GitLab Duo AI 能力（2-3 周）](#第五阶段gitlab-duo-ai-能力23-周)
7. [第六阶段：GitLab Duo Agent Platform（1-2 周）](#第六阶段gitlab-duo-agent-platform12-周)
8. [实践项目建议](#-实践项目建议)
9. [资源汇总](#-资源汇总)
10. [速查表](#-速查表)

---

## 🗺️ 学习路线图

```
┌─────────────┐    ┌──────────────┐    ┌───────────────┐
│  Git 基础    │───▶│  GitLab 平台  │───▶│  GitLab CI/CD │
│  (1-2 周)    │    │  (1-2 周)     │    │  (2-3 周)      │
└─────────────┘    └──────────────┘    └───────┬───────┘
                                               │
                                               ▼
┌─────────────┐    ┌──────────────┐    ┌───────────────┐
│ Duo Agent   │◀───│  GitLab Duo  │◀───│  GitLab 高级  │
│ Platform    │    │  AI 能力      │    │  功能         │
│ (1-2 周)    │    │  (2-3 周)     │    │  (1-2 周)      │
└─────────────┘    └──────────────┘    └───────────────┘
```

---

## 第一阶段：Git 基础（1-2 周）

### 1.1 核心概念

| 概念 | 说明 |
|------|------|
| **Repository** | 代码仓库，存储所有文件和版本历史 |
| **Commit** | 一次代码快照，包含变更内容和元信息 |
| **Branch** | 分支，独立开发线，互不干扰 |
| **Merge** | 合并，将不同分支的变更整合 |
| **Remote** | 远程仓库，团队协作的中央存储 |
| **Clone/Pull/Push** | 克隆/拉取/推送，与远程仓库交互 |

### 1.2 必学命令（21 个核心命令）

```bash
# === 配置与初始化 ===
git config --global user.name "Your Name"       # 设置用户名
git config --global user.email "your@email.com" # 设置邮箱
git init                                        # 初始化仓库
git clone <url>                                 # 克隆远程仓库

# === 日常开发 ===
git status                    # 查看工作区状态
git add <file>                # 暂存文件
git add .                     # 暂存所有变更
git commit -m "message"       # 提交暂存的变更
git push origin <branch>      # 推送到远程分支
git pull origin <branch>      # 从远程拉取并合并

# === 分支操作 ===
git branch                    # 列出本地分支
git branch <name>             # 创建新分支
git checkout <branch>         # 切换到分支
git checkout -b <name>        # 创建并切换到新分支
git merge <branch>            # 合并指定分支到当前分支
git branch -d <name>          # 删除分支

# === 历史查看 ===
git log                       # 查看提交历史
git log --oneline             # 紧凑格式
git diff                      # 查看未暂存的变更
git diff --staged             # 查看已暂存的变更

# === 撤销操作 ===
git reset HEAD <file>         # 取消暂存
git checkout -- <file>        # 丢弃工作区变更
git reset --soft HEAD~1       # 撤销最近一次 commit（保留变更）
```

### 1.3 Git 分支策略

```
GitLab Flow（推荐用于 GitLab）:

main ─────●─────●─────●─────●─────●  (生产就绪)
           \          /
feature-a   ●──●──●──┘              (功能分支)
                    \
feature-b            ●──●──●────    (功能分支)
```

**关键原则**：
- `main` 分支始终可部署
- 每个功能/修复使用独立分支
- 通过 Merge Request 合并，而非直接 push
- 合并前必须通过 CI 检查

### 1.4 动手练习

| 练习 | 目标 | 预计时间 |
|------|------|---------|
| [Make your first Git commit](https://docs.gitlab.com/tutorials/make_first_git_commit/) | 掌握基本 Git 工作流 | 30-45 分钟 |
| [Git cheat sheet](https://about.gitlab.com/images/press/git-cheat-sheet.pdf) | 打印速查表，随时参考 | 5 分钟 |
| 在自己的项目中练习分支合并 | 理解分支和冲突解决 | 1 小时 |
| [Take advantage of Git rebase](https://about.gitlab.com/blog/take-advantage-of-git-rebase/) | 学习 rebase 工作流 | 15-20 分钟 |

---

## 第二阶段：GitLab 平台入门（1-2 周）

### 2.1 GitLab 核心概念

| 概念 | 说明 |
|------|------|
| **Project** | 代码仓库 + 问题跟踪 + CI/CD + Wiki |
| **Group** | 项目集合，统一管理权限和设置 |
| **Issue** | 任务跟踪（Bug、功能需求、改进） |
| **Merge Request (MR)** | 代码审查与合并请求 |
| **Pipeline** | CI/CD 自动化流水线 |
| **Runner** | 执行 Pipeline 中 Job 的代理程序 |

### 2.2 平台关键功能

#### Issue 管理
- Issue Board（看板视图）
- Labels（标签分类）
- Milestones（里程碑规划）
- Epic（跨项目史诗级任务，Ultimate 版）
- Weight/Estimation（工作量估算）

#### Merge Request 工作流
```
1. 创建分支 → 2. 提交代码 → 3. 创建 MR → 4. 代码审查 → 5. CI 通过 → 6. 合并
```

#### Code Review 最佳实践
- 使用 Draft MR（WIP）标记未完成的工作
- 在 MR 描述中使用 closing pattern（`Closes #issue`）
- 利用 Suggested Reviewers 功能
- 设置必要审批人（Required Approvers）

### 2.3 动手练习

| 练习 | 目标 | 预计时间 |
|------|------|---------|
| [Navigate the GitLab interface](https://docs.gitlab.com/tutorials/left_sidebar/) | 熟悉 GitLab UI 导航 | 5-10 分钟 |
| [GitLab 101 课程](https://university.gitlab.com/courses/gitlab101) | 系统学习 GitLab 基础 | 1 小时 |
| [Create an issue](https://docs.gitlab.com/tutorials/create_issue_in_existing_project/) | 学会 Issue 使用 | 5-10 分钟 |
| [Set up your organization](https://docs.gitlab.com/tutorials/manage_user/) | 了解 Group/Project 结构 | 30-45 分钟 |
| [Use the personal homepage](https://docs.gitlab.com/tutorials/personal_homepage/) | 掌握工作台功能 | 15-20 分钟 |

---

## 第三阶段：GitLab CI/CD 精通（2-3 周）

### 3.1 CI/CD 核心概念

```
                    .gitlab-ci.yml
                          │
                          ▼
        ┌─────────────────────────────────┐
        │           Pipeline              │
        │                                 │
        │  Stage: build                   │
        │    └── Job: compile             │
        │                                 │
        │  Stage: test                    │
        │    ├── Job: unit-test           │
        │    ├── Job: lint                │
        │    └── Job: security-scan       │
        │                                 │
        │  Stage: deploy                  │
        │    └── Job: deploy-staging      │
        │                                 │
        └─────────────────────────────────┘
```

### 3.2 .gitlab-ci.yml 基础模板

```yaml
# 定义 Pipeline 各阶段执行顺序
stages:
  - build
  - test
  - deploy

# 定义全局变量
variables:
  DOCKER_IMAGE: myapp:latest

# 构建阶段
build-job:
  stage: build
  image: node:20
  script:
    - npm install
    - npm run build
  artifacts:
    paths:
      - dist/

# 测试阶段
unit-test-job:
  stage: test
  image: node:20
  script:
    - npm install
    - npm test
  coverage: '/Statements\s*:\s*(\d+\.\d+)%/'

# Lint 检查
lint-job:
  stage: test
  image: node:20
  script:
    - npm install
    - npm run lint

# 部署阶段 — 仅在 main 分支执行
deploy-job:
  stage: deploy
  image: alpine:latest
  script:
    - echo "Deploying to production..."
  only:
    - main
```

### 3.3 关键 CI/CD 概念

| 概念 | 说明 |
|------|------|
| **Stage** | 阶段，按顺序执行（build → test → deploy） |
| **Job** | 任务，同一 Stage 内的 Job 并行执行 |
| **Runner** | 执行器，运行 Job 的计算资源 |
| **Artifact** | 产物，Job 生成的文件（编译结果、测试报告） |
| **Cache** | 缓存，加速后续 Pipeline 的依赖下载 |
| **Environment** | 环境，管理部署目标（staging/production） |
| **Variable** | 变量，配置和密钥管理 |
| **Rules/Only/Except** | 条件控制，决定何时触发 Job |

### 3.4 CI/CD 最佳实践

- 保持 `.gitlab-ci.yml` 简洁，复杂逻辑用外部脚本
- 使用 `includes` 复用配置（子流水线、模板）
- 合理使用 `cache` 和 `artifacts` 加速构建
- 敏感信息用 CI/CD Variables（Masked/Protected）
- 利用 `rules` 替代 `only/except`（更灵活）
- 多环境部署使用 `environment` 关键字

### 3.5 动手练习

| 练习 | 目标 | 预计时间 |
|------|------|---------|
| [Create your first CI/CD pipeline](https://docs.gitlab.com/ci/quick_start/) | 编写第一份 `.gitlab-ci.yml` | 15-20 分钟 |
| [Create a complex pipeline](https://docs.gitlab.com/ci/quick_start/tutorial/) | 学习常用 CI/CD 关键字 | 30-45 分钟 |
| [GitLab CI Fundamentals 课程](https://university.gitlab.com/learn/learning-path/gitlab-ci-fundamentals) | 系统学习 CI/CD | 3 小时 |
| [Create a project runner](https://docs.gitlab.com/tutorials/create_register_first_runner/) | 理解 Runner 概念 | 15-20 分钟 |
| [CI/CD examples](https://docs.gitlab.com/ci/examples/) | 参考各类实战配置 | 按需 |

---

## 第四阶段：GitLab 高级功能（1-2 周）

### 4.1 安全扫描（DevSecOps）

| 功能 | 说明 | 适用场景 |
|------|------|---------|
| **SAST** | 静态应用安全测试 | 代码层漏洞检测 |
| **DAST** | 动态应用安全测试 | 运行时漏洞检测 |
| **Dependency Scanning** | 依赖扫描 | 第三方库漏洞 |
| **Container Scanning** | 容器镜像扫描 | Docker 镜像安全 |
| **Secret Detection** | 密钥检测 | 防止密码泄露 |
| **License Compliance** | 许可证合规 | 开源许可证管理 |

### 4.2 项目管理

- **Issue Boards**: 看板视图管理任务
- **Milestones**: 按时间节点跟踪进度
- **Epics**: 跨项目的大任务分解（Ultimate）
- **Roadmaps**: 可视化项目路线图
- **OKR**: 目标与关键结果管理
- **Wiki**: 项目文档协作

### 4.3 包管理与容器仓库

- **Package Registry**: 托管 NPM/Maven/PyPI/NuGet 包
- **Container Registry**: 内置 Docker 镜像仓库
- **Terraform Module Registry**: IaC 模块管理

### 4.4 动手练习

| 练习 | 目标 | 预计时间 |
|------|------|---------|
| [Secure your application](https://docs.gitlab.com/tutorials/secure_application/) | 了解安全扫描 | 按需 |
| [Deploy and release your application](https://docs.gitlab.com/tutorials/deploy_release/) | 学习部署流程 | 按需 |
| [Plan and track your work](https://docs.gitlab.com/tutorials/plan_and_track/) | 项目管理实践 | 按需 |

---

## 第五阶段：GitLab Duo AI 能力（2-3 周）

### 5.1 GitLab Duo 套餐概览

| 功能 | Duo Core | Duo Pro ($19/月) | Duo Enterprise |
|------|:---:|:---:|:---:|
| **Code Suggestions** | ✅ | ✅ | ✅ |
| **Duo Chat** | ❌ | ✅ | ✅ |
| **Code Explanation** | ❌ | ✅ | ✅ |
| **Refactor Code** | ❌ | ✅ | ✅ |
| **Fix Code** | ❌ | ✅ | ✅ |
| **Test Generation** | ❌ | ✅ | ✅ |
| **Code Review** | ❌ | ❌ | ✅ |
| **Discussion Summary** | ❌ | ❌ | ✅ |
| **Root Cause Analysis** | ❌ | ❌ | ✅ |
| **Vulnerability Explanation** | ❌ | ❌ | ✅ |
| **Vulnerability Resolution** | ❌ | ❌ | ✅ |
| **MR Summary (Beta)** | ❌ | ❌ | ✅ |
| **Code Review Summary (Beta)** | ❌ | ❌ | ✅ |
| **Issue Description Gen (Beta)** | ❌ | ❌ | ✅ |

> **推荐**: 从 **Duo Pro** 开始，涵盖核心 AI 编程功能；团队协作和安全场景升级到 **Enterprise**。

### 5.2 Code Suggestions（代码补全）

```javascript
// 输入: 函数签名 + 注释
// Duo 自动补全函数体
function calculateDiscount(price, userLevel) {
  // AI 生成:
  const discounts = {
    bronze: 0.05,
    silver: 0.10,
    gold: 0.15,
    platinum: 0.20
  };
  const rate = discounts[userLevel] || 0;
  return price * (1 - rate);
}
```

**最佳实践**:
- 写清晰的函数名和注释作为提示
- 保持上下文文件打开（相关类型定义、接口）
- 接受建议后仍要 Review
- 配合安全扫描使用

### 5.3 Duo Chat（AI 对话）

**对话能力**:
- **解释代码**: 选中代码 → 右键 → "Explain Code"
- **重构代码**: 告诉 Chat 你的重构意图
- **修复 Bug**: 粘贴错误信息和相关代码
- **生成测试**: `/tests` 命令（IDE 中）
- **回答提问**: 如 "这个 MR 改了什么？"

**在 IDE 中的常用命令**:
```
/explain     — 解释选中的代码
/refactor    — 重构选中的代码
/fix         — 修复选中的代码中的问题
/tests       — 为选中的代码生成测试
```

**在 GitLab UI 中**:
- Issue 页面直接向 Duo Chat 提问
- MR 页面获取代码审查帮助
- CI/CD Pipeline 失败时获取根因分析

### 5.4 安全相关 AI 功能（Enterprise）

**Vulnerability Explanation（漏洞解释）**:
- 将复杂的 CVE/安全扫描结果翻译成通俗语言
- 解释漏洞原理：是什么、为什么危险、如何修复

**Vulnerability Resolution（漏洞自动修复）**:
- AI 自动生成修复代码
- 一键创建包含修复的 MR

### 5.5 协作增强 AI 功能（Enterprise）

**Code Review Summary**:
- 自动总结 MR 的变更内容
- 高亮关键改动区域（如认证逻辑、数据库查询变更）

**Discussion Summary**:
- 将长讨论浓缩为要点
- 帮助晚加入者快速了解上下文

**Merge Request Summary（Beta）**:
- 基于代码 diff 自动生成 MR 描述

### 5.6 动手练习

| 练习 | 目标 | 预计时间 |
|------|------|---------|
| [Getting started with Duo Agentic Chat](https://about.gitlab.com/blog/getting-started-with-gitlab-duo-agentic-chat/) | 上手 Duo Chat | 5-10 分钟 |
| [Manage issues with Duo Chat](https://docs.gitlab.com/tutorials/duo_chat_issues/) | 用 Duo Chat 管理 Issue | 15-20 分钟 |
| [Build a web app with Duo](https://docs.gitlab.com/user/gitlab_duo/tutorials/fix_code_python_shop/) | 全套实践 | 1 小时 |
| [Fix a web app with Duo](https://docs.gitlab.com/user/gitlab_duo/tutorials/duo_python_fix_errors/) | 测试与调试 | 1 小时 |
| [10 best practices for Duo Chat](https://forum.gitlab.com/t/10-best-practices-for-using-ai-powered-gitlab-duo-chat/102298) | 最佳实践 | 10 分钟 |
| [Top tips for Code Suggestions](https://forum.gitlab.com/t/top-tips-for-efficient-ai-powered-code-suggestions-with-gitlab-duo/106004) | Code Suggestions 技巧 | 10 分钟 |

---

## 第六阶段：GitLab Duo Agent Platform（1-2 周）

### 6.1 概览

GitLab Duo Agent Platform（2026年1月 GA）是 GitLab 的 **智能体 AI 平台**，允许团队在整个软件生命周期中编排 AI Agent。

**核心能力**:
- **Agentic Chat**: 理解不熟悉的代码、依赖、架构和项目结构
- **Custom Agents**: 创建自定义 Agent 处理特定任务
- **Multi-Agent Flows**: 多个 Agent 协作完成复杂工作流
- **MCP Integration**: 通过 Model Context Protocol 连接外部工具

### 6.2 三种 Agent 类型

| 类型 | 说明 | 示例 |
|------|------|------|
| **Foundational Agent** | GitLab 内置的基础 Agent | Code Review Agent、CI/CD Debug Agent |
| **Custom Agent** | 用户自定义的专用 Agent | 自动生成 Release Notes 的 Agent |
| **External Agent** | 通过 MCP 连接的外部 Agent | Claude Desktop、自定义 LLM 应用 |

### 6.3 Agent 工作流示例

```
用户创建 Issue: "修复支付模块的性能问题"

        ┌──────────────────────┐
        │   Orchestrator Agent │  ← 接收任务，分解子任务
        └──────┬───────────────┘
               │
       ┌───────┼───────┐
       ▼       ▼       ▼
  ┌────────┐┌────────┐┌────────┐
  │ Code   ││ Test   ││ Review │
  │ Analysis││ Agent  ││ Agent  │
  │ Agent  ││        ││        │
  └────┬───┘└───┬────┘└───┬────┘
       │        │         │
       └────────┼─────────┘
                ▼
         ┌──────────┐
         │ 创建 MR   │
         │ + 测试报告 │
         └──────────┘
```

### 6.4 MCP (Model Context Protocol) 集成

MCP 允许 GitLab Duo 连接外部工具和数据源：
- 连接企业内部知识库进行上下文增强
- 连接第三方 API 获取实时数据
- 允许外部 AI 工具（如 Claude Desktop）访问 GitLab 实例

### 6.5 动手练习

| 练习 | 目标 | 预计时间 |
|------|------|---------|
| [Understanding agents](https://about.gitlab.com/blog/understanding-agents-foundational-custom-external/) | 理解 Agent 类型 | 15-20 分钟 |
| [Understanding flows](https://about.gitlab.com/blog/understanding-flows-multi-agent-workflows/) | 创建多 Agent 工作流 | 15-20 分钟 |
| [MCP integration guide](https://about.gitlab.com/blog/duo-agent-platform-with-mcp/) | 连接外部工具 | 15-20 分钟 |
| [Configure a custom agent](https://gitlab.navattic.com/custom-agents) | 交互式体验 | 10 分钟 |
| [Configure a custom flow](https://gitlab.navattic.com/custom-flows) | 交互式体验 | 10 分钟 |
| [Connect Claude Desktop to GitLab MCP](https://docs.gitlab.com/tutorials/connect_claude_desktop_with_gitlab_mcp_server/) | MCP 实战 | 15-20 分钟 |

---

## 🏗️ 实践项目建议

### 项目 1：个人博客 CI/CD（适合阶段 1-3）

```
目标: 从零搭建一个带 CI/CD 的个人博客

步骤:
1. GitLab 创建项目，用 Hugo/Hexo 生成静态博客
2. 编写 .gitlab-ci.yml 自动化构建
3. 配置 GitLab Pages 自动部署
4. 添加 SAST 安全扫描
5. 配置 Merge Request 审批流程
```

### 项目 2：Todo API + Duo 辅助开发（适合阶段 5）

```
目标: 全程使用 GitLab Duo 辅助开发一个 REST API

步骤:
1. 用 Duo Chat 规划项目结构
2. 用 Code Suggestions 加速编码
3. 用 /tests 命令自动生成测试
4. 用 Code Review Summary 审查 MR
5. 用 Vulnerability Explanation 检查安全问题
```

### 项目 3：微服务 DevOps 流水线（适合阶段 4-6）

```
目标: 构建完整的微服务 DevOps 流水线

步骤:
1. 多项目结构 + Group 管理
2. 子流水线（Child Pipeline）编排
3. Docker 构建 + Container Registry
4. Kubernetes 部署配置
5. 创建 Custom Agent 自动生成 Release Notes
6. 配置多 Agent Workflow（构建→测试→部署→通知）
```

---

## 📚 资源汇总

### 官方文档
| 资源 | 链接 |
|------|------|
| GitLab Docs 首页 | https://docs.gitlab.com |
| 所有教程 | https://docs.gitlab.com/tutorials |
| GitLab Duo 文档 | https://docs.gitlab.com/user/gitlab_duo |
| GitLab Duo 功能对比 | https://docs.gitlab.com/user/gitlab_duo/feature_summary |
| Duo Agent Platform | https://docs.gitlab.com/user/duo_agent_platform/ |
| CI/CD 快速入门 | https://docs.gitlab.com/ci/quick_start/ |
| CI/CD 示例集 | https://docs.gitlab.com/ci/examples/ |

### 免费课程（GitLab University）
| 课程 | 预计时间 |
|------|---------|
| [GitLab 101](https://university.gitlab.com/courses/gitlab101) | 1 小时 |
| [GitLab CI Fundamentals](https://university.gitlab.com/learn/learning-path/gitlab-ci-fundamentals) | 3 小时 |
| [GitLab with Git Basics](https://university.gitlab.com/) | 2 小时 |

### 社区与博客
| 资源 | 链接 |
|------|------|
| GitLab Duo Forum | https://forum.gitlab.com/c/gitlab-duo/52 |
| GitLab Blog | https://about.gitlab.com/blog/ |
| Duo Agent Platform 入门 8 篇系列 | https://forum.gitlab.com/t/gitlab-duo-agent-platform-learning-resources/96847 |
| 10 Best Practices for Duo Chat | https://forum.gitlab.com/t/10-best-practices-for-using-ai-powered-gitlab-duo-chat/102298 |

### 视频教程
| 视频 | 时长 |
|------|------|
| [GitLab CI/CD Masterclass (2026)](https://www.youtube.com/watch?v=ptjmtckAIno) | 2 小时 |
| [GitLab Tutorial 2026 (Simplilearn)](https://www.youtube.com/watch?v=kbbSdV7Nvgs) | 按需 |
| [Learn about CI/CD](https://www.youtube.com/watch?v=sIegJaLy2ug) | 9 分钟 |
| [CI deep dive](https://www.youtube.com/watch?v=ZVUbmVac-m8) | 22 分钟 |
| [Understand CI/CD rules](https://www.youtube.com/watch?v=QjQc-zeL16Q) | 8 分钟 |

---

## 📋 速查表

### Git 工作流速查

```bash
# 开始新功能
git checkout -b feature/my-feature main

# 日常提交
git add .
git commit -m "feat: add user authentication"

# 保持同步
git fetch origin
git rebase origin/main          # 或 git merge origin/main

# 推送并创建 MR
git push origin feature/my-feature
# → 在 GitLab UI 中创建 Merge Request

# MR 审查后合并（在 GitLab UI 操作）
# 本地清理
git checkout main
git pull origin main
git branch -d feature/my-feature
```

### CI/CD 常用配置片段

```yaml
# 仅在 MR 时运行
only:
  - merge_requests

# 仅在 main 分支运行
only:
  - main

# 使用 rules（推荐）
rules:
  - if: $CI_PIPELINE_SOURCE == "merge_request_event"
  - if: $CI_COMMIT_BRANCH == "main"

# 缓存 node_modules
cache:
  key: ${CI_COMMIT_REF_SLUG}
  paths:
    - node_modules/

# 手动触发的部署
deploy-prod:
  stage: deploy
  when: manual
  only:
    - main

# 定时 Pipeline
schedule:
  - cron: "0 4 * * *"
```

---

## ✅ 学习检查清单

### Git 基础
- [ ] 能独立完成 clone → branch → commit → push → MR 全流程
- [ ] 理解 merge vs rebase 的区别和使用场景
- [ ] 能解决简单的合并冲突
- [ ] 掌握 `git log`、`git diff`、`git stash` 等调试命令

### GitLab 平台
- [ ] 能创建和管理 Project、Group
- [ ] 能用 Issue Board 跟踪任务
- [ ] 能创建和管理 Merge Request
- [ ] 理解 Draft MR 和审批流程

### GitLab CI/CD
- [ ] 能独立编写 `.gitlab-ci.yml`
- [ ] 理解 Stage → Job → Runner 的层级关系
- [ ] 能配置 Artifacts 和 Cache
- [ ] 能设置多环境部署（staging / production）

### GitLab Duo
- [ ] 能在 IDE 中使用 Code Suggestions
- [ ] 能用 Duo Chat 解释代码、生成测试
- [ ] 能利用 AI 进行代码审查
- [ ] 理解 Vulnerability Explanation/Resolution

### GitLab Duo Agent Platform
- [ ] 理解三种 Agent 类型（Foundational/Custom/External）
- [ ] 能创建自定义 Agent
- [ ] 能配置 Multi-Agent Flow
- [ ] 了解 MCP 集成

---

> 📅 **总预计时间**: 8-13 周（按每周 5-10 小时投入）
>
> 🔗 **任务**: MYO-67 | **状态**: ✅ 已完成

---

*本文档基于 GitLab 官方文档（docs.gitlab.com）及社区资源整理，更新于 2026年7月。*
