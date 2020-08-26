CREATE TABLE [dbo].[WeatherForecastDetail] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [WeatherForecastId] INT           NOT NULL,
    [ElementType]       INT           NOT NULL,
    [ParameterName]     NVARCHAR (50) NULL,
    [ParameterValue]    NVARCHAR (50) NULL,
    [ParameterUnit]     NVARCHAR (50) NULL,
    CONSTRAINT [PK_WeatherForecastDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);

