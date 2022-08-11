-- tablas



GO
CREATE TABLE CATEGORIA(
	IdCategoria int primary key identity,
	Descripcion varchar(100),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
);
GO

CREATE TABLE MARCA (
	IdMarca int primary key identity,
	Descripcion varchar(100),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
);
GO

CREATE TABLE PRODUCTO (
	IdProducto int primary key identity,
	Nombre varchar(500),
	Descripcion varchar(500),
	IdMarca int references Marca(IdMarca),
	IdCategoria int references Categoria(IdCategoria),
	Precio decimal(10,2) default 0,
	Stock int,
	RutaImagen varchar(100),
	NombreImagen varchar(100),
	Activo bit default 1,
	FechaRegistro datetime default getdate()
);
GO

CREATE TABLE CLIENTE(
	IdCliente int primary key identity,
	Nombres varchar(100),
	Apellidos varchar(100),
	Correo varchar(100),
	Clave varchar(150),
	Reestablecer bit default 1,
	FechaRegistro datetime default getdate()
);
GO

CREATE TABLE CARRITO(
	IdCarrito int primary key identity,
	Nombres varchar(100),
	IdCliente int references CLIENTE(IdCliente),
	IdProducto int references PRODUCTO(IdProducto),
	Cantidad int
);
GO

CREATE TABLE VENTA(
	IdVenta int primary key identity,
	IdCliente int references CLIENTE(IdCliente),
	TotalProducto int,
	MontoTotal decimal(10,2),
	Contacto varchar(50),
	Telefono varchar(50),
	Direccion varchar(500),
	IdTransaccion varchar(50),
	FechaVenta datetime default getdate()
);
GO

CREATE TABLE DETALLE_VENTA(
	IdDetalleVenta int primary key identity,
	IdVenta int references VENTA(IdVenta),
	IdProducto int references PRODUCTO(IdProducto),
	Cantidad int,
	Total decimal(10,2)
);

GO
CREATE TABLE DEPARTAMENTO(
	IdDepartamento varchar(10) NOT NULL,
	Descripcion varchar(45) NOT NULL
);
GO

CREATE TABLE MUNICIPIO (
	IdProvincia int NOT NULL,
	Descripcion varchar(45) NOT NULL,
	IdDepartamento varchar(10) NOT NULL,
);

GO
-- ESTRUCTURAS DE TABLAS
CREATE TYPE [dbo].[EDetalle_Venta] AS TABLE(
	[IdProducto] int NULL,
	[Cantidad] int NULL,
	[Total] decimal(18,2) NULL
)

-- procedimientos almacenados

GO
create procedure sp_RegistrarCliente(
	@Nombres varchar(100),
	@Apellidos varchar(100),
	@Correo varchar(100),
	@Clave varchar(100),
	@Mensaje varchar(500) output,
	@Resultado int output
)
as 
begin
	SET @Resultado = 0
	IF NOT EXISTS (SELECT * FROM CLIENTE WHERE Correo = @Correo)
	begin
		INSERT INTO CLIENTE (Nombres,Apellidos,Correo,Clave,Reestablecer) VALUES (@Nombres,@Apellidos,@Correo,@Clave,0)
		SET @Resultado = scope_identity()
	end
	else
		SET @Mensaje = 'Lo sentimos, este correo ya esta en uso'
end


GO
create procedure sp_ExisteCarrito(
	@IdCliente int,
	@IdProducto int,
	@Resultado bit output
)
as
begin
	IF EXISTS (SELECT * FROM CARRITO WHERE idcliente = @IdCliente and idproducto = @IdProducto)
		SET @Resultado = 1
	ELSE
		SET @Resultado = 0
end


