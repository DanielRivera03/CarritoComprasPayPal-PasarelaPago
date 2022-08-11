# Carrito de Compras + Pasarela de Pagos (PayPal)

![PresentacionBG](https://user-images.githubusercontent.com/44457989/184070906-9e83810b-a001-41a3-9c1a-8e039cbb22a9.png)


<h1>Configuración Inicial</h1>

<p>Estimado(a) visitante, reciba una cordial bienvenida a este repositorio. Antes de iniciar y ejecutar este sistema en su servidor, debe realizar algunas configuraciones correspondientes para el óptimo funcionamiento del mismo, las cuales son las siguientes:

<h4>* Archivo de conexión (Web.config)</h4>

Usted debe de realizar los correspondientes cambios a la cadena de conexión, dicho archivo se llama <strong>Web.config</strong> en dónde Data Source= es el nombre de su servidor, Initial Catalog= el nombre de la base de datos, User ID=;Password= nombre de usuario y contraseña de acceso de su usuario configurado en su SGBD SQL SERVER. <strong>Único punto en dónde usted deberá conectar la base de datos a esta aplicación. NO se utilizan más cadenas de conexiones.</strong>

<img width="920" alt="raycast-untitled" src="https://user-images.githubusercontent.com/44457989/184071771-89563455-562a-49f4-bf5f-38c32917e3ac.png">


<h4>* Conectividad API Pasarela de Pagos PayPal (Web.config)</h4>

Dentro del mismo archivo Web.config, usted encontrará la configuración general de la conexión hacia la API de PayPal. Para ello usted debe crear una cuenta en PayPal Developer ( https://developer.paypal.com ) y configurar su cuenta personal para utilizar el entorno de pruebas PayPal SandBox (Para mayor información específica de la creación de su cuenta, configuración y demás gestiones relacionadas, por favor visite la documentación oficial de PayPal Developer).


Una vez configurada su cuenta, usted debe ubicar los siguientes bloques de código, y sustituir por toda la información que la plataforma antes mencionada le solicita. Dónde ClientId= es su usuario encriptado generado por la plataforma y Secret= su contraseña de acceso generada por la plataforma. <strong>Por favor NO tocar UrlPaypal esa es la URL de comunicación con la API de los servicios de PayPal.</strong>


<img width="920" alt="raycast-untitled (1)" src="https://user-images.githubusercontent.com/44457989/184073047-72c1c6cf-326e-44c0-9b66-c1f29580a9e1.png">



<h4>* URL Redireccionamiento Pagos Procesados / Fallidos - PayPal ( Controllers/TiendaController.cs )</h4>


Por favor ubique el siguiente metodo <strong>public async Task<JsonResult> ProcesarPago(List<Carrito> oListaCarrito, Venta oVenta)</strong> Dentro de la carpeta Controllers - Archivo llamado TiendaController.cs <strong>Usted debe sustituir únicamente el https inicial el cuál corresponde a su servidor. Todo lo demás por favor NO MODIFICAR</strong> Caso contrario, si modifica de manera incorrecta, o bien no realiza el ajuste pertinente, obtendrá un error 404 al momento de finalizar la compra.


<img width="920" alt="raycast-untitled (2)" src="https://user-images.githubusercontent.com/44457989/184073915-6a1f07dd-fd9e-48fd-bbac-07c037159622.png">



<h4>* Ajustes SMTP .NET - Envío de Correos Automáticos ( CapaNegocio/CN_Recursos.cs )</h4>

Usted debe de configurar el SMTP que funciona únicamente para enviar correos automáticos al momento de reestablecer su contraseña si cualquier usuario pierde el acceso a la plataforma. Por favor tome nota de la imagen citada abajo y realizar los respectivos cambios que los comentarios hacen mención. <strong>Únicamente necesita configurar ya sea su correo personal como receptor de envío de correos automáticos para aplicaciones, o bien un correo institucional real dentro de un servicio de hosting.</strong>

<img width="920" alt="raycast-untitled (3)" src="https://user-images.githubusercontent.com/44457989/184075296-f7baa64d-28db-4300-acd9-bc3e7d83602d.png">

Y de esta manera hemos concluído con éxito toda la configuración necesaria para que este sistema funcione perfectamente en su servidor. Sí tiene problemas, por favor haga uso del Pull & Request o bien siga los pasos antes mencionados nuevamente.

</p>


<h1>Importante</h1>

<p>Se ha compartido todas las sentencias SQL que usted necesita para crear la respectiva base de datos. <strong>Esta en total libertad de dar el nombre que usted desee al momento de crear la misma, pero deberá reflejar ese cambio en el archivo de conexión de la aplicación correspondiente.</strong> Al momento de registrar los respectivos productos, usted se dará cuenta que hay una columna llamada <strong>RutaImagen</strong> en dónde usted deberá asignar su URL completa de su máquina en específico. Si usted no realiza este cambio, lamentamos informarle que las imágenes no se mostrarán al momento de ejecutar está aplicación. <strong>La URL por defecto es la siguiente: C:\Users\dany_\OneDrive\Escritorio\CarritoMVC\FotosProductos  -> Usted debe cambiar según la ubicación en dónde ejecute esta aplicación.</strong></p>



<h1>Descripción General</h1>

<p>Sistema desarrollado para simular las típicas tiendas en línea, en dónde usted puede agregar los productos ofertados a un carrito de compras, elegir la cantidad de artículos de cada uno de los elementos que usted agregué y realizar la respectiva compra de los mismos. En este caso únicamente se ha manejado la interfaz de clientes, no existen más roles de usuarios, ni un panel de administración para realizar las respectivas tareas de gestión de inventarios y productos de una tienda, pero perfectamente pueden ser integradas si usted decide mejorar y añadir dichas funcionalidades.</p>


<p>Este sistema a nivel de código y base de datos se encuentra distribuido de la siguiente manera:<ul><li>Base de Datos:</li><ul><li>10 Tablas + 1 Tabla Virtual.</li><li>6 Procedimientos Almacenados.</li><li>1 Función.</li></ul></ul><ul><li>Sistema:</li><ul><li>Lenguaje de Programación C# ASP.NET - ADO.NET.</li><li>Patrón MVC (Modelo, Vista, Controlador).</li><li>Gestiones AJAX, JQuery.</li><li>Desarrollo basado en capas (Capa de Negocio, Capa de Datos, Capa de Presentación).</li><li>Complementos JQuery, Javascript</li><li>Envío de Correos Automáticos - SMTP .NET.</li><li>Plantilla Bootstrap.</li><li>Mantenimientos y gestiones asíncronos, es decir, todo se realiza en tiempo real sin refrescar la página.</li></ul></ul></p>


<h1>Capturas</h1>

<h4>* Vista principal (index) sin inicio de sesión</h4>


![cc1](https://user-images.githubusercontent.com/44457989/184077750-2636b249-e850-4bb6-a323-e28bb1b73312.png)



<h4>* Detalle de productos sin inicio de sesión</h4>


![cc2](https://user-images.githubusercontent.com/44457989/184077964-48055152-5878-4b9f-ac96-240fc8886a9d.png)



<h4>* Login - Inicio de sesión clientes</h4>


![cc3](https://user-images.githubusercontent.com/44457989/184078065-709dbad5-e2d3-4ab5-b381-db9fe4293fe7.png)



<h4>* Recuperación de cuentas - Reestablecer contraseñas</h4>


![cc4](https://user-images.githubusercontent.com/44457989/184078130-a6a13d32-31e4-40e8-b913-59a0831936ea.png)


<h4>* Vista principal (index) usuario logueado</h4>


![cc6](https://user-images.githubusercontent.com/44457989/184078209-cc2a4577-d285-4745-b52f-b5cd98ce1617.png)



<h4>* Confirmación producto añadido a carrito de compras, y validación de productos repetidos</h4>

![cc7](https://user-images.githubusercontent.com/44457989/184078356-9c9a286c-2bbb-49c1-99be-2feaa20a0a2e.png)


![cc8](https://user-images.githubusercontent.com/44457989/184078355-d28b608f-6ca6-4efa-b148-8c0b75e0dff3.png)




<h4>* Detalle general de productos agregados en carrito de compras</h4>



![cc9](https://user-images.githubusercontent.com/44457989/184078487-16bbb672-33fd-4399-889d-cb998882f633.png)



<h4>* Proceso de compra - ¿Cómo Configurar? - Leer Detalle Arriba</h4>


![cc10](https://user-images.githubusercontent.com/44457989/184078765-c0baab31-5d29-4191-be44-110990c8d74c.png)



![cc11](https://user-images.githubusercontent.com/44457989/184078762-9336bfb0-ddf4-41c7-9131-33825396a7df.png)



![cc12](https://user-images.githubusercontent.com/44457989/184078760-8f49f7c3-c0a8-4c16-a29d-d2a12a9cafcb.png)



<h4>* Listado de todas mis compras procesadas</h4>



![cc13](https://user-images.githubusercontent.com/44457989/184078931-dd0af6c4-1dc6-465c-8e5e-5929916337ac.png)




<h4>* Recepción de correos automáticos - recuperación de cuentas clientes</h4>


![cc14](https://user-images.githubusercontent.com/44457989/184247251-5f4a679d-176d-4495-a0d4-62ebf3e6ce4d.png)




---

<h2>Créditos correspondientes dentro de código fuente de esta aplicación | Muchas gracias por obtener este repositorio hecho con algunas tazas de café ☕ ❤️</h2>




 ![CSharp](https://user-images.githubusercontent.com/44457989/70968018-afb2cf00-205d-11ea-9b79-2ff5a0a100ac.png)<br>
 
 
 
 <h4>*** Fecha de Subida: 11 agosto 2022 ***</h4>




