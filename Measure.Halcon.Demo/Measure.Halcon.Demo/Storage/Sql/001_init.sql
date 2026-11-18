-- ============================================================
--  Measure.Halcon.Demo 数据库结构  001_init
--  目标库：SQLite 3
--  编码：UTF-8（无 BOM）
--
--  L0 阶段：只建 User 一张表，程序运行时不读取、不验证。
--  目的：先把"表结构"这一步单独走通，确认字段设计合理，
--        再进入 L1（登录窗体 + 校验 + 登录审计）。
--
--  约定（与《知识库-数据库需求清单.md》§5 一致）：
--   · 表名 / 列名 PascalCase，不用拼音、不用中文
--   · 时间一律存 UTC 的 ISO8601 文本：2026-11-18T05:45:00Z
--   · 布尔用 INTEGER 0/1（SQLite 无 bool 类型）
--   · 表名 User 在 SQLite 里不是保留字，但 MySQL / SQL Server 下
--     有歧义，所以统一加双引号书写
-- ============================================================

PRAGMA foreign_keys = ON;

-- ------------------------------------------------------------
-- User：能登录这台设备的人
--
-- 一行 = 一个账号。它会被 OperationLog.OperatorId、
-- AlarmRecord.AckedBy、Measurement.OperatorId 引用，
-- 所以只禁用（IsEnabled=0），永远不物理删除。
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS "User"
(
    UserId            INTEGER PRIMARY KEY AUTOINCREMENT,  -- 自增主键
    UserName          TEXT    NOT NULL,                   -- 登录名，唯一
    EmployeeNo        TEXT,                               -- 工号（客户现场常按工号找人）
    RealName          TEXT    NOT NULL,                   -- 真实姓名，审计时显示这个
    PasswordHash      TEXT    NOT NULL,                   -- 不可逆哈希，绝不存明文
    PasswordSalt      TEXT    NOT NULL,                   -- 每用户独立盐
    RoleCode          TEXT    NOT NULL DEFAULT 'Operator',-- Operator / Engineer / Admin
    IsEnabled         INTEGER NOT NULL DEFAULT 1,         -- 0 = 禁用（不删）
    FailedLoginCount  INTEGER NOT NULL DEFAULT 0,         -- 连续失败次数，用于锁定
    LockedUntil       TEXT,                               -- 锁定到期时间，NULL = 未锁
    PasswordChangedAt TEXT,                               -- 密码最后修改时间（有效期判断）
    LastLoginAt       TEXT,                               -- 最后登录时间
    CreatedAt         TEXT    NOT NULL,                   -- 建号时间
    Note              TEXT                                -- 备注
);

-- 登录名唯一：代码里查重不可靠，约束必须建在库里
CREATE UNIQUE INDEX IF NOT EXISTS UX_User_UserName
    ON "User" (UserName);

-- 按角色筛人（"列出所有工程师"）时才用得上，暂时可以不加
-- CREATE INDEX IF NOT EXISTS IX_User_RoleCode ON "User" (RoleCode);
