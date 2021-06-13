IF OBJECT_ID('dbo.pe_Ubigeos', 'U') IS NOT NULL
	drop table pe_Ubigeos
GO
create table pe_Ubigeos (
	ubigeo_codigo varchar(6),
	ubigeo_descripcion varchar(100),
	ubigeo_padre varchar(6),
	eleccion char(1),
	nivel int,
	ambito char(1)
)
GO

IF OBJECT_ID('dbo.pe_Locales', 'U') IS NOT NULL
	drop table pe_Locales
GO
create table pe_Locales (
	local_codigo varchar(20),
	local_ubigeo varchar(6),
	local_nombre varchar(200),
	local_direccion varchar(200),
	eleccion char(1)
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
	mesa_imagen varchar(max),
	eleccion char(1)
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
	votantes_numero int,
	eleccion char(1),
	tipo_proceso char(3)
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
	votos_total int,
	eleccion char(1),
	tipo_proceso char(3)
)
GO

--IF OBJECT_ID('dbo.pe_Proceso', 'U') IS NOT NULL
--	drop table pe_Proceso
--GO
--create table pe_Proceso (
--	proceso_vuelta char(1),
--	proceso_tipo char(3),
--	asistio_no_voto varchar(max),
--	lista_numero int,
--	votos_total int
--)
--GO



IF (OBJECT_ID('dbo.pe_PurgeData', 'P') IS NOT NULL)
	drop procedure pe_PurgeData
GO
create procedure pe_PurgeData (
	@eleccion char(1)
)
as
begin
	set nocount on;
	delete from pe_Ubigeos where eleccion = @eleccion
	delete from pe_Locales where eleccion = @eleccion
	delete from pe_Mesas where eleccion = @eleccion
	delete from pe_Actas where eleccion = @eleccion
	delete from pe_Votos where eleccion = @eleccion
end
GO