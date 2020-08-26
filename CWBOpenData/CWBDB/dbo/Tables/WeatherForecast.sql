CREATE TABLE [dbo].[WeatherForecast] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [LocationId] INT           NOT NULL,
    [StartTime]  DATETIME2 (7) NOT NULL,
    [EndTime]    DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_WeatherForecast] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_WeatherForecast]
    ON [dbo].[WeatherForecast]([LocationId] ASC, [StartTime] DESC, [EndTime] DESC);

