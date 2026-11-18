create table if not exists SchemaVersion
(
 	Version INTERVAL Primary key NOT null,--版本号
	ScriptName TEXT    NOT NULL,
	AppliedAt  TEXT    NOT NULL,--应用时间
	AppVersion TEXT,--执行这次升级的程序版本，售后诊断用
	Checksum   TEXT,--脚本内容的哈希，用于发现「脚本被改过但版本号没动」
    AppliedBy  TEXT,--执行人/系统账号，存文本快照
    Note       TEXT--备注

)