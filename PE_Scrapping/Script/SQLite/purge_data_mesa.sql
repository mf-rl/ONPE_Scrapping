delete from pe_Actas where eleccion = @eleccion and mesa_numero = @mesa_numero;
delete from pe_Votos where eleccion = @eleccion and mesa_numero = @mesa_numero;