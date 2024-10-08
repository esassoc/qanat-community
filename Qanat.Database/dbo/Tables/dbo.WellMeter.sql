CREATE TABLE [dbo].[WellMeter]
(
	[WellMeterID] int not null identity(1,1) constraint [PK_WellMeter_WellMeterID] primary key,
	[WellID] int not null constraint [FK_WellMeter_Well_WellID] foreign key references dbo.[Well]([WellID]),
	[MeterID] int not null constraint [FK_WellMeter_Meter_MeterID] foreign key references dbo.[Meter]([MeterID]),
	[StartDate] datetime not null,
	[EndDate] datetime null
)
