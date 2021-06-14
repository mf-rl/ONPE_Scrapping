delete from pe_Actas 
where mesa_numero || '-' || acta_numero in 
(
SELECT a.mesa_numero || '-' || a.acta_numero from pe_Actas a 
	inner join pe_Mesas m 
		on a.mesa_numero = m.mesa_numero 
		and a.eleccion = m.eleccion 
where m.local_ubigeo like @level || '%' 
and a.eleccion = @eleccion) 
and eleccion = @eleccion; 

delete from pe_Votos 
where mesa_numero || '-' || acta_numero in 
( 
select v.mesa_numero || '-' || v.acta_numero from pe_Votos v 
	inner join pe_Mesas m 
		on v.mesa_numero = m.mesa_numero 
		and v.eleccion = m.eleccion 
where m.local_ubigeo like @level || '%' 
and v.eleccion = @eleccion) 
and eleccion = @eleccion; 
 
delete from pe_Locales 
where local_ubigeo like @level || '%' and eleccion = @eleccion;

delete from pe_Mesas 
where local_ubigeo like @level || '%' and eleccion = @eleccion;