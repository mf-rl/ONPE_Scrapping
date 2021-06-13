IF OBJECT_ID('dbo.pe_Ubigeos', 'U') IS NOT NULL
	drop table pe_Ubigeos
GO
create table pe_Ubigeos (
	ubigeo_codigo varchar(6),
	ubigeo_descripcion varchar(100),
	ubigeo_padre varchar(6)
)
GO

IF OBJECT_ID('dbo.pe_Locales', 'U') IS NOT NULL
	drop table pe_Locales
GO
create table pe_Locales (
	local_codigo varchar(20),
	local_ubigeo varchar(6),
	local_nombre varchar(200),
	local_direccion varchar(200)
)
GO

IF OBJECT_ID('dbo.pe_Mesas', 'U') IS NOT NULL
	drop table pe_Mesas
GO
create table pe_Mesas (
	local_ubigeo varchar(6),
	local_codigo varchar(20),
	mesa_numero varchar(20),
	mesa_procesado int,
	mesa_imagen varchar(max)
)
GO

IF OBJECT_ID('dbo.pe_Actas', 'U') IS NOT NULL
	drop table pe_Actas
GO
create table pe_Actas (
	mesa_numero varchar(20),
	acta_numero varchar(20),
	acta_imagen varchar(max),
	habiles_numero int,
	votantes_numero int
)
GO

IF OBJECT_ID('dbo.pe_Votos', 'U') IS NOT NULL
	drop table pe_Votos
GO
create table pe_Votos (
	mesa_numero varchar(20),
	acta_numero varchar(20),
	auto_nombre varchar(max),
	lista_numero int,
	votos_total int
)
GO

IF (OBJECT_ID('dbo.pe_PurgeData', 'P') IS NOT NULL)
	drop procedure pe_PurgeData
GO
create procedure pe_PurgeData
as
begin
	set nocount on;
	truncate table pe_Ubigeos
	truncate table pe_Locales
	truncate table pe_Mesas
	truncate table pe_Actas
	truncate table pe_Votos
end
GO