GO
create procedure sp_OperacionCarrito(
	@IdCliente int,
	@IdProducto int,
	@Sumar bit,
	@Mensaje varchar(500) output,
	@Resultado bit output
)
as 
begin
	SET @Resultado = 1
	SET @Mensaje = ''
	declare @existecarrito bit = iif(exists(SELECT * FROM CARRITO WHERE idcliente = @IdCliente and idproducto = @IdProducto),1,0)
	declare @stockproducto int = (SELECT STOCK FROM PRODUCTO WHERE IdProducto = @IdProducto) 
	BEGIN TRY
		BEGIN TRANSACTION OPERACION
			if(@Sumar = 1)
				begin
				if(@stockproducto > 0)
					begin
					if(@existecarrito = 1)
						UPDATE CARRITO SET Cantidad = Cantidad + 1 WHERE idcliente = @IdCliente and idproducto = @IdProducto
					else
						INSERT INTO CARRITO (IdCliente, IdProducto, Cantidad) VALUES (@IdCliente, @IdProducto, 1)
						UPDATE PRODUCTO SET Stock = Stock - 1 WHERE IdProducto = @IdProducto
					end
				else
					begin
						SET @Resultado = 0
						SET @Mensaje = 'Lo sentimos, este producto ya no cuenta con stock disponible'
					end
				end
				else
					begin
						UPDATE CARRITO SET Cantidad = Cantidad - 1 WHERE idcliente = @IdCliente and idproducto = @IdProducto
						UPDATE PRODUCTO SET Stock = Stock + 1 WHERE IdProducto = @IdProducto
					end
				COMMIT TRANSACTION OPERACION
			END TRY
		BEGIN CATCH
			SET @Resultado = 0
			SET @Mensaje = ERROR_MESSAGE()
			ROLLBACK TRANSACTION OPERACION
		END CATCH
end


GO
create function fn_obtenerCarritoCliente(
	@idcliente int
	)
	returns table 
	as
	return(
		SELECT producto. IdProducto, marca.Descripcion[DesMarca], producto.Nombre, producto.Precio, carrito.Cantidad, producto.RutaImagen, producto.NombreImagen FROM CARRITO carrito
		INNER JOIN PRODUCTO producto ON producto.IdProducto = carrito.IdProducto
		INNER JOIN MARCA marca ON marca.IdMarca = producto.IdMarca
		WHERE carrito.IdCliente = @idcliente
	)



GO
create procedure sp_EliminarCarrito(
	@IdCliente int,
	@IdProducto int,
	@Resultado bit output
)
as
begin
	SET @Resultado = 1
	declare @cantidadproducto int = (SELECT cantidad FROM CARRITO WHERE IdCliente = @IdCliente and IdProducto = @IdProducto)
	BEGIN TRY
		BEGIN TRANSACTION OPERACION
			UPDATE PRODUCTO SET Stock = Stock + @cantidadproducto WHERE IdProducto = @IdProducto
			DELETE TOP(1) FROM CARRITO WHERE IdCliente = @IdCliente and IdProducto = @IdProducto
		COMMIT TRANSACTION OPERACION
	END TRY
	BEGIN CATCH 
		SET @Resultado = 0
		ROLLBACK TRANSACTION OPERACION
	END CATCH
end


GO
create procedure usp_RegistrarVenta(
	@IdCliente int,
	@TotalProducto int,
	@MontoTotal decimal(18,2),
	@Contacto varchar(100),
	@Telefono varchar(10),
	@Direccion varchar(100),
	@IdTransaccion varchar(50),
	@DetalleVenta [EDetalle_Venta] READONLY,
	@Resultado bit output,
	@Mensaje varchar(500) output
)
as
begin
	begin try
		declare @idventa int = 0 
		set @Resultado = 1
		set @Mensaje = ''
		begin transaction registro
			insert into venta (IdCliente,TotalProducto,MontoTotal,Contacto,Telefono,Direccion,IdTransaccion) VALUES (@IdCliente,@TotalProducto,@MontoTotal,@Contacto,@Telefono,@Direccion,@IdTransaccion)
			set @idventa = SCOPE_IDENTITY()
			insert into detalle_venta (IdVenta, IdProducto, Cantidad, Total) SELECT @idventa,IdProducto,Cantidad,Total FROM @DetalleVenta
			delete from carrito where IdCliente = @IdCliente
		commit transaction registro
	end try
	begin catch
		set @Resultado = 0
		set @Mensaje = ERROR_MESSAGE()
		rollback transaction registro
	end catch
end

-- funciones

GO

create function fn_ListarCompra(
	@idcliente int
)
RETURNS TABLE
AS
RETURN
(
	select producto.RutaImagen, producto.NombreImagen, producto.Nombre, producto.Precio, detalle.Cantidad, detalle.Total, venta.IdTransaccion  from detalle_venta detalle	
	inner join producto producto on producto.IdProducto = detalle.IdProducto
	inner join venta venta on venta.IdVenta = detalle.IdVenta
	where venta.IdCliente = @idcliente
)



-- inserción datos




