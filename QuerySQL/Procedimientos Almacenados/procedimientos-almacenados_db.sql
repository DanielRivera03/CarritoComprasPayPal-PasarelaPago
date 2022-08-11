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