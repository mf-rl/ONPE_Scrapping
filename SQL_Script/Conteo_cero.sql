set nocount on;
select * into #ambito from pe_Ubigeo where ubigeo_lv = 1
select * into #departamento from pe_Ubigeo where ubigeo_lv = 2
select * into #provincia from pe_Ubigeo where ubigeo_lv = 3
select * into #distrito from pe_Ubigeo where ubigeo_lv = 4
select * into #local from pe_Ubigeo where ubigeo_lv = 5


select 
	a.ubigeo_dc Ambito,
	m.amb_cod,
	dp.ubigeo_dc Departamento,
	m.dep_cod,
	p.ubigeo_dc Provincia,
	m.pro_cod,
	d.ubigeo_dc Distrito,
	m.dis_cod,
	m.loc_cod,
	m.Numero,
	m.FP China,
	m.PL Serrano,
	m.VB Whites,
	m.VN Vicios,
	m.VI Impug
from pe_Mesa m
inner join #distrito d
		on d.ubigeo_id = m.dis_cod
	inner join #provincia p
		on p.ubigeo_id = m.pro_cod
	inner join #departamento dp
		on dp.ubigeo_id = m.dep_cod
	inner join #ambito a
		on a.ubigeo_id = m.amb_cod

where m.FP = '0'


drop table #ambito
drop table #departamento
drop table #provincia
drop table #distrito
drop table #local