GO
insert into categoria VALUES('CALZADO',1,GETDATE());
insert into categoria VALUES('LAPTOPS Y COMPUTADORAS',1,GETDATE());
insert into categoria VALUES('LINEA BLANCA',1,GETDATE());
insert into categoria VALUES('MUEBLES',1,GETDATE());
insert into categoria VALUES('ELECTRODOMESTICOS',1,GETDATE());
insert into categoria VALUES('COSMETICOS Y BELLEZA',1,GETDATE());
insert into categoria VALUES('ENTRETENIMIENTO',1,GETDATE());


GO
insert into marca VALUES ('ADIDAS',1,GETDATE());
insert into marca VALUES ('NIKE',1,GETDATE());
insert into marca VALUES ('VANS',1,GETDATE());
insert into marca VALUES ('HP',1,GETDATE());
insert into marca VALUES ('LENOVO',1,GETDATE());
insert into marca VALUES ('DELL',1,GETDATE());
insert into marca VALUES ('SONY',1,GETDATE());
insert into marca VALUES ('MABE',1,GETDATE());
insert into marca VALUES ('WHIRLPOOL',1,GETDATE());
insert into marca VALUES ('HOME FORNITUNE',1,GETDATE());
insert into marca VALUES ('LOREAL',1,GETDATE());
insert into marca VALUES ('LG',1,GETDATE());

GO


insert into producto VALUES('Nike AIR Retro','Blazer Low x Sacai Iron Grey',3,1,89.99,1500,'C:\Users\dany_\OneDrive\Escritorio\CarritoMVC\FotosProductos','2.jpg',1,GETDATE());
insert into producto VALUES('Nike AIR Jordan Plus','Dunk Hi x Ambush Royal Orange',3,1,105.99,2000,'C:\Users\dany_\OneDrive\Escritorio\CarritoMVC\FotosProductos','3.jpg',1,GETDATE());
insert into producto VALUES('HP Envy Personal','HP ENVY CORE I3, 256GB, 4GB, W11',5,2,600.99,2000,'C:\Users\dany_\OneDrive\Escritorio\CarritoMVC\FotosProductos','4.jpg',1,GETDATE());
insert into producto VALUES('HP VIVOBOOK','INTEL PENTIUM, 512GB, 4GB, W11, 256GB, 4GB, W11',5,2,400.99,2000,'C:\Users\dany_\OneDrive\Escritorio\CarritoMVC\FotosProductos','5.jpg',1,GETDATE());
insert into producto VALUES('Lenovo Legion 5','LENOVO LEGION 5 17ACH, AMD RYZEN 5 5600H 3.3GHZ, 8GB, 256GB, W11, GTX 1650 4GB GDDR6',6,2,1200.99,2000,'C:\Users\dany_\OneDrive\Escritorio\CarritoMVC\FotosProductos','6.jpg',1,GETDATE());
insert into producto VALUES('Microondas','Microondas 110w MABE Oemfit',9,5,54.99,2000,'C:\Users\dany_\OneDrive\Escritorio\CarritoMVC\FotosProductos','7.jpg',1,GETDATE());
insert into producto VALUES('Cocina MABE 18"','Cocina a gas 18" 4 quemadores más horno incluido NO INCLUYE CHISPERO',9,3,250.99,2000,'C:\Users\dany_\OneDrive\Escritorio\CarritoMVC\FotosProductos','8.jpg',1,GETDATE());
insert into producto VALUES('Cocina Eléctrica MABE"','Cocina eléctrica MABE 20" 4 QUEMADORES + CHISPERO INCLUIDO + HORNO',9,3,399.99,2000,'C:\Users\dany_\OneDrive\Escritorio\CarritoMVC\FotosProductos','9.jpg',1,GETDATE());
insert into producto VALUES('Cosmetico DAMAS"','Cosmético tonalidad clara para damas',12,6,39.99,2000,'C:\Users\dany_\OneDrive\Escritorio\CarritoMVC\FotosProductos','9.jpg',1,GETDATE());
insert into producto VALUES('TV LED SONY"','Television LED SONY 75"',8,7,3250.99,2000,'C:\Users\dany_\OneDrive\Escritorio\CarritoMVC\FotosProductos','16.jpg',1,GETDATE());


GO


