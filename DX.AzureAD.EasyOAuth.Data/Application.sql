﻿CREATE TABLE [dbo].[Application]
(
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
	[TenantId] UNIQUEIDENTIFIER NOT NULL,
    [Secret] NVARCHAR(100) NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[Origins] NVARCHAR(1000) NULL
)
