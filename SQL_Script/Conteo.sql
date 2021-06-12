set nocount on;
select * into #ambito from pe_Ubigeo where ubigeo_lv = 1
select * into #departamento from pe_Ubigeo where ubigeo_lv = 2
select * into #provincia from pe_Ubigeo where ubigeo_lv = 3
select * into #distrito from pe_Ubigeo where ubigeo_lv = 4
select * into #local from pe_Ubigeo where ubigeo_lv = 5

--select ubigeo_dc Ambito 
--from #ambito

select 
	a.ubigeo_dc Ambito,
	dp.ubigeo_dc Departamento,
	--p.ubigeo_dc Provincia,
	--d.ubigeo_dc Distrito,
	sum(cast(m.FP as int)) China,
	sum(cast(m.PL as int)) Serrano,
	sum(cast(m.VB as int)) Whites,
	sum(cast(m.VN as int)) Vicios,
	sum(cast(m.VI as int)) Impug
from pe_Mesa m
	inner join #distrito d
		on d.ubigeo_id = m.dis_cod
	inner join #provincia p
		on p.ubigeo_id = m.pro_cod
	inner join #departamento dp
		on dp.ubigeo_id = m.dep_cod
	inner join #ambito a
		on a.ubigeo_id = m.amb_cod
group by 
	a.ubigeo_dc,
	dp.ubigeo_dc--,
	--p.ubigeo_dc,
	--d.ubigeo_dc
order by a.ubigeo_dc,
	dp.ubigeo_dc

select 
	a.ubigeo_dc Ambito,
	sum(cast(m.FP as int)) China,
	sum(cast(m.PL as int)) Serrano,
	sum(cast(m.VB as int)) Whites,
	sum(cast(m.VN as int)) Vicios,
	sum(cast(m.VI as int)) Impug
from pe_Mesa m
	inner join #distrito d
		on d.ubigeo_id = m.dis_cod
	inner join #provincia p
		on p.ubigeo_id = m.pro_cod
	inner join #departamento dp
		on dp.ubigeo_id = m.dep_cod
	inner join #ambito a
		on a.ubigeo_id = m.amb_cod
group by 
	a.ubigeo_dc
	
select 
	'TOTAL' Ambito,
	sum(cast(m.FP as int)) China,
	sum(cast(m.PL as int)) Serrano,
	sum(cast(m.VB as int)) Whites,
	sum(cast(m.VN as int)) Vicios,
	sum(cast(m.VI as int)) Impug
from pe_Mesa m
	inner join #distrito d
		on d.ubigeo_id = m.dis_cod
	inner join #provincia p
		on p.ubigeo_id = m.pro_cod
	inner join #departamento dp
		on dp.ubigeo_id = m.dep_cod
	inner join #ambito a
		on a.ubigeo_id = m.amb_cod


drop table #ambito
drop table #departamento
drop table #provincia
drop table #distrito
drop table #local