insert into departamento VALUES ('SV-AH','Ahuachapan')
insert into departamento VALUES ('SV-CA','Cabañas')
insert into departamento VALUES ('SV-CH','Chalatenango')
insert into departamento VALUES ('SV-CU','Cuscatlán')
insert into departamento VALUES ('SV-LI','La Libertad')
insert into departamento VALUES ('SV-MO','Morazán')
insert into departamento VALUES ('SV-PA','La Paz')
insert into departamento VALUES ('SV-SA','Santa Ana')
insert into departamento VALUES ('SV-SM','San Miguel')
insert into departamento VALUES ('SV-SO','Sonsonate')
insert into departamento VALUES ('SV-SS','San Salvador')
insert into departamento VALUES ('SV-SV','San Vicente')
insert into departamento VALUES ('SV-UN','La Unión')
insert into departamento VALUES ('SV-US','Usulutan')


GO


insert into municipio values (1,'Ahuachapan','SV-AH')
insert into municipio values (2,'Apaneca','SV-AH')
insert into municipio values (3,'Atiquizaya','SV-AH')
insert into municipio values (4,'Concepción de Ataco','SV-AH')
insert into municipio values (5,'El Refugio','SV-AH')
insert into municipio values (6,'Guaymango','SV-AH')
insert into municipio values (7,'Jujutla','SV-AH')
insert into municipio values (8,'San Francisco Menéndez','SV-AH')
insert into municipio values (9,'San Lorenzo','SV-AH')
insert into municipio values (10,'San Pedro Puxtla','SV-AH')
insert into municipio values (11,'Tacuba','SV-AH')
insert into municipio values (12,'Turín','SV-AH')


GO


insert into municipio values (13,'Sensuntepeque','SV-CA')
insert into municipio values (14,'Cinquera','SV-CA')
insert into municipio values (15,'Dolores','SV-CA')
insert into municipio values (16,'Guacotecti','SV-CA')
insert into municipio values (17,'Ilobasco ','SV-CA')
insert into municipio values (18,'Jutiapa','SV-CA')
insert into municipio values (19,'San Isidro','SV-CA')
insert into municipio values (20,'Tejutepeque','SV-CA')
insert into municipio values (21,'Victoria','SV-CA')


GO


insert into municipio values (22,'Chalatenango','SV-CH')
insert into municipio values (23,'Agua Caliente','SV-CH')
insert into municipio values (24,'Arcatao','SV-CH')
insert into municipio values (25,'Azacualpa','SV-CH')
insert into municipio values (26,'Cancasque','SV-CH')
insert into municipio values (27,'Citalá','SV-CH')
insert into municipio values (28,'Comalapa','SV-CH')
insert into municipio values (29,'Concepción Quezaltepeque','SV-CH')
insert into municipio values (30,'Dulce Nombre de María','SV-CH')
insert into municipio values (31,'El Carrizal','SV-CH')
insert into municipio values (32,'El Paraíso','SV-CH')
insert into municipio values (33,'La Laguna','SV-CH')
insert into municipio values (34,'La Palma','SV-CH')
insert into municipio values (35,'La Reina','SV-CH')
insert into municipio values (36,'Las Flores','SV-CH')
insert into municipio values (37,'Las Vueltas','SV-CH')
insert into municipio values (38,'Nombre de Jesús','SV-CH')
insert into municipio values (39,'Nueva Concepción ','SV-CH')
insert into municipio values (40,'Nueva Trinidad','SV-CH')
insert into municipio values (41,'Ojos de Agua','SV-CH')
insert into municipio values (42,'Potonico','SV-CH')
insert into municipio values (43,'San Antonio de la Cruz','SV-CH')
insert into municipio values (44,'San Antonio Los Ranchos','SV-CH')
insert into municipio values (45,'San Fernando','SV-CH')
insert into municipio values (46,'San Francisco Lempa','SV-CH')
insert into municipio values (47,'San Francisco Morazán','SV-CH')
insert into municipio values (48,'San Ignacio','SV-CH')
insert into municipio values (49,'San Isidro Labrador','SV-CH')
insert into municipio values (50,'San Luis del Carmen','SV-CH')
insert into municipio values (51,'San Miguel de Mercedes','SV-CH')
insert into municipio values (52,'Santa Rita','SV-CH')
insert into municipio values (53,'San Rafael','SV-CH')
insert into municipio values (54,'Tejutla','SV-CH')



GO



