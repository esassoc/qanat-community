CREATE TABLE [dbo].[ModelBoundary]
(
	[ModelBoundaryID] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ModelBoundary_ModelBoundaryID] PRIMARY KEY,
	[ModelID] INT NOT NULL CONSTRAINT [FK_ModelBoundary_Model_ModelID] FOREIGN KEY REFERENCES [dbo].Model([ModelID]),
	[ModelBoundaryGeometry] GEOMETRY NOT NULL
)
