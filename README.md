# DeMisManosGUI

Solución ASP.NET Web API
Net 9.0

Creación de un nuevo proyecto basado en otro existente

1. Crear un nuevo repo en github para la Api
2. Crear un nuevo repo en github para la Gui
3. Clonar ambos proyectos con git en tu máquina local (Git Clone Nam_Repo)
4. Copiar el contenido del proyecto base en su nuevo repo (Api) excluyendo .git y .vs
5. Copiar el contenido del proyecto base en su nuevo repo (Gui) excluyendo .git, dist y node_modules
6. Abrir el proyecto Api, renombrar la solución y renombrar el proyecto
7. Cerrar el proyecto y abrir el archivo .sln con un bloc de notas y renombrar lo necesario
8. Abrir el proyecto y dar clic derecho sobre la solución y seleccionar Sincronizar espacios de nombres (Actualiza los namespaces)
9. Ejecutar nuevamente el proyecto y probar
10. Generar un Personal access tokens en github (foto/settings/Developer Settings/Personal access tokens/Tokens)
11. Actualizar credenciales y origin repo en GitBash con git remote set-url origin - git config user.name - git config user.email
12. Subir los cambios a GitHub
13. Actualizar Git en Visual Studio
14. Actualizar versión de net en visual studio .net en las propiedades del proyecto se sube a la versión mas reciente luego de actualizar
    visual studio .net desde el Visual Studio Installer que se busca en los programas de windows (Cuando se actualiza, trae los nuevos donets)
15. Actualizar paquetes por el Nugget Packages

Token GitHub
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
Comando Git para hacer push a través de un token|
git remote set-url origin https://carloscarmona0829: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@github.com/carloscarmona0829/SrcProjectApi.git
git config user.name "Carlos Carmona"
git config user.email "carloscarmona0829@gmail.com"
Comandos para verificar user.name y user.email
git config user.name
git config user.email

**********************************************************************************************************************************************
- Para correr este proyecto se debe:
	* Verificar que se tengan instalados estos paquetes para que funcione Identity
		Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
		Install-Package Microsoft.EntityFrameworkCore.Tools
		Install-Package Microsoft.EntityFrameworkCore.SqlServer
	* Verificar que exista la carpeta Configuration con las clases del contexto (ApplicationDbContext.cs y ApplicationDbContextFactory.cs)
	* Crear la clase ApplicationUserModel.cs con los atributos personalizados para la cración de los usuarios
	* Verificar que en Program.cs estén todas las configuraciones de identity y del contexto incluyendo la clase ApplicationUserModel.cs
	(ConnectionStrings debe ser igual en Program.cs y en appsettings.json)
	* Restaurar la base de datos si es que se sacó antes un backup.
	* En la tabla NetUsers hay un usuario carloscarmona0829@gmail.com con clave 123456
	* Ajustar la cadena de conexión.

- Si se quiere iniciar una base de datos nueva desde cero se debe:
	* Abrir la consola del administrador de paquetes de Visual Studio(ver/otras ventanas/consola del administrador de paquetes).
	* Eliminar la migración que hay en el proyecto con el comando remove-migration.

- En caso de que no funcione la eliminación con el anterior comando se debe:
	* Eliminar completa la Carpeta Migrations.
	* Se deben correr los siguientes comandos 
	  add-migration initial-migration
	  update-database

Nota: Si el campo personalizado PhoneNumber cuando se cree la base de datos no queda como opcional, si es necesario, toca cambiarlo manualmente.