insert into municipio values (55,'Cojutepeque','SV-CU')
insert into municipio values (56,'Candelaria','SV-CU')
insert into municipio values (57,'El Carmen','SV-CU')
insert into municipio values (58,'El Rosario','SV-CU')
insert into municipio values (59,'Monte San Juan','SV-CU')
insert into municipio values (60,'Oratorio de Concepción','SV-CU')
insert into municipio values (61,'San Bartolomé Perulapía','SV-CU')
insert into municipio values (62,'San Cristóbal','SV-CU')
insert into municipio values (63,'San José Guayabal','SV-CU')
insert into municipio values (64,'San Pedro Perulapán','SV-CU')
insert into municipio values (65,'San Rafael Cedros','SV-CU')
insert into municipio values (66,'San Ramón','SV-CU')
insert into municipio values (67,'Santa Cruz Analquito','SV-CU')
insert into municipio values (68,'Santa Cruz Michapa','SV-CU')
insert into municipio values (69,'Suchitoto','SV-CU')
insert into municipio values (70,'Tenancingo','SV-CU')


GO




insert into municipio values (71,'Santa Tecla','SV-LI')
insert into municipio values (72,'Antiguo Cuscatlán','SV-LI')
insert into municipio values (73,'Chiltiupán','SV-LI')
insert into municipio values (74,'Ciudad Arce','SV-LI')
insert into municipio values (75,'Colon','SV-LI')
insert into municipio values (76,'Comasagua','SV-LI')
insert into municipio values (77,'Huizúcar','SV-LI')
insert into municipio values (78,'Jayaque','SV-LI')
insert into municipio values (79,'Jicalapa','SV-LI')
insert into municipio values (80,'La Libertad','SV-LI')
insert into municipio values (81,'Nuevo Cuscatlan','SV-LI')
insert into municipio values (82,'Quezaltepeque','SV-LI')
insert into municipio values (83,'San Juan Opico','SV-LI')
insert into municipio values (84,'Sacacoyo','SV-LI')
insert into municipio values (85,'San José Villanueva','SV-LI')
insert into municipio values (86,'San Matías','SV-LI')
insert into municipio values (87,'San Pablo Tacachico','SV-LI')
insert into municipio values (88,'Talnique','SV-LI')
insert into municipio values (89,'Tamanique','SV-LI')
insert into municipio values (90,'Teotepeque','SV-LI')
insert into municipio values (91,'Tepecoyo','SV-LI')
insert into municipio values (92,'Zaragoza','SV-LI')

GO




insert into municipio values (93,'Zacatecoluca','SV-PA')
insert into municipio values (94,'Cuyultitán','SV-PA')
insert into municipio values (95,'El Rosario','SV-PA')
insert into municipio values (96,'Jerusalén','SV-PA')
insert into municipio values (97,'Mercedes La Ceiba','SV-PA')
insert into municipio values (98,'Olocuilta','SV-PA')
insert into municipio values (99,'Paraíso de Osorio','SV-PA')
insert into municipio values (100,'San Antonio Masahuat','SV-PA')
insert into municipio values (101,'San Emigdio','SV-PA')
insert into municipio values (102,'San Francisco Chinameca','SV-PA')
insert into municipio values (103,'San Pedro Masahuat','SV-PA')
insert into municipio values (104,'San Juan Nonualco','SV-PA')
insert into municipio values (105,'San Juan Talpa','SV-PA')
insert into municipio values (106,'San Juan Tepezontes','SV-PA')
insert into municipio values (107,'San Luis La Herradura','SV-PA')
insert into municipio values (108,'San Luis Talpa','SV-PA')
insert into municipio values (109,'San Miguel Tepezontes','SV-PA')
insert into municipio values (110,'San Pedro Nonualco','SV-PA')
insert into municipio values (111,'San Rafael Obrajuelo','SV-PA')
insert into municipio values (112,'Santa María Ostuma','SV-PA')
insert into municipio values (113,'Santiago Nonualco','SV-PA')
insert into municipio values (114,'Tapalhuaca','SV-PA')



GO



insert into municipio values (115,'La Unión','SV-UN')
insert into municipio values (116,'Anamorós','SV-UN')
insert into municipio values (117,'Bolívar','SV-UN')
insert into municipio values (118,'Concepción de Oriente','SV-UN')
insert into municipio values (119,'Conchagua','SV-UN')
insert into municipio values (120,'El Carmen','SV-UN')
insert into municipio values (121,'El Sauce','SV-UN')
insert into municipio values (122,'El Sauce','SV-UN')
insert into municipio values (123,'Lislique','SV-UN')
insert into municipio values (124,'Meanguera del Golfo','SV-UN')
insert into municipio values (125,'Nueva Esparta','SV-UN')
insert into municipio values (126,'Pasaquina','SV-UN')
insert into municipio values (127,'Polorós','SV-UN')
insert into municipio values (128,'San Alejo','SV-UN')
insert into municipio values (129,'San José','SV-UN')
insert into municipio values (130,'Santa Rosa de Lima','SV-UN')
insert into municipio values (131,'Yayantique','SV-UN')
insert into municipio values (132,'Yucuaiquín','SV-UN')

