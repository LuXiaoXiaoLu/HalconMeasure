
IF OBJECT_ID('dbo.SchemaVersion', 'U') is null
BEGIN
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
END

IF NOT EXISTS (SELECT 1 FROM dbo.SchemaVersion where  [Version]=0)
BEGIN
	insert into dbo.SchemaVersion( [Version] ,ScriptName,AppliedAt,AppVersion ,AppliedBy,Note )
	values(0,'000_InitialSchemaVersion.sql',SYSUTCDATETIME(),NULL, SUSER_SNAME(), N'引导表自身，尚无业务表')
END
