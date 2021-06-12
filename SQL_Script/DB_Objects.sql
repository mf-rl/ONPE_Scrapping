drop table pe_Ubigeo
drop table pe_Mesa
CREATE TABLE pe_Ubigeo (
	ubigeo_id varchar(10),
	ubigeo_dc varchar(100),
	ubigeo_lv int
)

create table pe_Mesa (
	Numero varchar(max),
    Procesada bit,
    FP varchar(max),
    PL varchar(max),
    VB varchar(max),
    VN varchar(max),
    VI varchar(max),
    amb_cod varchar(max),
    dep_cod varchar(max),
    pro_cod varchar(max),
    dis_cod varchar(max),
    loc_cod varchar(max),
    save_success bit
)

create procedure insert_Ubigeo (
	@ubigeo_id varchar(10),
	@ubigeo_dc varchar(100),
	@ubigeo_lv int
) AS
BEGIN
	set nocount on;
	insert into pe_Ubigeo 
	(ubigeo_id, ubigeo_dc, ubigeo_lv)
	values (@ubigeo_id, @ubigeo_dc, @ubigeo_lv)
END

create procedure insert_Mesa (
	@Numero varchar(max),
    @Procesada bit,
    @FP varchar(max),
    @PL varchar(max),
    @VB varchar(max),
    @VN varchar(max),
    @VI varchar(max),
    @amb_cod varchar(max),
    @dep_cod varchar(max),
    @pro_cod varchar(max),
    @dis_cod varchar(max),
    @loc_cod varchar(max),
    @save_success bit
) AS
BEGIN
	set nocount on;
	insert into pe_Mesa 
	(
	Numero,
    Procesada,
    FP,
    PL,
    VB,
    VN,
    VI,
    amb_cod,
    dep_cod,
    pro_cod,
    dis_cod,
    loc_cod,
    save_success)
	values (
	@Numero,
    @Procesada,
    @FP,
    @PL,
    @VB,
    @VN,
    @VI,
    @amb_cod,
    @dep_cod,
    @pro_cod,
    @dis_cod,
    @loc_cod,
    @save_success)
END