GO




insert into municipio values (133,'San Francisco Gotera','SV-MO')
insert into municipio values (134,'Arambala','SV-MO')
insert into municipio values (135,'Cacaopera','SV-MO')
insert into municipio values (136,'Chilanga','SV-MO')
insert into municipio values (137,'Corinto','SV-MO')
insert into municipio values (138,'Delicias de Concepción','SV-MO')
insert into municipio values (139,'El Divisadero','SV-MO')
insert into municipio values (140,'El Rosario','SV-MO')
insert into municipio values (141,'Gualococti','SV-MO')
insert into municipio values (142,'Gualococti','SV-MO')
insert into municipio values (143,'Joateca','SV-MO')
insert into municipio values (144,'Jocoaitique','SV-MO')
insert into municipio values (145,'Jocoro','SV-MO')
insert into municipio values (146,'Lolotiquillo','SV-MO')
insert into municipio values (147,'Meanguera','SV-MO')
insert into municipio values (148,'Osicala','SV-MO')
insert into municipio values (149,'Perquín','SV-MO')
insert into municipio values (150,'San Carlos','SV-MO')
insert into municipio values (151,'San Fernando','SV-MO')
insert into municipio values (152,'San Isidro','SV-MO')
insert into municipio values (153,'San Simón','SV-MO')
insert into municipio values (154,'Sensembra','SV-MO')
insert into municipio values (155,'Sociedad','SV-MO')
insert into municipio values (156,'Torola','SV-MO')
insert into municipio values (157,'Yamabal','SV-MO')
insert into municipio values (158,'Yoloaiquín','SV-MO')



GO



insert into municipio values (159,'San Miguel','SV-SM')
insert into municipio values (160,'Carolina','SV-SM')
insert into municipio values (161,'Chapeltique','SV-SM')
insert into municipio values (162,'Chinameca','SV-SM')
insert into municipio values (163,'Chirilagua','SV-SM')
insert into municipio values (164,'Ciudad Barrios','SV-SM')
insert into municipio values (165,'Comacarán','SV-SM')
insert into municipio values (166,'El Tránsito','SV-SM')
insert into municipio values (167,'Lolotique','SV-SM')
insert into municipio values (168,'Moncagua','SV-SM')
insert into municipio values (169,'Nueva Guadalupe','SV-SM')
insert into municipio values (170,'Nuevo Edén de San Juan','SV-SM')
insert into municipio values (171,'Quelepa','SV-SM')
insert into municipio values (172,'San Antonio','SV-SM')
insert into municipio values (173,'San Gerardo','SV-SM')
insert into municipio values (174,'San Jorge','SV-SM')
insert into municipio values (175,'San Luis de la Reina','SV-SM')
insert into municipio values (176,'San Rafael Oriente','SV-SM')
insert into municipio values (177,'Sesori','SV-SM')
insert into municipio values (178,'Uluazapa','SV-SM')



GO



insert into municipio values (179,'San Salvador','SV-SS')
insert into municipio values (180,'Aguilares','SV-SS')
insert into municipio values (181,'Apopa','SV-SS')
insert into municipio values (182,'Ayutuxtepeque','SV-SS')
insert into municipio values (183,'Ciudad Delgado','SV-SS')
insert into municipio values (184,'Cuscatancingo','SV-SS')
insert into municipio values (185,'El Paisnal','SV-SS')
insert into municipio values (186,'Guazapa','SV-SS')
insert into municipio values (187,'Ilopango','SV-SS')
insert into municipio values (188,'Mejicanos','SV-SS')
insert into municipio values (189,'Nejapa','SV-SS')
insert into municipio values (190,'Panchimalco','SV-SS')
insert into municipio values (191,'Panchimalco','SV-SS')
insert into municipio values (192,'San Marcos','SV-SS')
insert into municipio values (193,'San Martín','SV-SS')
insert into municipio values (194,'Santiago Texacuangos','SV-SS')
insert into municipio values (195,'Santo Tomás','SV-SS')
insert into municipio values (196,'Soyapango','SV-SS')
insert into municipio values (197,'Tonacatepeque','SV-SS')




