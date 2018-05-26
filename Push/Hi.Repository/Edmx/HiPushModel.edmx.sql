
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/27/2017 15:21:32
-- Generated from EDMX file: D:\Demo\HiPush\Hi.Repository\Edmx\HiPushModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [HiPush];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Application]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Application];
GO
IF OBJECT_ID(N'[dbo].[Message]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Message];
GO
IF OBJECT_ID(N'[dbo].[MessageBody]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MessageBody];
GO
IF OBJECT_ID(N'[dbo].[PushMessage]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PushMessage];
GO
IF OBJECT_ID(N'[dbo].[Device]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Device];
GO
IF OBJECT_ID(N'[dbo].[MessageEntryEvent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MessageEntryEvent];
GO
IF OBJECT_ID(N'[dbo].[MessageResendEvent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MessageResendEvent];
GO
IF OBJECT_ID(N'[dbo].[DeviceInvalidEvent]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeviceInvalidEvent];
GO
IF OBJECT_ID(N'[dbo].[DeviceConnection]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeviceConnection];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Application'
CREATE TABLE [dbo].[Application] (
    [Token] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [AppName] varchar(50)  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [LastUpdateTime] datetime  NOT NULL,
    [IsRemoved] bit  NOT NULL,
    [Status] tinyint  NOT NULL,
    [Version] binary(8)  NOT NULL
);
GO

-- Creating table 'Message'
CREATE TABLE [dbo].[Message] (
    [Token] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [BodyToken] uniqueidentifier  NOT NULL,
    [DeviceType] tinyint  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [LastUpdateTime] datetime  NOT NULL,
    [IsRemoved] bit  NOT NULL,
    [Status] tinyint  NOT NULL,
    [Version] binary(8)  NOT NULL
);
GO

-- Creating table 'MessageBody'
CREATE TABLE [dbo].[MessageBody] (
    [Token] uniqueidentifier  NOT NULL,
    [Title] nvarchar(125)  NOT NULL,
    [Content] nvarchar(4000)  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [LastUpdateTime] datetime  NOT NULL,
    [IsRemoved] bit  NOT NULL,
    [Status] tinyint  NOT NULL,
    [Version] binary(8)  NOT NULL
);
GO

-- Creating table 'PushMessage'
CREATE TABLE [dbo].[PushMessage] (
    [Token] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [MessageId] uniqueidentifier  NOT NULL,
    [DeviceId] uniqueidentifier  NOT NULL,
    [DeviceToken] uniqueidentifier  NOT NULL,
    [DeviceType] tinyint  NOT NULL,
    [NextSendTime] datetime  NULL,
    [CreateTime] datetime  NOT NULL,
    [LastUpdateTime] datetime  NOT NULL,
    [IsRemoved] bit  NOT NULL,
    [Status] tinyint  NOT NULL,
    [Version] binary(8)  NOT NULL
);
GO

-- Creating table 'Device'
CREATE TABLE [dbo].[Device] (
    [Token] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [DeviceToken] uniqueidentifier  NOT NULL,
    [DeviceType] tinyint  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [LastUpdateTime] datetime  NOT NULL,
    [IsRemoved] bit  NOT NULL,
    [Status] tinyint  NOT NULL,
    [Version] binary(8)  NOT NULL
);
GO

-- Creating table 'MessageEntryEvent'
CREATE TABLE [dbo].[MessageEntryEvent] (
    [Token] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [MessageId] uniqueidentifier  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [LastUpdateTime] datetime  NOT NULL,
    [IsRemoved] bit  NOT NULL,
    [Status] tinyint  NOT NULL,
    [Version] binary(8)  NOT NULL
);
GO

-- Creating table 'MessageResendEvent'
CREATE TABLE [dbo].[MessageResendEvent] (
    [Token] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [MessageId] uniqueidentifier  NOT NULL,
    [DeviceId] uniqueidentifier  NOT NULL,
    [DeviceToken] uniqueidentifier  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Content] nvarchar(max)  NOT NULL,
    [EventType] tinyint  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [LastUpdateTime] datetime  NOT NULL,
    [IsRemoved] bit  NOT NULL,
    [Status] tinyint  NOT NULL,
    [Version] binary(8)  NOT NULL
);
GO

-- Creating table 'DeviceInvalidEvent'
CREATE TABLE [dbo].[DeviceInvalidEvent] (
    [Token] uniqueidentifier  NOT NULL,
    [AppId] uniqueidentifier  NOT NULL,
    [DeviceId] uniqueidentifier  NOT NULL,
    [DeviceToken] uniqueidentifier  NOT NULL,
    [CreateTime] datetime  NOT NULL,
    [LastUpdateTime] datetime  NOT NULL,
    [IsRemoved] bit  NOT NULL,
    [Status] tinyint  NOT NULL,
    [Version] binary(8)  NOT NULL
);
GO

-- Creating table 'DeviceConnection'
CREATE TABLE [dbo].[DeviceConnection] (
    [Token] uniqueidentifier  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Token] in table 'Application'
ALTER TABLE [dbo].[Application]
ADD CONSTRAINT [PK_Application]
    PRIMARY KEY CLUSTERED ([Token] ASC);
GO

-- Creating primary key on [Token] in table 'Message'
ALTER TABLE [dbo].[Message]
ADD CONSTRAINT [PK_Message]
    PRIMARY KEY CLUSTERED ([Token] ASC);
GO

-- Creating primary key on [Token] in table 'MessageBody'
ALTER TABLE [dbo].[MessageBody]
ADD CONSTRAINT [PK_MessageBody]
    PRIMARY KEY CLUSTERED ([Token] ASC);
GO

-- Creating primary key on [Token] in table 'PushMessage'
ALTER TABLE [dbo].[PushMessage]
ADD CONSTRAINT [PK_PushMessage]
    PRIMARY KEY CLUSTERED ([Token] ASC);
GO

-- Creating primary key on [Token] in table 'Device'
ALTER TABLE [dbo].[Device]
ADD CONSTRAINT [PK_Device]
    PRIMARY KEY CLUSTERED ([Token] ASC);
GO

-- Creating primary key on [Token] in table 'MessageEntryEvent'
ALTER TABLE [dbo].[MessageEntryEvent]
ADD CONSTRAINT [PK_MessageEntryEvent]
    PRIMARY KEY CLUSTERED ([Token] ASC);
GO

-- Creating primary key on [Token] in table 'MessageResendEvent'
ALTER TABLE [dbo].[MessageResendEvent]
ADD CONSTRAINT [PK_MessageResendEvent]
    PRIMARY KEY CLUSTERED ([Token] ASC);
GO

-- Creating primary key on [Token] in table 'DeviceInvalidEvent'
ALTER TABLE [dbo].[DeviceInvalidEvent]
ADD CONSTRAINT [PK_DeviceInvalidEvent]
    PRIMARY KEY CLUSTERED ([Token] ASC);
GO

-- Creating primary key on [Token] in table 'DeviceConnection'
ALTER TABLE [dbo].[DeviceConnection]
ADD CONSTRAINT [PK_DeviceConnection]
    PRIMARY KEY CLUSTERED ([Token] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------