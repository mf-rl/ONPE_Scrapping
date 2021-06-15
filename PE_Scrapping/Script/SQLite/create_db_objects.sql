DROP TABLE IF EXISTS pe_Ubigeos;
create table pe_Ubigeos (
	ubigeo_codigo TEXT,
	ubigeo_descripcion TEXT,
	ubigeo_padre TEXT,
	eleccion char(1),
	nivel intEGER,
	ambito char(1)
);
DROP TABLE IF EXISTS pe_Locales;
create table pe_Locales (
	local_codigo TEXT,
	local_ubigeo TEXT,
	local_nombre TEXT,
	local_direccion TEXT,
	eleccion char(1)
);
DROP TABLE IF EXISTS pe_Mesas;
create table pe_Mesas (
	local_ubigeo TEXT,
	local_codigo TEXT,
	mesa_numero TEXT,
	mesa_procesado intEGER,
	mesa_imagen TEXT,
	eleccion char(1)
);
DROP TABLE IF EXISTS pe_Actas;
create table pe_Actas (
	mesa_numero TEXT,
	acta_numero TEXT,
	acta_imagen TEXT,
	habiles_numero intEGER,
	votantes_numero intEGER,
	eleccion char(1),
	tipo_proceso char(3)
);
DROP TABLE IF EXISTS pe_Votos;
create table pe_Votos (
	mesa_numero TEXT,
	acta_numero TEXT,
	auto_nombre TEXT,
	lista_numero intEGER,
	votos_total intEGER,
	eleccion char(1),
	tipo_proceso char(3)
)