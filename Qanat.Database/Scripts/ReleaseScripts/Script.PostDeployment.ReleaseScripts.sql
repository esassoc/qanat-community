/*
Post-Deployment Script
--------------------------------------------------------------------------------------
This file is generated on every build, DO NOT modify.
--------------------------------------------------------------------------------------
*/

PRINT N'Qanat.Database - Script.PostDeployment.ReleaseScripts.sql';
GO

:r ".\0170 - Add SCNY Water Measurement Types.sql"
GO
:r ".\0171 - Rename field definitions for Fee Calculator.sql"
GO
:r ".\0172 - Backfill Well data from WellRegistrationMetadatum.sql"
GO
:r ".\0173 - Set SCNY source of record and remove water accounts with no parcels.sql"
GO
:r ".\0174 - Configure Paso Basin Geography.sql"
GO
:r ".\0175 - Populate NewStatementBatch RTE.sql"
GO
:r ".\0175 - Populate StatementTemplate CustomLabels.sql"
GO
:r ".\0176 - Configure Fallow Self Reporting.sql"
GO
:r ".\0177 - Configure GMD3 Geography.sql"
GO
:r ".\0178 - Configure Cover Crop Self Reporting.sql"
GO
:r ".\0179 - Update GMD3 Water Measurement Types.sql"
GO
:r ".\0180 - Setup initial Usage Location Types.sql"
GO
:r ".\0181 - Rename StatementTemplate cols.sql"
GO
:r ".\0182 - Update custom rich texts for ETSGSA.sql"
GO
:r ".\0183 - Set self reporting bits for demo and ETSGSA.sql"
GO
:r ".\0184 - Add baseline usage location history.sql"
GO
:r ".\0185 - Configure cover crop effective precip for ETSGSA.sql"
GO
:r ".\0186 - Populate WaterAccountContact table.sql"
GO
:r ".\0187 - Add Well Types.sql"
GO
:r ".\0188 - Add RTE for Terms of Service.sql"
GO

