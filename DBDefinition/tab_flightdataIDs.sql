IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tab_flightdataIDs' and xtype='U')
    CREATE TABLE tab_flightdataIDs (
        ID bigint not null PRIMARY KEY IDENTITY(0,1),
		date datetime not null,
		X float not null,
		Y float not null,
		Z int not null,
		Track int null,
		Airplane nvarchar(50) null,
		From nvarchar(10) null,
		To nvarchar(10) null,
		FlightNo nvarchar(50) null,
		FlightNo2 nvarchar(50) null
    )
GO