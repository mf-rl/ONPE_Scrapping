declare @eleccion char(1) = '2';
with resumen as (
select 
	dep.ubigeo_descripcion departamento,
	prov.ubigeo_descripcion provincia,
	dis.ubigeo_descripcion distrito,
	--t.habiles_numero,
	--t.votantes_numero,
	t.lista_numero,
	t.auto_nombre,
	t.votos_total
from (
	select	
		m.local_ubigeo,
		--sum(a.habiles_numero) habiles_numero,
		--sum(a.votantes_numero) votantes_numero,
		v.auto_nombre,
		v.lista_numero,
		sum(v.votos_total) votos_total
	from (
		select * from pe_Mesas where eleccion = @eleccion
	) m
		inner join (
			select * from pe_Actas where eleccion = @eleccion
		) a 
			on m.mesa_numero = a.mesa_numero
		inner join (
			select * from pe_Votos where eleccion = @eleccion
		) v
			on a.mesa_numero = v.mesa_numero
			and a.acta_numero = v.acta_numero
	group by
		m.local_ubigeo,
		v.auto_nombre,
		v.lista_numero
) t
	inner join (
		select ubigeo_codigo, ubigeo_descripcion, ubigeo_padre  
		from pe_Ubigeos where eleccion = @eleccion and nivel = 3
	) dis
		on t.local_ubigeo = dis.ubigeo_codigo
	inner join (
		select ubigeo_codigo, ubigeo_descripcion, ubigeo_padre 
		from pe_Ubigeos where eleccion = @eleccion and nivel = 2
	) prov
		on dis.ubigeo_padre = prov.ubigeo_codigo
	inner join (
		select ubigeo_codigo, ubigeo_descripcion 
		from pe_Ubigeos where eleccion = @eleccion and nivel = 1
	) dep
		on prov.ubigeo_padre = dep.ubigeo_codigo
) 
select * into #resumen from resumen


select * from #resumen order by 
	departamento,
	provincia, 
	distrito, 
	lista_numero desc
select 
	departamento, 
	--SUM(habiles_numero),
	--SUM(votantes_numero),
	lista_numero,
	auto_nombre,
	SUM(votos_total) votos_total
from #resumen
group by departamento,
	lista_numero,
	auto_nombre
order by 
	lista_numero desc
drop table #resumen
