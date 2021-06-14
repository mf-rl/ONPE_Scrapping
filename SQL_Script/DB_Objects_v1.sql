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

IF (OBJECT_ID('dbo.pe_PurgeData', 'P') IS NOT NULL)
	drop procedure pe_PurgeData
GO
create procedure pe_PurgeData (
	@eleccion char(1)
)
as
begin
	set nocount on;
	delete from pe_Locales where eleccion = @eleccion
	delete from pe_Mesas where eleccion = @eleccion
	delete from pe_Actas where eleccion = @eleccion
	delete from pe_Votos where eleccion = @eleccion
end
GO


IF (OBJECT_ID('dbo.pe_PurgeDataUbigeo', 'P') IS NOT NULL)
	drop procedure pe_PurgeDataUbigeo
GO
create procedure pe_PurgeDataUbigeo (
	@eleccion char(1)
)
as
begin
	set nocount on;
	delete from pe_Ubigeos where eleccion = @eleccion
end
GO

IF (OBJECT_ID('dbo.pe_PurgeDataMesa', 'P') IS NOT NULL)
	drop procedure pe_PurgeDataMesa
GO
create procedure pe_PurgeDataMesa (
	@eleccion char(1),
	@mesa_numero varchar(20)
)
as
begin
	set nocount on;
	delete from pe_Actas where eleccion = @eleccion and mesa_numero = @mesa_numero
	delete from pe_Votos where eleccion = @eleccion and mesa_numero = @mesa_numero
end
GO

IF (OBJECT_ID('dbo.pe_PurgeDataDetalleUbigeo', 'P') IS NOT NULL)
	drop procedure pe_PurgeDataDetalleUbigeo
GO
create procedure pe_PurgeDataDetalleUbigeo (
	@eleccion char(1),
	@codigo_ubigeo varchar(6)
)
as
begin
	set nocount on;
	declare @level varchar(6)
	
	select @level = case 
		when left(@codigo_ubigeo, 2) + '0000' = @codigo_ubigeo then left(@codigo_ubigeo, 2) 
		when left(@codigo_ubigeo, 4) + '00' = @codigo_ubigeo then left(@codigo_ubigeo, 4) 
		else @codigo_ubigeo 
	end

	delete a from pe_Actas a
		inner join pe_Mesas m
			on a.mesa_numero = m.mesa_numero
	where m.local_ubigeo like @level + '%'

	delete v from pe_Votos v
		inner join pe_Mesas m
			on v.mesa_numero = m.mesa_numero
	where m.local_ubigeo like @level + '%'

	delete from pe_Locales where local_ubigeo like @level + '%'

	delete from pe_Mesas where local_ubigeo like @level + '%'
end
GO