GO




insert into municipio values (198,'San Vicente','SV-SV')
insert into municipio values (199,'Apastepeque','SV-SV')
insert into municipio values (200,'Guadalupe','SV-SV')
insert into municipio values (201,'San Cayetano Istepeque','SV-SV')
insert into municipio values (202,'San Esteban Catarina','SV-SV')
insert into municipio values (203,'San Ildefonso','SV-SV')
insert into municipio values (204,'San Lorenzo','SV-SV')
insert into municipio values (205,'San Sebastián','SV-SV')
insert into municipio values (206,'Santa Clara','SV-SV')
insert into municipio values (207,'Santo Domingo','SV-SV')
insert into municipio values (208,'Tecoluca','SV-SV')
insert into municipio values (209,'Tepetitán','SV-SV')
insert into municipio values (210,'Verapaz','SV-SV')



GO




insert into municipio values (211,'Santa Ana','SV-SA')
insert into municipio values (212,'Candelaria de la Frontera','SV-SA')
insert into municipio values (213,'Chalchuapa','SV-SA')
insert into municipio values (214,'Coatepeque','SV-SA')
insert into municipio values (215,'El Congo','SV-SA')
insert into municipio values (216,'El Porvenir','SV-SA')
insert into municipio values (217,'Masahuat','SV-SA')
insert into municipio values (218,'Metapán','SV-SA')
insert into municipio values (219,'San Antonio Pajonal','SV-SA')
insert into municipio values (220,'San Sebastián Salitrillo','SV-SA')
insert into municipio values (221,'Santa Rosa Guachipilín','SV-SA')
insert into municipio values (222,'Santiago de la Frontera','SV-SA')
insert into municipio values (223,'Texistepeque','SV-SA')



GO



insert into municipio values (224,'Sonsonate','SV-SO')
insert into municipio values (225,'Acajutla','SV-SO')
insert into municipio values (226,'Armenia','SV-SO')
insert into municipio values (227,'Caluco','SV-SO')
insert into municipio values (228,'Cuisnahuat','SV-SO')
insert into municipio values (229,'Izalco','SV-SO')
insert into municipio values (230,'Juayúa','SV-SO')
insert into municipio values (231,'Nahuizalco','SV-SO')
insert into municipio values (232,'Nahulingo','SV-SO')
insert into municipio values (233,'Salcoatitán','SV-SO')
insert into municipio values (234,'San Antonio del Monte','SV-SO')
insert into municipio values (235,'San Julián','SV-SO')
insert into municipio values (236,'Santa Catarina Masahuat','SV-SO')
insert into municipio values (237,'Santa Isabel Ishuatán','SV-SO')
insert into municipio values (238,'Santo Domingo de Guzmán','SV-SO')
insert into municipio values (239,'Sonzacate','SV-SO')



GO



insert into municipio values (240,'Usulutan','SV-US')
insert into municipio values (241,'Alegría','SV-US')
insert into municipio values (242,'Berlín','SV-US')
insert into municipio values (243,'California','SV-US')
insert into municipio values (244,'Concepción Batres','SV-US')
insert into municipio values (245,'El Triunfo','SV-US')
insert into municipio values (246,'Ereguayquín','SV-US')
insert into municipio values (247,'Estanzuelas','SV-US')
insert into municipio values (248,'Jiquilisco','SV-US')
insert into municipio values (249,'Jucuapa','SV-US')
insert into municipio values (250,'Jucuarán','SV-US')
insert into municipio values (251,'Mercedes Umaña','SV-US')
insert into municipio values (252,'Nueva Granada','SV-US')
insert into municipio values (253,'Ozatlán','SV-US')
insert into municipio values (254,'Puerto El Triunfo','SV-US')
insert into municipio values (255,'San Agustín','SV-US')
insert into municipio values (256,'San Buenaventura','SV-US')
insert into municipio values (257,'San Dionisio','SV-US')
insert into municipio values (258,'San Francisco Javier','SV-US')
insert into municipio values (259,'Santa Elena','SV-US')
insert into municipio values (260,'Santa María','SV-US')
insert into municipio values (261,'Santiago de María','SV-US')
insert into municipio values (262,'Tecapán','SV-US')
