CREATE TABLE dbo.SchemaVersion
(
    [Version]   INT           NOT NULL PRIMARY KEY,
    ScriptName  NVARCHAR(200) NOT NULL,
    AppliedAt   DATETIME2(3)  NOT NULL,
    AppVersion  NVARCHAR(50)  NULL,
    Checksum    CHAR(64)      NULL,
    AppliedBy   NVARCHAR(100) NULL,
    Note        NVARCHAR(500) NULL
);