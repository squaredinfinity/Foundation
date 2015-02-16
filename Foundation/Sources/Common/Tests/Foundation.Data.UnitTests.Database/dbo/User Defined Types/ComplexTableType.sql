CREATE TYPE [dbo].[ComplexTableType] AS TABLE (
    [number] INT          NOT NULL,
    [txt]    NVARCHAR (5) NULL,
    PRIMARY KEY CLUSTERED ([number] ASC));

