select case 
		when substr(@codigo_ubigeo, 1, 2) || '0000' = @codigo_ubigeo then substr(@codigo_ubigeo, 1, 2) 
		when substr(@codigo_ubigeo, 1, 4) || '00' = @codigo_ubigeo then substr(@codigo_ubigeo, 1, 4) 
		else @codigo_ubigeo END level;