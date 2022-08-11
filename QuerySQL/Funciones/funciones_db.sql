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