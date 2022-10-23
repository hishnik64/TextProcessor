CREATE TABLE [dbo].[Words] (
    [Id]      INT           IDENTITY (1, 1) NOT NULL,
    [word]    NVARCHAR (50) NOT NULL,
    [quanity] INT           NOT NULL DEFAULT 1,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

